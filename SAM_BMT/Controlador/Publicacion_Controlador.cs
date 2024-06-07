using EncryptionLibrary;
using SAM_BMT.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SAM_BMT.Controlador
{
    public class Publicacion_Controlador
    {
        private static Publicacion_Controlador _instancia;
        AESEncryptor AES;

        public static Publicacion_Controlador I
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new Publicacion_Controlador();
                }
                return _instancia;
            }
        }
        public DataTable Obtener_AppsPublicadas()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = ConnectorLibrary.App.GetCurrentConnector().Tabla(StoredProcedure.SP.sp_bmt_obtener_app_publicadas);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
            finally { dt = null; }
        }

        public void Publicar(Publicacion_Modelo modelo)
        {
            try
            {
                string[,] parametro =
                {
                    {"@nombre",modelo.name_app },

                    {"@ruta",modelo.ruta_origen },

                    {"@servidor",modelo.servidor },

                    {"@tipo_publicacion",modelo.tipo_publicacion.ToString() },

                    {"@tipo_app",modelo.tipo_app.ToString() },

                    {"@sql",modelo.sql.ToString() },

                    {"@estatus",modelo.status }
                };
                ConnectorLibrary.App.GetCurrentConnector().Tabla(StoredProcedure.SP.sp_bmt_crear_app_publicada,parametro); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error"); 
            }
        }
    }
}


