//using BillingCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public  interface IMessageRepository : IBaseRepository<Message>
    {
        Receiver getReceiverfromMessageID(int Id);
        Sender   getSenderdromMessageID(int Id);

        Receiver addReceiver(Receiver receiver, int? id);

        Sender addSender(Sender sender, int? id);
    }
}
