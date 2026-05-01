using InfrastructureManagmentCore.Domains.Messages;
//using InfrastructureManagmentDataAccesss;
using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentDataAccess;

namespace InfrastructureManagmentServices.Message
{
    public class SendService : ISendService
    {
        private readonly IRepository irepository;
        private readonly IDataCollector<ReceiveModel> collector;
        public SendService(IRepository irepository, IDataCollector<ReceiveModel> collector)
        {
            this.irepository = irepository;
            this.collector = collector;
        }

        public async Task<SendModel> getSMSfromHTS(HTSSendSMSModel model)
        {
            SendModel sendModel = new SendModel() { account_id = 1640, text =model.message ,msisdn ="2"+model.Telephonenumber,mcc= "602" ,mnc="1",sender="Sohag water"};
            return sendModel;
        }

        public async Task<ReceiveModel> Send(SendModel model)
        {
            InfrastructureManagmentCore.Domains.Messages.Message m = new InfrastructureManagmentCore.Domains.Messages.Message() { };
            
                ReceiveModel model2 = await collector.GetHTTPData(await irepository.CallHTTPTemplateRequest<SendModel>("http://weapi.connekio.com/sms/single", HttpMethod.Post, "", model, null, null));
            
            
            
            return model2;
            //throw new NotImplementedException();
        }
    }
}
