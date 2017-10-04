QuickFind.Net
=============

This is a .NET re-write of a tool I wrote many years ago in VB6 and placed on ArcScripts.  
http://arcscripts.esri.com/details.asp?dbid=14236

It allows you to quickly filter the current contents view in ArcCatalog.   The filter is based simply on a string 'contains' function.  Regex support could be added at some point but not everyone is comfortable with the syntax.
After adding the AddIn, you should have a new toolbar called 'Quick Find'.  The toolbar has a combobox that you can type into and a clear button.  If you hit 'Enter' the filter text should get added to the combo box list entries so you can go re-use it or see what filters you've used.

Currently it is built for ArcGIS Desktop 10.1 with Visual Studio 2010 Pro.

Also this a work in progress so let me know if things are not working.  There might be some refinement needed in the dropdown list entry management.
