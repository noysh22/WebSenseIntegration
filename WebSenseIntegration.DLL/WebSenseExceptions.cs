using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemplify.Integrations.WebSense
{
    public class WebSenseException : Exception
    {
        public WebSenseException() { }
        public WebSenseException(string message) : base(message) { }
        public WebSenseException(string message, Exception innerEx) : base(message, innerEx) { }
    }

    public class AddToBlocklistException : WebSenseException
    {
        public AddToBlocklistException(string message) : base(message) { }
    }

    public class InvalidBlockListEntryException : WebSenseException
    {
        public InvalidBlockListEntryException(string message) : base(message) { }
    }
}
