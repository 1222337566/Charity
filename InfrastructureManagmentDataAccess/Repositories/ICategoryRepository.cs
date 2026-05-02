using InfrastructureManagmentCore.Domains.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public  interface ICategoryRepository : IBaseRepository<Category>   
    {
        IEnumerable<Message> getMessagesfromSenderId(long senderId);

        Limit GetLimit(long senderId);

        Balance GetBalance(long senderId);

       // Category GetCategory(int senderId);
    }
}
