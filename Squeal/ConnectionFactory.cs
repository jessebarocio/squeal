using Azure.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Squeal
{
    static class ConnectionFactory
    {
        public static IDbConnection CreateConnection(SquealConfig config)
        {
            string accessToken = null;
            if(!String.IsNullOrEmpty(config.AzureAdAuthentication.TenantId)
                && !String.IsNullOrEmpty(config.AzureAdAuthentication.ClientId))
            {
                var credential = new ClientSecretCredential(config.AzureAdAuthentication.TenantId,
                    config.AzureAdAuthentication.ClientId, config.AzureAdAuthentication.ClientSecret);

                var token = credential.GetToken(new Azure.Core.TokenRequestContext(
                    new string[] { "https://database.windows.net/.default" }));

                accessToken = token.Token;
            }

            var connection = new SqlConnection(config.ConnectionString);
            connection.AccessToken = accessToken;
            return connection;
        }
    }
}
