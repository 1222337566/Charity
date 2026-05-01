//using BillingDataAccess.EntityFramework;
using Microsoft.AspNetCore.Mvc.Filters;
//using BillingCore.Domains.Requests;
using NuGet.Protocol;
using System.Text;
using System.Text.Json;
//using BillingWebFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Sender = InfrastructureManagmentCore.Domains.Messages.Sender;
using Receiver = InfrastructureManagmentCore.Domains.Messages.Receiver;
using Message = InfrastructureManagmentCore.Domains.Messages.Message;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using InfrastructureManagmentDataAccess;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentWebFramework.Models;
namespace BillingWebApi.ActionFilters
{
    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            using StreamReader reader = new(requestBody, Encoding.UTF8, true, 1024, true);
            var bodyAsString =  reader.ReadToEndAsync();
            return await bodyAsString;
        }
    }
    public class RecordFilterAttribute : IActionFilter
    {
        public int reqid { get; set; }
        private readonly IUnitofWork _unitofWork;
        private Requests req = new Requests();
        private Balance balance = new Balance();
        private Sender sender = new Sender();
        private readonly IDataCollector<ReceiveModel> receivedataCollector;
        private readonly IDataCollector<HTSSendSMSModel> senddatacollector;
        private Receiver receiver = new Receiver();
         private Message message = new Message();
        public RecordFilterAttribute(IUnitofWork unitofWork, IDataCollector<ReceiveModel> dataCollectorr, IDataCollector<HTSSendSMSModel> dataCollectors)
        {
            _unitofWork = unitofWork;
            receivedataCollector = dataCollectorr;
            senddatacollector = dataCollectors;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            //string s = context.Result.ToJson();
            // if(context.Result.ToJson()==null)
            
            try
            {
                  
                req.response = JsonSerializer.Serialize(context.Result.ToJson(), jso);
                req.endReqTime = DateTime.Now;
                req.URI = context.HttpContext.Request.Host.ToString();
                req.Name = context.HttpContext.Request.Path;
                try
                {
                    ContentResult contentResult = (ContentResult)context.Result;
                    JObject g = JsonConvert.DeserializeObject<JObject>(contentResult.Content);
                    object b = g.GetType().GetProperty("First").GetValue(g, null);
                    //object d = b.GetType().GetProperty("First").GetValue(b, null);
                    
                    //object h = d.GetType().GetProperty("First").GetValue(d, null);
                    ReceiveModel r=JsonConvert.DeserializeObject<ReceiveModel>(g.ToJson());
                    //object s = JsonConvert.DeserializeObject<object>(.ToJson());
                    message.ErrorCode = 1000;
                    message.messageId = r.message_id;
                    message.CustomerName = "billing customer";
                    message.ErrorMessage = r.status_description;
                    receiver.Name = sender.Balance.balance.ToString();
                    if (_unitofWork.ReceiverRepository.Find(f => f.telephonenum == receiver.telephonenum) == null)
                        receiver = _unitofWork.ReceiverRepository.Add(receiver);
                    else
                        receiver = _unitofWork.ReceiverRepository.Find(f => f.telephonenum == receiver.telephonenum);
                    message.ReceiverId = receiver.Id;
                    _unitofWork.MessageRepository.Add(message);
                    _unitofWork.Complete();
                }
                catch (Exception x)
                {


                }
                balance.balance++;
                _unitofWork.BalanceRepository.Update(balance);
                _unitofWork.Complete();
                
                _unitofWork.RequestRepository.Add(req);
                _unitofWork.Complete();
                //ContentResult e = new ContentResult() { Content = e.ToJson(), ContentType = "application/json" };
                if (context.Result.ToJson().Contains("\"ErrorCode\\\":500"))
                {
                    ErrorModel e = new ErrorModel() { ErrorID = 101, ErrorCode = 500, ErrorMessage = "يوجد خطأ برمجى فى الوصول الى نظام اصدار الفواتير", Status = "internal Server error" };
                    context.Result = new ContentResult() { Content = e.ToJson(), ContentType = "application/json" };
                 
                }
              
            }
            catch (Exception ex)
            {
                ErrorModel e = new ErrorModel() { ErrorID = 101, ErrorCode = 500, ErrorMessage = "يوجد خطأ برمجى فى الوصول الى نظام اصدار الفواتير", Status = "internal Server error" };
                context.Result = new ContentResult() { Content = e.ToJson(), ContentType = "application/json" };
            }
           
            
           // throw new NotImplementedException();
        }

        public  void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (context.HttpContext.User.Identities.First().Claims.First().Value =="HTS")
            {
                 sender = _unitofWork.SenderRepository.Find(c => c.Name == "HTS", new[] {"Limt","Balance"});
                if (sender != null)
                {
                    if (sender.Balance.balance >= sender.Limt.limit)
                    {
                        ErrorModel e = new ErrorModel() { ErrorID = 101, ErrorCode = 555, ErrorMessage = "Message Quota exceeded please contact Administrator", Status = "internal Server error" };
                        context.Result = new ContentResult() { Content = e.ToJson(), ContentType = "application/json" };
                    }
                    else
                    {
                        JObject g = JsonConvert.DeserializeObject<JObject>(context.ActionArguments.ToJson());
                       object b = g.GetType().GetProperty("First").GetValue(g, null);
                        object d = b.GetType().GetProperty("First").GetValue(b, null);
                        //object h= d.GetType().GetProperty("First").GetValue(d, null);
                        HTSSendSMSModel r = JsonConvert.DeserializeObject<HTSSendSMSModel>(d.ToJson());
                         message = new Message() { SenderID=sender.Id,Name = sender.Name,header_text="Sohag water",CatId=sender.CatId,SendTime=DateTime.Now,AccountNumber=0,body_text=r.message};
                        receiver = new Receiver() { Name = balance.balance.ToString(), Telephone = r.Telephonenumber, telephonenum = long.Parse(r.Telephonenumber.Substring(1)), Type = "customer" };
                        balance = sender.Balance;
                    }
                }
               // ErrorModel e = new ErrorModel() { ErrorID = 101, ErrorCode = 500, ErrorMessage = "يوجد خطأ برمجى فى الوصول الى نظام اصدار الفواتير", Status = "internal Server error" };
                //context.Result = new ContentResult() { Content = e.ToJson(), ContentType = "application/json" };

            }
            //var req = new Requests();
            req.querystring =context.HttpContext.Request.QueryString.Value.ToString();
            context.HttpContext.Request.EnableBuffering();
            //req.Body =  await context.HttpContext.Request.Body.ReadAsStringAsync(true);
            context.HttpContext.Request.Body.Position = 0;
            //req.Body = req.Body == "" ? JsonSerializer.Serialize(context.ActionArguments) : req.Body;
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            req.Body = JsonSerializer.Serialize(context.ActionArguments,jso);
            




            //req.Body = bodyStr;
            req.Header = context.HttpContext.Request.Headers.ToList().ToJson().ToString();
            req.startReqTime = DateTime.Now;
           // req.endReqTime = DateTime.Now;
           req.querystring=context.HttpContext.Request.QueryString.ToString();
            req.HostIP=context.HttpContext.Connection.RemoteIpAddress.ToString();
            //var parms =context.HttpContext.Connection.
            
        }
    }
}
