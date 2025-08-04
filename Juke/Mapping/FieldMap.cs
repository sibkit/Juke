namespace Juke.Mapping;

public class FieldMap {
    public required string DbColumnName { get; init; }
    public required string FieldName { get; init; }
    public bool IsKeyField { get; init; } = false;
    public bool IsRequiredField { get; init; } = false;
    public required Type FieldValueType { get; init; }
    public Type? DbValueType { get; init; }
    public int Index { get; set; }
    public IValueConverter? ValueConverter { get; init; }
}