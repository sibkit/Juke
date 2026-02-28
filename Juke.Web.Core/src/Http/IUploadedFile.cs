namespace Juke.Web.Core.Http;

public interface IUploadedFile 
{
    string Name { get; }      // Имя поля в HTML форме (например, "gerberArchive")
    string FileName { get; }  // Реальное имя файла (например, "my_board_v1.zip")
    long Length { get; }      // Размер в байтах
    Stream OpenReadStream();  // Поток для чтения
}