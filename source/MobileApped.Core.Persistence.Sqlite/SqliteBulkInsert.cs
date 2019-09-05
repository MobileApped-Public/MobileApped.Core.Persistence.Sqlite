using Microsoft.Data.Sqlite;
using MobileApped.Core.Persistence.Sqlite.Schema;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApped.Core.Persistence.Sqlite
{
    public class SqliteBulkInsert : IDisposable
    {
        private readonly SqliteDataContext context;
        public SqliteBulkInsert(string connectionString)
        {
            context = new SqliteDataContext(connectionString);
        }

        public async Task<int> Insert(string tableName, SqliteColumn[] columns, IEnumerable<string[]> rows)
        {
            await context.ExecuteNonQuery("PRAGMA journal_mode = MEMORY;");
            await context.ExecuteNonQuery("PRAGMA synchronous = OFF;");
            await context.ExecuteNonQuery("PRAGMA foreign_keys = false;");

            await context.CreateTable(tableName, columns);
            var columnDefinitions = string.Join(",", columns.Select(col => $"[{col.Name}]"));
            var paramPlaceholders = columns.Select((col, i) => $"?");
            int insertCount = 0;
            using (DbTransaction transaction = context.Connection.BeginTransaction())
            {
                sqlite3_stmt statement;
                string insertStatement = $"INSERT INTO {tableName} ({columnDefinitions}) VALUES ({paramPlaceholders});";
                raw.sqlite3_prepare_v2(context.Connection.Handle, insertStatement, out statement);
                foreach (var row in rows)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        int columnIndex = i + 1;
                        string value = row[i];
                        if (value == null)
                        {
                            raw.sqlite3_bind_null(statement, columnIndex);
                            continue;
                        }

                        raw.sqlite3_bind_text(statement, columnIndex, value);
                    }
                    int step = raw.sqlite3_step(statement);
                    raw.sqlite3_reset(statement);
                    insertCount++;
                }
                statement.Dispose();
                transaction.Commit();
            }
            return insertCount;
        }

        public void Dispose()
        {
            try
            {
                context.Dispose();
            }
            catch {

            }
        }
    }
}
