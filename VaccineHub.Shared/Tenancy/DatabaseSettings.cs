namespace VaccineHub.Shared.Tenancy
{
    public sealed class DatabaseSettings
    {
        /// <summary>
        /// Database server
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Database port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Database name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Database Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Database password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Send SQL queries to the logging frame
        /// </summary>
        public bool IsQueryLoggingEnabled { get; set; }

        public ConnectionPooling Pooling { get; set; }
    }
}