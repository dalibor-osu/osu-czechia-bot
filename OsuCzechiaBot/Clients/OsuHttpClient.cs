using System.Net.Http.Headers;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Models;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Clients;

public class OsuHttpClient(HttpClient httpClient, ILogger<OsuHttpClient> logger, ConfigurationAccessor configurationAccessor)
{
    public async Task<TokenResponse?> GetTokenFromCode(string code)
    {
        const string url = "oauth/token";
        var request = new TokenFromCodeRequest
        {
            ClientId = configurationAccessor.Osu.ClientId,
            ClientSecret = configurationAccessor.Osu.ClientSecret,
            Code = code,
        };

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(url, JsonContent.Create(request));
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while trying to get token from osu.ppy");
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to get token from osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStreamAsync());
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
            logger.LogError("Failed to get user info from osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStreamAsync());
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