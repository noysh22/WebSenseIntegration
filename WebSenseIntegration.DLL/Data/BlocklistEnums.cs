using System;

namespace Siemplify.Integrations.WebSense.Data
{
    public enum BlocklistDestType
    {
        DOMAIN = 0,
        HOSTNAME,
        IP
    }

    public enum BlocklistRuleType
    {
        BLOCK = 0,
        ALLOW
    }

    [Flags]
    public enum BlocklistOptionalFlags
    {
        NONE = 0x0,
        SOURCEIP = 0x1,
        PORT = 0x2,
        IPANDPORT = SOURCEIP | PORT
    }
}
