namespace RegStamps.Infrastructure.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] ToByteArray(this string text)
            => Convert.FromBase64String(text);

        public static string ToBase64String(this byte[] array)
            => Convert.ToBase64String(array);
    }
}
