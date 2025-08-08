using Juke.Mapping;

namespace Juke.Tests;

public class CompanyMapper : EntityMapper<Company> {
    public override object? ReadValue(Company entity, FieldMap fieldMap) {
        return fieldMap.FieldName switch {
            "id" => entity.ID,
            "name" => entity.Name,
            _ => throw new NotImplementedException()
        };
    }
    public override void WriteValue(Company entity, FieldMap fieldMap, object? value) {
        switch (fieldMap.FieldName) {
            case "id": entity.ID = (uint?)value; break;
            case "name": entity.Name = (string?)value; break;
            default: throw new NotImplementedException();
        }
    }
    protected override EntityMap CreateMap() {
        return new EntityMap {
            EntityName = "Company",
            DbTableName = "companies",
            FieldMaps = [
                new FieldMap {
                    FieldName = "id",
                    DbColumnName = "id",
                    IsKeyField = true,
                    IsRequiredField = true,
                    FieldValueType = typeof(uint?),
                    DbValueType = typeof(uint),
                },
                new FieldMap {
                    FieldName = "name",
                    DbColumnName = "name",
                    IsKeyField = false,
                    IsRequiredField = true,
                    FieldValueType = typeof(string),
                    DbValueType = typeof(string),
                }
            ]
        };
    }
}