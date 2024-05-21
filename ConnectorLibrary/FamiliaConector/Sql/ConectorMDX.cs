using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ConnectorLibrary.FamiliaConector.Sql
{
    public class ConectorMDX : IFamiliaConectorMDX
    {
        private string conect;
        private OleDbConnection conexion;

        public ConectorMDX()
        {

            this.conect = string.Empty;
            this.conect = ConfigurationManager.ConnectionStrings["CNN_Cube"].ConnectionString;
            this.conexion = new OleDbConnection(this.conect);
        }

        public System.Data.DataTable Ejecutar(string q)
        {
            DataTable tabla = new DataTable("tablaDatos");

            using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
            {
                try
                {
                    OleDbDataAdapter adap = new OleDbDataAdapter(q, this.conect);
                    adap.Fill(tabla);
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                    throw ex;
                }
            }
            return tabla;
        }

        public System.Data.DataTable Ejecutar(string q, string conexion)
        {
            DataTable tabla = new DataTable("tablaDatos");
            using (new Impersonator(UsuarioServicio.GetCurrentUsuarioServicio().NombreUsuario, UsuarioServicio.GetCurrentUsuarioServicio().Dominio, UsuarioServicio.GetCurrentUsuarioServicio().Pass))
            {
                try
                {
                    OleDbDataAdapter adap = new OleDbDataAdapter(q, conexion);
                    adap.Fill(tabla);
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                    throw ex;
                }
            }
            return tabla;
        }
    }
}
