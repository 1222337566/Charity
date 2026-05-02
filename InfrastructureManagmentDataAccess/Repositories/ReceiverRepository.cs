using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentDataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message = InfrastructureManagmentCore.Domains.Messages.Message;
using Receiver = InfrastructureManagmentCore.Domains.Messages.Receiver;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ReceiverRepository : BaseRepository<Receiver>, IReceiverRepository
    {
        public ReceiverRepository(AppDbContext appDBContext) : base(appDBContext)
        {
        }

        public IEnumerable<Message> getMessagesfromSenderId(long receiverId)
        {

            Receiver r = _appDBContext.Receiver.Include(c => c.Messages).Where(c => c.Id == receiverId).FirstOrDefault();
            return r.Messages;
        }
    }
}
