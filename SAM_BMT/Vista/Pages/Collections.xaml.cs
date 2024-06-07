using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System;
using SAM_BMT.Modelo;
using SAM_BMT.Controlador;

namespace SAM_BMT.Vista.Pages
{
    public partial class Collections : Page
    {
        public ObservableCollection<ScriptFile> ScriptFiles { get; set; }

        public Collections()
        {
            InitializeComponent();
            ScriptFiles = new ObservableCollection<ScriptFile>();
            dgScripts.ItemsSource = ScriptFiles;
        }

        private void BtnSelectSolution_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                txtSolutionPath.Text = dialog.FileName;

                // Detectar y cargar el archivo de configuración
                string configFile = Directory.GetFiles(Path.GetDirectoryName(dialog.FileName), "web.config", SearchOption.AllDirectories).FirstOrDefault() ??
                                    Directory.GetFiles(Path.GetDirectoryName(dialog.FileName), "app.config", SearchOption.AllDirectories).FirstOrDefault();

                if (configFile != null)
                {
                    txtConfigPath.Text = configFile;
                    txtConfigContent.Text = File.ReadAllText(configFile);
                }
            }
        }

        private void BtnSelectConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Config Files (web.config, app.config)|web.config;app.config",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                txtConfigPath.Text = dialog.FileName;
                txtConfigContent.Text = File.ReadAllText(dialog.FileName);
            }
        }

        private void BtnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtConfigPath.Text))
            {
                File.WriteAllText(txtConfigPath.Text, txtConfigContent.Text);
                MessageBox.Show("Archivo de configuración guardado correctamente.", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSelectScripts_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "SQL Files (*.sql)|*.sql",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    ScriptFiles.Add(new ScriptFile { FileName = Path.GetFileName(fileName), FilePath = fileName });
                }
            }
        }

        private void DgScripts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgScripts.SelectedItem is ScriptFile selectedFile)
            {
                txtScriptContent.Text = File.ReadAllText(selectedFile.FilePath);
            }
        }

        private void BtnSelectPublishPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPublishPath.Text = dialog.SelectedPath;
            }
        }

        private async void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            Publicacion_Modelo modelo = new Publicacion_Modelo();
            // Cambiar al tab de Consola
            tabControl.SelectedIndex = 2;

            string projectPath = txtSolutionPath.Text;
            string publishPath = txtPublishPath.Text;
            string projectType = (cmbProjectType.SelectedItem as ComboBoxItem)?.Content.ToString();
            string environment = (cmbEnvironment.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(projectPath) || string.IsNullOrEmpty(publishPath) || string.IsNullOrEmpty(projectType) || string.IsNullOrEmpty(environment))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de publicar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            progressBar.Value = 0;
            txtProgress.Text = "0% - Iniciando publicación";
            AppendToConsole("Iniciando publicación...");

            if (projectType == "Web")
            {
                await PublishWebApp(projectPath, publishPath, modelo);
            }
            else if (projectType == "Escritorio")
            {
                modelo.tipo_publicacion = 1;
                modelo.tipo_app = 1;
                await PublishDesktopApp(projectPath, publishPath, modelo);
            }

            AppendToConsole("Ejecutando scripts SQL...");
            modelo.sql = false;
            modelo.status = "Completada";
            modelo.activo = true;
            progressBar.Value = 100;
            txtProgress.Text = "100% - Publicación completada";
            AppendToConsole("Publicación completada.");
            MessageBox.Show("Publicación terminada", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            Publicacion_Controlador.I.Publicar(modelo);
        }

        private async Task PublishWebApp(string projectPath, string publishPath, Publicacion_Modelo modelo)
        {
            AppendToConsole("Publicando aplicación web...");
            var processInfo = new ProcessStartInfo("dotnet", $"publish \"{projectPath}\" -c Release -o \"{publishPath}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();

            await ReadProcessOutputAsync(process);

            if (process.ExitCode == 0)
            {
                AppendToConsole("Publicación web completada.");
            }
            else
            {
                AppendToConsole("Error en la publicación web.");
                MessageBox.Show("Error en la publicación web.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            progressBar.Value = 60;
            txtProgress.Text = "60% - Publicación web completada";
        }

        private async Task PublishDesktopApp(string solutionPath, string publishPath, Publicacion_Modelo modelo)
        {
            AppendToConsole("Publicando aplicación de escritorio...");

            string msbuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
            if (!File.Exists(msbuildPath))
            {
                AppendToConsole("No se encontró msbuild.exe en la ruta especificada. Asegúrese de que MSBuild esté instalado.");
                MessageBox.Show("No se encontró msbuild.exe en la ruta especificada. Asegúrese de que MSBuild esté instalado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string solutionDir = Path.GetDirectoryName(solutionPath);
            string projectFile = Directory.GetFiles(solutionDir, "*.csproj", SearchOption.AllDirectories)
                                          .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == Path.GetFileNameWithoutExtension(solutionPath));

            if (string.IsNullOrEmpty(projectFile))
            {
                AppendToConsole("No se encontró el archivo de proyecto principal.");
                MessageBox.Show("No se encontró el archivo de proyecto principal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var processInfo = new ProcessStartInfo(msbuildPath, $"\"{projectFile}\" /t:Publish /p:Configuration=Release /p:PublishDir=\"{publishPath}\" /p:PublishProfile=FolderProfile /p:RuntimeIdentifier=win-x86")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();

            await ReadProcessOutputAsync(process);

            if (process.ExitCode == 0)
            {
                AppendToConsole("Publicación de escritorio completada.");

                // Determinar el nombre de la aplicación publicada
                string appName = Path.GetFileNameWithoutExtension(projectFile) + ".application";
                string destinationPath = $@"\\"+txt_ip_server.Text+@"\Apps\Desktop\{appName}";
                modelo.name_app = appName;
                modelo.ruta_origen = destinationPath;
                modelo.servidor = txt_ip_server.Text;
                CopyFilesToServer(publishPath, destinationPath); // Ajusta la ruta de destino según sea necesario
            }
            else
            {
                AppendToConsole("Error en la publicación de escritorio.");
                MessageBox.Show("Error en la publicación de escritorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            progressBar.Value = 60;
            txtProgress.Text = "60% - Publicación de escritorio completada";
        }
        private async Task ReadProcessOutputAsync(Process process)
        {
            var tcs = new TaskCompletionSource<bool>();

            process.Exited += (sender, args) => tcs.SetResult(true);
            process.EnableRaisingEvents = true;

            while (!process.StandardOutput.EndOfStream)
            {
                string line = await process.StandardOutput.ReadLineAsync();
                AppendToConsole(line);
            }

            await tcs.Task;
        }
        private void CopyFilesToServer(string sourcePath, string destinationPath)
        {
            try
            {
                // Create all directories in destination path if they do not exist
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }

                // Copy all the files & replace any files with the same name
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }

                AppendToConsole("Archivos copiados al servidor correctamente.");
            }
            catch (Exception ex)
            {
                AppendToConsole($"Error al copiar archivos al servidor: {ex.Message}");
                MessageBox.Show($"Error al copiar archivos al servidor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string FindMSBuildPath()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var possiblePaths = new[]
            {
        Path.Combine(programFiles, @"Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"),
        Path.Combine(programFiles, @"Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"),
    };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Search the system PATH for msbuild.exe
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (pathEnv != null)
            {
                var paths = pathEnv.Split(Path.PathSeparator);
                foreach (var path in paths)
                {
                    var msbuildPath = Path.Combine(path, "msbuild.exe");
                    if (File.Exists(msbuildPath))
                    {
                        return msbuildPath;
                    }
                }
            }

            return null;
        }


        private void AppendToConsole(string message)
        {
            txtConsole.Dispatcher.Invoke(() =>
            {
                txtConsole.AppendText($"{message}\n");
                txtConsole.ScrollToEnd();
            });
        }

        private void ExecuteSqlScripts()
        {
            
            foreach (var scriptFile in ScriptFiles)
            {
                string scriptContent = File.ReadAllText(scriptFile.FilePath);
                // Implementar lógica para ejecutar scripts SQL en el servidor de base de datos
                // Esto puede hacerse utilizando ADO.NET, Dapper, o cualquier otra biblioteca de acceso a datos
                // Ejemplo básico:
                /*
                using (var connection = new SqlConnection("your_connection_string"))
                {
                    connection.Open();
                    using (var command = new SqlCommand(scriptContent, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                */
                AppendToConsole($"Ejecutando script: {scriptFile.FileName}");
            }
        }

        private void btn_ExecuteSQL_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Este dispositivo aún no tiene permisos para ejecutar scripts.\nSolo cuentas con permisos de visualizar.",
                "Solicita permisos con el administrador", MessageBoxButton.OK, MessageBoxImage.Error);
            //progressBar.Value = 80;
            //txtProgress.Text = "80% - Ejecutando scripts SQL";
            //ExecuteSqlScripts();
        }
    }

    public class ScriptFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
