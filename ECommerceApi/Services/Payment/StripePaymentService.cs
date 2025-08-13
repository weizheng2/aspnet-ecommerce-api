using System.Text.Json;
using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.Utils;
using Stripe;
using Stripe.Checkout;

namespace ECommerceApi.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public StripePaymentService(ApplicationDbContext context, ICartService cartService, IUserService userService, IConfiguration configuration)
        {
            _context = context;
            _cartService = cartService;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<Result<string>> CreateCheckoutSession(string successUrl, string cancelUrl)
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result<string>.Failure(ResultErrorType.NotFound, "User not found");

            var cartResult = await _cartService.GetCartAsync();
            if (!cartResult.IsSuccess || cartResult.Data.Items.Count == 0)
                return Result<string>.Failure(ResultErrorType.BadRequest, "Cart is empty or not found.");

            var cart = cartResult.Data;

            var lineItems = cart.Items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = (long)(item.Product!.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            { "id", item.Product!.Id.ToString() }
                        },
                        Name = item.Product.Name,
                        Description = item.Product.Description
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,

                Metadata = new Dictionary<string, string>
                {
                    { "user_id", user.Id }
                }
            };

            // Create checkout Url
            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return Result<string>.Success(session.Url);
            }
            catch (StripeException ex)
            {
                return Result<string>.Failure(ResultErrorType.BadRequest, $"Stripe error: {ex.Message}");
            }
        }

        public async Task<Result<string>> OnPaymentSuccess(string sessionId)
        {
            try
            {
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(sessionId);

                string result = $"Payment Success!";
                return Result<string>.Success(result);
            }
            catch
            {
                return Result<string>.Failure(ResultErrorType.BadRequest, "Invalid session");
            }
        }

        public async Task<Result> OnWebhookReceived(string jsonData, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(jsonData, signature, _configuration["Stripe:EndPointSecret"]);

                switch (stripeEvent.Type)
                {
                    case EventTypes.CheckoutSessionCompleted:
                        var session = stripeEvent.Data.Object as Session;
                        await HandleCheckoutCompleted(session);
                        break;

                    default:
                        Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }

                return Result.Success();
            }
            catch (StripeException e)
            {
                return Result.Failure(ResultErrorType.BadRequest, "Error obtaining Stripe info");
            }
            catch (Exception e)
            {
                return Result.Failure(ResultErrorType.ServerError, "Error obtaining Stripe info");
            }
        }
        
        private async Task HandleCheckoutCompleted(Session session)
        {
            try
            {
                // Get user_id from metadata
                var userIdStr = session.Metadata?.GetValueOrDefault("user_id");
                var userId = !string.IsNullOrEmpty(userIdStr) ? userIdStr : "-1";

                if (userId == "-1")
                    Console.WriteLine($"Warning: user_id missing in session metadata for session {session.Id}. Using fallback '-1'.");
  
                // Retrieve full session details with line items
                var options = new SessionGetOptions();
                options.AddExpand("line_items.data.price.product");

                var sessionService = new SessionService();
                var sessionWithLineItems = sessionService.Get(session.Id, options);
                
                await SaveOrderAndClearCart(userId!, session, sessionWithLineItems);

                //Console.WriteLine($"Order created successfully for session {session.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling checkout completion: {ex.Message}");
                throw;
            }
        }

        private async Task SaveOrderAndClearCart(string userId, Session session, Session sessionWithLineItems)
        {
            // Create order
            var order = new Order
            {
                UserId = userId!,
                PaymentMethod = Constants.PaymentMethodStripe,
                PaymentToken = session.Id,
                PaymentStatus = session.PaymentStatus,
                TotalAmount = (decimal)(session.AmountTotal ?? 0) / 100,
                Currency = session.Currency,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            //Console.WriteLine($"Order created: {JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true })}");

            // Order Items
            var orderItems = new List<OrderItem>();
            foreach (var item in sessionWithLineItems.LineItems.Data)
            {
                if (!item.Price.Product.Metadata.TryGetValue("id", out var idValue) || !int.TryParse(idValue, out var productId))
                    productId = -1;

                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = (int)item.Quantity!,
                    UnitPrice = (decimal)(item.Price.UnitAmount ?? 0) / 100
                };

                orderItems.Add(orderItem);
            }

            //Console.WriteLine($"Order items: {JsonSerializer.Serialize(orderItems, new JsonSerializerOptions { WriteIndented = true })}");

            order.Items = orderItems;

            // Ensure correct transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            // Save Order
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); 

            // Clear cart
            await _cartService.ClearCartAsync(userId); 
            await transaction.CommitAsync();
        }
    }

}

