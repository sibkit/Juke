using Juke.Linking;
using Juke.Querying;

namespace Juke.Tests.Orm;

public class LinkManagerTest {
    [Fact]
    void Test1() {

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
                new AvgField(new LinkField("id")){
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
                new QueryField( new GroupQuery {
                        Fields = {
                            new SumField(new LinkField("Id"))
                        },
                        Source = new EntityQuery<Contact>(),
                        GroupFields = {
                            new LinkField("CompanyId") 
                        },
                        Condition = new EqualsCondition(
                            fCmpId,
                            new LinkField("CompanyId")
                        )
                    }
                )
            },
            Alias = "cmp",
            Limit = 50
        };
        
        lm = new LinkManager(q2, mappingData);
        var tg2 = lm.FindTarget(fCmpId);
        
        Assert.True(typeof(EntityTarget) == tg2.GetType());
    }
}