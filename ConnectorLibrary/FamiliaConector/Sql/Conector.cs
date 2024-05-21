using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Configuration; 
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using System.Windows.Forms;
namespace ConnectorLibrary.FamiliaConector.Sql
{
    public class Conector : IFamiliaConector
    {
        // Fields
        private static Conector con = null;
        private string conect;
        private SqlConnection conexion;
        private bool conCredenciales = false;

        // Methods
        public Conector()
        {
            this.conect = string.Empty;
            this.conect = ConfigurationManager.AppSettings["DEV"].ToString();
            this.conexion = new SqlConnection(this.conect);

            if (ConfigurationManager.AppSettings["con_credenciales"] != null)
                this.conCredenciales = bool.Parse(ConfigurationManager.AppSettings["con_credenciales"]);
            else
                this.conCredenciales = false;
        }

        public Conector(string strConnection)
        {
            this.conect = strConnection;
            this.conexion = new SqlConnection(this.conect);

            if (ConfigurationManager.AppSettings["con_credenciales"] != null)
                this.conCredenciales = bool.Parse(ConfigurationManager.AppSettings["con_credenciales"]);
            else
                this.conCredenciales = false;
        }

        /*public Conector(string strCadena)
        {
            this.conect = string.Empty;
            this.conect = strCadena;
            this.conexion = new SqlConnection(this.conect);

            if (ConfigurationManager.AppSettings["con_credenciales"] != null)
                this.conCredenciales = bool.Parse(ConfigurationManager.AppSettings["con_credenciales"]);
            else
                this.conCredenciales = false;
        }*/

