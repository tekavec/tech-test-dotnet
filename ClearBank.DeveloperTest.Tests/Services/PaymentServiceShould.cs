using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceShould
    {
        private const string BackupDataStoreType = "Backup";
        private const string NormalDataStoreType = "data-store";
        private const string SomeAccountNumber = "12345678";
        private readonly Account NonExistentAccount = null;

        [Theory]
        [InlineData(PaymentScheme.Bacs, 1)]
        [InlineData(PaymentScheme.FasterPayments, 1)]
        [InlineData(PaymentScheme.Chaps, 1)]
        public void Not_Make_Successful_Backup_Payment_If_Account_Does_Not_Exist(
            PaymentScheme paymentScheme,
            decimal amount)
        {
            var makePaymentRequest = GetMakePaymentRequest(amount, paymentScheme);
            var paymentService = GetPaymentService(NormalDataStoreType, NonExistentAccount);

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
            var makePaymentRequest = GetMakePaymentRequest(amount, paymentScheme);
            var paymentService = GetPaymentService(NormalDataStoreType, NonExistentAccount);

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
            var account = GetAccount(AllowedPaymentSchemes.Bacs, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, PaymentScheme.Bacs);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.True(result.Success);
            Assert.True(account.Balance == accountBalance - amount);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 1, 100)]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 1, 1)]
        [InlineData(BackupDataStoreType, AccountStatus.Disabled, 1, 1)]
        [InlineData(BackupDataStoreType, AccountStatus.InboundPaymentsOnly, 1, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 1, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Disabled, 1, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.InboundPaymentsOnly, 1, 1)]
        public void Make_Successful_Payment_For_FasterPayments_And_Update_Account_Balance_If_Account_Balance_Is_Adequate(
            string dataStoreType,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            var account = GetAccount(AllowedPaymentSchemes.FasterPayments, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, PaymentScheme.FasterPayments);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.True(result.Success);
            Assert.True(account.Balance == accountBalance - amount);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 1, 0)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 1, 0)]
        public void Not_Make_Payment_For_FasterPayments_If_Account_Balance_Is_Inadequate(
            string dataStoreType,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            var account = GetAccount(AllowedPaymentSchemes.FasterPayments, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, PaymentScheme.FasterPayments);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
            Assert.True(account.Balance == accountBalance);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(BackupDataStoreType, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 100, 1)]
        [InlineData(NormalDataStoreType, AccountStatus.Live, 1, 100)]
        public void Make_Successful_Payment_For_Chaps_And_Update_Account_Balance_If_Account_Is_Live(
            string dataStoreType,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            var account = GetAccount(AllowedPaymentSchemes.Chaps, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, PaymentScheme.Chaps);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.True(result.Success);
            Assert.True(account.Balance == accountBalance - amount);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AccountStatus.Disabled, 1, 100)]
        [InlineData(BackupDataStoreType, AccountStatus.InboundPaymentsOnly, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.Disabled, 1, 100)]
        [InlineData(NormalDataStoreType, AccountStatus.InboundPaymentsOnly, 1, 100)]
        public void Not_Make_Payment_For_Chaps_If_Account_Is_Not_Live(
            string dataStoreType,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            var account = GetAccount(AllowedPaymentSchemes.Chaps, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, PaymentScheme.Chaps);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
            Assert.True(account.Balance == accountBalance);
        }

        [Theory]
        [InlineData(BackupDataStoreType, AllowedPaymentSchemes.Bacs, PaymentScheme.FasterPayments, AccountStatus.Live, 1, 100)]
        [InlineData(BackupDataStoreType, AllowedPaymentSchemes.FasterPayments, PaymentScheme.Chaps, AccountStatus.Live, 1, 100)]
        [InlineData(BackupDataStoreType, AllowedPaymentSchemes.Chaps, PaymentScheme.Bacs, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AllowedPaymentSchemes.Bacs, PaymentScheme.FasterPayments, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AllowedPaymentSchemes.FasterPayments, PaymentScheme.Chaps, AccountStatus.Live, 1, 100)]
        [InlineData(NormalDataStoreType, AllowedPaymentSchemes.Chaps, PaymentScheme.Bacs, AccountStatus.Live, 1, 100)]
        public void Not_Make_Payment_If_Requested_PaymentScheme_Is_Not_Allowed_By_Account(
            string dataStoreType,
            AllowedPaymentSchemes allowedPaymentSchemes,
            PaymentScheme paymentScheme,
            AccountStatus status,
            decimal amount,
            decimal accountBalance)
        {
            var account = GetAccount(allowedPaymentSchemes, accountBalance, status);
            var makePaymentRequest = GetMakePaymentRequest(amount, paymentScheme);
            var paymentService = GetPaymentService(dataStoreType, account);

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
            Assert.True(account.Balance == accountBalance);
        }

        private static TestablePaymentService GetPaymentService(string dataStoreType, Account account) => 
            new TestablePaymentService
            {
                DataStoreType = dataStoreType,
                BackupAccountDataStore = new TestableBackupAccountDataStore { Account = account },
                AccountDataStore = new TestableAccountDataStore { Account = account }
            };

        private static Account GetAccount(
            AllowedPaymentSchemes allowedPaymentSchemes,
            decimal accountBalance,
            AccountStatus status) =>
                new Account { AllowedPaymentSchemes = allowedPaymentSchemes, Balance = accountBalance, Status = status };

        private static MakePaymentRequest GetMakePaymentRequest(decimal amount, PaymentScheme paymentScheme) => 
            new MakePaymentRequest
            {
                DebtorAccountNumber = SomeAccountNumber,
                PaymentScheme = paymentScheme,
                Amount = amount
            };
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
