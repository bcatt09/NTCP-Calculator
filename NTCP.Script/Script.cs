using NTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext context)
        {
            MainViewModel viewModel = new MainViewModel(context.Patient, context.PlanSetup);
            MainWindow window = new MainWindow()
            {
                DataContext = viewModel
            };
            window.ShowDialog();
        }
    }
}
