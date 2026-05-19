using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

public static class Requests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<TResponse?> Post<TResponse>(
        string url,
        object? json = null,
        Dictionary<string, object>? arguments = null,
        Dictionary<string, object>? headers = null)
    {
        using var client = new HttpClient();

        var finalUrl = BuildUrl(url, arguments);
        using var request = new HttpRequestMessage(HttpMethod.Post, finalUrl);

        ApplyHeaders(request, headers);

        if (json != null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(json, JsonOptions),
                Encoding.UTF8,
                "application/json"
            );
        }

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        // 👇 this is the magic
        if (typeof(TResponse) == typeof(string))
            return (TResponse)(object)content;

        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
    }

    public static async Task<TResponse?> Get<TResponse>(
        string url,
        Dictionary<string, object>? arguments = null,
        Dictionary<string, object>? headers = null)
    {
        using var client = new HttpClient();

        var finalUrl = BuildUrl(url, arguments);
        using var request = new HttpRequestMessage(HttpMethod.Get, finalUrl);

        ApplyHeaders(request, headers);

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        if (typeof(TResponse) == typeof(string))
            return (TResponse)(object)content;

        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
    }

    private static string BuildUrl(string url, Dictionary<string, object>? arguments)
    {
        if (arguments == null || arguments.Count == 0)
            return url;

        var builder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(builder.Query);

        foreach (var (key, value) in arguments)
            query[key] = value?.ToString();

        builder.Query = query.ToString()!;
        return builder.ToString();
    }

    private static void ApplyHeaders(HttpRequestMessage request, Dictionary<string, object>? headers)
    {
        if (headers == null) return;

        foreach (var (key, value) in headers)
            request.Headers.TryAddWithoutValidation(key, value?.ToString());
    }
}
