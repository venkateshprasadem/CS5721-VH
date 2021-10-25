namespace VaccineHub.Shared.Tenancy
{
    public sealed class Tenant
    {
        public Tenant(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
            CacheKey = name;
        }

        public string Name { get; }

        public string ConnectionString { get; }

        public string CacheKey { get; }
    }
}