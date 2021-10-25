namespace VaccineHub.Shared.Tenancy
{
    public class Database
    {
        public Database(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public string Name { get; }

        public string ConnectionString { get; }
    }
}