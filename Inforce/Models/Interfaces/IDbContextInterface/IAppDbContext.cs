using Inforce_Task.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inforce_Task.Models.Interfaces.IDbContextInterface
{
    public interface IAppDbContext
    {
        public DbSet<User> Users { get; set; }   
        public DbSet<Url> Urls { get; set; }
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
