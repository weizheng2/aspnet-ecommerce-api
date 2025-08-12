using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Authorization;
using ECommerceApi.Utils;

namespace ECommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [EnableRateLimiting(Constants.RateLimitGeneral)]
    [ControllerName("Payment"), Tags("Payment")]
    [ApiController, Route("api/v{version:apiVersion}/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var apiVersion = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1";

            var successUrl = $"{baseUrl}/api/v{apiVersion}/payment/payment-success?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{baseUrl}/api/v{apiVersion}/payment/payment-cancelled";

            var result = await _paymentService.CreateCheckoutSession(successUrl, cancelUrl);
            if (result.IsSuccess)
                return Ok(result.Data);

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [HttpGet("payment-success")]
        [Authorize]
        public async Task<IActionResult> OnPaymentSuccess([FromQuery] string session_id)
        {
            var result = await _paymentService.OnPaymentSuccess(session_id);
            return Content(result.Data);     
        }

        [HttpGet("payment-cancelled")]
        [Authorize]
        public ActionResult OnPaymentCancelled()
        {
            return Content($"Payment cancelled!");
        }
        
        // Add when the app is hosted
        // [HttpPost("webhook")]
        // public async Task<IActionResult> StripeWebhook()

        // }
    }
}


