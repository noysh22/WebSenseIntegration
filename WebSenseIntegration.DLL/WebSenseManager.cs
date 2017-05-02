
using System;
using System.Diagnostics;
using Siemplify.Integrations.WebSense.Data;
using Siemplify.Integrations.WebSense.SSH;

namespace Siemplify.Integrations.WebSense
{
    public class WebSenseManager : IDisposable
    {
        public const string DefaultApiHost = @"https://10.0.0.2:15873/web/api/v1/";

        private readonly string _apiUsername;
        private readonly string _apiKey;
        private readonly string _websenseHost;
        private readonly string _websenseUser;

        private readonly ContentGatewayShell _shell;
        private readonly BlocklistController _blocklistController;

        //public WebSenseManager(string apiUsername, string apiKey, string apiHost = DefaultApiHost)
        //{
        //    _apiUsername = apiUsername;
        //    _apiKey = apiKey;
        //}

        public WebSenseManager(string websenseHost, string websenseUser, string websensePass)
        {
            _websenseHost = websenseHost;
            _websenseUser = websenseUser;

            _shell = new ContentGatewayShell(_websenseHost, _websenseUser, websensePass);
            _blocklistController = new BlocklistController(_shell);
        }

        private void EnsureConnection()
        {
            if (!_shell.IsConnected)
            {
                _shell.Connect();
                Debug.WriteLine("SSH client connected to {0} using username {1}", _websenseHost, _websenseUser);
            }
        }

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

        public bool AddToBlocklist(IBlocklistable blocklistEntry, bool shouldValidateResult = false)
        {
            EnsureConnection();
            return _blocklistController.AddToBlocklist(blocklistEntry, shouldValidateResult);
        }

        public void Dispose()
        {
            if (null != _shell)
                _shell.Dispose();
        }
    }
}
