using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace ConnectorLibrary.FamiliaConector
{
    public interface IFamiliaConector
    {
        //IFamiliaConector getConn();

        int Actualizar(string q);
        int Actualizar(string q, IDataParameter[] parameters);
        DataTable Ejecutar(string q);
        DataTable Ejecutar(string q, out string ex);
        ArrayList Ejecutar(string q, IDataParameter[] parameters);
        ArrayList Ejecutar(string q, IDataParameter[] parameters, int nivelResultSet);
        DataSet EjecutarDS(string queries);
        DataTable EjecutarDataTable(string q, IDataParameter[] parameters);
        int EjecutarTransaccionInsertar(string q1, IDataParameter[] parameters1, string q2, IDataParameter[] parameters2, int indexClave);
        int EjecutarTransaccionInsertarMasivoSinCommit(string[] inserts, out string status);
        int EjecutarTransaccionInsertarMasivo(string[] inserts, out string status);
        int InsertarIdentity(string q);
        IDbConnection GetConnection();
        DataTable Tabla(string Stord, string[,] Parametros = null);
        DataSet Coleccion(string Stord, string[,] Parametros = null);
        bool BulkCopy(DataTable TablaSube, String NombreTabla, bool TieneColumnaIdentidad);
        DataSet Coleccion(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null);
        bool Ejecutar(string Stord, string[,] Parametros = null);
        int EjecutarIdentity(string Stord, string[,] Parametros = null);
        int EjecutarStoreExecuteReader(string Stord, string[,] Parametros = null);
        bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null);
        int EjecutarInsertUpdate(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null);
        int EjecutarInsertUpdate(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube2, String NombreTabla2, string[,] Parametros = null);
        DataTable Tabla(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null);
        DataTable EjecutarDataAdapter(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null);
        DataTable EjecutarDataAdapter(string Stord, string[,] Parametros = null);
        bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube1, String NombreTabla1, string[,] Parametros = null);
        bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube1, String NombreTabla1, DataTable TablaSube2, String NombreTabla2, string[,] Parametros = null);
        int Ejecutar(string Stord, DataTable TablaSube1, String NombreTabla1, DataTable TablaSube2, String NombreTabla2, DataTable TablaSube3, String NombreTabla3, DataTable TablaSube4, String NombreTabla4, DataTable TablaSube6, String NombreTabla6, DataTable TablaSube5, String NombreTabla5, string[,] Parametros = null);
        DataTable Tabla(object sp_upin_get_list_backup_assignment, string[,] parametro);
    }
}
