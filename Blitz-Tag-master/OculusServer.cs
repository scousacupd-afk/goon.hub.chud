using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable

namespace Blitz_Tag
{
    public class OculusServer
    {
        private static readonly HttpClient Http = new();

        public static async Task<bool> VerifyNonceAsync(long userId, string? nonce)
        {
            if (nonce == null) return false;

            var content = new FormUrlEncodedContent(
            [
                new("access_token", Constants.AppSecret),
                new("nonce", nonce),
                new("user_id", userId.ToString())
            ]);

            HttpResponseMessage resp = await Http.PostAsync("https://graph.oculus.com/user_nonce_validate", content);
            if (!resp.IsSuccessStatusCode) return false;

            string respJson = await resp.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<NonceResponse>(respJson);

            return result?.IsValid ?? false;
        }

        public static async Task<(bool IsValid, string Json)> VerifyNonceWithJsonAsync(long userId, string? nonce)
        {
            if (nonce == null) return (false, "{}");

            var content = new FormUrlEncodedContent(
            [
                new("access_token", Constants.AppSecret),
                new("nonce", nonce),
                new("user_id", userId.ToString())
            ]);

            HttpResponseMessage resp = await Http.PostAsync("https://graph.oculus.com/user_nonce_validate", content);
            string json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode) return (false, json);

            try
            {
                var result = JsonConvert.DeserializeObject<NonceResponse>(json);
                return (result?.IsValid ?? false, json);
            }
            catch
            {
                return (false, json);
            }
        }

        public static async Task<bool> VerifyNonceAsync(string userId, string? nonce)
        {
            if (long.TryParse(userId, out long res))
                return await VerifyNonceAsync(res, nonce);
            return false;
        }

        public static async Task<(bool IsValid, string Json)> VerifyNonceWithJsonAsync(string? userId, string? nonce)
        {
            if (userId == null) return (false, "{}");
            if (long.TryParse(userId, out long res))
                return await VerifyNonceWithJsonAsync(res, nonce);
            return (false, "{}");
        }

        public static async Task<MetaUser?> GetUserAsync(long userId)
        {
            var uri = $"https://graph.oculus.com/{userId}?fields=org_scoped_id,alias,display_name&access_token={Constants.AppSecret}";
            HttpResponseMessage resp = await Http.GetAsync(uri);

            if (resp.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(await resp.Content.ReadAsStringAsync());
                return null;
            }

            string respJson = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MetaUser>(respJson);
        }

