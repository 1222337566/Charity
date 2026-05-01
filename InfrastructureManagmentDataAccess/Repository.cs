using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using BillingCore.Domains.Customer;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
//using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Net.Mime;
using NuGet.Protocol;
using System.Net.Http.Json;
using Newtonsoft.Json;
//using BillingCore.Domains.Prepaid;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Http;
using Azure.Core;
using System.Net;
using System.Net.Http.Headers;
using InfrastructureManagmentDataAccess.Extensions;

namespace InfrastructureManagmentDataAccess
{
    public class Repository : IRepository
    {
       // private readonly BillingServer _billingServer;
        public Repository()
        {
            //_billingServer = billingServer;
        }

        public async Task<JObject> CallHTTPRequest(string Uri, HttpMethod m, string entityprocess, List<string> parameternames = null, Dictionary<string, short> parameter1 = null, Dictionary<string, int> parameter2 = null, Dictionary<string, long> parameters4 = null, Dictionary<string, DateTime> parameters3 = null, Dictionary<string, string> parameters5 = null,string responsetype=null)
        {
            HttpClient client = new HttpClient();
            if (entityprocess == "body")
            {
                HttpContent content  = new StringContent(JsonConvert.SerializeObject(parameters5), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                }

                var responseBody = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(responseBody);

                return jObject;



            }
            else
            {
                HttpContent content;
                if (parameters5 != null)
                    content = new FormUrlEncodedContent(parameters5);
                else
                    content = null;

                var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject jObject = JObject.Parse(responseBody);

                return jObject;

            }
        }
        public async Task<JObject> CallHTTPFromURL(string Uri, HttpMethod m,Dictionary<string,string> parameters=null, Dictionary<string,int> parameter1=null) 
        {
            HttpClient client = new HttpClient();
            HttpContent content=null;
            Dictionary<string,string> parameters5= new Dictionary<string, string>();
            parameters5.Add(parameter1.FirstOrDefault().Key, parameter1.FirstOrDefault().Value.ToString());
            if (parameter1 != null)
                Uri=Uri+"/"+parameter1.FirstOrDefault().Value.ToString();
            else
                content = null;

            var request = new HttpRequestMessage()
            {
                Method = m,
                Content = content,
                RequestUri = new Uri(Uri)
            };
            var response = await client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            JObject jObject = JObject.Parse(responseBody);

            return jObject;

        }
        public async Task<JArray> CallHTTPTemplateRequest<G,T>(string Uri, HttpMethod m, string entityprocess, G model, List<string> parameternames = null, string responsetype = null,List<T> Headers= null)
        {
            HttpClient client = new HttpClient();
            
            if (entityprocess == "body" && responsetype ==null)
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                
                var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                }

                var responseBody = await response.Content.ReadAsStringAsync();
                JArray jObject = JArray.Parse(responseBody);

                return jObject;

            }
            else if(entityprocess =="" && responsetype ==null)
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JArray jObject = JArray.Parse(responseBody);

