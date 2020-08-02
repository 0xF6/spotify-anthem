namespace spotify.controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Flurl.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using services;
    using SpotifyAPI.Web;

    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly FirestoreService _firestore;
        private readonly SpotifyClient _spotifyClient;

        public SpotifyController(IConfiguration config, FirestoreService firestore, SpotifyClient spotifyClient)
        {
            _config = config;
            _firestore = firestore;
            _spotifyClient = spotifyClient;
        }
        [HttpGet("/api/@me/anthem")]
        public async Task<IActionResult> GetAnthem()
        {
            var anthemRef = await _firestore.Cache.Document("anthem").GetSnapshotAsync();

            if (anthemRef.Exists)
            {
                var anthem = anthemRef.ConvertTo<Anthem>();

                if (!anthem.IsExpired())
                    return Ok(anthem);
                await _firestore.Cache.Document("anthem").DeleteAsync();
            }
            var q = await _spotifyClient.Player.GetRecentlyPlayed(new PlayerRecentlyPlayedRequest
            {
                After = GetAfterMarkerTime(),
                Limit = 50
            });

            if (q.Items is null)
                return StatusCode(500);

            var topTrack = q.Items
                .Select(x => x.Track)
                .GroupBy(x => x.Uri)
                .OrderByDescending(x => x.Count())
                .Select(x => x.First())
                .First();


            var result = Anthem.From(await _spotifyClient.Tracks.Get(topTrack.Id));

            await _firestore.Cache.Document("anthem").SetAsync(result);
                
            return Ok(result);
        }

        [HttpGet("/api/auth/@init")]
        public IActionResult GetRequestToAuth()
        {
            var request = new LoginRequest(new Uri(GetRedirectUrl()), _config["client_id"], LoginRequest.ResponseType.Code)
            {
                Scope = new List<string>
                {
                    "user-read-recently-played",
                    "user-read-private",
                    "user-read-email"
                }
            };
            return Redirect(request.ToUri().ToString());
        }
        [HttpGet("/callback")]
        public async Task<IActionResult> AuthCallback([FromQuery] string code)
        {
            var form = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"}, 
                {"code", code}, 
                {"redirect_uri", GetRedirectUrl()}
            };

            var result = await "https://accounts.spotify.com/api/token"
                    .WithBasicAuth(_config["client_id"], _config["client_secret"])
                    .PostAsync(new FormUrlEncodedContent(form))
                    .ReceiveJson<TokenData>();

            await _firestore.Cache.Document("auth").SetAsync(result);
            
            return Ok();
        }
        public string GetRedirectUrl() =>
            _config["CallbackUrl"];

        private long GetAfterMarkerTime() =>
            (long)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}
