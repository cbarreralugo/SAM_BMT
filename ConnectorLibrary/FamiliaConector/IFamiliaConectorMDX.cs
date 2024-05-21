using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorLibrary.FamiliaConector
{
    public  interface IFamiliaConectorMDX
    {
        DataTable Ejecutar(string q);
        DataTable Ejecutar(string q, string conexion);
    }
}
