using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient
{
    public class Connect : IDisposable
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        private NetworkModelGDAProxy gdaQueryProxy = null;
        private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }

        public Connect()
        {
            gdaQueryProxy = GdaQueryProxy;
        }

        public List<long> GetAllGids()
        {
            ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
            List<ModelCode> properties = new List<ModelCode>();
            List<long> ids = new List<long>();

            int iteratorId = 0;
            int numberOfResources = 1000;
            DMSType currType = 0;
            properties.Add(ModelCode.IDOBJ_GID);
            try
            {
                foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
                {
                    currType = type;

                    if (type != DMSType.MASK_TYPE)
                    {
                        iteratorId = GdaQueryProxy.GetExtentValues(modelResourcesDesc.GetModelCodeFromType(type), properties);
                        int count = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

                        while (count > 0)
                        {
                            List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                            for (int i = 0; i < rds.Count; i++)
                            {
                                ids.Add(rds[i].Id);
                            }

                            count = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                        }

                        bool ok = GdaQueryProxy.IteratorClose(iteratorId);

                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            return ids;
        }

        public string GetValues(long globalId, List<ModelCode> list)
        {
            ResourceDescription rd = null;
            string ret = "";
            
            try
            {
                List<ModelCode> properties = list;

                rd = GdaQueryProxy.GetValues(globalId, properties);
                ret += String.Format("Item with gid: 0x{0:x16}:\n", globalId);
                foreach (Property p in rd.Properties)
                {
                    ret += String.Format("{0} =", p.Id);
                    switch (p.Type)
                    {
                        case PropertyType.Float:
                            ret += String.Format(" {0}:\n", p.AsFloat());
                            break;
                        case PropertyType.Bool:
                            if (p.PropertyValue.LongValue == 1)
                                ret += String.Format(" True\n");
                            else
                                ret += String.Format(" False\n");
                            break;
                        case PropertyType.Int32:
                        case PropertyType.Int64:
                        case PropertyType.DateTime:
                            if (p.Id == ModelCode.IDOBJ_GID)
                            {
                                ret += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                            }
                            else
                            {
                                ret += String.Format(" {0}\n", p.ToString());
                            }

                            break;

                        case PropertyType.Reference:
                            if (p.PropertyValue.LongValue == 0)
                            {
                                ret += (String.Format(" No Reference\n"));
                            }
                            else
                            {
                                ret += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                            }
                            break;
                        case PropertyType.String:
                            if (p.PropertyValue.StringValue == null)
                            {
                                p.PropertyValue.StringValue = String.Empty;
                            }
                            ret += String.Format(" {0}\n", p.AsString());
                            break;


                        case PropertyType.ReferenceVector:
                            if (p.AsLongs().Count > 0)
                            {
                                string s = "";
                                for (int j = 0; j < p.AsLongs().Count; j++)
                                {
                                    s += (String.Format(" 0x{0:x16},\n", p.AsLongs()[j]));
                                }

                                ret += s;
                            }
                            else
                            {
                                ret += (" There is no references\n");
                            }

                            break;

                        case PropertyType.Enum:
                            if (p.Id == ModelCode.REGCONTROL_MODE)
                            {
                                ret += String.Format(" {0}\n", (RegulatingControlModeKind)p.PropertyValue.LongValue);
                            }
                            if (p.Id == ModelCode.REGCONTROL_MONITOREDPHASE)
                            {
                                ret += String.Format(" {0}\n", (PhaseCode)p.PropertyValue.LongValue);
                            }
                            break;

                        default:
                            throw new Exception("Failed to export.");

                    }
                }
            }
            catch (Exception)
            {

            }

            return ret;
        }

        public string GetExtentValues(DMSType type, List<ModelCode> list)
        {
            int iteratorId = 0;
            string ret = "";
            bool showGID = true;
            int numberOfResources = 2;
            int resourcesLeft = 0;
            ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(type);

            try
            {

                List<ModelCode> properties = list;
                if (!list.Contains(ModelCode.IDOBJ_GID))
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    showGID = false;
                }
                iteratorId = GdaQueryProxy.GetExtentValues(modelCode, properties);
                resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                ret += String.Format("Items with ModelCode: {0}:\n", modelCode.ToString());
                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ret += String.Format("\tItem with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (!(p.Id == ModelCode.IDOBJ_GID && !showGID))
                            {
                                ret += String.Format("\t\t{0} =", p.Id);
                                switch (p.Type)
                                {

                                    case PropertyType.Float:
                                        ret += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Bool:
                                        if (p.PropertyValue.LongValue == 1)
                                            ret += String.Format(" True\n");
                                        else
                                            ret += String.Format(" False\n");
                                        break;
                                    case PropertyType.Int32:
                                    case PropertyType.Int64:
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ret += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ret += String.Format(" {0}\n", p.ToString());
                                        }

                                        break;

                                    case PropertyType.Reference:
                                        if (p.PropertyValue.LongValue == 0)
                                        {
                                            ret += (String.Format(" No Reference\n"));
                                        }
                                        else
                                        {
                                            ret += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                                        }
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)
                                        {
                                            p.PropertyValue.StringValue = String.Empty;
                                        }
                                        ret += String.Format(" {0}\n", p.AsString());
                                        break;


                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format(" 0x{0:x16},\n", p.AsLongs()[j]));
                                            }

                                            ret += s;
                                        }
                                        else
                                        {
                                            ret += (" There is no references\n");
                                        }

                                        break;

                                    case PropertyType.Enum:
                                        if (p.Id == ModelCode.REGCONTROL_MODE)
                                        {
                                            ret += String.Format(" {0}\n", (RegulatingControlModeKind)p.PropertyValue.LongValue);
                                        }
                                        if (p.Id == ModelCode.REGCONTROL_MONITOREDPHASE)
                                        {
                                            ret += String.Format(" {0}\n", (PhaseCode)p.PropertyValue.LongValue);
                                        }
                                        break;

                                    default:
                                        throw new Exception("Failed to export.");
                                }

                            }
                        }
                    }
                    resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                GdaQueryProxy.IteratorClose(iteratorId);

            }
            catch (Exception)
            {

            }


            return ret;
        }

        public string GetRelatedValues(long sourceGlobalId, Association association, List<ModelCode> props)
        {
            int iteratorId = 0;
            string ret = "";
            bool showGID = true;
            int numberOfResources = 2;
            int resourcesLeft = 0;

            try
            {
                List<ModelCode> properties = props;
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    showGID = false;
                }
                iteratorId = GdaQueryProxy.GetRelatedValues(sourceGlobalId, properties, association);
                resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ret += String.Format("Item with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (!(p.Id == ModelCode.IDOBJ_GID && !showGID))
                            {
                                ret += String.Format("\t{0} =", p.Id);
                                switch (p.Type)
                                {
                                    case PropertyType.Float:
                                        ret += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Bool:
                                        if (p.PropertyValue.LongValue == 1)
                                            ret += String.Format(" True\n");
                                        else
                                            ret += String.Format(" False\n");
                                        break;
                                    case PropertyType.Int32:
                                    case PropertyType.Int64:
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)
                                        {
                                            ret += (String.Format(" 0x{0:x16}\n", p.AsLong()));
                                        }
                                        else
                                        {
                                            ret += String.Format(" {0}\n", p.ToString());
                                        }

                                        break;

                                    case PropertyType.Reference:
                                        if (p.PropertyValue.LongValue == 0)
                                        {
                                            ret += (String.Format(" No Reference\n"));
                                        }
                                        else
                                        {
                                            ret += (String.Format(" 0x{0:x16}\n", p.AsReference()));
                                        }
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)
                                        {
                                            p.PropertyValue.StringValue = String.Empty;
                                        }
                                        ret += String.Format(" {0}\n", p.AsString());
                                        break;


                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format(" 0x{0:x16},\n", p.AsLongs()[j]));
                                            }

                                            ret += s;
                                        }
                                        else
                                        {
                                            ret += ("There is no references\n");
                                        }

                                        break;

                                    case PropertyType.Enum:
                                        if (p.Id == ModelCode.REGCONTROL_MODE)
                                        {
                                            ret += String.Format(" {0}\n", (RegulatingControlModeKind)p.PropertyValue.LongValue);
                                        }
                                        if (p.Id == ModelCode.REGCONTROL_MONITOREDPHASE)
                                        {
                                            ret += String.Format(" {0}\n", (PhaseCode)p.PropertyValue.LongValue);
                                        }
                                        break;

                                    default:
                                        throw new Exception("Failed to export.");

                                }
                            }
                        }
                    }
                    resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                GdaQueryProxy.IteratorClose(iteratorId);


            }
            catch (Exception)
            {

            }

            return ret;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
