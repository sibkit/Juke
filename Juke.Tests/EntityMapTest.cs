using Juke.Mapping;

namespace Juke.Tests;

public class EntityMapTest {

                               
    [Fact]
    public void TestEntityMapper() {
        var mapper = new CompanyMapper();
        var c = new Company {
            ID = 1,
            Name = "John Doe"
        };
        Assert.Equal(mapper.ReadValue(c, mapper.Map.FieldMap("id")), c.ID);
        Assert.Equal(mapper.ReadValue(c, mapper.Map.FieldMap("name")), c.Name);
    }
}