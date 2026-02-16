using Juke.Mapping;

namespace Juke.Tests;

public class CompanyMapper : EntityMapper<Company> {
    public override object? ReadValue(Company entity, FieldMap fieldMap) {
        return fieldMap.FieldName switch {
            "Id" => entity.ID,
            "Name" => entity.Name,
            "Country" => entity.Country,
            _ => throw new NotImplementedException()
        };
    }
    public override void WriteValue(Company entity, FieldMap fieldMap, object? value) {
        switch (fieldMap.FieldName) {
            case "Id": entity.ID = value is DBNull? null : (long?)value; break;
            case "Name": entity.Name = value is DBNull? null : (string?)value; break;
            case "Country": entity.Country = value is DBNull? null : (string?)value; break;
            default: throw new NotImplementedException();
        }
    }
    protected override EntityMap CreateMap() {
        return new EntityMap {
            EntityName = "Company",
            DbTableName = "companies",
            FieldMaps = [
                new FieldMap {
                    FieldName = "Id",
                    DbColumnName = "id",
                    IsKeyField = true,
                    IsRequiredField = true,
                    FieldValueType = typeof(uint?),
                    DbValueType = typeof(uint),
                },
                new FieldMap {
                    FieldName = "Name",
                    DbColumnName = "name",
                    IsKeyField = false,
                    IsRequiredField = true,
                    FieldValueType = typeof(string),
                    DbValueType = typeof(string),
                },
                new FieldMap {
                    FieldName = "Country",
                    DbColumnName = "country",
                    IsKeyField = false,
                    IsRequiredField = true,
                    FieldValueType = typeof(string),
                    DbValueType = typeof(string),
                }
            ]
        };
    }
}

public class ContactMapper : EntityMapper<Contact> {
    public override object? ReadValue(Contact entity, FieldMap fieldMap) {
        return fieldMap.FieldName switch {
            "Id" => entity.ID,
            "Name" => entity.Name,
            "Post" => entity.Post,
            "CompanyId" => entity.CompanyID,
            _ => throw new NotImplementedException()
        };
    }
    public override void WriteValue(Contact entity, FieldMap fieldMap, object? value) {
        switch (fieldMap.FieldName) {
            case "Id": entity.ID = (long?)value; break;
            case "Name": entity.Name = (string?)value; break;
            case "Post": entity.Post = (string?)value; break;
            case "CompanyId": entity.CompanyID = (long?)value; break;
            default: throw new NotImplementedException();
        }
    }
    protected override EntityMap CreateMap() {
        return new EntityMap {
            EntityName = "Contact",
            DbTableName = "contacts",
            FieldMaps = [
                new FieldMap {
                    FieldName = "Id",
                    DbColumnName = "id",
                    IsKeyField = true,
                    IsRequiredField = true,
                    FieldValueType = typeof(long?),
                    DbValueType = typeof(long),
                },
                new FieldMap {
                    FieldName = "Name",
                    DbColumnName = "name",
                    IsKeyField = false,
                    IsRequiredField = true,
                    FieldValueType = typeof(string),
                    DbValueType = typeof(string),
                },
                new FieldMap {
                    FieldName = "Post",
                    DbColumnName = "post",
                    IsKeyField = false,
                    IsRequiredField = true,
                    FieldValueType = typeof(string),
                    DbValueType = typeof(string),
                },
                new FieldMap {
                    FieldName = "CompanyId",
                    DbColumnName = "company_id",
                    IsKeyField = false,
                    IsRequiredField = false,
                    FieldValueType = typeof(long?),
                    DbValueType = typeof(long?),
                }
            ]
        };
    }
}