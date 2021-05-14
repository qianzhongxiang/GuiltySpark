using GuiltySpark;
using System;
using System.Collections.Generic;
using System.IO;
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
using Serilog;

namespace UpdaterClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Patcher.Tornado2000S.PatchLauncher launcher;
        public string ApplicationDir
        {
            get => this.rootDir.Content.ToString(); set
            {
                this.rootDir.Content = value;
            }
        }
        public MainWindow()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            InitializeComponent();
            
        }
        private void UpdateItems()
        {
            try
            {
                var items = launcher.GetItems(0, int.MaxValue, false);
                dataGrid.ItemsSource = items.Select(i =>
                {
                    var item = i as LocalDataItem;
                    return new { dir = new System.IO.DirectoryInfo(item.Directory).Name, version = item.Info.DataVersion };
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ApplicationDir = dialog.SelectedPath;
                    launcher.TargetRootDir = dialog.SelectedPath;
                    UpdateItems();
                }
                else
                {
                    return;
                }
            }
        }

        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            launcher.TargetRootDir = ApplicationDir;
            Console.WriteLine($"launcher.TargetRootDir:{ launcher.TargetRootDir }");
            try
            {
                Task.Run(delegate
                {
                    launcher.Run();
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Windows.Forms.MethodInvoker)delegate { UpdateItems(); });
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btn_restore_Click(object sender, RoutedEventArgs e)
        {
            var ww = new Restore();
            ww.ResetLauncher(launcher.TargetRootDir);
            ww.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Information("1");
            var textBoxOutputter = new TextBoxOutputter(consoleViewer);
            Console.SetOut(textBoxOutputter);
            Log.Information("2");
            launcher = new Patcher.Tornado2000S.PatchLauncher();
            launcher.Logger = new ConsoleLogger { TextWriter = textBoxOutputter };
            Log.Information("3");
            var infos = launcher.GetPatchInfos();
            patchsViewer.Items.Clear();
            Log.Information("4");
            ApplicationDir = launcher.TargetRootDir;
            foreach (var item in infos)
            {
                patchsViewer.Items.Add(new ListViewItem { Height = 32, Content = $"Data Patch : v{item.DataVersion}" });
                Console.WriteLine($"loaded a patch:v{item.DataVersion}");
            }
            Log.Information("5");

            UpdateItems();
        }
    }

    public class TextBoxOutputter : TextWriter
    {
        TextBox textBox = null;

        public TextBoxOutputter(TextBox output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.AppendText(value.ToString());
            }));
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }


}
