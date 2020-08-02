namespace spotify.services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Flurl.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SpotifyAPI.Web;

    public static class SpotifyServiceEx
    {
        public static IServiceCollection AddSpotify(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof (services));
            services.AddScoped<SpotifyClient>(x => Factory(x).Result);
            return services;
        }

        private static async Task<SpotifyClient> Factory(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var firestore = scope.ServiceProvider.GetRequiredService<FirestoreService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IServiceCollection>>();

            var authRef = await firestore.Cache.Document("auth").GetSnapshotAsync();

            if (!authRef.Exists)
            {
                logger.LogCritical($"Failed fetch token data from db, maybe service is not configured.");
                return null;
            }

            var token = authRef.ConvertTo<TokenData>();

            if (token.IsExpired()) 
                await refreshTokenAsync();
            return new SpotifyClient(token.access_token);


            async Task refreshTokenAsync()
            {
                var form = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"}, 
                    {"refresh_token", token.refresh_token}
                };

                var result = await "https://accounts.spotify.com/api/token"
                    .WithBasicAuth(config["client_id"], config["client_secret"])
                    .PostAsync(new FormUrlEncodedContent(form))
                    .ReceiveJson<TokenData>();

                var updateFields = new Dictionary<string, object>
                {
                    {"access_token", result.access_token}, 
                    {"expires_in", result.expires_in}
                };

                await firestore.Cache.Document("auth").UpdateAsync(updateFields);
            }
        }
    }
}