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

    public class InvalidBlocklistEntryException : WebSenseException
    {
        public InvalidBlocklistEntryException(string message) : base(message) { }
    }

    public class InvalidBlocklistEntryDestTypeException : WebSenseException
    {
        public InvalidBlocklistEntryDestTypeException(string message) : base(message) { }
    }

    public class InvalidBlocklistEntryOptionalFlagException : WebSenseException
    {
        public InvalidBlocklistEntryOptionalFlagException(string message) : base(message) { }
    }

    public class WebSenseDbConfigurationException : WebSenseException
    {
        public WebSenseDbConfigurationException(string message) : base(message) { }
    }

    public class WebSenseRestartContentGatewayException : WebSenseException
    {
        public WebSenseRestartContentGatewayException(string message) : base(message) { }
    }

}
