
using System.Windows;
using System.Windows.Controls;

namespace MC_SVGeneralDatCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Control control;

        public MainWindow()
        {
            InitializeComponent();
            control = new Control();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void starvalorpath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (starvalorpath.Text == "")
                return;

            if (!control.SetPath(starvalorpath.Text))
            {
                MessageBox.Show("Star Valor.exe could not be found in the selected path.");
                starvalorpath.Text = "";
            }
        }

        private void pathbrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                    starvalorpath.Text = dialog.SelectedPath;
            }
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            if (control.DeserialiseGeneraDat())
            {
                listBox.ItemsSource = control.ShipList;
                listBox.Items.Refresh();
            }
            else
                MessageBox.Show("Failed to load general.dat.  Ensure you have set the correct path to the Star Valor folder.");
        }

        private void listBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                DeleteItem();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }

        private void DeleteItem()
        {
            if (listBox.SelectedItem == null)
                return;

            if(control.RemoveShip((int)listBox.SelectedItem))
            {
                listBox.ItemsSource = control.ShipList;
                listBox.Items.Refresh();
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "Data Files (*.dat)|*.dat";
                dialog.FileName = "general.dat";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    if (!control.SerialiseGeneralDat(dialog.FileName))
                        MessageBox.Show("Failed to save general.dat");
            }
        }
    }
}
