using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Blitz_Tag
{
    public static class JsonWebToken
    {
        private static readonly JwtSecurityTokenHandler TokenHandler = new();

        private static readonly byte[] PrivateKeyBytes = Convert.FromBase64String("MHcCAQEEIHkwTi1kGwwKzJlsBw1REjoi0WigrCMci3AwiURWXqhOoAoGCCqGSM49AwEHoUQDQgAEsghvEGj5qTa36FB1bUWIWegGc9ZHJe5HzuRe2ly39I7BNOarTFyNs2OM4yA0mIhaQSEVvoTgvYt9j7azYQoPxg==");

        private static readonly byte[] PublicKeyBytes = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEsghvEGj5qTa36FB1bUWIWegGc9ZHJe5HzuRe2ly39I7BNOarTFyNs2OM4yA0mIhaQSEVvoTgvYt9j7azYQoPxg==");

        private static readonly ECDsa PrivateEcdsa;
        private static readonly ECDsa PublicEcdsa;

        static JsonWebToken()
        {
            PrivateEcdsa = ECDsa.Create();
            PrivateEcdsa.ImportECPrivateKey(PrivateKeyBytes, out _);

            PublicEcdsa = ECDsa.Create();
            PublicEcdsa.ImportSubjectPublicKeyInfo(PublicKeyBytes, out _);
        }

        public static string Generate(Dictionary<string, object?> claims, TimeSpan expiresIn)
        {
            ECDsaSecurityKey key = new(PrivateEcdsa);
            SigningCredentials creds = new(key, SecurityAlgorithms.EcdsaSha256);

            List<Claim> jwtClaims = [];

            foreach (KeyValuePair<string, object?> pair in claims)
            {
                switch (pair.Value)
                {
                    case null:
                        jwtClaims.Add(new Claim(pair.Key, "null", JsonClaimValueTypes.Json));
                        break;

                    case string s:
                        jwtClaims.Add(new Claim(pair.Key, s));
                        break;

                    case int i:
                        jwtClaims.Add(new Claim(pair.Key, i.ToString(), ClaimValueTypes.Integer));
                        break;

                    case long l:
                        jwtClaims.Add(new Claim(pair.Key, l.ToString(), ClaimValueTypes.Integer64));
                        break;

                    case bool b:
                        jwtClaims.Add(new Claim(pair.Key, b.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean));
                        break;

                    case float:
                    case double:
                    case decimal:
                        jwtClaims.Add(new Claim(pair.Key, Convert.ToString(pair.Value, System.Globalization.CultureInfo.InvariantCulture)!, ClaimValueTypes.Double));
                        break;

                    default:
                        jwtClaims.Add(new Claim(pair.Key, JsonConvert.SerializeObject(pair.Value), JsonClaimValueTypes.Json));
                        break;
                }
            }

            JwtSecurityToken token = new(
                claims: jwtClaims,
                expires: DateTime.UtcNow.Add(expiresIn),
                signingCredentials: creds
            );

            return TokenHandler.WriteToken(token);
        }

        public static bool Verify(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            ECDsaSecurityKey key = new(PublicEcdsa);

            TokenValidationParameters parameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256]
            };

            try
            {
                TokenHandler.ValidateToken(token, parameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("JWT verification failed: " + ex.Message);
                return false;
            }
        }

        public static bool Verify(string? token, out string? mothershipId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                mothershipId = null;
                return false; 
            }

            ECDsaSecurityKey key = new(PublicEcdsa);

            TokenValidationParameters parameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256]
            };

            try
            {
                TokenHandler.ValidateToken(token, parameters, out _);

                JwtSecurityToken jwt = TokenHandler.ReadJwtToken(token);
                object mothershipIdObj = new Dictionary<string, object>(jwt.Payload)["sub"];
                if (mothershipIdObj is not string)
                {
                    mothershipId = null;
                }
                mothershipId = (string?)new Dictionary<string, object>(jwt.Payload)["sub"];

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("JWT verification failed: " + ex.Message);
                mothershipId = null;
                return false;
            }
        }
    }
}