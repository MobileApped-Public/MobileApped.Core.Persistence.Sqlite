using Microsoft.Data.Sqlite;
using MobileApped.Core.Persistence.Sqlite.Schema;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MobileApped.Core.Persistence.Sqlite.Tests.UnitTests
{
    public class Tests
    {
        [TestCase(100, 50)]
        [TestCase(1_000, 50)]
        [TestCase(10_000, 50)]
        [TestCase(100_000, 150)]
        [TestCase(1_000_000, 1500)]
        [TestCase(5_000_000, 7000)]
        [Description("Bulk insert works as expected with string values but defined column definitions, and in a specified amount of time")]
        public async Task BulkInsert(int numberOfRows, long maxTime)
        {
            using (SqliteBulkInsert bulkInsert = new SqliteBulkInsert(@"data source=:memory:"))
            {
                SqliteColumn[] columns = new List<SqliteColumn> {
                    new SqliteColumn { Name = "Id", DataType = SqliteType.Integer, },
                    new SqliteColumn { Name = "Name", DataType = SqliteType.Text },
                    new SqliteColumn { Name = "Age", DataType = SqliteType.Real },
                    new SqliteColumn { Name = "Town", DataType = SqliteType.Text },
                }.ToArray();

                var timer = Stopwatch.StartNew();
                int count = await bulkInsert.Insert("TestTable", columns, GenerateRows(numberOfRows));
                timer.Stop();

                Assert.AreEqual(numberOfRows, count);
                var executionTime = (long)timer.Elapsed.TotalMilliseconds;
                Assert.LessOrEqual(executionTime, maxTime);
                await TestContext.Out.WriteLineAsync($"Elapsed: {executionTime}ms");
            }
        }

        private IEnumerable<string[]> GenerateRows(int numberOfRows = 100)
        {
            for(int i = 1; i <= numberOfRows; i++)
            {
                yield return new string[4] {
                    i.ToString(),
                    $"Name",
                    i.ToString(),
                    "Town"
                };
            }
        }
    }
}