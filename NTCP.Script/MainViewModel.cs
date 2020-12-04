﻿using GalaSoft.MvvmLight;
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
        private bool _initialLoad = true;
        // Constructor
        public MainViewModel(Patient patient, PlanSetup plan)
        {
            var excludedDicomTypes = new List<string> { "BODY", "EXTERNAL", "SUPPORT", "MARKER" };

            _patient = patient;
            _plan = plan;
            ParameterSet = new ParameterSet();
            StructureList = Plan.StructureSet.Structures.Where(x => !x.IsEmpty && !excludedDicomTypes.Contains(x.DicomType)).ToList();

            _initialLoad = false;
            SelectedStructure = StructureList.First();
            SelectedDefaultParameterSet = DefaultParameterSets.First();
        }

        // Data members
        private ParameterSet _parameterSet;
        public ParameterSet ParameterSet
        {
            get { return _parameterSet; }
            set 
            {
                Set(ref _parameterSet, value);
                RaisePropertyChanged(() => AlphaBetaDose);
                RaisePropertyChanged(() => TD50Dose);
                if (!_initialLoad) GetEqd2Data();
            }
        }

        private Patient _patient;
        public Patient Patient { get { return _patient; } }

        private PlanSetup _plan;
        public PlanSetup Plan { get { return _plan; } }

        private List<Structure> _structureList;
        public List<Structure> StructureList
        {
            get { return _structureList; }
            set { Set(ref _structureList, value); }
        }

        private Structure _selectedStructure;
        public Structure SelectedStructure
        {
            get { return _selectedStructure; }
            set
            {
                Set(ref _selectedStructure, value);
                GetDifferentialDVH();
                RaisePropertyChanged(() => MeanDose);
            }
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

        private DoseValue _meanEqd2Dose;
        public DoseValue MeanEqd2Dose
        {
            get { return _meanEqd2Dose; }
            set 
            { 
                Set(ref _meanEqd2Dose, value);
                //RaisePropertyChanged(() => MeanEqd2DoseDisplay);
            }
        }

        // Private Members for use with NTCP Calculations
        private DVHPoint[] diffDvhData;
        private DVHPoint[] eqd2DvhData;

        // Display fields
        public string PatientName { get { return Patient.Name; } }
        public string PlanName { get { return Plan.Id; } }
        public string TotalDose { get { return Plan.TotalDose.ToString(); } }
        public string NumberOfFractions { get { return Plan.NumberOfFractions.ToString(); } }
        public string DosePerFraction { get { return Plan.DosePerFraction.ToString(); } }
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
        public string MeanDose { get { return Plan.GetDVHCumulativeData(SelectedStructure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 100).MeanDose.ToString(); } }
        //public string MeanEqd2DoseDisplay { get { return MeanEqd2Dose.ToString(); } }

        // Constants
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

        public void GetDifferentialDVH()
        {
            //System.Windows.MessageBox.Show("GetDifferentialDVH");
            diffDvhData = Calculator.getDifferentialDvhData(Plan, SelectedStructure);
            if (!_initialLoad) GetEqd2Data();
        }

        public void GetEqd2Data()
        {
            //System.Windows.MessageBox.Show("GetEqd2Data");
            eqd2DvhData = Calculator.getEqd2DvhData(diffDvhData, Plan.NumberOfFractions.Value, ParameterSet.AlphaBeta);
            CalcNTCP();
        }

        public void CalcNTCP()
        {
            var result = Calculator.getNTCPs(eqd2DvhData, ParameterSet.TD50, ParameterSet.n, ParameterSet.m, SelectedStructure.Volume, Plan.GetDVHCumulativeData(SelectedStructure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 100).MaxDose, Plan.TotalDose);
            NTCP = result.NTCP;
            NTCPEUD = result.NTCPEUD;
            MeanEqd2Dose = result.MeanEqd2Dose;
        }
    }
}
