using Inforce_Task.Models.Entities;

namespace Inforce_Task.Models.Interfaces.IUrlServiceInterface
{
    public interface IUrlService
    {
        string ShortUrlAlgorithm(string longUrl);
        string ToBase62(long value);
        Task AddUrl(string link, Guid userId);
        Task<List<Url>> GetAllData();
    }
}
