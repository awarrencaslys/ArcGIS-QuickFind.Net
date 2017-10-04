using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Catalog;
using System.Timers;
using ESRI.ArcGIS.CatalogUI;
using System.Diagnostics;
using ESRI.ArcGIS.esriSystem;

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
            //base.OnEnter();
            UpdateItems(this.Value);
        }        


        protected override void OnUpdate()
        {
            Enabled = ArcCatalog.Application != null;
        }

        protected override void OnEditChange(string editString)
        {
            //base.OnEditChange(editString);
            Debug.WriteLine("OnEditChange: " + editString);
            _timer.Enabled = false;
            _timer.Enabled = true;
            //DoFilter(editString);
         
        }

        protected override void OnSelChange(int cookie)
        {
            //base.OnSelChange(cookie);
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

        IGxObjectFilter pGxObjectFilter = new GxFilterCustom();

        private List<IGxObject> filteredObjects = new List<IGxObject>();

        public void DoFilter()
        {
            try
            {
                IFilterValue filter = pGxObjectFilter as IFilterValue;
                filter.Filter = "";

                filteredObjects.Clear();

                Debug.WriteLine(string.Format("DoFilter: {0}", this.Value));
                string filterString = this.Value;
                Debug.WriteLine(filterString);

                IGxObject pGxObj;
                IEnumGxObject pEnumGxObject;


                IGxSelection pGxSelection = _pGxApp.Selection;

                if (pGxSelection == null) return;

                IEnumGxObject pSelectedGxObjects = pGxSelection.SelectedObjects;

                IGxObject pGxObject_Current = pGxSelection.Location;


                IGxObjectContainer pGxObjectContainer = null;
                if (pGxObject_Current is IGxObjectContainer)
                {
                    pGxObjectContainer = pGxObject_Current as IGxObjectContainer;
                }


                if (!pGxObjectContainer.HasChildren) return;

                pEnumGxObject = pGxObjectContainer.Children;

                if (pEnumGxObject == null) return;

                filter = pGxObjectFilter as IFilterValue;
                filter.Filter = filterString;

                pGxObj = pEnumGxObject.Next();
                while (pGxObj != null)
                {

                    bool canDisplay = pGxObjectFilter.CanDisplayObject(pGxObj);
                    if (canDisplay == false)
                    {
                        //* if the object is not going to be displayed (due to being filtered out) we will track it so we can unselect it later (if it was selected)
                        filteredObjects.Add(pGxObj);
                    }

                    pGxObj = pEnumGxObject.Next();
                }

                //* Since selected objects stay selected after filtering, we need to manually unselect hidden items due to the filter
                IGxObject pGxObjSelected = pSelectedGxObjects.Next();
                while (pGxObjSelected != null)
                {
                    if (filteredObjects.Contains(pGxObjSelected))
                    {
                        Debug.WriteLine("Selected Object that is hidden: " + pGxObjSelected.FullName);
                        pGxSelection.Unselect(pGxObjSelected, null);
                    }

                    pGxObjSelected = pSelectedGxObjects.Next();
                }

                pGxContentsView.ObjectFilter = pGxObjectFilter;

            }
            catch (Exception ex)
            {
                ArcCatalog.Application.StatusBar.set_Message(0, ex.Message);
            }
            finally
            {
                _pGxView.Refresh();
            }

            //UpdateItems(filterString);

        }

        private void UpdateItems(string filterString)
        {
            Debug.WriteLine(string.Format("UpdateItems: {0}",filterString));
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
