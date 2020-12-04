using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using static VMS.TPS.Common.Model.Types.DoseValue;

namespace NTCP
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(Patient patient, PlanSetup plan)
        {
            _patient = patient;
            _plan = plan;
            ParameterSet = new ParameterSet();

            SelectedDefaultParameterSet = DefaultParameterSets.First();
        }

        private ParameterSet _parameterSet;
        public ParameterSet ParameterSet
        {
            get { return _parameterSet; }
            set 
            {
                Set(ref _parameterSet, value);
                RaisePropertyChanged(() => AlphaBetaDose);
                RaisePropertyChanged(() => TD50Dose);
            }
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
            set 
            {
                Set(ref _selectedDefaultParameterSet, value);
                SelectNewDefaultParameterSet();
            }
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

        public double AlphaBetaDose
        {
            get { return ParameterSet.AlphaBeta.Dose; }
            set { ParameterSet.AlphaBeta = new DoseValue(value, ParameterSet.AlphaBeta.Unit); }
        }

        public double TD50Dose
        {
            get { return ParameterSet.TD50.Dose; }
            set { ParameterSet.TD50 = new DoseValue(value, ParameterSet.TD50.Unit); }
        }

        public string PatientName { get { return Patient.Name; } }

        public List<ParameterSet> DefaultParameterSets { get; } = new List<ParameterSet>()
        {
            new ParameterSet()
            {
                Name = "Lung",
                Type = ParameterSetType.Lung,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(3080, DoseUnit.cGy),
                n = 0.99,
                m = 0.37
            },
            new ParameterSet()
            {
                Name = "Liver Primary",
                Type = ParameterSetType.LiverPrimary,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(3540, DoseUnit.cGy),
                n = 0.97,
                m = 0.12
            },
            new ParameterSet()
            {
                Name = "Liver Mets",
                Type = ParameterSetType.LiverMet,
                AlphaBeta = new DoseValue(2.5, DoseUnit.Gy),
                TD50 = new DoseValue(4070, DoseUnit.cGy),
                n = 0.97,
                m = 0.12
            }
        };

        public void SelectNewDefaultParameterSet()
        {
            var set = DefaultParameterSets.Where(x => x.Type == SelectedDefaultParameterSet.Type).Single();
            ParameterSet = new ParameterSet()
            {
                AlphaBeta = set.AlphaBeta,
                TD50 = set.TD50,
                n = set.n,
                m = set.m
            };
        }
    }
}
