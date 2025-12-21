using System.Net;
using System.Net.Http.Headers;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Helpers.Optionals;
using OsuCzechiaBot.Models;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Clients;

public class OsuHttpClient(HttpClient httpClient, ILogger<OsuHttpClient> logger, ConfigurationAccessor configurationAccessor)
{
    public async Task<Optional<TokenResponse>> GetTokenFromCode(string code)
    {
        var request = new TokenFromCodeRequest
        {
            ClientId = configurationAccessor.Osu.ClientId,
            ClientSecret = configurationAccessor.Osu.ClientSecret,
            Code = code,
        };

        return await GetTokenAsync(JsonContent.Create(request));
    }

    public async Task<Optional<TokenResponse>> GetTokenFromRefreshToken(string refreshToken)
    {
        var request = new TokenFromRefreshTokenRequest
        {
            ClientId = configurationAccessor.Osu.ClientId,
            ClientSecret = configurationAccessor.Osu.ClientSecret,
            RefreshToken = refreshToken,
        };

        return await GetTokenAsync(JsonContent.Create(request));
    }

    private async Task<Optional<TokenResponse>> GetTokenAsync(JsonContent content)
    {
        const string url = "oauth/token";
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(url, content);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while trying to get token from osu.ppy");
            return new Error { ErrorType = ErrorType.ServiceError, Message = e.Message };
        }


        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                logger.LogError("Failed authorize user in osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode, body);
                return new Error { ErrorType = ErrorType.Forbidden, Message = "Failed authorize user in osu.ppy" };
            }

            logger.LogError("Failed to get token from osu.ppy. Status code: {StatusCode}, Body: {Body}", response.StatusCode, body);
            return new Error { ErrorType = ErrorType.ServiceError, Message = "Failed authorize user in osu.ppy" };
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return result is not null ? result : new Error { ErrorType = ErrorType.ServiceError, Message = "Failed to serialize token response" };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to read token response");
            return new Error { ErrorType = ErrorType.ServiceError, Message = "Failed to serialize token response" };
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
            logger.LogError(e, "Failed to get user info from osu.ppy for DiscordUserId: {DiscordId}, OsuUserId: {OsuUserId}", user.Id,
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