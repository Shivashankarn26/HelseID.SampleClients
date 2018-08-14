using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Clients;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MvcCoreHybrid.Models;
using Newtonsoft.Json.Linq;

namespace MvcCoreHybrid.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetStringAsync(Constants.SampleApi + "values");
            ViewBag.Json = JArray.Parse(response).ToString();

            return View();
        }

        public async Task<IActionResult> RenewTokens()
        {
            var disco = await DiscoveryClient.GetAsync(Constants.Authority);
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = new TokenClient(disco.TokenEndpoint, "mvc.core.hybrid", "secret");
            var rt = await HttpContext.GetTokenAsync("refresh_token");
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);

            if (!tokenResult.IsError)
            {
                var old_id_token = await HttpContext.GetTokenAsync("id_token");
                var new_access_token = tokenResult.AccessToken;
                var new_refresh_token = tokenResult.RefreshToken;

                var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token }
                };

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

                var info = await HttpContext.AuthenticateAsync("Cookies");
                info.Properties.StoreTokens(tokens);
                await HttpContext.SignInAsync("Cookies", info.Principal, info.Properties);

                return Redirect("~/Home/Secure");
            }

            ViewData["Error"] = tokenResult.Error;
            return View("Error");
        }

        public IActionResult Logout()
        {
            return new SignOutResult(new[] { "Cookies", "oidc" });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
