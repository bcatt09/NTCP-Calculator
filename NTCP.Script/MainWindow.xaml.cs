using MahApps.Metro.Controls;
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

namespace NTCP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler((obj, e) => { if (e.Key == Key.Escape) this.Close(); });
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InfoWindow infoWindow = new InfoWindow();
                infoWindow.Show();

                infoWindow.KeyDown += (o, e1) => { if (e1.Key == Key.Escape) (o as InfoWindow).Close(); };
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n\n{ex.InnerException}\n\n{ex.StackTrace}");
            }
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                //if (binding != null) 
                binding.UpdateSource();
            }
        }
    }
}
