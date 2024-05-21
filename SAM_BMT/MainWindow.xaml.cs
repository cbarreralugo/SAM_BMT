﻿using System;
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
using System.Runtime.Remoting.Messaging;
using SAM_BMT.Vista.Pages;

namespace SAM_BMT
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ventanaDefault();
        }

        private void ventanaDefault()
        {
            rdCollections_Click(null,null);
        }

        private void Themes_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void rdHome_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Home());
        }

        private void rdAnalytics_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Analytics());
        }

        private void rdMessages_Click(object sender, RoutedEventArgs e)
        {
         //   frameContent.Navigate(new Messages());
        }

        private void rdCollections_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Collections());
        }

        private void rdUsers_Click(object sender, RoutedEventArgs e)
        {
          //  frameContent.Navigate(new Users());
        }
    }
}
