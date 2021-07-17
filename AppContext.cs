using KeyReassigner.Core.Interfaces;
using KeyReassigner.Infrastructure;
using KeyReassigner.Interfaces;
using KeyReassigner.Services;
using KeyReassigner.UI;
using System.Windows.Forms;

namespace KeyReassigner
{
    class AppContext : ApplicationContext, INavigator
    {
        private readonly TrayIcon _trayIcon;
        private readonly IKeyReassignmentsRepository _keyReassignmentsRepository;
        private readonly IKeyReassignService _keyReassignService;
        public readonly IConfigurationStorage _configuration;
        private Form _currentWindow;

        public AppContext()
        {
            _configuration = new XmlConfigurationStorage();
            _keyReassignmentsRepository = new KeyReassignmentsRepository(_configuration);
            _keyReassignService = new KeyReassignService(new WindowsKeyboardHook(), _keyReassignmentsRepository);
            _trayIcon = new TrayIcon(this);

            _keyReassignService.Start();
            _trayIcon.Show();
        }

        public void NavigateTo(WindowTypes windowType)
        {
            switch (windowType)
            {
                case WindowTypes.Configuration:
                    {
                        if (!(_currentWindow is ConfigWindow))
                        {
                            _currentWindow?.Close();
                            _currentWindow = new ConfigWindow(_keyReassignmentsRepository);
                        }
                        _currentWindow.Show();
                        _currentWindow.Disposed += (s, e) => { _currentWindow = null; };
                        _currentWindow.Activate();
                        break;
                    }
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(windowType));
            }
        }

        public void Exit()
        {
            _trayIcon.Dispose();
            Application.Exit();
        }
    }
}
