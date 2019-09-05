using Microsoft.Data.Sqlite;
using MobileApped.Core.Persistence.Sqlite.Constants;
using MobileApped.Core.Persistence.Sqlite.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MobileApped.Core.Persistence.Sqlite
{
    public class SqliteDataContext : IDisposable
    {
        public SqliteConnection Connection { get; private set; }
        public ConcurrentDictionary<string, DatabaseAttachment> Attachements { get; private set; }

        public SqliteDataContext(string connectionString)
        {
            Connection = new SqliteConnection(connectionString);
            Attachements = new ConcurrentDictionary<string, DatabaseAttachment>();
            Connection.Open();
        }

        public async Task AttachDatabases(params DatabaseAttachment[] attachements)
        {
            var newAttachements = attachements.Where(d => !Attachements.ContainsKey(d.Alias));
            foreach (DatabaseAttachment db in newAttachements)
            {
                SqliteCommand attachCommand = Connection.CreateCommand();
                attachCommand.CommandText = $"ATTACH DATABASE '{db.FilePath}' AS {db.Alias}";
                await attachCommand.ExecuteNonQueryAsync();
                Attachements.TryAdd(db.Alias, db);
            }
        }

        public async Task DetachDatabase(string alias)
        {
            SqliteCommand detachCommand = Connection.CreateCommand();
            detachCommand.CommandText = $"DETACH DATABASE {alias}";
            await detachCommand.ExecuteNonQueryAsync();
            Attachements.TryRemove(alias, out _);
        }

        public async Task DetachAllDatabases()
        {
            foreach (DatabaseAttachment db in Attachements.Values)
            {
                SqliteCommand detachCommand = Connection.CreateCommand();
                detachCommand.CommandText = $"DETACH DATABASE {db.Alias}";
                await detachCommand.ExecuteNonQueryAsync();
                Attachements.TryRemove(db.Alias, out _);
            }
        }

        public async Task CreateTable(string tableName, params SqliteColumn[] columns)
        {
            var columnDefinitions = string.Join(",",
                columns.Select(col =>
                {
                    string key = $"{(col.IsPrimaryKey ? "PRIMARY KEY" : null)} {(col.AutoIncrement ? "AUTOINCREMENT" : null)}";
                    return $"[{col.Name}] {key} {col.DataType?.ToString()}";
                }));

            var createTableCommand = Connection.CreateCommand();
            createTableCommand.CommandText = $"CREATE TABLE IF NOT EXISTS [{tableName}] ({columnDefinitions})";
            await createTableCommand.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteNonQuery(string query)
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<List<DatabaseAttachment>> GetAttachedDatabasesAsync()
        {
            SqliteCommand query = Connection.CreateCommand();
            query.CommandText = "PRAGMA database_list";
            var reader = await query.ExecuteReaderAsync();

            List<DatabaseAttachment> objectList = new List<DatabaseAttachment>();
            while(await reader.ReadAsync())
            {
                await reader.NextResultAsync();
                objectList.Add(new DatabaseAttachment {
                    Alias = reader.GetString(1),
                    FilePath = reader.GetString(2)
                });
            }
            return objectList;
        }

        public async Task<List<object[]>> GetPragmaValue(PragmaName pragma)
        {
            string pragmaName = pragma.GetEnumMemberValue();
            SqliteCommand query = Connection.CreateCommand();
            query.CommandText = $"PRAGMA {pragmaName}";
            SqliteDataReader reader = await query.ExecuteReaderAsync();
            List<object[]> resultList = new List<object[]>();

            while (await reader.ReadAsync())
            {
                await reader.NextResultAsync();
                object[] results = new object[reader.FieldCount];
                reader.GetValues(results);
                resultList.Add(results);
            }
            return resultList;
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
