using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Desktop.AddIns;


namespace QuickFindArcCatalogAddin
{
    public class ButtonClear : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ButtonClear()
        {
        }

        protected override void OnClick()
        {

            QuickFindArcCatalogAddin.ComboFilter cboFilter = AddIn.FromID<QuickFindArcCatalogAddin.ComboFilter>(ThisAddIn.IDs.ComboFilter);
            cboFilter.ClearFilter();
            
        }

        protected override void OnUpdate()
        {
            Enabled = ArcCatalog.Application != null;
        }
    }
}