        public static async Task<SuccessResponse?> GetEntitlementAsync(long userId)
        {
            var uri = $"https://graph.oculus.com/{Constants.AppId}/verify_entitlement?user_id={userId}&access_token={Constants.AppSecret}";
            HttpResponseMessage resp = await Http.PostAsync(uri, new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()));
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(await resp.Content.ReadAsStringAsync());
                return null;
            }

            string respJson = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SuccessResponse>(respJson);
        }

        public static async Task<MetaUser?> GetUserAsync(string? userId)
        {
            if (userId == null) return null;
            if (long.TryParse(userId, out long res))
                return await GetUserAsync(res);
            return null;
        }

        public static async Task<SuccessResponse?> GetEntitlementAsync(string? userId)
        {
            if (userId == null) return null;
            if (long.TryParse(userId, out long res))
                return await GetEntitlementAsync(res);
            return null;
        }

        public static async Task<MetaAttestationClaims> VerifyAttestationTokenAsync(string attestationToken)
        {
            if (string.IsNullOrEmpty(attestationToken))
                throw new Exception("no attestation token");

            HttpResponseMessage resp = await Http.GetAsync($"https://graph.oculus.com/platform_integrity/verify?token={attestationToken}&access_token={Constants.AppSecret}");
            string respJson = await resp.Content.ReadAsStringAsync();

            var rootObj = new MetaAttestationClaims();

            dynamic respObj = JsonConvert.DeserializeObject(respJson) ?? throw new Exception("respObj is null");
            if (respObj?.data != null && respObj.data.Count > 0)
            {
                var item = respObj.data[0];
                if (item.message != "success")
                    throw new Exception(item.message.ToString());

                string? encodedClaims = item.claims?.ToString();
                if (!string.IsNullOrEmpty(encodedClaims))
                {
                    string padded = encodedClaims.Replace('-', '+').Replace('_', '/');
                    switch (padded.Length % 4)
                    {
                        case 2: padded += "=="; break;
                        case 3: padded += "="; break;
                    }

                    byte[] data = Convert.FromBase64String(padded);
                    string jsonClaims = Encoding.UTF8.GetString(data);

                    rootObj = JsonConvert.DeserializeObject<MetaAttestationClaims>(jsonClaims) ?? new MetaAttestationClaims();
                }
            }

            return rootObj;
        }

        // Response DTOs
        public class NonceResponse
        {
            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }
        }

        public class MetaUser
        {
            [JsonProperty("alias")]
            public string? Alias { get; set; }

            [JsonProperty("org_scoped_id")]
            public string? OrgScopedId { get; set; }

            [JsonProperty("id")]
            public string? UserId { get; set; }
        }

        public class SuccessResponse
        {
            [JsonProperty("success")]
            public bool? Success { get; set; }

            [JsonProperty("grant_time")]
            public long? GrantTime { get; set; }
        }

        public class MetaAttestationClaims
        {
            [JsonProperty("request_details")]
            public RequestDetails RequestDetails { get; set; } = new();

            [JsonProperty("app_state")]
            public AppState AppState { get; set; } = new();

            [JsonProperty("device_state")]
            public DeviceState DeviceState { get; set; } = new();

            [JsonProperty("device_ban")]
            public DeviceBan DeviceBan { get; set; } = new();
        }

        public class RequestDetails
        {
            [JsonProperty("exp")]
            public long Expiration { get; set; }

            [JsonProperty("nonce")]
            public string Nonce { get; set; } = "";

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }
        }

        public class AppState
        {
            [JsonProperty("app_integrity_state")]
            private string _appIntegrityState { get; set; } = "NotEvaluated";

            [JsonIgnore]
            public AppIntegrityState AppIntegrityState
            {
                get => _appIntegrityState switch
                {
                    "StoreRecognized" => AppIntegrityState.StoreRecognized,
                    "NotRecognized" => AppIntegrityState.NotRecognized,
                    "NotEvaluated" => AppIntegrityState.NotEvaluated,
                    _ => AppIntegrityState.Unknown
                };
            }

            [JsonProperty("package_cert_sha256_digest")]
            public List<string> PackageCertSha256Digest { get; set; } = new();

            [JsonProperty("package_id")]
            public string PackageId { get; set; } = "";

            [JsonProperty("version")]
            public string Version { get; set; } = "";
        }

        public class DeviceState
        {
            [JsonProperty("device_integrity_state")]
            private string _deviceIntegrityState { get; set; } = "";

            [JsonIgnore]
            public DeviceIntegrityState DeviceIntegrityState
            {
                get => _deviceIntegrityState switch
                {
                    "Advanced" => DeviceIntegrityState.Advanced,
                    "Basic" => DeviceIntegrityState.Basic,
                    "NotTrusted" => DeviceIntegrityState.NotTrusted,
                    _ => DeviceIntegrityState.Unknown
                };
            }

            [JsonProperty("unique_id")]
            public string UniqueId { get; set; } = "";
        }

        public class DeviceBan
        {
            [JsonProperty("is_banned")]
            public bool IsBanned { get; set; } = false;

            [JsonProperty("remaining_ban_time")]
            public string RemainingBanTime { get; set; } = "";
        }

        public enum DeviceIntegrityState { Advanced, Basic, NotTrusted, Unknown }
        public enum AppIntegrityState { StoreRecognized, NotRecognized, NotEvaluated, Unknown }
    }
}