        public int Actualizar(string q)
        {
            int num = 0;
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        SqlCommand command = this.conexion.CreateCommand();
                        command.CommandText = q;
                        this.conexion.Open();
                        num = command.ExecuteNonQuery();
                        if (num >= 0)
                        {
                            return num;
                        }
                        if (command.ExecuteReader().Read())
                        {
                            return 1;
                        }
                        return 0;
                    }
                    catch (Exception exception)
                    {
                        string message = exception.Message;
                    }
                    finally
                    {
                        this.conexion.Close();
                    }
                }
            }
            else
            {
                try
                {
                    SqlCommand command = this.conexion.CreateCommand();
                    command.CommandText = q;
                    command.CommandTimeout = 600000;

                    this.conexion.Open();
                    num = command.ExecuteNonQuery();
                    if (num >= 0)
                    {
                        return num;
                    }
                    if (command.ExecuteReader().Read())
                    {
                        return 1;
                    }
                    return 0;
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
                finally
                {
                    this.conexion.Close();
                }
            }
            return num;
        }

        public int Actualizar(string q, IDataParameter[] parameters)
        {
            int num2;
            try
            {
                SqlCommand command = this.conexion.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = q;
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.Add(parameters[i]);
                }
                this.conexion.Open();
                num2 = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(ConfigurationManager.AppSettings["errorConexion"] + ":" + exception.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return num2;
        }

        public DataTable Ejecutar(string q)
        {
            DataTable tabla = new DataTable("tablaDatos");
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        SqlDataAdapter adap = new SqlDataAdapter(q, this.conect);
                        adap.Fill(tabla);
                    }
                    catch (Exception ex)
                    {
                        string x = ex.Message;
                    }
                }
            }
            else
            {
                try
                {
                    SqlDataAdapter adap = new SqlDataAdapter(q, this.conect);

                    adap.SelectCommand.CommandTimeout = 600000;
                    adap.Fill(tabla);
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                }
            }
            return tabla;
        }

        public DataSet EjecutarDS(string queries)
        {
            DataSet tablas = new DataSet("tablaDatos");
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        SqlDataAdapter adap = new SqlDataAdapter(queries, this.conect);
                        adap.Fill(tablas);
                    }
                    catch (Exception ex)
                    {
                        string x = ex.Message;
                    }
                }
            }
            else
            {
                try
                {
                    SqlDataAdapter adap = new SqlDataAdapter(queries, this.conect);

                    adap.SelectCommand.CommandTimeout = 600000;
                    adap.Fill(tablas);
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                }
            }
            return tablas;
        }

        public DataTable Ejecutar(string q, out string ex)
        {
            DataTable dataTable = new DataTable("tablaDatos");
            try
            {
                new SqlDataAdapter(q, this.conect).Fill(dataTable);
                ex = "OK ";
            }
            catch (Exception exception)
            {
                ex = exception.Message;
                return dataTable;
            }

            return dataTable;
        }

        public ArrayList Ejecutar(string q, IDataParameter[] parameters)
        {
            ArrayList list = new ArrayList();
            object[] values = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(q, this.conexion);
            command.CommandType = CommandType.Text;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null)
                {
                    command.Parameters.Add(parameters[i]);
                }
            }
            try
            {
                this.conexion.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    list.Add(values);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(ConfigurationManager.AppSettings["errorConexion"] + ":" + exception.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return list;
        }

        public ArrayList Ejecutar(string q, IDataParameter[] parameters, int nivelResultSet)
        {
            int num;
            ArrayList list = new ArrayList();
            object[] values = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(q, this.conexion);
            command.CommandType = CommandType.Text;
            for (num = 0; num < parameters.Length; num++)
            {
                if (parameters[num] != null)
                {
                    command.Parameters.Add(parameters[num]);
                }
            }
            try
            {
                this.conexion.Open();
                reader = command.ExecuteReader();
                for (num = 1; num < nivelResultSet; num++)
                {
                    reader.NextResult();
                }
                while (reader.Read())
                {
                    values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    list.Add(values);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(ConfigurationManager.AppSettings["errorConexion"] + ":" + exception.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return list;
        }

        public DataTable EjecutarDataTable(string q, IDataParameter[] parameters)
        {
            DataTable table = new DataTable("Datos");
            object[] values = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(q, this.conexion);
            command.CommandType = CommandType.Text;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.Add(parameters[i]);
            }
            try
            {
                this.conexion.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                bool flag = true;
                if (reader.Read())
                {
                    if (flag)
                    {
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            table.Columns.Add(reader.GetName(j));
                        }
                        flag = false;
                    }
                    values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    table.Rows.Add(values);
                }
                return table;
            }
            catch (Exception exception)
            {
                throw new Exception(ConfigurationManager.AppSettings["errorConexion"] + ":" + exception.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return table;
        }

        public int EjecutarTransaccionInsertar(string q1, IDataParameter[] parameters1, string q2, IDataParameter[] parameters2, int indexClave)
        {
            SqlTransaction transaction = null;
            int num = 0;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(q1, this.conexion);
            command.CommandType = CommandType.Text;
            try
            {
                int num2;
                this.conexion.Open();
                transaction = this.conexion.BeginTransaction();
                command.Transaction = transaction;
                command.CommandText = q1;
                for (num2 = 0; num2 < parameters1.Length; num2++)
                {
                    command.Parameters.Add(parameters1[num2]);
                }
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int.TryParse(reader.GetValue(0).ToString(), out indexClave);
                    num++;
                }
                reader.Close();
                command.CommandText = q2;
                command.Parameters.Clear();
                parameters2[indexClave].Value = num;
                for (num2 = 0; num2 < parameters2.Length; num2++)
                {
                    command.Parameters.Add(parameters2[num2]);
                }
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception exception)
            {
                transaction.Rollback();
                throw new Exception(ConfigurationManager.AppSettings["errorConexion"] + ":" + exception.Message);
            }
            finally
            {
                this.conexion.Close();
            }
            return num;
        }
        
        public int EjecutarTransaccionInsertarMasivo(string[] inserts, out string status)
        {
            SqlTransaction transaction = null;
            int num = 0;

            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    SqlCommand command = new SqlCommand("", this.conexion);
                    status = "";
                    int index = 0;
                    try
                    {
                        this.conexion.Open();
                        transaction = this.conexion.BeginTransaction();
                        command.Transaction = transaction;
                        for (index = 0; index < inserts.Length; index++)
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = inserts[index];
                            if (command.ExecuteNonQuery() != 0)
                            {
                                num++;
                            }
                            else
                            {
                                status = status + "no insertada: " + inserts[index] + "|";
                            }
                        }
                        //borrar tabla
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        status = status + "\nRollback";
                        status = status + "\n" + exception.Message;
                    }
                    finally
                    {
                        this.conexion.Close();
                    }
                }
            }
            else
            {
                SqlCommand command = new SqlCommand("", this.conexion);
                status = "";
                int index = 0;
                try
                {
                    this.conexion.Open();
                    transaction = this.conexion.BeginTransaction();
                    command.Transaction = transaction;
                    for (index = 0; index < inserts.Length; index++)
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = inserts[index];
                        if (command.ExecuteNonQuery() != 0)
                        {
                            num++;
                        }
                        else
                        {
                            status = status + "no insertada: " + inserts[index] + "|";
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    status = status + "\nRollback";
                    status = status + "\n" + exception.Message;
                }
                finally
                {
                    this.conexion.Close();
                }
            }

            return num;
        }

        public int EjecutarTransaccionInsertarMasivoSinCommit(string[] inserts, out string status)
        {
            SqlTransaction transaction = null;
            int num = 0;

            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    SqlCommand command = new SqlCommand("", this.conexion);
                    status = "";
                    int index = 0;
                    try
                    {
                        this.conexion.Open();
                        transaction = this.conexion.BeginTransaction();
                        command.Transaction = transaction;
                        index = 0;
                        while (index < inserts.Length)
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = inserts[index];
                            if (command.ExecuteNonQuery() != 0)
                            {
                                num++;
                            }
                            else
                            {
                                status = status + "no insertada: " + inserts[index] + "|";
                            }
                            index++;
                        }
                        status = status + "OK Pre Commit";
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        num = 0;
                        status = status + "\nRollback";
                        status = status + "\n" + exception.Message;
                        object obj2 = status;
                        status = string.Concat(new object[] { obj2, "\n Linea: ", index, ", ", inserts[index].Replace("'", "|") });
                    }
                    finally
                    {
                        this.conexion.Close();
                    }
                }
            }
            else
            {
                SqlCommand command = new SqlCommand("", this.conexion);
                status = "";
                int index = 0;
                try
                {
                    this.conexion.Open();
                    transaction = this.conexion.BeginTransaction();
                    command.Transaction = transaction;
                    index = 0;
                    while (index < inserts.Length)
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = inserts[index];
                        if (command.ExecuteNonQuery() != 0)
                        {
                            num++;
                        }
                        else
                        {
                            status = status + "no insertada: " + inserts[index] + "|";
                        }
                        index++;
                    }
                    status = status + "OK Pre Commit";
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    num = 0;
                    status = status + "\nRollback";
                    status = status + "\n" + exception.Message;
                    object obj2 = status;
                    status = string.Concat(new object[] { obj2, "\n Linea: ", index, ", ", inserts[index].Replace("'", "|") });
                    MessageBox.Show("ERROR: " + exception.Message + " n/ " + status);
                }
                finally
                {
                    this.conexion.Close();
                }
            }
            return num;
        }

        public static Conector getConn()
        {
            if (con == null)
            {
                return (con = new Conector());
            }
            return con;
        }

        public IDbConnection GetConnection()
        {
            return this.conexion;
        }

        public int InsertarIdentity(string q)
        {
            int num = 0;
            SqlDataReader rdr = null;
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        SqlCommand command = this.conexion.CreateCommand();
                        command.CommandText = q;
                        this.conexion.Open();
                        return command.ExecuteNonQuery();

                    }
                    catch (Exception exception)
                    {
                        string message = exception.Message;
                        MessageBox.Show("ERROR: " + exception.Message);
                    }
                    finally
                    {
                        this.conexion.Close();
                    }
                }
            }
            else
            {
                try
                {
                    this.conexion.Open();
                    SqlCommand command = this.conexion.CreateCommand();
                    command.CommandText = q;
                    //command.CommandType = CommandType.StoredProcedure;

                    rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        return int.Parse(rdr[0].ToString());
                    }
                    return 0;
                    //return command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                    MessageBox.Show("ERROR: " + exception.Message);
                }
                finally
                {
                    this.conexion.Close();
                }
            }
            return num;
        }


        /// <summary>
        /// Retorna un DatatTable con la información de BD
        /// </summary>
        /// <param name="Stord">Stord a ejecutar</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Objeto DatatTable con la coleccion de datos</returns>
        public DataTable Tabla(string Stord, string[,] Parametros = null)
        {

            DataTable dt = new DataTable("Datos");
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
                        {
                            if (Parametros != null)
                                for (int i = 0; i < Parametros.Length / 2; i++)
                                    cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                            cmd.CommandType = CommandType.StoredProcedure;
                            try
                            {
                                int i = 0;
                                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                {
                                    cmd.CommandTimeout = 0;
                                    cmd.Connection.Open();
                                    da.Fill(dt);
                                    var id = (int)cmd.ExecuteScalar();
                                    i = id;
                                    cmd.Connection.Close();
                                }
                                Utilidades.Constantes.id = i;
                                i=0;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("ERROR: " + e.Message);
                                cmd.Connection.Close();
                                throw new Exception(e.Message);
                            }
                            return dt;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR: " + ex.Message);
                        string x = ex.Message;
                    }
                }
            }
            else
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
                    {
                        if (Parametros != null)
                            for (int i = 0; i < Parametros.Length / 2; i++)
                                cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                        cmd.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                cmd.CommandTimeout = 0;
                                cmd.Connection.Open();
                                da.Fill(dt);
                                cmd.Connection.Close();
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("ERROR: " + e.Message);
                            cmd.Connection.Close();
                            throw new Exception(e.Message);
                        }
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                    string x = ex.Message;
                }
            }
            return dt;
        }
        /// <summary>
        /// Retorna un coleccion de Tablas con la información de BD
        /// </summary>
        /// <param name="Stord">nombre del stord a ejecutar</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Coleccion de tablas</returns>
        public DataSet Coleccion(string Stord, string[,] Parametros = null)
        {
            DataSet ds = new DataSet();
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection.Open();
                        da.Fill(ds);
                        cmd.Connection.Close();
                    }
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
                return ds;
            }


        }
        /// <summary>
        /// Retorna un coleccion de Tablas con la información de BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Coleccion de tablas</returns>

        public bool BulkCopy(DataTable TablaSube, string NombreTabla, bool TieneColumnaIdentidad)
        {
            DataTable dt = new DataTable("Datos");
            Stopwatch stopwatch = new Stopwatch();
            long tiempo;
            bool reply = true;
            if (this.conCredenciales)
            {
                using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("", this.conexion))
                        {
                              // Código sugerido para insertar DataTable en SQL Server

                            try
                            {
                                //cmd.CommandTimeout = 120;
                                this.conexion.Open();
                                foreach (DataColumn column in TablaSube.Columns)
                                {
                                    foreach (DataRow row in TablaSube.Rows)
                                    {
                                        if (row.IsNull(column) || string.IsNullOrEmpty(row[column].ToString()))
                                        {
                                            row[column] = DBNull.Value;
                                        }

                                    }
                                }
                                SqlBulkCopyOptions options = TieneColumnaIdentidad == true ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(this.conexion, options, null))
                                {
                                    bulkCopy.DestinationTableName = NombreTabla;
                                    bulkCopy.BulkCopyTimeout = 300;
                                    foreach (DataColumn column in TablaSube.Columns)
                                    {
                                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                                    }
                                    bulkCopy.WriteToServer(TablaSube);
                                }
                                this.conexion.Close();
                                reply = true;
                            }
                            catch (Exception ex)
                            {

                                reply = false;
                                this.conexion.Close();
                                throw new Exception("Error al insertar datos: " + ex.Message);
                            }

                            return reply;
                        }
                    }
                    catch (Exception ex)
                    {
                        reply = false;
                        this.conexion.Close();
                        string x = ex.Message;
                    }
                }
            }
            else
            {
                reply = false;

            }

            return reply;
        }

        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>"true" si se ejecuto el stord, Exception: error</returns>
        public bool Ejecutar(string Stord, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// Ejecuta un stord en la BD y retorna Identity
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>"true" si se ejecuto el stord, Exception: error</returns>
        public int EjecutarIdentity(string Stord, string[,] Parametros = null)
        {
            int identity = 0;
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();


                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT @@IDENTITY";
                    identity = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.Connection.Close();
                    return identity;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>regresa el numero de filas afectadas.</returns>
        public int EjecutarStoreExecuteReader(string Stord, string[,] Parametros = null)
        {
            int rowsAfected = 0;
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    rowsAfected = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return rowsAfected;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    return rowsAfected;
                }
            }
        }
        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Objeto DatatTable con la coleccion de datos</returns>
        public bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null)
        {//POSIBLE USO
            using (SqlCommand cmd = new SqlCommand(Stord, /*this.conexion*/ new SqlConnection(this.conect)))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        public int EjecutarInsertUpdate(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                int lastRow = 0;
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    lastRow = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.Connection.Close();
                    return lastRow;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        public int EjecutarInsertUpdate(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube2, String NombreTabla2, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                int lastRow = 0;
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.Parameters.Add(new SqlParameter(NombreTabla2, TablaSube2));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    lastRow = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.Connection.Close();
                    return lastRow;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        public DataTable Tabla(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                DataTable dt = new DataTable();
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection.Open();
                        da.Fill(dt);
                        cmd.Connection.Close();
                    }
                    return dt;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Regresa un objeto Datatable</returns>
        /// 
        public DataTable EjecutarDataAdapter(string Stord, DataTable TablaSube, String NombreTabla, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                DataTable dt = new DataTable();
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandTimeout = 0;
                        da.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception e)
                {

                    DataTable dtError = new DataTable("error");
                    DataColumn dataColumn = null;
                    for (int i = 0; i < 2; i++)
                    {
                        dataColumn = new DataColumn(i.ToString());
                        dtError.Columns.Add(dataColumn);
                    }

                    DataRow row = dtError.NewRow();
                    row["0"] = e.Message;
                    row["1"] = e.Message;
                    dtError.Rows.Add(row);
                    return dtError;
                }
            }
        }
        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Regresa un objeto Datatable</returns>
        /// 
        public DataTable EjecutarDataAdapter(string Stord, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, new SqlConnection(this.conect)))
            {
                DataTable dt = new DataTable();
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());

                cmd.CommandType = CommandType.StoredProcedure;

                try
                {

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandTimeout = 0;
                        da.Fill(dt);
                    }


                    // return dt;
                }
                catch (Exception e)
                {

                    //DataTable dtError = new DataTable("error");
                    DataColumn dataColumn = null;
                    for (int i = 0; i < 2; i++)
                    {
                        dataColumn = new DataColumn(i.ToString());
                        dt.Columns.Add(dataColumn);
                    }

                    DataRow row = dt.NewRow();
                    row["0"] = e.Message;
                    row["1"] = e.StackTrace;
                    dt.Rows.Add(row);
                    // return dtError;
                }
                finally
                {
                    cmd.Connection.Close();
                }
                return dt;
            }
        }
        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Objeto DatatTable con la coleccion de datos</returns>
        public bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube1, String NombreTabla1, string[,] Parametros = null)
        {
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla1, TablaSube1));
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }

            }
        }

        /// <summary>
        /// Ejecuta un stord en la BD
        /// </summary>
        /// <param name="Stord">Nombre del Stord a ejecutar</param>
        /// <param name="TablaSube">objeto DataTable que envía al stord</param>
        /// <param name="NombreTabla">nombre del parametro de tabla</param>
        /// <param name="Parametros">Parametros que requiere el stord</param>
        /// <returns>Objeto DatatTable con la coleccion de datos</returns>
        public bool Ejecutar(string Stord, DataTable TablaSube, String NombreTabla, DataTable TablaSube1, String NombreTabla1, DataTable TablaSube2, String NombreTabla2, string[,] Parametros = null)
        {

            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla2, TablaSube2));
                cmd.Parameters.Add(new SqlParameter(NombreTabla1, TablaSube1));
                cmd.Parameters.Add(new SqlParameter(NombreTabla, TablaSube));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }

        public int Ejecutar(string Stord, DataTable TablaSube1, String NombreTabla1, DataTable TablaSube2, String NombreTabla2, DataTable TablaSube3, String NombreTabla3, DataTable TablaSube4, String NombreTabla4, DataTable TablaSube6, String NombreTabla6, DataTable TablaSube5, String NombreTabla5, string[,] Parametros = null)
        {

            DataTable dt = new DataTable();
            int lastRow = 0;
            using (SqlCommand cmd = new SqlCommand(Stord, this.conexion))
            {
                if (Parametros != null)
                    for (int i = 0; i < Parametros.Length / 2; i++)
                        cmd.Parameters.AddWithValue(Parametros[i, 0].ToString(), Parametros[i, 1].ToString());
                cmd.Parameters.Add(new SqlParameter(NombreTabla1, TablaSube1));
                cmd.Parameters.Add(new SqlParameter(NombreTabla2, TablaSube2));
                cmd.Parameters.Add(new SqlParameter(NombreTabla3, TablaSube3));
                cmd.Parameters.Add(new SqlParameter(NombreTabla4, TablaSube4));
                cmd.Parameters.Add(new SqlParameter(NombreTabla6, TablaSube6));
                cmd.Parameters.Add(new SqlParameter(NombreTabla5, TablaSube5));
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.CommandTimeout = 0;
                    cmd.Connection.Open();
                    lastRow = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.Connection.Close();
                    return lastRow;
                }
                catch (Exception e)
                {
                    cmd.Connection.Close();
                    throw new Exception(e.Message);
                }



            }
        }


        public void ExecuteProcedure(string storeProcedure, bool useDataTable, string connectionString, IEnumerable<long> ids, string[,] Parametros = null)
        {
            using (SqlConnection connection = new SqlConnection(this.conexion.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = storeProcedure;
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameter;
                    if (useDataTable)
                    {
                        parameter = command.Parameters.AddWithValue("@Display", CreateDataTable(ids));
                    }
                    else
                    {
                        parameter = command.Parameters.AddWithValue("@Display", CreateSqlDataRecords(ids));
                    }
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "dbo.PageViewTableType";

                    command.ExecuteNonQuery();
                }
            }
        }

        private static DataTable CreateDataTable(IEnumerable<long> ids)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(long));
            foreach (long id in ids)
            {
                table.Rows.Add(id);
            }
            return table;
        }

        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<long> ids)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];
            metaData[0] = new SqlMetaData("ID", SqlDbType.BigInt);
            SqlDataRecord record = new SqlDataRecord(metaData);
            foreach (long id in ids)
            {
                record.SetInt64(0, id);
                yield return record;
            }
        }

        public DataSet Coleccion(string Stord, DataTable TablaSube, string NombreTabla, string[,] Parametros = null)
        {
            throw new NotImplementedException();
        }

        public DataTable Tabla(object sp_upin_get_list_backup_assignment, string[,] parametro)
        {
            return ((IFamiliaConector)con).Tabla(sp_upin_get_list_backup_assignment, parametro);
        }
    }
}
