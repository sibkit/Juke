using System.Reflection;
using Juke.Mapping;
using Juke.Sqlite;

namespace Juke.Tests;

public class SqliteTest {
    [Fact]
    public void TestConnection() {
        var mappingData = new MappingData();
        mappingData.AddMapper(new CompanyMapper());
        mappingData.RegisterSequence(new SequenceMap {
            SequenceName = "PkCompany",
            DbSequenceName = "pk_company",
            SequenceValueType = typeof(long)
        });
        
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        var driver = new SqliteDriver {
            MappingData = mappingData,
            ConnectionString = $"{exeDir}\\sqlite.db",
            SequencesTableInfo = new SequencesTableInfo {
                NameColumn = "name",
                ValueColumn = "value",
                TableName = "sequences"
            }
        };

        var ctx = new Database();

    }
}