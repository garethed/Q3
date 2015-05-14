namespace Q3Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class HashtagParser
    {
        private static string HashtagPattern = @"\w*[\p{L}_]\w*";
        private static readonly Regex SearchRegex = new Regex(@"(?<=\B#)" + HashtagPattern + @"\b(?!#)");
        private static readonly Regex MatchRegex = new Regex("^" + HashtagPattern + "$");

        public static bool IsValidHashtagName(string hashtagName)
        {
            return MatchRegex.IsMatch(hashtagName);
        }

        public static IEnumerable<string> FindHashtags(string message)
        {
            return message == null ? Enumerable.Empty<string>() : SearchRegex.Matches(message).Cast<Match>().Select(m => m.ToString());
        }
    }
}
