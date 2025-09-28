using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation
{
    public class BacsPaymentValidator : IPaymentValidator
    {
        public MakePaymentResult IsPaymentAllowed(PaymentContext paymentContext) =>
            paymentContext.MakePaymentRequest.PaymentScheme == PaymentScheme.Bacs
            && paymentContext.Account != null
            && paymentContext.Account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs)
                ? new MakePaymentResult { Success = true }
                : new MakePaymentResult { Success = false };
    }
}
