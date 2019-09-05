using Microsoft.Data.Sqlite;

namespace MobileApped.Core.Persistence.Sqlite.Schema
{
    public class SqliteColumn
    {
        public bool IsPrimaryKey { get; set; }

        public bool AutoIncrement { get; set; }

        public string Name { get; set; }

        public SqliteType? DataType { get; set; }

        public object Value { get; set; }
    }
}
