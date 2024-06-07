using EncryptionLibrary;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SAM_BMT.Vista.Pages
{
    /// <summary>
    /// Lógica de interacción para Encryption.xaml
    /// </summary>
    public partial class Encryption : Page
    {
        public Encryption()
        {
            InitializeComponent();
        }

        private void btn_LimpiarKey_Click(object sender, RoutedEventArgs e)
        {
            this.txt_Key.Password = "";
        }

        private void btn_GuardarKey_Click(object sender, RoutedEventArgs e)
        {
            // Implementación de guardar la clave
        }

        private void btn_AES_limpiar_Click(object sender, RoutedEventArgs e)
        {
            txt_Enc_AES.Text = "";
            txt_Text_AES.Text = "";
        }

        private void btn_AES_En_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string entrada = txt_Text_AES.Text;
                string key = txt_Key.Password;

                if (!string.IsNullOrEmpty(key))
                {
                    if (!string.IsNullOrEmpty(entrada))
                    {
                        AESEncryptor encryptor = new AESEncryptor(key);
                        txt_Enc_AES.Text = encryptor.Encrypt(entrada);
                    }
                    else
                    {
                        MessageBox.Show("Ingresa la cadena de texto a encriptar.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Ingresa la key para encriptar.", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al encriptar la cadena: \n" + ex.Message, "Error");
            }
        }

        private void btn_AES_Des_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string entrada = txt_Enc_AES.Text;
                string key = txt_Key.Password;

                if (!string.IsNullOrEmpty(key))
                {
                    if (!string.IsNullOrEmpty(entrada))
                    {
                        AESEncryptor encryptor = new AESEncryptor(key);
                        txt_Text_AES.Text = encryptor.Decrypt(entrada);
                    }
                    else
                    {
                        MessageBox.Show("Ingresa la cadena encriptada que deseas desencriptar.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Ingresa la key para desencriptar.", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desencriptar la cadena: \n" + ex.Message, "Error");
            }
        }
    }
}
