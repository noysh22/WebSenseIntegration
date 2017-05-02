using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Converter = Siemplify.Integrations.WebSense.Data.BlocklistEntryConverter;

namespace Siemplify.Integrations.WebSense.Data
{
    public class BlocklistEntry : IBlocklistable
    {
        public BlocklistDestType DestType { get; private set; }
        public BlocklistRuleType RuleType { get; private set; }
        public string Dest { get; private set; }
        public BlocklistOptionalFlags OptionalFlags{ get; private set; }

        private readonly string _sourceIp;
        private readonly ushort _port;

        public BlocklistEntry(
            BlocklistRuleType ruleType,
            BlocklistDestType destType,
            string dest, 
            BlocklistOptionalFlags optionalFlags = BlocklistOptionalFlags.NONE,
            string sourceIp = null,
            ushort port = 0)
        {
            RuleType = ruleType;
            DestType = destType;
            Dest = dest;
            OptionalFlags = optionalFlags;

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.SOURCEIP) && string.IsNullOrEmpty(sourceIp))
            {
                throw new InvalidBlocklistEntryException("Cannot create entry without source ip when source ip flag was given");
            }

            _sourceIp = sourceIp;

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.PORT) && 0 == port)
            {
                throw new InvalidBlocklistEntryException("Cannot create entry without port when port flag was given");
            }

            _port = port;
        }

        public string BuildBlocklistEntry()
        {
            var entryBuilder = new StringBuilder();
            var optionalFlagsBuilder = new StringBuilder();

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.SOURCEIP))
            {
                optionalFlagsBuilder.AppendFormat(Converter.BlockListOptionalFlagFormat,
                    Converter.OptionalFlagToStr[BlocklistOptionalFlags.SOURCEIP],
                    _sourceIp);
            }

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.PORT))
            {
                optionalFlagsBuilder.AppendFormat(Converter.BlockListOptionalFlagFormat,
                    Converter.OptionalFlagToStr[BlocklistOptionalFlags.PORT],
                    _port);
            }

            entryBuilder.AppendFormat(
                Converter.BlockListEntryBaseFormat,
                Converter.DestTypeToStr[DestType],
                Dest,
                optionalFlagsBuilder,
                Converter.RuleTypeToStr[RuleType]);            

            return entryBuilder.ToString();
        }

        public void ValidateEntry()
        {
            const string ipAddressRegex = @"^(?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(?=\.?\d)\.)){4}$";
            const string domainRegex = @"^(?<link>((?<prot>http:\/\/)*(?<subdomain>(www|[^\-\n]*)*)(\.)*(?<domain>[^\-\n]+)\.(?<after>[a-zA-Z]{2,3}[^>\n]*)))$";
            const string hostnameRegex = @"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*(?:\.[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*)*$";

            // Validate the the dest type value matches to the dest type enum using a regex pattern
            switch (DestType)
            {
                case BlocklistDestType.DOMAIN:
                    if (!Regex.IsMatch(Dest, domainRegex))
                    {
                        throw new InvalidBlocklistEntryDestTypeException(
                            string.Format("Domain name given: {0}, did not match the allowed domain name format", Dest));
                    }
                    break;
                case BlocklistDestType.HOSTNAME:
                    if (!Regex.IsMatch(Dest, hostnameRegex))
                    {
                        throw new InvalidBlocklistEntryDestTypeException(
                            string.Format("Hostname given: {0}, did not match the allowed hostname format", Dest));
                    }
                    break;
                case BlocklistDestType.IP:
                    if (!Regex.IsMatch(Dest, ipAddressRegex))
                    {
                        throw new InvalidBlocklistEntryDestTypeException(
                            string.Format("IP given: {0}, did not match the allowed ip addresses format", Dest));
                    }
                    break;
                default:
                    throw new InvalidBlocklistEntryException("Invalid DestType was given");
            }

            // Validate source ip to the ip regex pattern
            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.SOURCEIP))
            {
                if (!Regex.IsMatch(_sourceIp, ipAddressRegex))
                {
                    throw new InvalidBlocklistEntryOptionalFlagException(string.Format("Invalid source ip was given: {0}", _sourceIp));
                }
            }

        }
    }
}
