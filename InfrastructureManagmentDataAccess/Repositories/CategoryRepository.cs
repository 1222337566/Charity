using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentDataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext appDBContext) : base(appDBContext)
        {
        }

        public Balance GetBalance(long senderId)
        {
            Category c = _appDBContext.Category.Include(s => s.Balance).Where(s => s.Id == senderId).FirstOrDefault();
            return c.Balance;
        }

        public Limit GetLimit(long senderId)
        {

            Category c = _appDBContext.Category.Include(s => s.Limt).Where(s => s.Id == senderId).FirstOrDefault();
            return c.Limt;
        }

        public IEnumerable<Message> getMessagesfromSenderId(long senderId)
        {
            Category c = _appDBContext.Category.Include(s => s.Messages).Where(s => s.Id == senderId).FirstOrDefault();
            return c.Messages;
        }
    }
}
