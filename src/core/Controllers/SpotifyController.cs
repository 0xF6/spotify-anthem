namespace core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using SpotifyAPI.Web;
    using SpotifyAPI.Web.Auth;

    [ApiController]
    [Route("[controller]")]
    public class SpotifyController : ControllerBase
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;

        public SpotifyController(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        [HttpGet("/api/@me/anthem")]
        public async Task<IActionResult> GetAnthem()
        {
            var s = new SpotifyClient(_config["token"]);

            var q = await s.Player.GetRecentlyPlayed(new PlayerRecentlyPlayedRequest
            {
                After = GetAfterMarkerTime(),
                Limit = 50
            });

            foreach (var item in q.Items)
            {
                var ctx = item.Context;
            }

            return Ok();
        }

        [HttpGet("/api/auth/@init")]
        public IActionResult GetRequestToAuth()
        {
            var request = new LoginRequest(new Uri(GetRedirectUrl()), _config["ClientID"], LoginRequest.ResponseType.Token)
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
        [HttpGet("/api/auth/@refresh")]
        public IActionResult RefreshToken([FromForm(Name = "code")]string code)
        {
            var request = new LoginRequest(new Uri(GetRedirectUrl()), _config["ClientID"], LoginRequest.ResponseType.Token)
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
        public IActionResult AuthCallback([FromQuery]string code)
        {
            return Ok();
        }



        public string GetRedirectUrl() =>
            _config["CallbackUrl"];

        private long GetAfterMarkerTime() =>
            (long)(DateTime.UtcNow.AddMinutes(-2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}
