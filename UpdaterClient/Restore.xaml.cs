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

namespace UpdaterClient
{
    /// <summary>
    /// Interaction logic for Restore.xaml
    /// </summary>
    public partial class Restore : Window
    {
        Patcher.Tornado2000S.PatchLauncher launcher = new Patcher.Tornado2000S.PatchLauncher();
       
        TextBoxOutputter txtBoxPutter;
        public Restore()
        {
            InitializeComponent();
        }

        public void ResetLauncher(string appDir) {
            launcher.TargetRootDir = appDir;
        }

        void UpdateHistorys()
        {
            HistoryList.Items.Clear();
            var enumer = GetHistorys().GetEnumerator();
            while (enumer.MoveNext())
            {
                var item = new ListViewItem { Content = enumer.Current };
                HistoryList.Items.Add(item);
            }
        }

        IEnumerable<string> GetHistorys()
        {
            var history = launcher.LocalDataDirectory("history");
            if (history is null) {
                yield break;
            }
            var dir = new System.IO.DirectoryInfo(history);
            if (!dir.Exists)
            {
                yield break;
            }
            var dirs = dir.GetDirectories();
            foreach (var item in dirs)
            {
                if (int.TryParse(item.Name.Split('.')[1], out int dataVersion))
                {
                    yield return item.Name;
                }
                else
                {
                    continue;
                }
            }
        }

        private void btn_restore_Click(object sender, RoutedEventArgs e)
        {
            var items = HistoryList.SelectedItems;
            if (items.Count != 1)
            {
                MessageBox.Show("Select only one history item, please.");
                return;
            }
            var dirName = (items[0] as ListViewItem).Content as string;
            var dir = new System.IO.DirectoryInfo(System.IO.Path.Combine(launcher.LocalDataDirectory("history"), dirName));
            Task.Run(delegate
            {
                launcher.Restore(dir.FullName, int.Parse(dirName.Split('.')[1]));
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBoxPutter = new TextBoxOutputter(tb_console);
            Console.SetOut(txtBoxPutter);
            launcher.Logger = new GuiltySpark.ConsoleLogger { TextWriter = txtBoxPutter };
            UpdateHistorys();
        }
    }
}
