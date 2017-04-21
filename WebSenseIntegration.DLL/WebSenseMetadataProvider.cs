using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Siemplify.Common;
using Siemplify.Common.Extensions;

namespace Siemplify.Integrations.WebSense
{
    public class WebSenseMetadataProvider : MetadataProviderBase
    {
        public const string ProviderIdentifier = "WebSense";
        private List<ModuleSettingsProperty> _requiredSettings = null;

        public WebSenseMetadataProvider()
        {
            ProviderIcon = "Siemplify.Integrations.WebSense.Resources.WebSense.jpg";
        }

        public override Stream IconStream
        {
            get
            {
                var data = Convert.FromBase64String(IconBase64);
                var ms = new MemoryStream(data);
                return ms;
            }
        }

        public override string Identifier
        {
            get { return ProviderIdentifier; }
        }

        public override string DisplayName
        {
            get { return ProviderIdentifier; }
        }

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("https://www.forcepoint.com/");
                sb.AppendLine(
                    @"PROTECTING THE HUMAN POINT Where critical data and IP are most valuable — and most vulnerable");
                return sb.ToString();
            }
        }

        public override List<ModuleSettingsProperty> RequiredSettings
        {
            get
            {
                if (_requiredSettings == null)
                {
                    _requiredSettings = new List<ModuleSettingsProperty>
                    {
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.ApiUsername,
                            PropertyDisplayName = Settings.ApiUsername,
                            PropertyType = ParamTypeEnum.String,
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.ApiKey,
                            PropertyDisplayName = Settings.ApiKey,
                            PropertyType = ParamTypeEnum.Password,
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.ApiHost,
                            PropertyDisplayName = Settings.ApiHost,
                            PropertyType = ParamTypeEnum.URL,
                            Value = WebSenseManager.DefaultApiHost
                        }
                    };
                }

                return _requiredSettings;
            }
        }

        public override Task Test(Dictionary<string, string> paramsWithValues)
        {
            var apiUsername = paramsWithValues.GetOrDefault(Settings.ApiUsername);
            if (apiUsername.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.ApiUsername));
            }
            var apiKey = paramsWithValues.GetOrDefault(Settings.ApiKey);
            if (apiKey.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.ApiKey));
            }
            var apiHost = paramsWithValues.GetOrDefault(Settings.ApiHost);
            if (apiHost.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.ApiHost));
            }

            var manager = new WebSenseManager(apiUsername, apiKey, apiHost);
            //await manager.GetDomainProfile("https://google.com").ConfigureAwait(false);
            throw new NotImplementedException();
        }
    }
}
