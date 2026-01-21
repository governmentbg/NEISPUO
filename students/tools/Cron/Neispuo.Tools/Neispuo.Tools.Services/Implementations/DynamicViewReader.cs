using Microsoft.EntityFrameworkCore;
using Neispuo.Tools.DataAccess;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neispuo.Tools.Services.Implementations
{
    public class DynamicViewReader
    {
        private readonly NeispuoContext _context;

        public DynamicViewReader(NeispuoContext context)
        {
            _context = context;
        }

        public async Task<List<ExpandoObject>> ReadViewAsync(string viewName)
        {
            var results = new List<ExpandoObject>();

            using (var conn = _context.Database.GetDbConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM {viewName}";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            results.Add((ExpandoObject)row);
                        }
                    }
                }
            }

            return results;
        }
    }
}
