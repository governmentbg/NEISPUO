namespace MON.API.Helpers
{
    public static class StringExtensions
    {
        public static int ToInt32(this string value, int fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }
            else
            {
                int result;
                result = int.TryParse(value, out result) ? result : 0;
                return result;
            }
        }
    }
}
