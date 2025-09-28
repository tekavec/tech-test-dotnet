using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceShould
    {
        [Fact]
        public void Not_Make_Successful_Payment_If_Account_Does_Not_Exist()
        {
            var paymentService = new PaymentService();
            var makePaymentRequest = new MakePaymentRequest { DebtorAccountNumber = "12345678", PaymentScheme = PaymentScheme.Bacs };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
        }
    }
}
