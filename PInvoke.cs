using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayApp
{
    class PInvoke
	{ 
		public struct KeyboardHookStruct
		{
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT
		{
			public uint type;
			public InputUnion U;
			public static int Size
			{
				get { return Marshal.SizeOf(typeof(INPUT)); }
			}
			public INPUT(Keys wVk)
			{
				this.type = 1;
				KEYBDINPUT keybdInput = new PInvoke.KEYBDINPUT(wVk);
				this.U = new PInvoke.InputUnion
				{
					ki = keybdInput
				};
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InputUnion
		{
			[FieldOffset(0)]
			internal MOUSEINPUT mi;
			[FieldOffset(0)]
			internal KEYBDINPUT ki;
			[FieldOffset(0)]
			internal HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public short wVk;
			public short wScan;
			public uint dwFlags;
			public int time;
			public UIntPtr dwExtraInfo;

			public KEYBDINPUT(Keys wVk)
			{
				this.wVk = (short)wVk;
				this.wScan = 0;
				this.dwFlags = 0;
				this.time = 0;
				this.dwExtraInfo = UIntPtr.Zero;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
		{
			internal int dx;
			internal int dy;
			internal int mouseData;
			internal uint dwFlags;
			internal uint time;
			internal UIntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			internal int uMsg;
			internal short wParamL;
			internal short wParamH;
		}


		/// <summary>
		/// defines the callback type for the hook
		/// </summary>
		public delegate int keyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

		#region DLL imports
		/// <summary>
		/// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
		/// </summary>
		/// <param name="idHook">The id of the event you want to hook</param>
		/// <param name="callback">The callback.</param>
		/// <param name="hInstance">The handle you want to attach the event to, can be null</param>
		/// <param name="threadId">The thread you want to attach the event to, can be null</param>
		/// <returns>a handle to the desired hook</returns>
		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

		/// <summary>
		/// Unhooks the windows hook.
		/// </summary>
		/// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
		/// <returns>True if successful, false otherwise</returns>
		[DllImport("user32.dll")]
		public static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		/// <summary>
		/// Calls the next hook.
		/// </summary>
		/// <param name="idHook">The hook id</param>
		/// <param name="nCode">The hook code</param>
		/// <param name="wParam">The wparam.</param>
		/// <param name="lParam">The lparam.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

		/// <summary>
		/// Loads the library.
		/// </summary>
		/// <param name="lpFileName">Name of the library</param>
		/// <returns>A handle to the library</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		/// <summary>
		/// Synthesizes keystrokes, mouse motions, and button clicks.
		/// </summary>
		[DllImport("user32.dll")]
		public static extern uint SendInput(
			uint nInputs,
			[MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
			int cbSize
		);
		#endregion
	}
}
