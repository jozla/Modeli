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

        private List<long> comboBoxGids2 = new List<long>();
        private long relatedGid;
        private List<ModelCode> relatedProperties = new List<ModelCode>();
        private List<ModelCode> comboBoxPropIDs = new List<ModelCode>();
        private ModelCode propID;
        private List<ModelCode> types = new List<ModelCode>();
        private ModelCode type;




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

        public List<long> ComboBoxGids2
        {
            get
            {
                return comboBoxGids2;
            }
            set
            {
                comboBoxGids2 = value;
                OnPropertyChanged("ComboBoxGids2");
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


        public long RelatedGid
        {
            get
            {
                return relatedGid;
            }
            set
            {
                relatedGid = value;
                OnPropertyChanged("RelatedGid");
                OnPropertyChanged("RelatedProperties");
                OnPropertyChanged("Types");
                OnPropertyChanged("ComboBoxPropIDs");
            }
        }

        public List<ModelCode> RelatedProperties
        {
            get
            {
                if (type != 0)
                {
                    return FindPropertiesForModelCode(type);
                }
                return null;
            }
            set
            {
                relatedProperties = value;
                OnPropertyChanged("RelatedProperties");
                OnPropertyChanged("RelatedProperties");
            }
        }

        public List<ModelCode> ComboBoxPropIDs
        {
            get
            {
                if (relatedGid != 0)
                {
                    return FindPropertyIDs(relatedGid);
                }
                return null;
            }
            set
            {
                comboBoxPropIDs = value;
                OnPropertyChanged("ComboBoxPropIDs");
                OnPropertyChanged("Types");
            }
        }

        public ModelCode PropID
        {
            get
            {
                return propID;
            }
            set
            {
                propID = value;
                OnPropertyChanged("PropID");
                FindTypes(propID);
                OnPropertyChanged("Types");
            }
        }

        public List<ModelCode> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                OnPropertyChanged("Types");
                OnPropertyChanged("RelatedProperties");
            }
        }

        public ModelCode Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Type");
                OnPropertyChanged("RelatedProperties");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Connect proxy = new Connect();
            comboBoxGids = proxy.GetAllGids();
            comboBoxModelCodes = Enum.GetValues(typeof(DMSType)).Cast<DMSType>().ToList().FindAll(t => t != DMSType.MASK_TYPE);
            comboBoxGids2 = proxy.GetAllGids();

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

        public static List<ModelCode> FindPropertiesForModelCode(ModelCode modelCode)
        {
            ModelResourcesDesc mrd = new ModelResourcesDesc();
            List<ModelCode> list = mrd.GetAllPropertyIds(modelCode);

            return list;
        }

        public static List<ModelCode> FindPropertyIDs(long relatedGid)
        {
            ModelResourcesDesc mrd = new ModelResourcesDesc();
            List<ModelCode> properties = mrd.GetAllPropertyIdsForEntityId(relatedGid);
            List<ModelCode> ret = new List<ModelCode>();

            foreach (ModelCode mc in properties)
            {
                if (Property.GetPropertyType(mc) == PropertyType.Reference || Property.GetPropertyType(mc) == PropertyType.ReferenceVector)
                {
                    ret.Add(mc);
                }
            }

            return ret;

        }

        private void FindTypes(ModelCode propID)
        {

            string[] props = (propID.ToString()).Split('_');
            props[1] = props[1].TrimEnd('S');

            DMSType propertyCode = ModelResourcesDesc.GetTypeFromModelCode(propID);


            ModelCode mc;
            ModelCodeHelper.GetModelCodeFromString(propertyCode.ToString(), out mc);

            foreach (ModelCode modelCode in Enum.GetValues(typeof(ModelCode)))
            {

                if ((String.Compare(modelCode.ToString(), mc.ToString()) != 0) && (String.Compare(propID.ToString(), modelCode.ToString()) != 0) && (String.Compare(props[1], modelCode.ToString())) == 0)
                {
                    DMSType type = ModelCodeHelper.GetTypeFromModelCode(modelCode);
                    if (type == 0)
                    {
                        FindChildrenTypes(modelCode);
                    }
                    else
                    {
                        types = new List<ModelCode>();
                        types.Add(modelCode);
                    }

                }
            }
        }

        private void FindChildrenTypes(ModelCode modelCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            List<ModelCode> retCodes = new List<ModelCode>();

            long lmc = (long)modelCode;
            string smc = String.Format("0x{0:x16}", lmc);

            string[] newS = smc.Split('x');
            char[] c = (newS[1]).ToCharArray();


            foreach (char ch in c)
            {
                if (ch != '0')
                {
                    sb.Append(ch);
                }
                else
                {
                    break;
                }

            }

            foreach (ModelCode mc in Enum.GetValues(typeof(ModelCode)))
            {
                DMSType type = ModelCodeHelper.GetTypeFromModelCode(mc);
                short sh = (short)mc;
                if ((modelCode != mc) && (sh == 0) && (type != 0))
                {
                    lmc = (long)mc;
                    smc = String.Format("0x{0:x16}", lmc);
                    if (smc.StartsWith(sb.ToString()))
                    {
                        retCodes.Add(mc);
                    }
                }
            }

            types = retCodes;
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
                MessageBox.Show("Select model code and property first.");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in lstProperties2.SelectedItems)
            {
                l.Add((ModelCode)v);
            }
            txtResult2.Text = new Connect().GetExtentValues(ExtentModelCode, l);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lstProperties3.SelectedItems == null || PropID == 0 || RelatedGid == 0 || Type == 0)
            {
                MessageBox.Show("Select GID, Property ID, type and property first.");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in lstProperties3.SelectedItems)
            {
                l.Add((ModelCode)v);
            }

            Association association = new Association();
            association.PropertyId = PropID;
            association.Type = Type;

            txtResult3.Text = new Connect().GetRelatedValues(RelatedGid, association, l);
        }
    }
}
