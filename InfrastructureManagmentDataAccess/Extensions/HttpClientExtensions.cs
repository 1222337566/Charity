using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddTokenToHeader(this HttpClient cl, string token= "MTEwMzExODc5NzM6e3pdQSRUSXFbMHt2dlBlOjE2NDA=")
        {
            
            //int timeoutSec = 90;
            //cl.Timeout = new TimeSpan(0, 0, timeoutSec);
            string contentType = "application/json";
           // cl.DefaultRequestHeaders.Add("Content-Type", contentType);
            //cl.DefaultRequestHeaders.Add("Content-Type", "application/json");
            cl.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", token));
           // var userAgent = "d-fens HttpClient";
            //cl.DefaultRequestHeaders.Add("User-Agent", userAgent);
            return cl;
        }
    }
}