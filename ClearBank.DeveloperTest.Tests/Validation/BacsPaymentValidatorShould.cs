using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;

namespace ClearBank.DeveloperTest.Tests.Validation
{
    public class BacsPaymentValidatorShould
    {
        private readonly Account NonExistentAccount = null;

        [Theory]
        [InlineData(PaymentScheme.FasterPayments)]
        [InlineData(PaymentScheme.Chaps)]
        public void Not_Allow_Payment_If_Requested_Payment_Scheme_Is_Not_Bacs(PaymentScheme paymentScheme)
        {
            var bacsPaymentValidator = new BacsPaymentValidator();
            var paymentContext = GetBacsPaymentContext(paymentScheme, GetAccount(AllowedPaymentSchemes.Bacs));

            MakePaymentResult result = bacsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Fact]
        public void Not_Allow_Payment_If_No_Account_Provided()
        {
            var bacsPaymentValidator = new BacsPaymentValidator();
            var paymentContext = GetBacsPaymentContext(PaymentScheme.Bacs, NonExistentAccount);

            MakePaymentResult result = bacsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
        public void Allow_Payment_If_Account_Allows_Bacs_Payment_Scheme(AllowedPaymentSchemes allowedPaymentSchemes)
        {
            var bacsPaymentValidator = new BacsPaymentValidator();
            var paymentContext = GetBacsPaymentContext(PaymentScheme.Bacs, GetAccount(allowedPaymentSchemes));

            MakePaymentResult result = bacsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.FasterPayments)]
        [InlineData(AllowedPaymentSchemes.Chaps)]
        [InlineData(AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
        public void Not_Allow_Payment_If_Account_Does_Not_Allow_Bacs_Payment_Scheme(AllowedPaymentSchemes allowedPaymentSchemes)
        {
            var bacsPaymentValidator = new BacsPaymentValidator();
            var paymentContext = GetBacsPaymentContext(PaymentScheme.Bacs, GetAccount(allowedPaymentSchemes));

            MakePaymentResult result = bacsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        private static Account GetAccount(AllowedPaymentSchemes allowedPaymentSchemes) =>
            new Account { AllowedPaymentSchemes = allowedPaymentSchemes };

        private static MakePaymentRequest GetMakePaymentRequest(PaymentScheme paymentScheme) =>
            new MakePaymentRequest { PaymentScheme = paymentScheme };

        private static PaymentContext GetBacsPaymentContext(PaymentScheme paymentScheme, Account account) =>
            new PaymentContext
            {
                Account = account,
                MakePaymentRequest = GetMakePaymentRequest(paymentScheme)
            };
    }
}