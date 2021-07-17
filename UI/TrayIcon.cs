using KeyReassigner.Interfaces;
using System;
using System.Windows.Forms;

namespace KeyReassigner.UI
{
    class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenu _contextMenu;
        private readonly INavigator _navigator;

        public TrayIcon(INavigator navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            _navigator = navigator;
            _notifyIcon = new NotifyIcon();
            _contextMenu = new ContextMenu();
            ConfigureContextMenu();
            ConfigureNotifyIcon();
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
        }

        public void Hide()
        {
            _notifyIcon.Visible = false;
        }

        private void ConfigureNotifyIcon()
        {
            _notifyIcon.Icon = Properties.Resources.AppIcon;
            _notifyIcon.Text = "Key Reassigner";
            _notifyIcon.ContextMenu = _contextMenu;
        }

        private void ConfigureContextMenu()
        {
            MenuItem[] menuItems = new[] {
                new MenuItem("Configurations", new EventHandler(ShowConfigWindow)),
                new MenuItem("Exit", new EventHandler(Exit)),
            };
            _contextMenu.MenuItems.AddRange(menuItems);
        }

        private void ShowConfigWindow(object sender, EventArgs e)
        {
            _navigator.NavigateTo(WindowTypes.Configuration);
        }

        private void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            Hide();
            _navigator.Exit();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contextMenu.Dispose();
                _notifyIcon.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
