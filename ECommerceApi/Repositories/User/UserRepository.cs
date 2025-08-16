using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

      
    }
}