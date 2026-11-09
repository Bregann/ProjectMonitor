using System.Security.Cryptography;
using System.Text;

namespace ProjectMonitor.Domain.Helpers
{
    public static class ProjectKeyHelper
    {
        /// <summary>A random 64-character key for a project to report with.</summary>
        public static string GenerateApiKey()
        {
            return Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));
        }

        /// <summary>"Bregan Twitch Bot" → "bregan-twitch-bot".</summary>
        public static string ToSlug(string name)
        {
            StringBuilder slug = new();

            foreach (char c in name.Trim().ToLowerInvariant())
            {
                if (char.IsAsciiLetterOrDigit(c))
                {
                    slug.Append(c);
                }
                else if (slug.Length > 0 && slug[^1] != '-')
                {
                    slug.Append('-');
                }
            }

            string result = slug.ToString().Trim('-');
            return result.Length == 0 ? "project" : result;
        }
    }
}
