namespace RegStamps.Infrastructure.Extensions
{
    public static class IdTypeExtensions
    {
        public static string ToIdTypeFriendlyName(this int idType)
        {
            switch (idType)
            {
                case 1:
                    return "ЛНЧ";
                case 2:
                    return "ИДН";
                case 0:
                    return "ЕГН";
                default:
                    return string.Empty;
            }
        }
    }
}