                return jObject;

            }
            else if(entityprocess =="body" && responsetype =="header")
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
              

                var responseHeader =  response.Headers.ToJson();
                JArray jObject = JArray.Parse(responseHeader);

                return jObject;


            }
            else
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                
                var request = new HttpRequestMessage()
                {
                   
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                }

                var responseHeader = response.Headers.ToJson();
                JArray jObject = JArray.Parse(responseHeader);

                return jObject;


            }
        }

        public async Task<JObject> CallHTTPTemplateRequest<G>(string Uri, HttpMethod m, string entityprocess, G model, List<string> parameternames = null, string responsetype = null)
        {
            HttpClient client = new HttpClient();

            client.AddTokenToHeader();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
               // content.Headers.Add("Content-Type", "application/json");
            var request = new HttpRequestMessage()
                {
                    Method = m,
                    Content = content,
                    RequestUri = new Uri(Uri)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                }

                var responseBody = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseBody);

            return jObject;


        }
        /*   public List<TEntity> ExecSToredProcedure(string entityprocess,string connectionstring)
{
DataTable DT = null;
List<TEntity> result = new List<TEntity>();
//BillingServer billingServer = new BillingServer();
using (SqlConnection con = new SqlConnection(connectionstring))
{
 string SQL = @"EXEC HTS.Get" + entityprocess + "Proc ";
 using (SqlDataAdapter ad = new SqlDataAdapter(SQL, con))
 {
     DT = new DataTable();
     ad.Fill(DT);
     if (DT.Rows.Count > 0)
     {
         foreach (DataRow item in DT.Rows)
         {
             TEntity obj = new TEntity();
             obj.Id = Convert.ToInt16(item[entityprocess + "Id"]);
             obj.Name = Convert.ToString(item[entityprocess + "Name"]);
             result.Add(obj);
         }
     }
 }
 return result;
}
}

public List<TEntity> ExecSToredProcedure<T1>(string entityprocess, T1 t1)
{
throw new NotImplementedException();
}

public List<TEntity> ExecSToredProcedure<T1, T2>(string entityprocess, T1 t1, T2 t2)
{
throw new NotImplementedException();
}
*/
        //    public DataTable ExecSToredProcedure(string SPname, string user, string entityprocess, List<string>parameternames = null, Dictionary<string,Int16> parameter1 = null, Dictionary<string,int> parameter2 = null,Dictionary<string,Int64> parameters4 =null, Dictionary<string,DateTime> parameters3 = null,Dictionary<string,string> parameters5 =null,Dictionary<string,Guid> parameter6 =null,Dictionary<string,decimal> parameters7 =null,Dictionary< string,DataTable> Table =null,Dictionary<string,DateOnly>parameters8=null,Dictionary<string,Byte> parameters9 =null,Dictionary<string,Boolean> parameters10 =null)
        //{
        //    string connectionstring = "";
        //    if (entityprocess == "")


        //        connectionstring = _billingServer._connectionString.Where(c => c.Key == user).FirstOrDefault().Value;


        //    else if (entityprocess == "prepaid")
        //        connectionstring = _billingServer._connectionString.Where(c => c.Key == user + "-Prepaid" && c.Value.Contains("EInvoice")).FirstOrDefault().Value;
        //    else
        //        connectionstring = _billingServer._connectionString.Where(c => c.Key == user + @"\SECOND").FirstOrDefault().Value;
        //    DataTable DT = new DataTable();
        //    //ParameterDirection result = new ParameterDirection();
        //   // List<TEntity> list = new List<TEntity>();
        //   // BillingServer billingServer;
        //    using (SqlConnection conn = new SqlConnection(connectionstring))
        //    using (SqlCommand cmd = conn.CreateCommand())
        //    {
        //        cmd.CommandText = SPname;
        //        cmd.CommandType = CommandType.StoredProcedure;



        //                if (parameter1 != null) 
        //                {
        //                    foreach (KeyValuePair<string,short> parameter in parameter1)
        //                        cmd.Parameters.Add(parameter.Key, SqlDbType.SmallInt).Value = parameter.Value != null ? parameter.Value : DBNull.Value;
        //        }
        //                if (parameter2 != null)
        //                {
        //                    foreach (KeyValuePair<string,int> patameter in parameter2)
        //                        cmd.Parameters.Add(patameter.Key, SqlDbType.Int).Value = patameter.Value!=null ? patameter.Value: DBNull.Value;
        //                }
        //                if(parameters3 != null)
        //                {
        //                    foreach (KeyValuePair<string,DateTime> patameter in parameters3)
        //                        cmd.Parameters.Add(patameter.Key, SqlDbType.Date).Value = patameter.Value != null ? patameter.Value : DBNull.Value;
        //        }
        //                if (parameters4 != null)
        //                {
        //                    foreach (KeyValuePair<string,Int64> u in parameters4)
        //                        cmd.Parameters.Add(u.Key, SqlDbType.BigInt).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //            if (parameters5 != null)
        //            {
        //                foreach (KeyValuePair<string, string> u in parameters5)
        //                    cmd.Parameters.Add(u.Key, SqlDbType.NVarChar).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //            if (parameter6 != null)
        //            {
        //                foreach (KeyValuePair<string, Guid> u in parameter6)
        //                    cmd.Parameters.Add(u.Key, SqlDbType.UniqueIdentifier).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //            if (parameters7 != null)
        //            {
        //                foreach (KeyValuePair<string, decimal> u in parameters7)
        //                    cmd.Parameters.Add(u.Key, SqlDbType.Decimal).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //        if (parameters8 != null)
        //        {
        //            foreach (KeyValuePair<string, DateOnly> u in parameters8)
        //                cmd.Parameters.Add(u.Key, SqlDbType.Date).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //        if (parameters9 != null)
        //        {
        //            foreach (KeyValuePair<string, Byte> u in parameters9)
        //                cmd.Parameters.Add(u.Key, SqlDbType.TinyInt).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //        if (parameters10 != null)
        //        {
        //            foreach (KeyValuePair<string, Boolean> u in parameters10)
        //                cmd.Parameters.Add(u.Key, SqlDbType.Bit).Value = u.Value != null ? u.Value : DBNull.Value;
        //        }
        //        if (Table != null)
        //        {
        //            foreach (KeyValuePair<string, DataTable> u in Table)
        //                cmd.Parameters.Add(u.Key, SqlDbType.Structured).Value = u.Value!=null?u.Value:DBNull.Value;
        //        }
        //        //cmd.Parameters.Add(s3, SqlDbType.BigInt).Value = t3;


        //        // @ReturnVal could be any name
        //      //  var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
        //        //returnParameter.Direction = ParameterDirection.ReturnValue;
        //        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
        //        {
        //            ad.Fill(DT);
        //        }

        //       // TEntity OBJ = new TEntity();
        //        /*foreach (DataRow row in DT.Rows)
        //        {
        //            OBJ.Id = (short)Convert.ToInt32(row["customer_no"]);
        //            OBJ.Name = (string)Convert.ChangeType(row["customer_name"], typeof(string));


        //            list.Add(OBJ);
        //        }*/

        //    }
        //    return DT;  
        //    //BillingServer billingServer = new BillingServer();
        //    /* using (SqlConnection con = new SqlConnection(BillingServer.ConStr))
        //     {
        //         using (SqlCommand cmd = new SqlCommand("EXEC HTS.Get" + entityprocess + @"Proc
        //                @branch_no		
        //                ,@group_no		
        //                ,@account_no	

        //                                       ", con))
        //         {
        //            // cmd.CommandType = CommandType.StoredProcedure;

        //             cmd.Parameters.Add(s1, SqlDbType.SmallInt).Value = t1;
        //             cmd.Parameters.Add(s2, SqlDbType.SmallInt).Value = t2;
        //             cmd.Parameters.Add(s3, SqlDbType.BigInt).Value = t3;
        //             SqlDataAdapter ad = new SqlDataAdapter();
        //             ad.SelectCommand = cmd;

        //                 DT = new DataTable();
        //                 ad.Fill(DT);
        //                 if (DT.Rows.Count > 0)
        //                 {
        //                     foreach (DataRow item in DT.Rows)
        //                     {
        //                         TEntity obj = new TEntity();
        //                         obj.Id = Convert.ToInt16(item[entityprocess + "Id"]);
        //                         obj.Name = Convert.ToString(item[entityprocess + "Name"]);
        //                         result.Add(obj);
        //                     }
        //                 }


        //             con.Open();
        //             cmd.ExecuteNonQuery();
        //         }

        //         return result;
        //     }*/
        //}

    }
}
