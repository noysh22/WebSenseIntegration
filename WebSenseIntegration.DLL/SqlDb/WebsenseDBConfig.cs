using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Siemplify.Integrations.WebSense.SqlDb
{
    public sealed class WebSenseDbConfig
    {
        private const string WebsenseConnStringName = "WebSenseContext";

        private readonly string _defaultConnString;

        public string DbHost { get; private set; }
        public string DbUser { get; private set; }
        public string DbPass { get; private set; }

        public WebSenseDbConfig(string dbPass, string dbHost = null, string dbUser = null)
        {
            DbHost = dbHost;
            DbUser = dbUser;
            DbPass = dbPass;

            var config = ConfigurationManager.OpenExeConfiguration(
                Path.Combine(Environment.CurrentDirectory,
                Assembly.GetExecutingAssembly().ManifestModule.Name));

            _defaultConnString = config.ConnectionStrings.ConnectionStrings[WebsenseConnStringName].ConnectionString;

            if (string.IsNullOrEmpty(_defaultConnString))
            {
                throw new WebSenseDbConfigurationException("Invalid configuration file, missing web sense database connection string");
            }
        }

        public string BuildDbConnectionString()
        {
            const string passKeyName = "Password";
            const string userKeyName = "User ID";
            const string dbHostKeyName = "Data Source";

            var entityConnBuilder = new EntityConnectionStringBuilder(_defaultConnString);
            var dbProviderFactory = DbProviderFactories.GetFactory(entityConnBuilder.Provider);
            var dbConnBuilder = dbProviderFactory.CreateConnectionStringBuilder();
 
            if (null == dbConnBuilder)
            {
                throw new WebSenseDbConfigurationException("Failed to build db provider connection string CreateConnectionStringBuilder() returned null");
            }

            dbConnBuilder.ConnectionString = entityConnBuilder.ProviderConnectionString;

            // Add the db password to the connection string
            dbConnBuilder.Add(passKeyName, DbPass);

            // Determine to which db host to connect to
            if (string.IsNullOrEmpty(dbConnBuilder[dbHostKeyName].ToString()) && !string.IsNullOrEmpty(DbHost))
            {
                dbConnBuilder.Add(dbHostKeyName, DbHost);
            }
            else if (!dbConnBuilder.ContainsKey(dbHostKeyName) || string.IsNullOrEmpty(dbConnBuilder[dbHostKeyName].ToString()))
            {
                throw new WebSenseDbConfigurationException("Cannot create a db connection string without a db host or ip address");
            }

            // Determine with which user to connect with to the DB
            if (string.IsNullOrEmpty(dbConnBuilder[userKeyName].ToString()) && !string.IsNullOrEmpty(DbUser))
            {
                dbConnBuilder.Add(userKeyName, DbUser);
            }
            else if (!dbConnBuilder.ContainsKey(userKeyName) || string.IsNullOrEmpty(dbConnBuilder[userKeyName].ToString()))
            {
                throw new WebSenseDbConfigurationException("Cannot create a db connection string without a db user info");
            }

            entityConnBuilder.ProviderConnectionString = dbConnBuilder.ToString();

            return entityConnBuilder.ToString();
        }
    }
}
