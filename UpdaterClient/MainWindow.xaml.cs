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

namespace UpdaterClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PatchClient.PatchLauncher launcher;
        public string ApplicationDir
        {
            get => this.rootDir.Content.ToString(); set
            {
                this.rootDir.Content = value;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            var textBoxOutputter = new TextBoxOutputter(consoleViewer);
            Console.SetOut(textBoxOutputter);
            launcher = new PatchClient.PatchLauncher();
        
            var infos = launcher.GetPatchInfos();
            patchsViewer.Items.Clear();
            ApplicationDir = launcher.TargetRootDir;
            foreach (var item in infos)
            {
                patchsViewer.Items.Add(new ListViewItem { Height = 32, Content = $"patch:v{item.DataVersion}" });
                Console.WriteLine($"loaded a patch:v{item.DataVersion}");
            }

            UpdateItems();
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
                    launcher.TargetRootDir = ApplicationDir;
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
            try
            {
                launcher.Run();
                UpdateItems();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
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
