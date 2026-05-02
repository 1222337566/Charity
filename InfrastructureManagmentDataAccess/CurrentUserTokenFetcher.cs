using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using InfrastructureManagmentDataAccess;

using Jose;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class CurrentUserTokenFetcher : ICurrentUserTokenFetcher
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserTokenFetcher(IHttpContextAccessor http) => _http = http;

    public async Task<string> GetTokenFromSelfAsync(CancellationToken ct = default)
    {
        var ctx = _http.HttpContext ?? throw new InvalidOperationException("No HttpContext");
        var baseUri = new Uri($"{ctx.Request.Scheme}://{ctx.Request.Host.Value}");

        // انسخ الكوكيز الحالية (كوكي تسجيل الدخول) للطلب الداخلي
        var cookies = new CookieContainer();
        foreach (var kv in ctx.Request.Cookies)
            cookies.Add(baseUri, new Cookie(kv.Key, kv.Value));

        using var handler = new HttpClientHandler
        {
            CookieContainer = cookies,
            UseCookies = true,
            // للسيرفر المحلي https أثناء التطوير:
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        using var client = new HttpClient(handler) { BaseAddress = baseUri };

        var res = await client.GetAsync("/auth/realtime-token", ct);
        res.EnsureSuccessStatusCode();
        var obj = await res.Content.ReadFromJsonAsync<TokenDto>(cancellationToken: ct);
        return obj?.token ?? throw new Exception("No token returned");
    }

    private record TokenDto(string token);
}