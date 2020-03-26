using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayApp
{
    class AppContext: ApplicationContext
    {
        // private ConfigWindow configWindow = new ConfigWindow();
        private NotifyIcon notifyIcon = new NotifyIcon();
        private KeyboardHook keyboardHook = new KeyboardHook();

        public AppContext()
        {
            // MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = TrayApp.Properties.Resources.AppIcon;
            notifyIcon.Text = "Volume to Media";
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { 
                    // configMenuItem, 
                    exitMenuItem 
                }
            );

            keyboardHook.HookedKeys.Add(Keys.VolumeUp, Keys.MediaNextTrack);
            keyboardHook.HookedKeys.Add(Keys.VolumeDown, Keys.MediaPreviousTrack);
            keyboardHook.HookedKeys.Add(Keys.VolumeMute, Keys.MediaPlayPause);
            //keyboardHook.KeyDown += new KeyEventHandler(kh_KeyDown);

            notifyIcon.Visible = true;
        }

        void kh_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
