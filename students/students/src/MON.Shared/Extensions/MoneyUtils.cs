using System.Globalization;

namespace MON.Shared.Extensions
{
    public static class MoneyUtils
    {
        public static string ToExtString(this decimal? price, string currency, decimal? altPrice, string altCuurrency)
        {
            return price.HasValue ? price.Value.ToExtString(currency, altPrice, altCuurrency) : "";
        }

        public static string ToExtString(this decimal price, string currency, decimal? altPrice, string altCuurrency)
        {
            return $"{price.ToString("0.00", CultureInfo.InvariantCulture)} {currency} / {altPrice?.ToString("0.00", CultureInfo.InvariantCulture)} {altCuurrency}".Trim(' ', '/');
        }

        public static string ToExtString(this decimal price)
        {
            return $"{price.ToString("0.00", CultureInfo.InvariantCulture)} лв.";
        }
    }
}
