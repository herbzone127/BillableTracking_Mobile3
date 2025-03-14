using BillableTracking_Mobile3.Models;
using BillableTracking_Mobile3.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MauiAuth
{
    public class OAuth2Authenticator
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string scope;
        private readonly Uri authorizeUrl;
        private readonly Uri redirectUrl;
        private readonly Uri accessTokenUrl;
        private string codeVerifier;
        private string codeChallenge;
        private TaskCompletionSource<WebAuthenticatorResult> _tcs;
        private readonly State requestState = new State();

        public static OAuth2Authenticator Instance { get; private set; }

        public event EventHandler<OAuthResponse> Authenticated;
        public event EventHandler<string> Error;

        public OAuth2Authenticator(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl)
        {
            this.clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            this.clientSecret = clientSecret;
            this.scope = scope ?? string.Empty;
            this.authorizeUrl = authorizeUrl ?? throw new ArgumentNullException(nameof(authorizeUrl));
            this.redirectUrl = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));
            this.accessTokenUrl = accessTokenUrl ?? throw new ArgumentNullException(nameof(accessTokenUrl));

            if (string.IsNullOrEmpty(clientSecret))
                GeneratePkceValues();

            Instance = this;
        }

        private void GeneratePkceValues()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            codeVerifier = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
            codeChallenge = Convert.ToBase64String(challengeBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public async Task<Uri> GetInitialUrlAsync()
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", Uri.EscapeDataString(clientId) },
                { "redirect_uri", Uri.EscapeDataString(redirectUrl.ToString()) },
                { "scope", Uri.EscapeDataString(scope) },
                { "response_type", "code" },
                { "state", Uri.EscapeDataString(requestState.RandomString) }
            };

            if (!string.IsNullOrEmpty(codeChallenge))
            {
                parameters["code_challenge"] = codeChallenge;
                parameters["code_challenge_method"] = "S256";
            }

            var queryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return new Uri($"{authorizeUrl}?{queryString}");
        }

        public void HandleRedirectUri(Uri uri)
        {
            if (_tcs == null || _tcs.Task.IsCompleted)
            {
                _tcs = new TaskCompletionSource<WebAuthenticatorResult>();
                Console.WriteLine("Initialized new TCS for redirect handling.");
            }

            var queryString = uri.Query.TrimStart('?');
            var properties = ParseQueryString(queryString);
            var result = new WebAuthenticatorResult
            {
                Properties = properties,
                AccessToken = properties.ContainsKey("access_token") ? properties["access_token"] : null
            };

            _tcs.TrySetResult(result);
            Task.Run(() => ProcessResult(result)); // Process asynchronously
        }

        //private async Task ProcessResult(WebAuthenticatorResult result)
        //{
        //    var query = result.Properties;

        //    if (query.ContainsKey("state") && query["state"] != requestState.RandomStringUriEscaped)
        //    {
        //        Error?.Invoke(this, "Invalid state from server. Possible forgery!");
        //        return;
        //    }

        //    if (query.ContainsKey("code"))
        //    {
        //        var code = query["code"];
        //        var tokenProperties = await RequestAccessTokenAsync(code);
        //        await OnRetrievedAccountProperties(tokenProperties);
        //    }
        //    else if (query.ContainsKey("error"))
        //    {
        //        Error?.Invoke(this, $"OAuth error: {query["error"]}");
        //    }
        //    else
        //    {
        //        Error?.Invoke(this, "Expected code in response, but did not receive one.");
        //    }
        //}

        private async Task<OAuthResponse> RequestAccessTokenAsync(string code)
        {
            var queryValues = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUrl.ToString() },
            { "client_id", clientId }
        };

            if (!string.IsNullOrEmpty(clientSecret))
                queryValues["client_secret"] = clientSecret;
            if (!string.IsNullOrEmpty(codeVerifier))
                queryValues["code_verifier"] = codeVerifier;

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var content = new FormUrlEncodedContent(queryValues);
            var response = await httpClient.PostAsync(accessTokenUrl, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse= Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthResponse>(responseString);
            return jsonResponse;
        }

        private async Task OnRetrievedAccountProperties(OAuthResponse response)
        {
            Authenticated?.Invoke(this, response);
        }

        private async Task ProcessResult(WebAuthenticatorResult result)
        {
            var query = result.Properties;

            if (query.ContainsKey("state") && query["state"] != requestState.RandomStringUriEscaped)
            {
                Error?.Invoke(this, "Invalid state from server. Possible forgery!");
                return;
            }

            if (query.ContainsKey("code"))
            {
                var code = query["code"];
                var tokenResponse = await RequestAccessTokenAsync(code);
                await OnRetrievedAccountProperties(tokenResponse);
            }
            else if (query.ContainsKey("error"))
            {
                Error?.Invoke(this, $"OAuth error: {query["error"]}");
            }
            else
            {
                Error?.Invoke(this, "Expected code in response, but did not receive one.");
            }
        }

        

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query)) return result;

            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                    result[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
            }
            return result;
        }

        public class State
        {
            public string RandomString { get; } = Guid.NewGuid().ToString("N");
            public string RandomStringUriEscaped => Uri.EscapeDataString(RandomString);
        }
    }

    public class WebAuthenticatorResult
    {
        public IDictionary<string, string> Properties { get; set; }
        public string AccessToken { get; set; }
    }
}