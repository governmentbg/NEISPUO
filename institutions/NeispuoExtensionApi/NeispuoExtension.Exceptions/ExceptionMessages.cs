namespace NeispuoExtension.Exceptions
{
    public class ExceptionMessages
    {
        public const string Base = "Error occurred";

        public static class Http
        {
            public const string BaseHttp = "Http error occurred";

            public const string Unauthorized = "The requested resource requires authentication";

            public const string Forbidden = "The server refuses to fulfill the request";

            public const string ExpectationFailed = "The server cannot meet the requirements";

            public const string Locked = "The resource that is being accessed is locked";
        }

        public static class Database
        {
            public const string BaseDatabase = "Database error occurred";

            public const string MsSq = "MS sql database error occurred";

            public const string PostgreSql = "Postgre sql database error occurred";
        }
    }
}
