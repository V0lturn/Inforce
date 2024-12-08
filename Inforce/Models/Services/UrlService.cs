using Inforce_Task.Models.Entities;
using Inforce_Task.Models.Interfaces.IDbContextInterface;
using Inforce_Task.Models.Interfaces.IUrlServiceInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Inforce_Task.Models.Services
{
    public class UrlService : IUrlService
    {
        private readonly IAppDbContext _appDbContext;
        const string Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public UrlService(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public string ShortUrlAlgorithm(string longUrl)
        {
            byte[] hashBytes;
            using (MD5 md5 = MD5.Create())
            {
                hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(longUrl));
            }

            byte[] shortHashBytes = hashBytes.Take(6).ToArray();

            if (shortHashBytes.Length < 6)
            {
                shortHashBytes = shortHashBytes.Concat(new byte[6 - shortHashBytes.Length]).ToArray();
            }

            byte[] paddedBytes = shortHashBytes.Concat(new byte[2]).ToArray();  

            long decimalValue = (long)BitConverter.ToUInt64(paddedBytes, 0);

            string base62String = ToBase62(decimalValue);

            return base62String;
        }

        public string ToBase62(long value)
        {
            StringBuilder result = new StringBuilder();
            do
            {
                int remainder = (int)(value % 62);
                result.Insert(0, Base62Characters[remainder]);
                value /= 62;
            } while (value > 0);

            return result.ToString();
        }



        public async Task AddUrl(string link, Guid userId)
        {
            var shortUlr = ShortUrlAlgorithm(link);

            var newRecord = new Url(link, shortUlr, userId);

            await  _appDbContext.Urls.AddAsync(newRecord);
            await  _appDbContext.SaveChanges();
        }

        public async Task<List<Url>> GetAllData()
        {
            return await _appDbContext.Urls.OrderBy(u => u.CreatedDate).ToListAsync();
        }
    }
}
