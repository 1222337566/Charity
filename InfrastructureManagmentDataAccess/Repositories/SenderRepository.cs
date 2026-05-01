//using NuGet.Protocol.Plugins;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentDataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class SenderRepository : BaseRepository<Sender>, ISenderRepository
    {
        public SenderRepository(AppDbContext appDBContext) : base(appDBContext)
        {
        }

        public InfrastructureManagmentCore.Domains.Messages.Balance GetBalance(long senderId)
        {
            Sender s = _appDBContext.Sender.Include(c=>c.Balance).Where(c=>c.Id ==senderId).FirstOrDefault();
             
            return s.Balance;
        }

        public InfrastructureManagmentCore.Domains.Messages.Category GetCategory(long senderId)
        {
            Sender s = _appDBContext.Sender.Include(c => c.Category).Where(c => c.Id == senderId).FirstOrDefault();
            return s.Category;
        }

        public InfrastructureManagmentCore.Domains.Messages.Limit GetLimit(long senderId)
        {

            Sender s = _appDBContext.Sender.Include(c => c.Limt).Where(c => c.Id == senderId).FirstOrDefault();
            return s.Limt;
        }

        public IEnumerable<InfrastructureManagmentCore.Domains.Messages.Message> getMessagesfromSenderId(long senderId)
        {

            Sender s = _appDBContext.Sender.Include(c => c.Messages).Where(c => c.Id == senderId).FirstOrDefault();
            return s.Messages;
        }
    }
}
