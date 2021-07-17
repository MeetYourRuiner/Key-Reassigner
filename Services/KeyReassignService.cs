using KeyReassigner.Core.Entity;
using KeyReassigner.Core.CustomEventArgs;
using KeyReassigner.Interfaces;
using System;
using System.Windows.Forms;

namespace KeyReassigner.Services
{
    class KeyReassignService : IKeyReassignService
    {
        private readonly IKeyboardHook _keyboardHook;
        private readonly IKeyReassignmentsRepository _keyReassignmentsRepository;

        public KeyReassignService(IKeyboardHook keyboardHook, IKeyReassignmentsRepository keyReassignmentsRepository)
        {

            if (keyboardHook == null)
                throw new ArgumentNullException(nameof(keyboardHook));
            if (keyReassignmentsRepository == null)
                throw new ArgumentNullException(nameof(keyReassignmentsRepository));

            _keyboardHook = keyboardHook;
            _keyReassignmentsRepository = keyReassignmentsRepository;
            _keyboardHook.KeyUp += OnKeyUp;
            _keyboardHook.KeyDown += OnKeyDown;
        }

        public void Start()
        {
            _keyboardHook.Hook();
        }

        public void Stop()
        {
            _keyboardHook.Unhook();
        }

        private void OnKeyUp(object sender, KeyboardHookEventArgs e) { }

        private void OnKeyDown(object sender, KeyboardHookEventArgs e)
        {
            Keys key = e.Key;
            if (_keyReassignmentsRepository.ReassignmentExists(key))
            {
                Reassignment reassignment = _keyReassignmentsRepository.Get(key);
                bool toIgnore = e.Control && reassignment.IsIgnoredWithCtrl;
                if (!toIgnore)
                {
                    Keys keyToInvoke = reassignment.ReassignedTo;
                    _keyboardHook.InvokeKey(keyToInvoke);
                    e.Handled = true;
                }
            }
        }
    }
}
