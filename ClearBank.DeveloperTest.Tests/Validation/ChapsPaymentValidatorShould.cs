using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;

namespace ClearBank.DeveloperTest.Tests.Validation
{
    public class ChapsPaymentValidatorShould
    {
        private readonly Account NonExistentAccount = null;

        [Theory]
        [InlineData(PaymentScheme.Bacs)]
        [InlineData(PaymentScheme.FasterPayments)]
        public void Not_Allow_Payment_If_Requested_Payment_Scheme_Is_Not_Chaps(PaymentScheme paymentScheme)
        {
            var chapsPaymentValidator = new ChapsPaymentValidator();
            var paymentContext = GetChapsPaymentContext(
                paymentScheme,
                GetLiveAccount(AllowedPaymentSchemes.Chaps));

            MakePaymentResult result = chapsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Fact]
        public void Not_Allow_Payment_If_No_Account_Provided()
        {
            var chapsPaymentValidator = new ChapsPaymentValidator();
            var paymentContext = GetChapsPaymentContext(
                PaymentScheme.Chaps,
                NonExistentAccount);

            MakePaymentResult result = chapsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps)]
        public void Allow_Payment_If_Account_Allows_Chaps_Payment_Scheme(
            AllowedPaymentSchemes allowedPaymentScheme,
            PaymentScheme paymentScheme)
        {
            var chapsPaymentValidator = new ChapsPaymentValidator();
            var paymentContext = GetChapsPaymentContext(
                paymentScheme,
                GetLiveAccount(allowedPaymentScheme));

            MakePaymentResult result = chapsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs)]
        [InlineData(AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, PaymentScheme.Bacs)]
        public void Not_Allow_Payment_If_Account_Does_Not_Allow_Chaps_Payment_Scheme(
            AllowedPaymentSchemes allowedPaymentScheme,
            PaymentScheme paymentScheme)
        {
            var chapsPaymentValidator = new ChapsPaymentValidator();
            var paymentContext = GetChapsPaymentContext(
                paymentScheme,
                GetLiveAccount(allowedPaymentScheme));

            MakePaymentResult result = chapsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps, AccountStatus.Disabled)]
        [InlineData(AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps, AccountStatus.InboundPaymentsOnly)]
        public void Not_Allow_Payment_If_Account_Is_Not_live(
            AllowedPaymentSchemes allowedPaymentScheme,
            PaymentScheme paymentScheme,
            AccountStatus accountStatus)
        {
            var chapsPaymentValidator = new ChapsPaymentValidator();
            var paymentContext = GetChapsPaymentContext(
                paymentScheme,
                GetAccount(allowedPaymentScheme, accountStatus));

            MakePaymentResult result = chapsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        private static Account GetLiveAccount(AllowedPaymentSchemes allowedPaymentSchemes) =>
            GetAccount(allowedPaymentSchemes, AccountStatus.Live);

        private static Account GetAccount(AllowedPaymentSchemes allowedPaymentSchemes, AccountStatus accountStatus) =>
            new Account { AllowedPaymentSchemes = allowedPaymentSchemes, Status = accountStatus };

        private static MakePaymentRequest GetMakePaymentRequest(PaymentScheme paymentScheme) =>
            new MakePaymentRequest { PaymentScheme = paymentScheme };

        private static PaymentContext GetChapsPaymentContext(PaymentScheme paymentScheme, Account account) =>
            new PaymentContext
            {
                Account = account,
                MakePaymentRequest = GetMakePaymentRequest(paymentScheme)
            };
    }
}