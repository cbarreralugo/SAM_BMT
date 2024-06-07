﻿using SAM_BMT.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SAM_BMT.Vista.Pages
{
    /// <summary>
    /// Lógica de interacción para Analytics.xaml
    /// </summary>
    public partial class Analytics : Page
    {
        public Analytics()
        {
            InitializeComponent();
            CargarTabla();
        }

        private void CargarTabla()
        {
            
            dg_appPublicadas.ItemsSource = null;
            dg_appPublicadas.ItemsSource = (Publicacion_Controlador.I.Obtener_AppsPublicadas()).DefaultView;
        }
    }
}
