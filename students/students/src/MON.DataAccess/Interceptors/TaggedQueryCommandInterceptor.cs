namespace MON.DataAccess.Interceptors
{
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public class TaggedQueryCommandInterceptor : DbCommandInterceptor
    {
        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, 
            CommandEventData eventData, 
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            ManipulateCommand(command);

            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result)
        {
            ManipulateCommand(command);

            return base.ReaderExecuted(command, eventData, result);
        }

        private static void ManipulateCommand(DbCommand command)
        {
            if (command.CommandText.StartsWith("-- Use hint: RECOMPILE", System.StringComparison.OrdinalIgnoreCase))
            {
                command.CommandText += " OPTION (RECOMPILE) ";
            }
        }
    }
}
