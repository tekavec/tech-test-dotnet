namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStoreFactory
    {
        IAccountDataStore CreateDataStore(string dataStoreType);
    }
}