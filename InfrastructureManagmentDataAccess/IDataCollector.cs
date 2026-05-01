//using BillingCore;
//using BillingCore.Domains.Customer;
//using BillingCore.Domains.Jobs;
using Newtonsoft.Json.Linq;
using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess
{
    public interface IDataCollector<T>
    {
        public  Task<List<T>> GetData(DataTable dataTable);
        public Task<T> GetHTTPData(JObject obj);
        public Task<int> GetHTTPData(JArray obj);
        public Task<T> getfromoject(Object obj);
        public Task<ParameterAggModel>SetDataParms(T obj,DataTable g=null);
        //public  Task<List<Customer>> GetCustomerData(DataTable dataTable);
    }
}
