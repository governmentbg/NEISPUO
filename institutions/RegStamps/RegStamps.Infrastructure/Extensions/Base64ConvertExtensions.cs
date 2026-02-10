namespace RegStamps.Infrastructure.Extensions
{
    public static class Base64ConvertExtensions
    {
        public static string ToBase64Convert(this int number)
            => Convert.ToBase64String(BitConverter.GetBytes(number));

        public static int FromBase64ConvertToInt(this string text)
            => BitConverter.ToInt32(Convert.FromBase64String(text));
    }
}
