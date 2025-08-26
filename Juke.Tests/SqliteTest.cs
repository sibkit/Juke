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
        mappingData.AddMapper(new ContactMapper());
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
            Name = "Amazon",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Kalashnikov",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Chrysler",
        });
        session.Insert(new Company {
            ID = seq.NextValue(),
            Name = "Audi",
        });

        // var qCompanies = new EntityQuery<Company>{
        //     Condition = new LikeCondition(
        //         new LinkField("Name"),
        //         new ValueField("%e%")
        //     )
        // };

        var qCompanies = new EntityQuery<Company> {
            SortOrders = {
                new SortOrder(new LinkField("Id"), SortOrderDirection.Desc)
            },
            Offset = 5,
            Limit = 3
        };

        // var q = new JoinQuery {
        //     Fields = {
        //       new LinkField("cmp","Name"),
        //       new LinkField("Post"),
        //       new QueryField(new GroupQuery {
        //               Source = new EntityQuery<Company>(),
        //               Fields = {
        //                   new SumField(new LinkField("Id"))
        //                   },
        //           }
        //       )
        //     },
        //     LeftSource = new EntityQuery<Company> {
        //         Alias = "cmp"
        //     },
        //     RightSource = new EntityQuery<Contact>(),
        //     Condition = new EqualCondition(
        //         new LinkField("cmp", "Id"),
        //         new LinkField("CompanyId")
        //     )
        // };
        var q = new JoinQuery {
            Fields = {
                new LinkField("cts", "Name"),
                new LinkField("cts", "Post"),
                new LinkField("cmps", "Name")
            },
            LeftSource = new EntityQuery<Contact>() {
                Alias = "cts"
            },
            RightSource = new EntityQuery<Company>() {
                Alias = "cmps"
            },
            Condition = new EqualsCondition(
                new LinkField("cts", "CompanyId"),
                new LinkField("cmps", "Id")),
            JoinType = JoinType.LeftOuter
        };
        
        var companies = session.Read(q);

        foreach (var company in companies) {
            _testOutputHelper.WriteLine($"{company[0]} - {company[1]} - {company[2]};");
        }
    }
}