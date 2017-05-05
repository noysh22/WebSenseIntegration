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
            //Console.WriteLine("This is the main exe");
            //BlocklistEntry entry = new BlocklistEntry(
            //    BlocklistRuleType.BLOCK,
            //    BlocklistDestType.DOMAIN,
            //    "ynet.co.il");

            //var entry = new BlocklistEntry(BlocklistRuleType.BLOCK, BlocklistDestType.DOMAIN, "ynet.co.il");
            //Console.WriteLine(entry.BuildBlocklistEntry());

            WebSenseManager manager = new WebSenseManager(
                "192.168.184.136",
                "root",
                "CHANGEHERE",
                "CHANGEHERE");
            //var result = manager.AddToBlocklist(BlocklistRuleType.BLOCK, BlocklistDestType.DOMAIN, "cnn.com");

            //if (result)
            //{
            //    Console.WriteLine("Great Success");
            //}
            //else
            //{
            //    Console.WriteLine("Failed miserably");
            //}

            var urls = manager.GetAllUrlsForUser("192.168.184.137");
            if (null != urls)
            {
                urls.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("No urls found for this user");
            }

            var usersForUrl = manager.GetAllUsersForUrl("google.com");
            if (null != usersForUrl)
            {
                usersForUrl.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("No users found for this url");
            }
            
        }
    }
}
