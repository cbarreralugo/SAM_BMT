using System;
using System.Data;
using Oracle.ManagedDataAccess.Client; // Asegúrate de usar el cliente gestionado si es el caso
using System.Configuration;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectorLibrary.FamiliaConector.Oracle
{
    public class ConectorOracle : IFamiliaConector
    {
        private OracleConnection conexion;
        private string connectionString;

        public ConectorOracle()
        {
            this.connectionString = ConfigurationManager.AppSettings["DEV"].ToString();
            this.conexion = new OracleConnection(connectionString);
        }
        public DataTable Tabla(string Stord, string[,] Parametros = null)
        {
            DataTable dt = new DataTable();
            using (OracleCommand cmd = new OracleCommand(Stord, this.conexion))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Agregar parámetros
                if (Parametros != null)
                {
                    for (int i = 0; i < Parametros.GetLength(0); i++)
                    {
                        string paramName = Parametros[i, 0];
                        string paramValue = Parametros[i, 1];

                        OracleParameter param = new OracleParameter(paramName, OracleDbType.Varchar2);
                        param.Value = paramValue;
                        cmd.Parameters.Add(param);
                    }
                }

                // Agregar un parámetro de salida para el cursor
                // Oracle usa cursores para devolver conjuntos de datos
                OracleParameter outputParam = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                try
                {
                    this.conexion.Open();
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al ejecutar procedimiento almacenado Oracle: " + e.Message);
                }
                finally
                {
                    this.conexion.Close();
                }
            }
            return dt;
        }
        public int Actualizar(string q)
        {
            throw new NotImplementedException();
        }

        public int Actualizar(string q, IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public bool BulkCopy(DataTable TablaSube, string NombreTabla, bool TieneColumnaIdentidad)
        {
            throw new NotImplementedException();
        }

        public DataSet Coleccion(string Stord, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataSet Coleccion(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        // Implementa los métodos de la interfaz IFamiliaConector aquí
        public DataTable Ejecutar(string q)
        {
            DataTable dataTable = new DataTable();
            using (OracleCommand command = new OracleCommand(q, this.conexion))
            {
                OracleDataAdapter adapter = new OracleDataAdapter(command);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        public DataTable Ejecutar(string q, out string ex)
        {
            DataTable dataTable = new DataTable();
            ex = "";
            try
            {
                using (OracleCommand command = new OracleCommand(q, this.conexion))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception e)
            {
                ex = e.Message;
                return new DataTable();
            }
            return dataTable;
        }

        public ArrayList Ejecutar(string q, IDataParameter[] parameters)
        {
            ArrayList list = new ArrayList();
            try
            {
                using (OracleCommand command = new OracleCommand(q, this.conexion))
                {
                    foreach (OracleParameter param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    this.conexion.Open();
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        object[] values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        list.Add(values);
                    }
                }
            }
            catch (Exception e)
            {
                // Manejo de excepciones
                throw new Exception(e.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return list;
        }

        public ArrayList Ejecutar(string q, IDataParameter[] parameters, int nivelResultSet)
        {
            throw new NotImplementedException();
        }

        public bool Ejecutar(string Stord, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public bool Ejecutar(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public bool Ejecutar(string Stord, DataTable TablaSube, string NombreTabla, DataTable TablaSube1, string NombreTabla1, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public bool Ejecutar(string Stord, DataTable TablaSube, string NombreTabla, DataTable TablaSube1, string NombreTabla1, DataTable TablaSube2, string NombreTabla2, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public int Ejecutar(string Stord, DataTable TablaSube1, string NombreTabla1, DataTable TablaSube2, string NombreTabla2, DataTable TablaSube3, string NombreTabla3, DataTable TablaSube4, string NombreTabla4, DataTable TablaSube6, string NombreTabla6, DataTable TablaSube5, string NombreTabla5, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataTable EjecutarDataAdapter(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataTable EjecutarDataAdapter(string Stord, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataTable EjecutarDataTable(string q, IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public DataSet EjecutarDS(string queries)
        {
            throw new NotImplementedException();
        }

        public int EjecutarIdentity(string Stord, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public int EjecutarInsertUpdate(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public int EjecutarInsertUpdate(string Stord, DataTable TablaSube, string NombreTabla, DataTable TablaSube2, string NombreTabla2, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public int EjecutarStoreExecuteReader(string Stord, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public int EjecutarTransaccionInsertar(string q1, IDataParameter[] parameters1, string q2, IDataParameter[] parameters2, int indexClave)
        {
            throw new NotImplementedException();
        }

        public int EjecutarTransaccionInsertarMasivo(string[] inserts, out string status)
        {
            throw new NotImplementedException();
        }

        public int EjecutarTransaccionInsertarMasivoSinCommit(string[] inserts, out string status)
        {
            throw new NotImplementedException();
        }

        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public int InsertarIdentity(string q)
        {
            throw new NotImplementedException();
        }



        public DataTable Tabla(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataTable Tabla(object sp_upin_get_list_backup_assignment, string[,] parametro)
        {
            throw new NotImplementedException();
        }

    }
}
