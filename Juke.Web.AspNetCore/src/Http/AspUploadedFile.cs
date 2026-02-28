using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspUploadedFile : IUploadedFile
{
    private readonly IFormFile _file;
    public AspUploadedFile(IFormFile file) => _file = file;

    public string Name => _file.Name;
    public string FileName => _file.FileName;
    public long Length => _file.Length;
    public Stream OpenReadStream() => _file.OpenReadStream();
}