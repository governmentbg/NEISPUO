using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neispuo.Tools.Services.Implementations
{
    public class DictionaryViewReader
    {
        private readonly MONContext _context;

        public DictionaryViewReader(MONContext context)
        {
            _context = context;
        }

        public async Task<List<Dictionary<string, object>>> ReadViewAsync(string viewName, string AndOperands, string OrOperands = "{}")
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(AndOperands);

            string whereClause = string.Empty;
            if (dict != null && dict.Count > 0)
            {

                var conditions = new List<string>();
                foreach (var kv in dict)
                {
                    // Escape single quotes in values to avoid SQL injection
                    conditions.Add($"{kv.Key} = {kv.Value}");
                }

                whereClause = " WHERE " + string.Join($" And ", conditions);
            }

            var orDict = JsonSerializer.Deserialize<Dictionary<string, int>>(OrOperands);
            if (orDict != null && orDict.Count > 0)
            {

                var conditions = new List<string>();
                foreach (var kv in dict)
                {
                    // Escape single quotes in values to avoid SQL injection
                    conditions.Add($"{kv.Key} = {kv.Value}");
                }

                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause = " WHERE " + string.Join(" OR ", conditions);
                }
                else
                {
                    whereClause += $" AND ({(string.Join(" OR ", conditions))})";
                }

            }

            var results = new List<Dictionary<string, object>>();

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {viewName} {whereClause}";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>(reader.FieldCount,
                            System.StringComparer.OrdinalIgnoreCase);

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        results.Add(row);
                    }
                }

            }

            return results;
        }
    }
}
