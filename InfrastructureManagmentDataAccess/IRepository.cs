//using BillingCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess
{
    public interface IRepository 
    {
        public  Task<JObject> CallHTTPRequest(string Uri,HttpMethod m, string entityprocess, List<string> parameternames = null, Dictionary<string, Int16> parameter1 = null, Dictionary<string, int> parameter2 = null, Dictionary<string, Int64> parameters4 = null, Dictionary<string, DateTime> parameters3 = null, Dictionary<string, string> parameters5 = null,string responsetype=null);
        public Task<JArray> CallHTTPTemplateRequest<G,T>(string Uri, HttpMethod m, string entityprocess,G model, List<string> parameternames = null, string responsetype = null, List<T> Headers = null);
        public Task<JObject> CallHTTPTemplateRequest<G>(string Uri, HttpMethod m, string entityprocess, G model, List<string> parameternames = null, string responsetype = null);
        public Task<JObject> CallHTTPFromURL(string Uri, HttpMethod m, Dictionary<string, string> parameters = null, Dictionary<string, int> parameter1=null);
    }
}

