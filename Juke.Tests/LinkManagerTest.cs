using Juke.Querying;
using Juke.Sqlite.Sql;

namespace Juke.Tests;

public class LinkManagerTest {
    [Fact]
    void TestLinkManager() {

        var c = new Company {
            ID = 1,
            Name = "Rusal",
            Country = "Russia"
        };
        
        var mappingData = new MappingData();
        mappingData.AddMapper(new CompanyMapper());
        mappingData.AddMapper(new ContactMapper());
        
        var q = new GroupQuery {
            Source = new EntityQuery<Company>(),
            GroupFields = {
                new LinkField("name")
            },
            Fields = {
                new LinkField("name"),
                new FunctionField {
                    Type = FunctionType.Avg,
                    NestedFields = {
                        new LinkField("id"),
                    },
                    Alias = "id_avg"
                }
            }
        };
        
        var lm = new LinkManager(q, mappingData);
        var tg = lm.FindTarget(q.Fields[0] as LinkField ?? throw new InvalidOperationException());


        var fCmpId = new LinkField("cmp", "Id");

        var q2 = new EntityQuery<Company> {
            Fields = {
                new LinkField("Name"),
                new QueryField {
                    Query = new GroupQuery {
                        Fields = {
                            new FunctionField {
                                Type = FunctionType.Sum,
                                NestedFields = { new LinkField("Id") }
                            }
                        },
                        Source = new EntityQuery<Contact>(),
                        GroupFields = {
                            new LinkField("CompanyId") 
                        },
                        Condition = new EqualCondition(
                            fCmpId,
                            new LinkField("CompanyId")
                        )
                    }
                }
            },
            Alias = "cmp",
            Limit = 50
        };
        
        lm = new LinkManager(q2, mappingData);
        var tg2 = lm.FindTarget(fCmpId);
        
        Assert.True(typeof(EntityTarget) == tg2.GetType());
    }
}