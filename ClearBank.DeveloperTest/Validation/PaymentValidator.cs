using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Validation
{
    public class PaymentValidator
    {
        private IList<IPaymentValidator> innerValidators;

        public PaymentValidator(IList<IPaymentValidator> innerValidators)
        {
            this.innerValidators = innerValidators;
        }

        public MakePaymentResult IsPaymentAllowed(PaymentContext paymentContext)
        {
            foreach (var validator in innerValidators)
            {
                var makePaymentResult = validator.IsPaymentAllowed(paymentContext);
                if (makePaymentResult.Success)
                    return makePaymentResult;
            }

            return new MakePaymentResult { Success = false };
        }
    }
}
