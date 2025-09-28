using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory accountDataStoreFactory;
        private readonly string dataStoreType;

        [Obsolete]
        public PaymentService()
        {
            this.accountDataStoreFactory = new AccountDataStoreFactory();
            this.dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
        }

        public PaymentService(
            IAccountDataStoreFactory accountDataStoreFactory,
            IOptions<DataStoreOptions> dataStoreOptions)
        {
            this.accountDataStoreFactory = accountDataStoreFactory;
            this.dataStoreType = dataStoreOptions.Value.DataStoreType;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = this.dataStoreType;

            Account account = null;

            var accountDataStore = this.accountDataStoreFactory.CreateDataStore(dataStoreType);
            account = accountDataStore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            result.Success = true;

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                    }
                    break;
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;

                accountDataStore.UpdateAccount(account);
            }

            return result;
        }
    }
}
