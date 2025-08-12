using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface IPaymentService
    {
        Task<Result<string>> CreateCheckoutSession(string successUrl, string cancelUrl);
        Task<Result<string>> OnPaymentSuccess(string paymentId);

    }
}

