using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BillableTracking_Mobile3.Services
{
    public interface IAuthService
    {
        string GetAuthorizeUrl();
        Task<(string AccessToken, string RefreshToken)> ExchangeCodeForTokenAsync(string code, string codeVerifier);
    }

    public class AuthService : IAuthService
    {
        private readonly string _clientId = "BillableTracking.Mobile";
        private readonly string _authority = "https://billabletracking.azurewebsites.net";
        private readonly string _redirectUri = "https://billabletracking.azurewebsites.net/callback"; // WebView-friendly redirect
        private readonly string[] _scopes = new[] { "openid", "profile", "email", "api://IdentityServerSPA/access_as_user", "offline_access" };

        public string GetAuthorizeUrl()
        {
            var codeVerifier = Guid.NewGuid().ToString("N");
            var codeChallenge = Base64UrlEncode(ComputeSha256Hash(codeVerifier));
            SecureStorage.SetAsync("code_verifier", codeVerifier).Wait(); // Store for later use

            return $"{_authority}/connect/authorize?" +
                   $"client_id={_clientId}&" +
                   $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
                   $"response_type=code&" +
                   $"scope={Uri.EscapeDataString(string.Join(" ", _scopes))}&" +
                   $"code_challenge={codeChallenge}&" +
                   $"code_challenge_method=S256";
        }

        public async Task<(string AccessToken, string RefreshToken)> ExchangeCodeForTokenAsync(string code, string codeVerifier)
        {
            var client = new HttpClient();
            var response = await client.PostAsync($"{_authority}/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _redirectUri },
                { "client_id", _clientId },
                { "code_verifier", codeVerifier }
            }));

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return (tokenResponse["access_token"], tokenResponse.GetValueOrDefault("refresh_token"));
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        private static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
