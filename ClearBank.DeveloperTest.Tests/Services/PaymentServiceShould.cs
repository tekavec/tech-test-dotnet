using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceShould
    {
        [Fact]
        public void Not_Make_Successful_Backup_Payment_If_Account_Does_Not_Exist()
        {
            var paymentService = new TestablePaymentService { DataStoreType = "Backup", BackupAccountDataStore = new TestableBackupAccountDataStore { Account = null} };
            var makePaymentRequest = new MakePaymentRequest { DebtorAccountNumber = "12345678", PaymentScheme = PaymentScheme.Bacs };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
        }

        [Fact]
        public void Not_Make_Successful_Payment_If_Account_Does_Not_Exist()
        {
            var paymentService = new TestablePaymentService { DataStoreType = "non-Backup", AccountDataStore = new TestableAccountDataStore { Account = null} };
            var makePaymentRequest = new MakePaymentRequest { DebtorAccountNumber = "12345678", PaymentScheme = PaymentScheme.Bacs };

            var result = paymentService.MakePayment(makePaymentRequest);

            Assert.False(result.Success);
        }
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
