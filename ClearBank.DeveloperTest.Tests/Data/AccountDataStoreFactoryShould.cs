using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Tests.Data
{
    public class AccountDataStoreFactoryShould
    {
        [Fact]
        public void Create_AccountDataStore()
        {
            var dataStoreFactory = new AccountDataStoreFactory();

            var dataStore = dataStoreFactory.CreateDataStore("some-non-backup-data-store-type");

            Assert.IsType<AccountDataStore>(dataStore);
        }

        [Fact]
        public void Create_BackupAccountDataStore()
        {
            var dataStoreFactory = new AccountDataStoreFactory();

            var dataStore = dataStoreFactory.CreateDataStore(AccountDataStoreFactory.BackupDataStoreType);

            Assert.IsType<BackupAccountDataStore>(dataStore);
        }
    }
}