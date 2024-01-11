using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<long> comboBoxGids = new List<long>();
        private long valuesGid;
        private List<ModelCode> valuesProperties = new List<ModelCode>();

        private List<DMSType> comboBoxModelCodes = new List<DMSType>();
        private DMSType extentModelCode;
        private List<ModelCode> extentProperties = new List<ModelCode>();



        public event PropertyChangedEventHandler PropertyChanged;

        public List<long> ComboBoxGids
        {
            get
            {
                return comboBoxGids;
            }
            set
            {
                comboBoxGids = value;
                OnPropertyChanged("ComboBoxGids");
            }

        }

        public List<DMSType> ComboBoxModelCodes
        {
            get
            {
                return comboBoxModelCodes;
            }
            set
            {
                comboBoxModelCodes = value;
                OnPropertyChanged("ComboBoxModelCodes");
            }

        }

        public long ValuesGid
        {
            get
            {
                return valuesGid;
            }
            set
            {
                valuesGid = value;
                OnPropertyChanged("ValuesGid");
                OnPropertyChanged("ValuesProperties");
            }
        }

        public List<ModelCode> ValuesProperties
        {
            get
            {
                if (valuesGid != 0)
                {
                    return FindPropertiesForGid(valuesGid);
                }
                return null;
            }
            set
            {
                valuesProperties = value;
                OnPropertyChanged("ValuesProperties");
                OnPropertyChanged("ValuesGid");
            }
        }

        public DMSType ExtentModelCode
        {
            get
            {
                return extentModelCode;
            }
            set
            {
                extentModelCode = value;
                OnPropertyChanged("ExtentModelCode");
                OnPropertyChanged("ExtentProperties");
            }
        }

        public List<ModelCode> ExtentProperties
        {
            get
            {
                if (ExtentModelCode != 0)
                {
                    return FindPropertiesForDMS(extentModelCode);
                }
                return null;
            }
            set
            {
                extentProperties = value;
                OnPropertyChanged("ExtentProperties");
                OnPropertyChanged("ExtentModelCode");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Connect proxy = new Connect();
            comboBoxGids = proxy.GetAllGids();
            comboBoxModelCodes = Enum.GetValues(typeof(DMSType)).Cast<DMSType>().ToList().FindAll(t => t != DMSType.MASK_TYPE);        

            DataContext = this;
        }


        public static List<ModelCode> FindPropertiesForGid(long gid)
        {
            ModelResourcesDesc mrd = new ModelResourcesDesc();
            List<ModelCode> list = mrd.GetAllPropertyIdsForEntityId(gid);

            return list;
        }


        public static List<ModelCode> FindPropertiesForDMS(DMSType type)
        {
            ModelResourcesDesc mrd = new ModelResourcesDesc();
            List<ModelCode> list = mrd.GetAllPropertyIds(type);

            return list;
        }

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstProperties.SelectedItems == null || ValuesGid == 0)
            {
                MessageBox.Show("Select GID and property first.");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in lstProperties.SelectedItems)
            {
                l.Add((ModelCode)v);
            }


            txtResult.Text = new Connect().GetValues(ValuesGid, l);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lstProperties2.SelectedItems == null || ExtentModelCode == 0)
            {
                MessageBox.Show("You must chose a propertie");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in lstProperties2.SelectedItems)
            {
                l.Add((ModelCode)v);
            }
            txtResult2.Text = new Connect().GetExtentValues(ExtentModelCode, l);
        }
    }
}
