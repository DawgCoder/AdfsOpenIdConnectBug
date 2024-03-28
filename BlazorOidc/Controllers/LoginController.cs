using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace BlazorOidc.Controllers
{
    public class LoginController : ControllerBase
    {
        [HttpGet("account/login")]
        [IgnoreAntiforgeryToken]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "https://localhost:7100/default" });
        }
    }
}