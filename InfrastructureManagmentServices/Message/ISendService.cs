using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Message
{
    public  interface ISendService
    {

        public Task<ReceiveModel> Send(SendModel model);


        public Task<SendModel> getSMSfromHTS(HTSSendSMSModel model);
    }

    
}
