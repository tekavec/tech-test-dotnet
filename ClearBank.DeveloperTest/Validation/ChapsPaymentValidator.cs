using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation
{
    public class ChapsPaymentValidator : IPaymentValidator
    {
        public MakePaymentResult IsPaymentAllowed(PaymentContext paymentContext) =>
            paymentContext.MakePaymentRequest.PaymentScheme == PaymentScheme.Chaps
            && paymentContext.Account != null
            && paymentContext.Account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
            && paymentContext.Account.Status == AccountStatus.Live
                ? new MakePaymentResult { Success = true }
                : new MakePaymentResult { Success = false };
    }
}