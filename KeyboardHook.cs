using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayApp
{
    class KeyboardHook 
	{
		#region Constant, Structure and Delegate Definitions

		const int WH_KEYBOARD_LL = 13;
		const int WM_KEYDOWN = 0x100;
		const int WM_KEYUP = 0x101;
		const int WM_SYSKEYDOWN = 0x104;
		const int WM_SYSKEYUP = 0x105;
		#endregion

		#region Instance Variables
		/// <summary>
		/// The collections of keys to watch for
		/// </summary>
		public Dictionary<Keys, Keys> HookedKeys = new Dictionary<Keys, Keys>();
		/// <summary>
		/// Handle to the hook, need this to unhook and call the next hook
		/// </summary>
		private IntPtr hhook = IntPtr.Zero;
		private bool isCtrlPressed = false;
		#endregion

		/*
		#region Events
		/// <summary>
		/// Occurs when one of the hooked keys is pressed
		/// </summary>
		public event KeyEventHandler KeyDown;
		/// <summary>
		/// Occurs when one of the hooked keys is released
		/// </summary>
		public event KeyEventHandler KeyUp;
		#endregion
		*/

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyboardHook"/> class and installs the keyboard hook.
		/// </summary>
		public KeyboardHook()
		{
			Hook();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="KeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
		/// </summary>
		~KeyboardHook()
		{
			Unhook();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Installs the global hook
		/// </summary>
		public void Hook()
		{
			IntPtr hInstance = PInvoke.LoadLibrary("User32");
			hhook = PInvoke.SetWindowsHookEx(WH_KEYBOARD_LL, HookProc, hInstance, 0);
		}

		/// <summary>
		/// Uninstalls the global hook
		/// </summary>
		public void Unhook()
		{
			PInvoke.UnhookWindowsHookEx(hhook);
		}

		/// <summary>
		/// The callback for the keyboard hook
		/// </summary>
		/// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
		/// <param name="wParam">The event type</param>
		/// <param name="lParam">The keyhook event information</param>
		/// <returns></returns>
		public int HookProc(int code, int wParam, ref PInvoke.KeyboardHookStruct lParam)
		{
			if (code >= 0)
			{
				Keys key = (Keys)lParam.vkCode;
				if (key == Keys.LControlKey)
				{
					if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
					{
						isCtrlPressed = true;
					}
					else if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
					{
						isCtrlPressed = false;
					}
				}
				else if (!isCtrlPressed)
				{
					Keys keyToInvoke;
					if (HookedKeys.TryGetValue(key, out keyToInvoke))
					{
						KeyEventArgs kea = new KeyEventArgs(key);
						if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) // && (KeyDown != null)
						{
							PInvoke.INPUT input = new PInvoke.INPUT(keyToInvoke);
							_ = PInvoke.SendInput(1, new PInvoke.INPUT[] { input }, PInvoke.INPUT.Size);
							kea.Handled = true;
							return 1;
						}
					}
				}
				
			}
			return PInvoke.CallNextHookEx(hhook, code, wParam, ref lParam);
		}
		#endregion
	}
}
