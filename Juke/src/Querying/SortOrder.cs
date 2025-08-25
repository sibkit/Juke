namespace Juke.Querying;

public enum SortOrderDirection {
    Asc, 
    Desc
}

public class SortOrder: QueryElement {
    public Field Field { get; }
    public SortOrderDirection Direction { get; }

    public SortOrder(Field field, SortOrderDirection direction) {
        field.Parent =  this;
        Field = field;
        Direction = direction;
    }
}