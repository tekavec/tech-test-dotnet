using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation
{
    public class FasterPaymentsPaymentValidator : IPaymentValidator
    {
        public MakePaymentResult IsPaymentAllowed(PaymentContext paymentContext) =>
            paymentContext.MakePaymentRequest.PaymentScheme == PaymentScheme.FasterPayments
            && paymentContext.Account != null
            && paymentContext.Account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
            && paymentContext.Account.Balance >= paymentContext.MakePaymentRequest.Amount
                ? new MakePaymentResult { Success = true }
                : new MakePaymentResult { Success = false };
    }
}