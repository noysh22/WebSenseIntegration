using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Siemplify.Integrations.WebSense;
using Siemplify.Integrations.WebSense.Data;

namespace WebSenseIntegration.EXE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This is the main exe");
            BlocklistEntry entry = new BlocklistEntry(
                BlocklistRuleType.BLOCK,
                BlocklistDestType.DOMAIN,
                "ynet.co.il");

            Console.WriteLine(entry.BuildBlocklistEntry());
        }
    }
}
