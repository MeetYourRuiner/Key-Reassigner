using System;
using System.Windows.Forms;

namespace KeyReassigner.Core.EventArgs
{
    class KeyboardHookEventArgs : EventArgs
    {
        public Keys Key { get; set; }
        public bool Control { get; set; }
        public bool Handled { get; set; }

        public KeyboardHookEventArgs(Keys key, bool isControlPressed)
        {
            Key = key;
            Control = isControlPressed;
        }
    }
}
