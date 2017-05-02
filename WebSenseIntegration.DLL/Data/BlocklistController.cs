using System;
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

        public bool AddToBlocklist(IBlocklistable blocklistEntry, bool shouldValidate = false)
        {
            // Validate first the entry is valid
            blocklistEntry.ValidateEntry();

            var blocklistEntryStr = blocklistEntry.BuildBlocklistEntry();
            string addToBlacklistCmd = string.Format(SshEchoCmdFormat, blocklistEntryStr);

            using (var cmd = _shell.CreateCommand(addToBlacklistCmd))
            {
                var result = cmd.Execute();
                if (null == result || !string.IsNullOrEmpty(cmd.Error))
                {
                    throw new AddToBlocklistException("Failed adding entry to blocklist");
                }
            }

            var retVal = true;
            if (shouldValidate)
            {
                retVal = IsAddedSuccessfully(blocklistEntryStr);
            }

            return retVal;
        }
    }
}
