using Newtonsoft.Json;
using System;
using System.IO;

namespace Squeal
{
    class SquealConfig
    {
        public string ConnectionString { get; set; }
        public AzureAdAuthentication AzureAdAuthentication { get; set; } = new AzureAdAuthentication();

        public static SquealConfig GetConfig(Squeal root)
        {
            string basePath = root.Path;
            var configPath = Path.Combine(basePath, "squeal.json");
            SquealConfig config = new SquealConfig();
            if (File.Exists(configPath))
            {
                config = JsonConvert.DeserializeObject<SquealConfig>(File.ReadAllText(configPath));
            }

            // Handle command line overrides
            config.ConnectionString = !String.IsNullOrEmpty(root.ConnectionString) 
                ? root.ConnectionString : config.ConnectionString;
            config.AzureAdAuthentication.TenantId = !String.IsNullOrEmpty(root.AzureAdTenantId)
                ? root.AzureAdTenantId : config.AzureAdAuthentication.TenantId;
            config.AzureAdAuthentication.ClientId = !String.IsNullOrEmpty(root.AzureAdClientId)
                ? root.AzureAdClientId : config.AzureAdAuthentication.ClientId;
            config.AzureAdAuthentication.ClientSecret = !String.IsNullOrEmpty(root.AzureAdClientSecret)
                ? root.AzureAdClientSecret : config.AzureAdAuthentication.ClientSecret;

            return config;
        }
    }

    class AzureAdAuthentication
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
