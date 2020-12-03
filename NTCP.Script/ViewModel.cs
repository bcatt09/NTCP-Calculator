using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using static VMS.TPS.Common.Model.Types.DoseValue;

namespace NTCP
{
    class ViewModel : ViewModelBase
    {
        public ViewModel(Patient patient, PlanSetup plan)
        {
            _patient = patient;
            _plan = plan;
        }

        private ParameterSet _parameterSet;
        public ParameterSet ParameterSet
        {
            get { return _parameterSet; }
            set { Set(ref _parameterSet, value); }
        }

        private Patient _patient;
        public Patient Patient { get { return _patient; } }

        private PlanSetup _plan;
        public PlanSetup Plan { get { return _plan; } }

        private Structure _selectedStructure;
        public Structure SelectedStructure
        {
            get { return _selectedStructure; }
            set { Set(ref _selectedStructure, value); }
        }

        private ParameterSet _selectedDefaultParameterSet;
        public ParameterSet SelectedDefaultParameterSet
        {
            get { return _selectedDefaultParameterSet; }
            set { Set(ref _selectedDefaultParameterSet, value); }
        }

        private double _ntcp;
        public double NTCP
        {
            get { return _ntcp; }
            set { Set(ref _ntcp, value); }
        }

        private double _ntcpEUD;
        public double NTCPEUD
        {
            get { return _ntcpEUD; }
            set { Set(ref _ntcpEUD, value); }
        }

        public List<ParameterSet> DefaultParameterSets = new List<ParameterSet>
        { 
            new ParameterSet
            {
                Name = "Lung",
                Type = ParameterSetType.Lung,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(3080, DoseUnit.cGy),
                n = 0.99,
                m = 0.37
            },
            new ParameterSet
            {
                Name = "Liver Primary",
                Type = ParameterSetType.LiverPrimary,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(3540, DoseUnit.cGy),
                n = 0.97,
                m = 0.12
            },
            new ParameterSet
            {
                Name = "Liver Mets",
                Type = ParameterSetType.LiverMet,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(4070, DoseUnit.cGy),
                n = 0.97,
                m = 0.12
            }
        };

        public RelayCommand SelectNewDefaultParameterSetCommand = new RelayCommand(SelectNewDefaultParameterSet);

        public void SelectNewDefaultParamaterSet()
        {
            var set = DefaultParameterSets.Where(x => x.Type == ParameterSetType.Lung).Single();
            ParameterSet.AlphaBeta = set.AlphaBeta;
            ParameterSet.TD50 = set.TD50;
            ParameterSet.n = set.n;
            ParameterSet.m = set.m;
        }
    }
}
