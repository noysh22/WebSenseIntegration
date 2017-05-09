using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Siemplify.Integrations.WebSense.SSH;

namespace Siemplify.Integrations.WebSense.Data
{
    public class BlocklistController
    {
        private static readonly string BlocklistFilePath = "/opt/WCG/config/filter.config";
        private static readonly string SshEchoCmdFormat = "echo \"{0}\" >> " + BlocklistFilePath;
        
        private readonly ContentGatewayShell _shell;

        public BlocklistController(ContentGatewayShell shell)
        {
            _shell = shell;
        }

        private bool IsAddedSuccessfully(string addedEntry)
        {
            var grepCommand = string.Format("grep \"{0}\" {1}", addedEntry, BlocklistFilePath);

            var result = _shell.RunCommand(grepCommand);

            return !string.IsNullOrEmpty(result.Result);
        }

        private bool RestartContentGateway()
        {
            const string restartCommand = ". /opt/WCG/WCGAdmin restart";
            const string restartSuccessIndicator1 = "Starting Content Gateway...";
            const string restartSuccessIndicator2 = "Started Content Gateway";

            // restarting the content gateway, this commands takes a few seconds, this line will block
            // until restart was successful.
            var restartResult = _shell.RunCommand(restartCommand);

            return restartResult.Result.Contains(restartSuccessIndicator1) &&
                   restartResult.Result.Contains(restartSuccessIndicator2);
        }

        public bool AddToBlocklist(IBlocklistable blocklistEntry, bool shouldValidate = false)
        {
            // Validate first the entry is valid
            Debug.WriteLine("Validating blocklist entry data");
            blocklistEntry.ValidateEntry();

            var blocklistEntryStr = blocklistEntry.BuildBlocklistEntry();
            string addToBlacklistCmd = string.Format(SshEchoCmdFormat, blocklistEntryStr);

            Debug.WriteLine("Adding entry {0} to {1} file in the content gateway", blocklistEntryStr, BlocklistFilePath);
            using (var cmd = _shell.CreateCommand(addToBlacklistCmd))
            {
                Debug.WriteLine("Executing bash command: {0} to host {1}", addToBlacklistCmd, _shell.ConnectionInfo.Host);
                var result = cmd.Execute();
                if (null == result || !string.IsNullOrEmpty(cmd.Error))
                {
                    throw new AddToBlocklistException("Failed adding entry to blocklist");
                }
            }

            var retVal = true;
            if (shouldValidate)
            {
                Debug.WriteLine("Validating entry was successfully added to file");
                retVal = IsAddedSuccessfully(blocklistEntryStr);
            }

            Debug.WriteLine("Restarting content gateway...");
            if (!RestartContentGateway())
            {
                throw new WebSenseRestartContentGatewayException("Failed restarting content gateway");
            }
            Debug.WriteLine("Content gateway restarted");

            return retVal;
        }
    }
}
