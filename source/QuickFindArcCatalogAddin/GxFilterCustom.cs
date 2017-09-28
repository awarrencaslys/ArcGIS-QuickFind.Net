using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Catalog;

namespace QuickFindArcCatalogAddin
{

    public interface IFilterValue
    {
        string Filter { get; set; }
    }

    class GxFilterCustom:IGxObjectFilter,IFilterValue
    {
        public bool CanChooseObject(IGxObject @object, ref esriDoubleClickResult result)
        {
            throw new NotImplementedException();
        }

        public bool CanDisplayObject(IGxObject @object)
        {          
            //* filter is evaluated to determine if the gxobject should be displayed
            if (@object.Name.ToLower().Contains(this.Filter.ToLower()))
                return true;
            else
                return false;
        }

        public bool CanSaveObject(IGxObject Location, string newObjectName, ref bool objectAlreadyExists)
        {
            throw new NotImplementedException();
        }

        public string Description
        {
            get { return "Custom Filter"; }
        }

        public string Name
        {
            get {return "CustomFilter";}
        }

        public string Filter { get; set; }
    }
}
