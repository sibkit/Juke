namespace Juke.Mapping;

public interface IValueConverter {
    object convertToDb(object fieldValue);
    object convertToField(object dbValue);
}