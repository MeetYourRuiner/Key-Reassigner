using KeyReassigner.Core.CustomEventArgs;
using System;
using System.Windows.Forms;

namespace KeyReassigner.Interfaces
{
    interface IKeyboardHook
    {
        event EventHandler<KeyboardHookEventArgs> KeyDown;
        event EventHandler<KeyboardHookEventArgs> KeyUp;

        void Hook();
        void Unhook();
        void InvokeKey(Keys key);
    }
}
