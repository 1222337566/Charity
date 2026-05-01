//using BillingCore;
//using BillingCore.Domains.Customer;
//using BillingCore.Domains.Jobs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InfrastructureManagmentWebFramework.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Boolean = System.Boolean;

namespace InfrastructureManagmentDataAccess
{
    public class DataCollector<T> : IDataCollector<T> where T : new()
    {
        IRepository _repository;
        //BillingServer _server;
        public DataCollector(IRepository repository)
        {
            _repository = repository;
            //_server = server;
        }

        

     
       

        public async Task<List<T>> GetData(DataTable dataTable)
        {
            List<T> dataarray = new List<T>();

            T data = new T() { };
            foreach (DataRow row in dataTable.Rows)
            {
                PropertyInfo[] propertyInfos = data.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo != null)
                    {
                        try
                        {
                            if (propertyInfo.PropertyType == typeof(Guid))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, new Guid(row[propertyInfo.Name].ToString()));
                            }
                                if (propertyInfo.PropertyType == typeof(Byte))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToByte(row[propertyInfo.Name]));
                            }
                            if (propertyInfo.PropertyType == typeof(DateOnly))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, DateOnly.FromDateTime(Convert.ToDateTime(row[propertyInfo.Name])));
                            }
                            else if (propertyInfo.PropertyType == typeof(decimal))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToDecimal(row[propertyInfo.Name]));

                            }
                            else if (propertyInfo.PropertyType == typeof(int))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt32(row[propertyInfo.Name]));
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, (string)Convert.ChangeType(row[propertyInfo.Name], typeof(string)));
                            }
                            else if (propertyInfo.PropertyType == typeof(DateTime))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToDateTime(row[propertyInfo.Name]));
                            }

                            else if (propertyInfo.PropertyType == typeof(long))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt64(row[propertyInfo.Name]));
                            }
                            else if (propertyInfo.PropertyType == typeof(Int16))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt16(row[propertyInfo.Name]));
                            }
                            if (propertyInfo.PropertyType == typeof(Byte?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToByte(row[propertyInfo.Name]));
                            }
                            if (propertyInfo.PropertyType == typeof(DateOnly?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, DateOnly.FromDateTime(Convert.ToDateTime(row[propertyInfo.Name])));
                            }
                            else if (propertyInfo.PropertyType == typeof(decimal?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToDecimal(row[propertyInfo.Name]));

                            }
                            else if (propertyInfo.PropertyType == typeof(int?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt32(row[propertyInfo.Name]));
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, (string)Convert.ChangeType(row[propertyInfo.Name], typeof(string)));
                            }
                            else if (propertyInfo.PropertyType == typeof(DateTime?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToDateTime(row[propertyInfo.Name]));
                            }

                            else if (propertyInfo.PropertyType == typeof(long?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt64(row[propertyInfo.Name]));
                            }
                            else if (propertyInfo.PropertyType == typeof(Int16?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToInt16(row[propertyInfo.Name]));
                            }
                            else if (propertyInfo.PropertyType == typeof(bool?))
                            {
                                if (row[propertyInfo.Name] != null)
                                    data.GetType().GetProperty(propertyInfo.Name).SetValue(data, Convert.ToBoolean(row[propertyInfo.Name]));
                            }
                        }

                        catch (System.ArgumentException ex)
                        {

                            continue;
                        }
                        catch (Exception ex)
                        {
                            // if(ex.Message == "Object cannot be cast from DBNull to other types.")
                            // data.GetType().GetProperty(propertyInfo.Name).SetValue(data, (DateTime?)null);
                            continue;

                        }

                    }




                }
                //          if (data..Id.GetType())
                //        data.Id = (data.Id.GetType())Convert.ToInt32(row["customer_no"]);
                //      OBJ.Name = (string)Convert.ChangeType(row["customer_name"], typeof(string));

                dataarray.Add(data);
                data = new T();
                //    list.Add(OBJ);
            }
            return dataarray;
        }

        public async Task<T> getfromoject(object obj)
        {
            //Type myType = obj.GetType();
            object s = obj.GetType().GetProperty("First").GetValue(obj, null);
            Type myType = s.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            T o = new T();
            PropertyInfo[] propertyInfos = o.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {

                var p = s.GetType().GetProperty("First").GetValue(s, null);
                var f = s.GetType().GetProperties();
                try
                {
                    if (propertyInfo.PropertyType == typeof(Guid))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, new Guid(p.ToString()));
                    }
                    if (propertyInfo.PropertyType == typeof(Byte))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToByte(p));
                    }
                    if (propertyInfo.PropertyType == typeof(DateOnly))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, DateOnly.FromDateTime(Convert.ToDateTime(p)));
                    }
                    else if (propertyInfo.PropertyType == typeof(decimal))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToDecimal(p));

                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt32(p));
                    }
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, (string)Convert.ChangeType(p, typeof(string)));
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToDateTime(p));
                    }

                    else if (propertyInfo.PropertyType == typeof(long))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt64(p));
                    }
                    else if (propertyInfo.PropertyType == typeof(Int16))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt16(p));
                    }
                    if (propertyInfo.PropertyType == typeof(Byte?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToByte(p));
                    }
                    if (propertyInfo.PropertyType == typeof(DateOnly?))
                    {
                        if (p != null)
                            p.GetType().GetProperty(propertyInfo.Name).SetValue(o, DateOnly.FromDateTime(Convert.ToDateTime(p)));
                    }
                    else if (propertyInfo.PropertyType == typeof(decimal?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToDecimal(p));

                    }
                    else if (propertyInfo.PropertyType == typeof(int?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt32(p));
                    }
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, (string)Convert.ChangeType(p, typeof(string)));
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToDateTime(p));
                    }

                    else if (propertyInfo.PropertyType == typeof(long?))
                    {
                        if (p!= null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt64(p));
                    }
                    else if (propertyInfo.PropertyType == typeof(Int16?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToInt16(p));
                    }
                    else if (propertyInfo.PropertyType == typeof(bool?))
                    {
                        if (p != null)
                            o.GetType().GetProperty(propertyInfo.Name).SetValue(o, Convert.ToBoolean(p));
                    }
                }

                catch (System.ArgumentException ex)
                {

                    continue;
                }
                catch (Exception ex)
                {
                    // if(ex.Message == "Object cannot be cast from DBNull to other types.")
                    // data.GetType().GetProperty(propertyInfo.Name).SetValue(data, (DateTime?)null);
                    continue;

                }

                // Do something with propValue
            }
            //  throw new NotImplementedException();
            return  o; 
        }

        public async Task<T> GetHTTPData(JObject obj)
        {
            T dataarray = JsonConvert.DeserializeObject<T>(obj.ToString());


            return dataarray;
        }

        public async Task<int> GetHTTPData(JArray obj)
        {
            int dataarray = int.Parse(obj[0]["Value"].First.ToString());


            return dataarray;
        }

        public async Task<ParameterAggModel> SetDataParms(T obj, DataTable g=null)
        {
            List<T> dataarray = new List<T>();
            ParameterAggModel model = new ParameterAggModel()
            {
                parameter1 = new Dictionary<string, short>(),
                parameter2 = new Dictionary<string, int>(),
                parameters3 = new Dictionary<string, DateTime>(),
                parameters4 = new Dictionary<string, long>(),
                parameters5 = new Dictionary<string, string>(),
                parameter6 = new Dictionary<string, Guid>(),
                parameters7 = new Dictionary<string, decimal>(),
                parameters8 = new Dictionary<string, DateOnly>(),
                parameters9 = new Dictionary<string, byte>(),
                parameters10 = new Dictionary<string, Boolean>(),
                Table = new Dictionary<string, DataTable>()
            };
            T data = obj;

            PropertyInfo[] propertyInfos = data.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo != null)
                {
                    try
                    {
                        if (propertyInfo.PropertyType == typeof(Byte?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters9.Add("@" + propertyInfo.Name, Convert.ToByte(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(decimal?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters7.Add("@" + propertyInfo.Name, Convert.ToDecimal(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters5.Add("@" + propertyInfo.Name, Convert.ToString(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(Int16?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameter1.Add("@" + propertyInfo.Name, Convert.ToInt16(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(Int16))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameter1.Add("@" + propertyInfo.Name, Convert.ToInt16(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(Int64?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters4.Add("@" + propertyInfo.Name, Convert.ToInt64(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(int?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameter2.Add("@" + propertyInfo.Name, Convert.ToInt32(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(int))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameter2.Add("@" + propertyInfo.Name, Convert.ToInt32(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(DateOnly?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters8.Add("@" + propertyInfo.Name, (DateOnly)data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null));
                        }
                        if (propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters3.Add("@" + propertyInfo.Name, Convert.ToDateTime(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(Guid?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameter6.Add("@" + propertyInfo.Name, new Guid(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null).ToString()));
                        }
                        if (propertyInfo.PropertyType == typeof(Boolean?))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.parameters10.Add("@" + propertyInfo.Name, Convert.ToBoolean(data.GetType().GetProperty(propertyInfo.Name).GetValue(data, null)));
                        }
                        if (propertyInfo.PropertyType == typeof(DataTable))
                        {
                            if (data.GetType().GetProperty(propertyInfo.Name) != null)
                                model.Table.Add("@" + propertyInfo.Name, g);
                        }

                    }
                    catch (Exception e)
                    {

                        return null;
                    }



                }



            }
            return model;
        }
    }
}
