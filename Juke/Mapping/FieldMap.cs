namespace Juke.Mapping;

public class FieldMap {
    public EntityMap EntityMap { get; init; }
    public string DbColumnName { get; init; }
    public string FieldName { get; init; }
    public bool IsKeyField { get; init; }
    public bool IsRequiredField { get; init; }
    public Type FieldValueType { get; init; }
    public Type DbValueType { get; init; }
    public int Index { get; set; }
    public IValueConverter ValueConverter { get; init; }
}