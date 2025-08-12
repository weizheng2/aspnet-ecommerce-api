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

        public StripePaymentService(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
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
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            string result = "Payment successful!";
            if (session.PaymentStatus != "paid")
                result = "Payment not confirmed";

            return Result<string>.Success(result);
        }
        
    }

}

