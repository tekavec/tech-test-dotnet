using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory accountDataStoreFactory;
        private readonly IPaymentValidator paymentValidator;
        private readonly string dataStoreType;

        [Obsolete]
        public PaymentService()
        {
            this.accountDataStoreFactory = new AccountDataStoreFactory();
            this.dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            this.paymentValidator = new PaymentValidator(
                new List<IPaymentValidator>
                {
                    new BacsPaymentValidator(),
                    new FasterPaymentsPaymentValidator(),
                    new ChapsPaymentValidator()
                });
        }

        public PaymentService(
            IAccountDataStoreFactory accountDataStoreFactory,
            IOptions<DataStoreOptions> dataStoreOptions,
            IPaymentValidator paymentValidator)
        {
            this.accountDataStoreFactory = accountDataStoreFactory;
            this.dataStoreType = dataStoreOptions.Value.DataStoreType;
            this.paymentValidator = paymentValidator;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = this.dataStoreType;

            Account account = null;

            var accountDataStore = this.accountDataStoreFactory.CreateDataStore(dataStoreType);
            account = accountDataStore.GetAccount(request.DebtorAccountNumber);

            var result = this.paymentValidator.IsPaymentAllowed(new PaymentContext (account, request));

            if (result.Success)
            {
                account.Balance -= request.Amount;

                accountDataStore.UpdateAccount(account);
            }

            return result;
        }
    }
}
