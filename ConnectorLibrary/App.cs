using System;
using System.Configuration;
using System.Diagnostics;
using ConnectorLibrary.FamiliaConector;
using ConnectorLibrary.FamiliaConector.Oracle;
using ConnectorLibrary.FamiliaConector.Sql;

namespace ConnectorLibrary
{
    public class App
    {
        private static readonly object _mutex = new object();

    
        private static IFamiliaConector connector = null;
       
        //public static IFamiliaConector GetCurrentConnector()
        //{
        //    lock (_mutex)
        //    {
        //        if (connector == null)
        //        {
        //            connector = new ConnectorLibrary.FamiliaConector.Sql.Conector(ConfigurationManager.AppSettings["DEV"].ToString());
        //        }
        //    }
        //    return connector;
        //}

        public static IFamiliaConector GetCurrentConnector()
        {
            string dbType = ConfigurationManager.AppSettings["instanciaBD"];
            switch (dbType)
            {
                case "Sql":
                    return new Conector();
                case "Oracle":
                    return new ConectorOracle();
                case "MySQL":
                // return new ConectorMySQL(); // Asumiendo que tienes una implementación similar para MySQL
                default:
                    throw new Exception("Tipo de base de datos no soportado.");
            }
        }

    }
}
 