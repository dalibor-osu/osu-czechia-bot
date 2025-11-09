using System.Net.Http.Headers;
using Newtonsoft.Json;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Models;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Clients;

public class OsuHttpClient(HttpClient httpClient, ILogger<OsuHttpClient> logger, ConfigurationAccessor configurationAccessor)
{
    public async Task<TokenResponse?> GetTokenFromCode(string code)
    {
        var request = new TokenFromCodeRequest
        {
            ClientId = configurationAccessor.Osu.ClientId,
            ClientSecret = configurationAccessor.Osu.ClientSecret,
            Code = code,
        };

        return await GetTokenAsync(JsonContent.Create(request));
    }

    public async Task<TokenResponse?> GetTokenFromRefreshToken(string refreshToken)
    {
        var request = new TokenFromRefreshTokenRequest
        {
            ClientId = configurationAccessor.Osu.ClientId,
            ClientSecret = configurationAccessor.Osu.ClientSecret,
            RefreshToken = refreshToken,
        };

        return await GetTokenAsync(JsonContent.Create(request));
    }

    private async Task<TokenResponse?> GetTokenAsync(JsonContent content) {
        const string url = "oauth/token";
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(url, content);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while trying to get token from osu.ppy");
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to get token from osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode, body);
            return null;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to read token response");
            return null;
        }
    }

    public async Task<OsuUserExtendedWithOptionalData?> GetUserData(AuthorizedUser user)
    {
        const string url = "api/v2/me/osu";
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(url);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get user info from osu.ppy for DiscordUserId: {DiscordId}, OsuUserId: {OsuUserId}", user.DiscordId,
                user.OsuId);
            return null;
        }
        finally
        {
            httpClient.DefaultRequestHeaders.Clear();
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to get user info from osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode,
                await response.Content.ReadAsStreamAsync());
            return null;
        }


        try
        {
            return await response.Content.ReadFromJsonAsync<OsuUserExtendedWithOptionalData>();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to read token response");
            return null;
        }
    }
}