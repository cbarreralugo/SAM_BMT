using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ConnectorLibrary
{
    public class UsuarioServicio
    {
        private string nombreUsuario;
        private string dominio;
        private string pass;
        private string encriptado;

        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set { nombreUsuario = value; }
        }

        public string Dominio
        {
            get { return dominio; }
            set { dominio = value; }
        }

        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }

        public string Encriptado
        {
            get { return encriptado; }
            set { encriptado = value; }
        }

        public UsuarioServicio(string nombreUsuario, string dominio, string pass)
        {
            this.nombreUsuario = nombreUsuario;
            this.dominio = dominio;
            this.pass = pass;
        }

        public UsuarioServicio(string encriptado)
        {
            this.encriptado = encriptado;
            string[] parametros = (encriptado).Split('|');
            //string[] parametros = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(this.encriptado)).Split('|');
            if (parametros.Length >= 3)
            {
                this.nombreUsuario = parametros[0];
                this.dominio = parametros[1];
                this.pass = parametros[2];
            }
        }

        private static UsuarioServicio currentUsuarioServicio = null;
        public static UsuarioServicio GetCurrentUsuarioServicio()
        {
            if (currentUsuarioServicio == null)
                if (ConfigurationManager.AppSettings["pass_servicio"] != null)
                    UsuarioServicio.currentUsuarioServicio = new UsuarioServicio(ConfigurationManager.AppSettings["pass_servicio"].ToString());
            return currentUsuarioServicio;
        }
    }
}

