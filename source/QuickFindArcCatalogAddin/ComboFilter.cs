using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Catalog;
using System.Timers;
using ESRI.ArcGIS.CatalogUI;
using System.Diagnostics;

namespace QuickFindArcCatalogAddin
{
    public class ComboFilter : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {

        Timer _timer;
    
        IGxApplication _pGxApp = ArcCatalog.Application as IGxApplication;
        IGxContentsView pGxContentsView;
        IGxView _pGxView;

        public ComboFilter()
        {
            
            _timer = new Timer(600);
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

            IGxViewContainer pGxViewContainer = _pGxApp as IGxViewContainer;
            IEnumGxView pEnumGxView;
            pEnumGxView = pGxViewContainer.Views;
    
            IGxView pGxView;
            pGxView = pEnumGxView.Next();
    
            while(pGxView != null)
            {
                if(pGxView is IGxContentsView)
                {
                    pGxContentsView = pGxView as IGxContentsView;
                    _pGxView = pGxView;
                    break;
                }
                pGxView = pEnumGxView.Next();
            }
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            DoFilter();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            UpdateItems(this.Value);
        }        


        protected override void OnUpdate()
        {
            Enabled = ArcCatalog.Application != null;
        }

        protected override void OnEditChange(string editString)
        {
            base.OnEditChange(editString);
            Debug.WriteLine("OnEditChange: " + editString);
            _timer.Enabled = false;
            _timer.Enabled = true;
            //DoFilter(editString);
         
        }

        protected override void OnSelChange(int cookie)
        {
            base.OnSelChange(cookie);
            _timer.Enabled = false;
            Debug.WriteLine("OnSelChange");
            DoFilter();
        }

        public void ClearFilter()
        {
            _timer.Enabled = false;
            this.Value = "";
            DoFilter();
        }

        public void DoFilter()
        {

            string filterString = this.Value;
            Debug.WriteLine(filterString);

            IGxObject pGxObj;
            IEnumGxObject pEnumGxObject;
                       

            IGxSelection pGxSelection = _pGxApp.Selection;

            if (pGxSelection == null) return;
            
            IGxObject pGxObject_Current = pGxSelection.Location;
            IGxObjectContainer pGxObjectContainer = null;
            if (pGxObject_Current is IGxObjectContainer)
            {
                pGxObjectContainer = pGxObject_Current as IGxObjectContainer;
            }

            if (!pGxObjectContainer.HasChildren) return;

            pEnumGxObject = pGxObjectContainer.Children;

            if (pEnumGxObject == null) return;


            IGxObjectFilter pGxObjectFilter = new GxFilterCustom();
            IFilterValue filter = pGxObjectFilter as IFilterValue;
            filter.Filter = filterString;
            
            pGxObj = pEnumGxObject.Next();
            while (pGxObj != null)
            {
                pGxObjectFilter.CanDisplayObject(pGxObj);
                pGxObj = pEnumGxObject.Next();
            }


            pGxContentsView.ObjectFilter = pGxObjectFilter;
            _pGxView.Refresh();

            //UpdateItems(filterString);

        }

        private void UpdateItems(string filterString)
        {
            Item existingItem = null;
            List<string> itemsHistory = new List<string>();
            foreach (var item in this.items)
            {
                itemsHistory.Add(item.Caption);
                this.Remove(item.Cookie);
                if (item.Caption == filterString)
                {
                    existingItem = item;
                    //break;
                }
            }
            if (existingItem == null)
            {
                if (filterString.Trim().Length > 0)
                    itemsHistory.Add(filterString);
            }
            else
            {
                itemsHistory.Remove(existingItem.Caption);
                itemsHistory.Add(existingItem.Caption);
            }
            itemsHistory.Reverse();
            foreach (var item in itemsHistory)
            {

                this.Add(item);
            }

            if (itemsHistory.Count > 10)
                itemsHistory.RemoveAt(10);
        }

    }

}
