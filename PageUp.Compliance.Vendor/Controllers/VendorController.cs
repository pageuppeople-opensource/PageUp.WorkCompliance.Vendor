using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PageUp.Compliance.Vendor.Core;

namespace PageUp.Compliance.Service.Controllers
{
    [Route("compliance-vendor")]
    public class VendorController : Controller
    {
        private IHostingEnvironment _env;
        public VendorController(IHostingEnvironment env)
        {
            _env = env;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("config")]
        public IActionResult Config()
        {
            var config = new Config
            {
                ClientId = HttpContext.Session.GetString("ClientId") ??
                           Environment.GetEnvironmentVariable("ClientId") ??
                           HttpContext.Session.GetString("ClientId") ?? "",
                ClientSecret = HttpContext.Session.GetString("ClientSecret") ??
                               Environment.GetEnvironmentVariable("ClientSecret") ?? "",
                InstanceId = HttpContext.Session.GetString("InstanceId") ??
                             Environment.GetEnvironmentVariable("InstanceId") ?? "",
                DataCenter = HttpContext.Session.GetString("DataCenter") ??
                             Environment.GetEnvironmentVariable("DataCenter") ?? ""
            };
            return View(config);
        }

        [Route("GetPendingInvitations")]
        public IActionResult GetPendingInvitations()
        {
            try
            {
                var pendingInvitations = GetInvitations("pending");
                return PartialView("_Invitations", pendingInvitations);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetInvitationsByStatus")]
        public IActionResult GetInvitationsByStatus(string status)
        {
            try
            {
                var invitations = GetInvitations(status);
                return PartialView("_Invitations", invitations);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("UpdateInvitationStatus")]
        public IActionResult UpdateInvitationStatus(string requestId, string status, string notes)
        {
            var accessToken = GetToken();

            using (var client = new HttpClient())
            {
                var apiUrl = $"https://integration.{HttpContext.Session.GetString("DataCenter")}.pageuppeople.com";
                client.BaseAddress = new Uri(apiUrl);
                var endpoint = $"/compliance/requests/{requestId}/statuses";

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(accessToken.TokenType, accessToken.ProtectedTicket);

                var payload = new
                {
                    Status = status,
                    Notes = new List<string> {notes}
                };

                var response = client.PutAsync(endpoint,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response);
                    return null;
                }

                var complianceRequestResponse =
                    JsonConvert.DeserializeObject<ComplianceInvitation>(response.Content.ReadAsStringAsync().Result);

                return Json(complianceRequestResponse);
            }
        }

        [HttpPost]
        [Route("SaveConfig")]
        public IActionResult SaveConfig(Config config)
        {
            HttpContext.Session.SetString("ClientId", config.ClientId ?? "");
            HttpContext.Session.SetString("ClientSecret", config.ClientSecret ?? "");
            HttpContext.Session.SetString("DataCenter", config.DataCenter ?? "");

            return RedirectToAction("Config");
        }

        private IEnumerable<ComplianceInvitation> GetInvitations(string status)
        {
            var accessToken = GetToken();
            if (accessToken == null) return null;
            using (var client = new HttpClient())
            {
                var apiUrl = $"https://integration.{HttpContext.Session.GetString("DataCenter")}.pageuppeople.com";
                client.BaseAddress = new Uri(apiUrl);
                var endpoint = $"compliance/requests/?status={status}";

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(accessToken.TokenType, accessToken.ProtectedTicket);

                var response = client.GetAsync(endpoint).Result;

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<ComplianceInvitation>>(response.Content
                        .ReadAsStringAsync().Result);

                var exception =
                    new Exception(
                        $"Error retrieving compliance request. Status code:{response.StatusCode} with error message: {response.RequestMessage}.");
                throw exception;
            }
        }

        [Route("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        private AccessToken GetToken()
        {
            using (var client = new HttpClient())
            {
                var dataCenter = HttpContext.Session.GetString("DataCenter");
                var clientId = HttpContext.Session.GetString("ClientId");
                var clientSecret = HttpContext.Session.GetString("ClientSecret");
                var authUrl = $"https://login.{dataCenter}.pageuppeople.com";

                client.BaseAddress = new Uri(authUrl);

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("scope", "Compliance.Write Compliance.Read")
                });

                var response = client.PostAsync("/connect/token", formContent).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var exception =
                        new Exception(
                            $"Error when getting token. Status code:{response.StatusCode} with error message: {response.RequestMessage}, for client_id:{clientId}");
                    Console.WriteLine(exception);
                    return null;
                }

                var resultContent = response.Content.ReadAsStringAsync().Result;
                var resultContentJson = JsonConvert.DeserializeObject<dynamic>(resultContent);

                var accessToken = new AccessToken
                {
                    ProtectedTicket = (resultContentJson as JObject).Value<string>("access_token"),
                    ExpiresIn = (resultContentJson as JObject).Value<int>("expires_in"),
                    TokenType = (resultContentJson as JObject).Value<string>("token_type")
                };

                return accessToken;
            }
        }
    }
}