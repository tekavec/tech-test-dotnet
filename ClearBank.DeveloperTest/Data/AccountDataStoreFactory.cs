namespace ClearBank.DeveloperTest.Data
{
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        public const string BackupDataStoreType = "Backup";

        public IAccountDataStore CreateDataStore(string dataStoreType) =>
            dataStoreType == BackupDataStoreType
                ? new BackupAccountDataStore()
                : new AccountDataStore();
    }
}
