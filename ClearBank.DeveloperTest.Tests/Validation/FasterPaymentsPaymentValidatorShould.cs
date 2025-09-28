using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;

namespace ClearBank.DeveloperTest.Tests.Validation
{
    public class FasterPaymentsPaymentValidatorShould
    {
        private readonly Account NonExistentAccount = null;
        private const int SmallAmount = 1;
        private const int LargeAmount = 100;

        [Theory]
        [InlineData(PaymentScheme.Bacs)]
        [InlineData(PaymentScheme.Chaps)]
        public void Not_Allow_Payment_If_Payment_Scheme_Is_Not_FasterPayments(PaymentScheme paymentScheme)
        {
            var fasterPaymentsPaymentValidator = new FasterPaymentsPaymentValidator();
            var paymentContext = GetFasterPaymentsPaymentContext(
                paymentScheme, 
                GetAccount(AllowedPaymentSchemes.FasterPayments, LargeAmount), 
                SmallAmount);

            MakePaymentResult result = fasterPaymentsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Fact]
        public void Not_Allow_Payment_If_No_Account_Provided()
        {
            var fasterPaymentsPaymentValidator = new FasterPaymentsPaymentValidator();
            var paymentContext = GetFasterPaymentsPaymentContext(
                PaymentScheme.FasterPayments, 
                NonExistentAccount, 
                SmallAmount);

            MakePaymentResult result = fasterPaymentsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Fact]
        public void Allow_Payment_If_Account_Allows_FasterPayments_Payment_Scheme_And_Account_Balance_Is_Adequate()
        {
            var fasterPaymentsPaymentValidator = new FasterPaymentsPaymentValidator();
            var paymentContext = GetFasterPaymentsPaymentContext(
                PaymentScheme.FasterPayments,
                GetAccount(AllowedPaymentSchemes.FasterPayments, LargeAmount),
                SmallAmount);

            MakePaymentResult result = fasterPaymentsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.True(result.Success);
        }

        [Fact]
        public void Not_Allow_Payment_If_Account_Allows_FasterPayments_But_Requested_Amount_Is_Greater_Than_Account_Balance()
        {
            var fasterPaymentsPaymentValidator = new FasterPaymentsPaymentValidator();
            var paymentContext = GetFasterPaymentsPaymentContext(
                PaymentScheme.FasterPayments,
                GetAccount(AllowedPaymentSchemes.FasterPayments, SmallAmount),
                LargeAmount);

            MakePaymentResult result = fasterPaymentsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs, SmallAmount, LargeAmount)]
        [InlineData(AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps, SmallAmount, LargeAmount)]
        [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, PaymentScheme.Bacs, SmallAmount, LargeAmount)]
        public void Not_Allow_Payment_If_Account_Does_Not_Allow_FasterPayments_Payment_Scheme(
            AllowedPaymentSchemes allowedPaymentScheme,
            PaymentScheme paymentScheme,
            decimal amount,
            decimal accountBalance)
        {
            var fasterPaymentsPaymentValidator = new FasterPaymentsPaymentValidator();
            var paymentContext = GetFasterPaymentsPaymentContext(
                paymentScheme,
                GetAccount(allowedPaymentScheme, accountBalance),
                amount);

            MakePaymentResult result = fasterPaymentsPaymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
        }

        private static Account GetAccount(AllowedPaymentSchemes allowedPaymentSchemes, decimal accountBalance) =>
            new Account { AllowedPaymentSchemes = allowedPaymentSchemes, Balance = accountBalance };

        private static MakePaymentRequest GetMakePaymentRequest(PaymentScheme paymentScheme, decimal amount) =>
            new MakePaymentRequest { PaymentScheme = paymentScheme, Amount = amount };

        private static PaymentContext GetFasterPaymentsPaymentContext(PaymentScheme paymentScheme, Account account, decimal amount) =>
            new PaymentContext(account, GetMakePaymentRequest(paymentScheme, amount));
    }
}
