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
using Sender = InfrastructureManagmentCore.Domains.Messages.Sender;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        
        public MessageRepository(AppDbContext appDBContext) : base(appDBContext)
        {
        }

        public Receiver addReceiver(Receiver receiver, int? id)
        {
            Receiver r =_appDBContext.Receiver.Add(receiver).Entity;
            Message m = _appDBContext.Message.Find(id);
            m.ReceiverId = id;
            m.Receiver = r;
            _appDBContext.Message.Update(m);
            _appDBContext.SaveChanges();

            return r;

        }

        public InfrastructureManagmentCore.Domains.Messages.Sender addSender(InfrastructureManagmentCore.Domains.Messages.Sender sender, int? id)
        {
            InfrastructureManagmentCore.Domains.Messages.Sender s = _appDBContext.Sender.Add(sender).Entity;
            InfrastructureManagmentCore.Domains.Messages.Message m = _appDBContext.Message.Find(id);
            m.SenderID = id;
            m.Sender = s;
            _appDBContext.Message
                .Update(m);
            _appDBContext.SaveChanges();

            return s;

        }

        public Receiver getReceiverfromMessageID(int Id)
        {
            Message message = _appDBContext.Message.Include(c=>c.Receiver).Where(f=>f.Id == Id).FirstOrDefault();
            return message.Receiver;
        }

        public Sender getSenderdromMessageID(int Id)
        {
            // throw new NotImplementedException();
            Message message = _appDBContext.Message.Include(c => c.Sender).Where(f => f.Id == Id).FirstOrDefault();
            return message.Sender;
        }
    }
}
