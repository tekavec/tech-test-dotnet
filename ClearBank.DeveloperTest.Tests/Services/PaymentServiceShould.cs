using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceShould
    {
        private const string BackupDataStoreType = "Backup";
        private const string NormalDataStoreType = "data-store";

        [Theory]
        [InlineData(PaymentScheme.Bacs, 1)]
        [InlineData(PaymentScheme.FasterPayments, 1)]
        [InlineData(PaymentScheme.Chaps, 1)]
        public void Not_Make_Successful_Backup_Payment_If_Account_Does_Not_Exist(
            PaymentScheme paymentScheme,
            decimal amount)
        {
            var makePaymentRequest = new MakePaymentRequest { DebtorAccountNumber = "12345678", PaymentScheme = paymentScheme, Amount = amount };
            var paymentService = new TestablePaymentService 
            { 
                DataStoreType = BackupDataStoreType, 
                BackupAccountDataStore = new TestableBackupAccountDataStore { Account = null} 
            };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(PaymentScheme.Bacs, 1)]
        [InlineData(PaymentScheme.FasterPayments, 1)]
        [InlineData(PaymentScheme.Chaps, 1)]
        public void Not_Make_Successful_Payment_If_Account_Does_Not_Exist(
            PaymentScheme paymentScheme,
            decimal amount)
        {
            var makePaymentRequest = new MakePaymentRequest { DebtorAccountNumber = "12345678", PaymentScheme = paymentScheme, Amount = amount };
            var paymentService = new TestablePaymentService
            {
                DataStoreType = NormalDataStoreType,
                AccountDataStore = new TestableAccountDataStore { Account = null }
            };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 1, 100)]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(BackupDataStoreType, AccountStatus.Disabled, 1, 100)]
        [InlineData(BackupDataStoreType, AccountStatus.InboundPaymentsOnly, 100, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Disabled, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.InboundPaymentsOnly, 100, 1)]
        public void Make_Successful_Payment_For_Bacs_And_Update_Account_Balance(
            string dataStoreType,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            Account account = GetAccount(AllowedPaymentSchemes.Bacs, accountBalance, status);
            var makePaymentRequest = new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = amount };
            var paymentService = new TestablePaymentService
            {
                DataStoreType = dataStoreType,
                BackupAccountDataStore = new TestableBackupAccountDataStore { Account = account },
                AccountDataStore = new TestableAccountDataStore { Account = account }
            };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.True(result.Success);
            Assert.True(account.Balance == accountBalance - amount);
        }

        private static Account GetAccount(
            AllowedPaymentSchemes allowedPaymentSchemes,
            decimal accountBalance,
            AccountStatus status) =>
                new Account { AllowedPaymentSchemes = allowedPaymentSchemes, Balance = accountBalance, Status = status };
    }

    public class TestablePaymentService : PaymentService
    {
        public string DataStoreType { get; set; }
        public AccountDataStore AccountDataStore { get; set; }
        public BackupAccountDataStore BackupAccountDataStore { get; set; }

        public override AccountDataStore GetAccountDataStore()
        {
            return this.AccountDataStore;
        }

        public override BackupAccountDataStore GetBackupAccountDataStore()
        {
            return this.BackupAccountDataStore;
        }

        public override string GetDataStoreType()
        {
            return this.DataStoreType;
        }
    }

    public class TestableAccountDataStore : AccountDataStore
    {
        public Account Account { get; set; }

        public override Account GetAccount(string accountNumber)
        {
            return this.Account;
        }
    }

    public class TestableBackupAccountDataStore : BackupAccountDataStore
    {
        public Account Account { get; set; }

        public override Account GetAccount(string accountNumber)
        {
            return this.Account;
        }
    }
}
