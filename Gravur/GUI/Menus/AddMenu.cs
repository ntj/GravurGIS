using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class AddMenu : ContextMenu
    {
        private MainControler _mainControler;
        private MenuItem newLayerMenuItem;
        private MenuItem newGeoImageMenuItem;
        private MenuItem newMandelbrotMenuItem;
        private MenuItem newMapServerLayer;
        
        private MenuItem newOGRLayer;

        public AddMenu(MainControler mainControler)
            : base()
        {
            newLayerMenuItem = new MenuItem();
            newLayerMenuItem.Text = "Layer hinzuf�gen...";
            newLayerMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newLayerMenuItem);
            newGeoImageMenuItem = new MenuItem();
            newGeoImageMenuItem.Text = "Geo-Bild hinzuf�gen...";
            newGeoImageMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newGeoImageMenuItem);
            newMandelbrotMenuItem = new MenuItem();
            newMandelbrotMenuItem.Text = "Mandelbrot hinzuf�gen...";
            newMandelbrotMenuItem.Click += new System.EventHandler(menuItemClick);

#if DEVELOP

            newOGRLayer = new MenuItem();
            newOGRLayer.Text = "OGR Layer hinzuf�gen...";
            newOGRLayer.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newOGRLayer);
#endif	
            newMapServerLayer = new MenuItem();
            newMapServerLayer.Text = "WMS-Layer hinzuf�gen...";
            newMapServerLayer.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newMapServerLayer);

			this._mainControler = mainControler;

			this._mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(MainControler_SettingsLoaded);

        }

        void MainControler_SettingsLoaded(Config config)
        {
            if (config.ShowSpecialLayers)
            {
                if (!this.MenuItems.Contains(newMandelbrotMenuItem))
                    this.MenuItems.Add(newMandelbrotMenuItem);
            }
            else
            {
                if (this.MenuItems.Contains(newMandelbrotMenuItem))
                    this.MenuItems.Remove(newMandelbrotMenuItem);
            }

        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == newGeoImageMenuItem)
				_mainControler.addGeoImage();
            else if (sender == newLayerMenuItem)
				_mainControler.addShapeFile();
            else if (sender == newMandelbrotMenuItem)
				_mainControler.addMandelbrot();
            else if (sender == newMapServerLayer)
				_mainControler.addMapserverLayer();
#if DEVELOP
            else if (sender == newOGRLayer)
				_mainControler.addOGRLayer();
#endif
        }
    }
}