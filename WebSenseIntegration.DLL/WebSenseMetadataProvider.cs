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
                            PropertyName = Settings.GatewayHost,
                            PropertyDisplayName = Settings.GatewayHost,
                            PropertyType = ParamTypeEnum.String
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.GatewayUserName,
                            PropertyDisplayName = Settings.GatewayUserName,
                            PropertyType = ParamTypeEnum.String
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.GatewayKey,
                            PropertyDisplayName = Settings.GatewayKey,
                            PropertyType = ParamTypeEnum.Password
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.DbHost,
                            PropertyDisplayName = Settings.DbHost,
                            PropertyType = ParamTypeEnum.String
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.DbUsername,
                            PropertyDisplayName = Settings.DbUsername,
                            PropertyType = ParamTypeEnum.String
                        },
                        new ModuleSettingsProperty
                        {
                            ModuleName = Identifier,
                            PropertyName = Settings.DbPass,
                            PropertyDisplayName = Settings.DbPass,
                            PropertyType = ParamTypeEnum.Password
                        }
                    };
                }

                return _requiredSettings;
            }
        }

        public override async Task Test(Dictionary<string, string> paramsWithValues)
        {
            var gwHost = paramsWithValues.GetOrDefault(Settings.GatewayHost);
            if (gwHost.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.GatewayHost));
            }
            var gwUsername = paramsWithValues.GetOrDefault(Settings.GatewayUserName);
            if (gwUsername.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.GatewayUserName));
            }
            var gwKey = paramsWithValues.GetOrDefault(Settings.GatewayKey);
            if (gwKey.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.GatewayKey));
            }

            var dbHost = paramsWithValues.GetOrDefault(Settings.DbHost);
            if (dbHost.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.DbHost));
            }

            var dbUser = paramsWithValues.GetOrDefault(Settings.DbUsername);
            if (dbHost.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.DbUsername));
            }

            var dbPass = paramsWithValues.GetOrDefault(Settings.DbPass);
            if (dbHost.IsEmpty())
            {
                throw new Exception(string.Format("Not found <{0}> Field.", Settings.DbPass));
            }

            var manager = new WebSenseManager(gwHost, gwUsername, gwKey, dbPass, dbHost, dbUser);
            await manager.GetAllUsersForUrlAsync("blabla.com");
        }
    }
}
