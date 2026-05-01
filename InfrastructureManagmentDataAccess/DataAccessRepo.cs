using Microsoft.VisualBasic.FileIO;
using System;
//using InfrastructureManagmentTaskSchedualer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Options;
using System.Runtime;
using NuGet.Configuration;

namespace InfrastructureManagmentDataAccess
{
    public enum Enviroment
    {
        PROD,
        Test,
        DEV
    }
    public  class DataAccessRepo
    {



        private readonly Settings settings;
        public string MainConnectionStringProd = "Data Source=10.20.20.178;Initial Catalog = InfraMGMTDb; User Id = InfraUser; Password=$cww#@123!@#**; TrustServerCertificate=True;";
        
        
        public string MainConnectionStringDev = "Data Source=10.20.20.55;Initial Catalog = InfraMGMTDb; User Id = InfraUser; Password=$cww#@123!@#**; TrustServerCertificate=True;";
        public string MainConnectionString;

        public DataAccessRepo(IOptions<Settings> options)
        {
            settings = options.Value;
            if (MainMode == Enviroment.DEV)
            {
                MainConnectionString = MainConnectionStringProd;
            }
            else
            {

                MainConnectionString = MainConnectionStringDev;
            }
        }
        public Enviroment MainMode =>this.MainConnectionString =="Prod"?Enviroment.PROD:Enviroment.DEV;
 
    
    }
}
