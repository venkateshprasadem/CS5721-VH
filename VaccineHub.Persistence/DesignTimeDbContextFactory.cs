using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VaccineHub.Persistence
{
    [UsedImplicitly]
    internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VaccineHubDbContext>
    {
        private const string ConnectionStringEnvVarName = "ConnectionString";

        private const string CommandTimeoutInSecsString = "CommandTimeoutInSecs";

        public VaccineHubDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvVarName);

            var commandTimeoutInSecsString = Environment.GetEnvironmentVariable(CommandTimeoutInSecsString);

            if (commandTimeoutInSecsString != null && ToNullableInt(commandTimeoutInSecsString) == null)
            {
                throw new InvalidOperationException(
                    $"Invalid CommandTimeoutInSecs found in environment variable: {CommandTimeoutInSecsString}");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"No connection string found in environment variable: {ConnectionStringEnvVarName}");
            }

            var optionsBuilder = new DbContextOptionsBuilder<VaccineHubDbContext>();

            optionsBuilder.UseNpgsql(connectionString, builder => builder.CommandTimeout(ToNullableInt(commandTimeoutInSecsString)));

            return new VaccineHubDbContext(optionsBuilder.Options);
        }

        private static int? ToNullableInt(string s)
        {
            if (int.TryParse(s, out var i)) return i;
            return null;
        }
    }
}