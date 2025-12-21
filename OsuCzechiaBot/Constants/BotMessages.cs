using System.Diagnostics.CodeAnalysis;

namespace OsuCzechiaBot.Constants;

public static class BotMessages
{
    public static class Events
    {
        public const string AuthTimeOut = "Too many messages in authorization channel. If you'd like to get help, contact anyone from the Mod team.";

        [StringSyntax("CompositeFormat")]
        public const string UserLeft = "User <@{0}> left.";

        [StringSyntax("CompositeFormat")]
        public const string UserLeftAndUnlinked = "User <@{0}> left. They were unlinked and removed from the database.";
    }

    public static class Commands
    {
        public static class Authorize
        {
            [StringSyntax("CompositeFormat")]
            public const string AlreadyAuthorized =
                "You are already authorized with this {0}. If you'd like to link a different account, use the */unlink* command first.";

            [StringSyntax("CompositeFormat")]
            public const string NotAMember =
                "It looks like you are not member of osu! Czechia server and thus can't perform an authorization. Please join using this invite link first: {0}";

            [StringSyntax("CompositeFormat")]
            public const string AuthorizeWithLink = "Please authorize via this link: {0}";

            [StringSyntax("CompositeFormat")]
            public const string SuccessfullyAuthorized = "Successfully authorized <@{0}> with {1}!";
        }

        public static class Unlink
        {
            public const string NotAMember =
                "It looks like you are not member of osu! Czechia server and thus can't perform an unlink. If you were verified and left the server, your profiles were automatically unlinked.";

            public const string Unlinked = "Your osu! profile has been unlinked. You will have to authorize again using the */authorize* command.";
        }
    }
}