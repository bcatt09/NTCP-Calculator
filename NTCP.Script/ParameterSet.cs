using GalaSoft.MvvmLight;
using VMS.TPS.Common.Model.Types;

namespace NTCP
{
    public enum ParameterSetType
    {
        Lung,
        LiverPrimary,
        LiverMet
    }
    public class ParameterSet : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
        private ParameterSetType _type;
        public ParameterSetType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }
        private double _n;
        public double n
        {
            get { return _n; }
            set { Set(ref _n, value); }
        }
        private double _m;
        public double m
        {
            get { return _m; }
            set { Set(ref _m, value); }
        }
        private DoseValue _alphaBeta;
        public DoseValue AlphaBeta
        {
            get { return _alphaBeta; }
            set { Set(ref _alphaBeta, value); }
        }
        private DoseValue _td50;
        public DoseValue TD50
        {
            get { return _td50; }
            set { Set(ref _td50, value); }
        }
    }
}
