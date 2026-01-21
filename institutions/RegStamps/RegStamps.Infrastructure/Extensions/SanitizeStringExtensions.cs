namespace RegStamps.Infrastructure.Extensions
{
    using System.Text;

    public static class SanitizeStringExtensions
    {
        private readonly static HashSet<char> removeChars = new HashSet<char>("%?&^$#@!()+-,:;<>’\'-_*");

        public static string ToSanitize(this string input)
        {
            StringBuilder cleanString = new StringBuilder(input.Length);
            foreach (char ch in input)
            {
                if (!removeChars.Contains(ch))
                {
                    cleanString.Append(ch);
                }
            }

            return cleanString.ToString();
        }
    }
}
