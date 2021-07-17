using KeyReassigner.Core.Interfaces;
using KeyReassigner.Infrastructure;
using KeyReassigner.Interfaces;
using KeyReassigner.Services;
using KeyReassigner.UI;
using System;
using System.Threading;
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
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; 

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
                    throw new ArgumentOutOfRangeException(nameof(windowType));
            }
        }

        public void Exit()
        {
            _trayIcon.Dispose();
            Application.Exit();
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled UI exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
