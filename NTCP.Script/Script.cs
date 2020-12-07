using NTCP;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext context)
        {
            if (context.PlanSetup != null)
            {
                MainViewModel viewModel = new MainViewModel(context);
                MainWindow window = new MainWindow()
                {
                    DataContext = viewModel
                };
                window.ShowDialog();
            }
            else 
                MessageBox.Show("Please select a single plan before running this script (plan sums are not supported)", "Invalid Plan", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
