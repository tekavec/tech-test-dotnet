using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation
{
    public interface IPaymentValidator
    {
        MakePaymentResult IsPaymentAllowed(PaymentContext paymentContext);
    }
}
