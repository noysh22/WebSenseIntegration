using System.Collections.Generic;

namespace Siemplify.Integrations.WebSense.Data
{
    internal static class BlocklistEntryConverter
    {
        public static Dictionary<BlocklistDestType, string> DestTypeToStr = new Dictionary<BlocklistDestType, string>
        {
            { BlocklistDestType.DOMAIN, "dest_domain"},
            { BlocklistDestType.HOSTNAME, "dest_host" },
            { BlocklistDestType.IP, "dest_ip" }
        };

        public static Dictionary<BlocklistRuleType, string> RuleTypeToStr = new Dictionary<BlocklistRuleType, string>
        {
            { BlocklistRuleType.ALLOW, "allow" },
            { BlocklistRuleType.BLOCK, "deny" }
        };

        public static Dictionary<BlocklistOptionalFlags, string> OptionalFlagToStr = new Dictionary<BlocklistOptionalFlags, string>
        {
            { BlocklistOptionalFlags.PORT, "port" },
            { BlocklistOptionalFlags.SOURCEIP, "src ip" }
        };

        public static string BlockListEntryBaseFormat = "{0}=\\\"{1}\\\"{2} action={3}";
        public static string BlockListOptionalFlagFormat = " {0}={1}";
    }
}