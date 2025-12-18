using System.Diagnostics;
using OsuCzechiaBot.Enums;
using OsuCzechiaBot.Helpers;
using OsuCzechiaBot.Models.OsuApi;

namespace OsuCzechiaBot.Extensions;

public static class OsuUserExtensions
{
    public static ulong GetMainGlobalRank(this OsuUserExtendedWithOptionalData userData)
    {
        var mainRuleset = userData.GetMainRuleset();
        return (mainRuleset switch
        {
            OsuRuleset.Osu => userData.RulesetsStatistics?.Osu?.GlobalRank,
            OsuRuleset.Taiko => userData.RulesetsStatistics?.Taiko?.GlobalRank,
            OsuRuleset.Fruits => userData.RulesetsStatistics?.Fruits?.GlobalRank,
            OsuRuleset.Mania => userData.RulesetsStatistics?.Mania?.GlobalRank,
            _ => throw new UnreachableException("Unknown ruleset")
        }) ?? 0;
    }

    public static ulong GetMainCountryGlobalRank(this OsuUserExtendedWithOptionalData userData)
    {
        return userData.MainRulesetStatistics?.CountryRank ?? 0;
    }

    public static OsuRuleset GetMainRuleset(this OsuUserExtendedWithOptionalData userData) =>
        EnumHelper.ParseOrDefault(userData.Playmode, OsuRuleset.Osu);
    
    public static bool IsCzech(this OsuUserExtendedWithOptionalData userData) => "CZ".Equals(userData.CountryCode, StringComparison.OrdinalIgnoreCase);
}