using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using Moq;

namespace ClearBank.DeveloperTest.Tests.Validation
{
    public class PaymentValidatorShould
    {
        private readonly MakePaymentResult SuccessfulResult = new MakePaymentResult { Success = true };
        private readonly MakePaymentResult UnsuccessfulResult = new MakePaymentResult { Success = false };
        private readonly Account SomeAccount = new Account();

        [Fact]
        public void Allow_Payment_When_First_Inner_Validator_Allows_Payment()
        {
            var paymentContext = new PaymentContext { Account = SomeAccount };
            var validatorOne = CreateValidatorWithExpectedResult(UnsuccessfulResult);
            var validatorTwo = CreateValidatorWithExpectedResult(SuccessfulResult);
            var validatorThree = CreateValidatorWithExpectedResult(SuccessfulResult);
            var paymentValidator = new PaymentValidator(
                new List<IPaymentValidator> 
                    { validatorOne.Object, validatorTwo.Object, validatorThree.Object });

            var result = paymentValidator.IsPaymentAllowed(paymentContext);

            Assert.True(result.Success);
            validatorOne.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Once);
            validatorTwo.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Once);
            validatorThree.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Never);
        }

        [Fact]
        public void Not_Allow_Payment_When_No_Inner_Validator_Allows_Payment()
        {
            var paymentContext = new PaymentContext { Account = SomeAccount };
            var validatorOne = CreateValidatorWithExpectedResult(UnsuccessfulResult);
            var validatorTwo = CreateValidatorWithExpectedResult(UnsuccessfulResult);
            var validatorThree = CreateValidatorWithExpectedResult(UnsuccessfulResult);
            var paymentValidator = new PaymentValidator(
                new List<IPaymentValidator>
                    { validatorOne.Object, validatorTwo.Object, validatorThree.Object });

            MakePaymentResult result = paymentValidator.IsPaymentAllowed(paymentContext);

            Assert.False(result.Success);
            validatorOne.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Once);
            validatorTwo.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Once);
            validatorThree.Verify(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>()), Times.Once);
        }

        private static Mock<IPaymentValidator> CreateValidatorWithExpectedResult(MakePaymentResult unsuccessfulResult)
        {
            Mock<IPaymentValidator> validator = new Mock<IPaymentValidator>();
            validator.Setup(a => a.IsPaymentAllowed(It.IsAny<PaymentContext>())).Returns(unsuccessfulResult).Verifiable();
            return validator;
        }
    }
}