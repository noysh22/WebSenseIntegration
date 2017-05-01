using System;
using System.Linq;
using System.Net;
using System.Text;
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

        private string _sourceIp;
        private ushort _port;

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
                throw new InvalidBlockListEntryException("Cannot create entry without source ip when source ip flag was given");
            }

            _sourceIp = sourceIp;

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.PORT) && 0 == port)
            {
                throw new InvalidBlockListEntryException("Cannot create entry without port when port flag was given");
            }

            _port = port;
        }

        public string BuildBlocklistEntry()
        {
            var entryBuilder = new StringBuilder();

            entryBuilder.AppendFormat(
                Converter.BlockListEntryBaseFormat,
                Converter.RuleTypeToStr[RuleType],
                Converter.DestTypeToStr[DestType],
                Dest);

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.SOURCEIP))
            {
                entryBuilder.AppendFormat(Converter.BlockListOptionalFlagFormat,
                    Converter.OptionalFlagToStr[BlocklistOptionalFlags.SOURCEIP],
                    _sourceIp);
            }

            if (OptionalFlags.HasFlag(BlocklistOptionalFlags.PORT))
            {
                entryBuilder.AppendFormat(Converter.BlockListOptionalFlagFormat,
                    Converter.OptionalFlagToStr[BlocklistOptionalFlags.PORT],
                    _port);
            }

            return entryBuilder.ToString();
        }

        public void ValidateEntry()
        {
            throw new NotImplementedException();
        }
    }
}
