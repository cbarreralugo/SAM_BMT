using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM_BMT.Modelo
{
    public class Publicacion_Modelo
    {
        public int id_publicacion {  get; set; }
        public string name_app { get; set; }
        public string ruta_origen { get; set; }
        public string servidor { get; set; }
        public int tipo_publicacion { get; set; }
        public int tipo_app { get; set; }
        public bool sql { get; set; }
        public string status { get;set; }
        public bool activo {  get; set; }
    }
}
