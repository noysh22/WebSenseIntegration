
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Siemplify.Integrations.WebSense.Data;
using Siemplify.Integrations.WebSense.SqlDb;
using Siemplify.Integrations.WebSense.SSH;

namespace Siemplify.Integrations.WebSense
{
    public class WebSenseManager : IDisposable
    {
        public const string DefaultApiHost = @"https://10.0.0.2:15873/web/api/v1/";

        private readonly string _apiUsername;
        private readonly string _apiKey;
        private readonly string _gatewayHost;
        private readonly string _gatewayUser;

        private readonly ContentGatewayShell _shell;
        private readonly BlocklistController _blocklistController;
        private readonly WebSenseDbConfig _dbConfig;

        //public WebSenseManager(string apiUsername, string apiKey, string apiHost = DefaultApiHost)
        //{
        //    _apiUsername = apiUsername;
        //    _apiKey = apiKey;
        //}

        public WebSenseManager(
            string gatewayHost,
            string gatewayUser,
            string gatewayPass,
            string websenseDbPass,
            string websenseDbHost = null,
            string websenseDbUser = null)
        {
            _gatewayHost = gatewayHost;
            _gatewayUser = gatewayUser;

            _shell = new ContentGatewayShell(_gatewayHost, _gatewayUser, gatewayPass);
            _blocklistController = new BlocklistController(_shell);
            _dbConfig = new WebSenseDbConfig(websenseDbPass, websenseDbHost, websenseDbUser);
        }

        private void EnsureConnection()
        {
            if (!_shell.IsConnected)
            {
                _shell.Connect();
                Debug.WriteLine("SSH client connected to {0} using username {1}", _gatewayHost, _gatewayUser);
            }
        }

        /// <summary>
        /// Add a url to the blocklist of websense content gateway
        /// </summary>
        /// <param name="ruleType">BLOCK or ALLOW</param>
        /// <param name="destType"> DOMAIN or HOSTNAME or IP</param>
        /// <param name="dest">The destination of the blocklist rule</param>
        /// <param name="optionalFlags">Optional flags, whether to add port and source ip to the blocklist rule</param>
        /// <param name="sourceIp">source ip to add if optional flag was provided</param>
        /// <param name="port">port to add if optional flag was provided</param>
        /// <returns>true if the blocklist entry was added</returns>
        public bool AddToBlocklist(
            BlocklistRuleType ruleType,
            BlocklistDestType destType,
            string dest,
            BlocklistOptionalFlags optionalFlags = BlocklistOptionalFlags.NONE,
            string sourceIp = null,
            ushort port = 0)
        {
            return AddToBlocklist(
                new BlocklistEntry(ruleType, destType, dest, optionalFlags, sourceIp, port),
                true);
        }

        public Task<bool> AddToBlocklistAsync(
            BlocklistRuleType ruleType,
            BlocklistDestType destType,
            string dest,
            BlocklistOptionalFlags optionalFlags = BlocklistOptionalFlags.NONE,
            string sourceIp = null,
            ushort port = 0)
        {
            return Task.Factory.StartNew(() => 
                AddToBlocklist(new BlocklistEntry(ruleType, destType, dest, optionalFlags, sourceIp, port),true)
            );
        }

        /// <summary>
        /// Add a url to the blocklist of websense content gateway
        /// </summary>
        /// <param name="blocklistEntry">Blocklist entry object</param>
        /// <param name="shouldValidateResult">whether or not to validate the result of the addition</param>
        /// <returns>
        /// true if the blocklist entry was added and shouldValidateResult is set to true
        /// else always returns true
        /// </returns>
        public bool AddToBlocklist(IBlocklistable blocklistEntry, bool shouldValidateResult = false)
        {
            EnsureConnection();
            return _blocklistController.AddToBlocklist(blocklistEntry, shouldValidateResult);
        }

        /// <summary>
        /// Get all the urls which a given user-name has accessed
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>List of all the urls as strings</returns>
        public List<string> GetAllUrlsForUser(string username)
        {
            using (var context = new WebSenseContext(_dbConfig))
            {
                return context.GetAllUrlsForUser(username);
            }
        }

        /// <summary>
        /// Query the DB for all the user which match a given url
        /// </summary>
        /// <param name="urlStr">The url as a sting</param>
        /// <returns>List of all the users matching the given url</returns>
        /// <remarks>
        /// The url is checked in case-sensitive meaning: http://google.com != google.com
        /// These two URLs will return different results
        /// </remarks>
        public List<string> GetAllUsersForUrl(string urlStr)
        {
            using (var context = new WebSenseContext(_dbConfig))
            {
                return context.GetAllUsersForUrl(urlStr);
            }
        }

        public Task<List<string>> GetAllUsersForUrlAsync(string urlStr)
        {
            using (var context = new WebSenseContext(_dbConfig))
            {
                return context.GetAllUsersForUrlAsync(urlStr);
            }
        }

        public void Dispose()
        {
            if (null != _shell)
                _shell.Dispose();
        }
    }
}
