using System.Reflection;
using Juke.Accessing;
using Juke.Mapping;
using Juke.Querying;
using Juke.Sqlite;
using Xunit.Abstractions;

namespace Juke.Tests;

public class SqliteTest {
    private readonly ITestOutputHelper _testOutputHelper;
    public SqliteTest(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

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
            ConnectionString = $"Data Source = {exeDir}\\sqlite.db",
            SequencesTableInfo = new SequencesTableInfo {
                NameColumn = "name",
                ValueColumn = "value",
                TableName = "sequences"
            }
        };

        var db = new Database(driver);
        var session = db.CreateSession();

        var qCompanies = new EntityQuery<Company>();
        
        var companies = session.Read<Company>(qCompanies);
        foreach (var company in companies) {
            _testOutputHelper.WriteLine(company.Name);
        }
    }
    
    [Fact]
    public void TestCommandsText() {
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
            ConnectionString = $"Data Source = {exeDir}\\sqlite.db",
            SequencesTableInfo = new SequencesTableInfo {
                NameColumn = "name",
                ValueColumn = "value",
                TableName = "sequences"
            }
        };

        var db = new Database(driver);
        var session = db.CreateSession();
        var seq = session.GetSequence<long>("PkCompany");
        
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Meta",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Sibtronic",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Ledmaster",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Google",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Chrysler",
        });

        var qCompanies = new EntityQuery<Company>{
            Condition = new LikeCondition {
                LeftField = new LinkField("Name"),
                RightField = new ValueField("BM%")
            }
        };
        var companies = session.Read<Company>(qCompanies);
        
        foreach (var company in companies) {
            _testOutputHelper.WriteLine($"{company.ID} - {company.Name};");
        }
        
        
    }
}