using Portal.Models;
using System.Text.RegularExpressions;

namespace Portal.Services.Interfaces
{
    public interface IImageService
    {
        public Task<Result<Image>> GetAsync(int imageId);
    }
}
