using KeyReassigner.Core.EventArgs;
using KeyReassigner.Infrastructure.External;
using KeyReassigner.Interfaces;
using System;
using System.Windows.Forms;

namespace KeyReassigner.Infrastructure
{
    class WindowsKeyboardHook : IKeyboardHook
    {
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public event EventHandler<KeyboardHookEventArgs> KeyDown;
        public event EventHandler<KeyboardHookEventArgs> KeyUp;

        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr _hookHandle = IntPtr.Zero;
        private bool _isCtrlPressed = false;

        public WindowsKeyboardHook()
        {
            KeyUp += CheckCtrlUp;
            KeyDown += CheckCtrlDown;
        }

        ~WindowsKeyboardHook()
        {
            Unhook();
        }

        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void Hook()
        {
            IntPtr hInstance = WindowsLowLevelKeyboardHook.LoadLibrary("User32");
            _hookHandle = WindowsLowLevelKeyboardHook.SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void Unhook()
        {
            WindowsLowLevelKeyboardHook.UnhookWindowsHookEx(_hookHandle);
        }

        public void InvokeKey(Keys key)
        {
            WindowsLowLevelKeyboardHook.INPUT input = new WindowsLowLevelKeyboardHook.INPUT(key);
            _ = WindowsLowLevelKeyboardHook.SendInput(1, new WindowsLowLevelKeyboardHook.INPUT[] { input }, WindowsLowLevelKeyboardHook.INPUT.Size);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookCallback(int code, int wParam, ref WindowsLowLevelKeyboardHook.KeyboardHookStruct lParam)
        {
            try
            {
                bool isCodeValid = code >= 0;
                if (isCodeValid)
                {
                    bool isKeyDown = wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN;
                    bool isKeyUp = wParam == WM_KEYUP || wParam == WM_SYSKEYUP;

                    Keys key = (Keys)lParam.vkCode;
                    KeyboardHookEventArgs keyEventArgs = new KeyboardHookEventArgs(key, _isCtrlPressed);
                    if (isKeyDown)
                    {
                        KeyDown?.Invoke(this, keyEventArgs);
                    }
                    else if (isKeyUp)
                    {
                        KeyUp?.Invoke(this, keyEventArgs);
                    }
                    if (keyEventArgs.Handled)
                    {
                        return 1;
                    }
                }
            }
            catch { }
            return WindowsLowLevelKeyboardHook.CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        private void CheckCtrlUp(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key == Keys.LControlKey || e.Key == Keys.RControlKey)
            {
                _isCtrlPressed = false;
            }
        }

        private void CheckCtrlDown(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key == Keys.LControlKey || e.Key == Keys.RControlKey)
            {
                _isCtrlPressed = true;
            }
        }
    }
}
