using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static PlayerSetter.View.win32;

namespace PlayerSetter.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class GetWindowHandle : Window
	{
		public const int WM_PASTE = 0x0302;

		public GetWindowHandle()
		{
			InitializeComponent();
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			captureRectangle.CaptureMouse();
			this.Cursor = Cursors.Hand;
		}

		private void captureRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var p = new POINT();
			win32.GetCursorPos(out p);
			IntPtr hWnd = win32.WindowFromPoint(p);

			Clipboard.SetText(hWnd.ToString());
			IntPtr wp = IntPtr.Zero;
			IntPtr lp = IntPtr.Zero;

			win32.SendMessage(hWnd, WM_PASTE, wp, lp );

			captureRectangle.ReleaseMouseCapture();
			this.Cursor = Cursors.Arrow;

			MessageBox.Show($"hwnd:[{hWnd}]");
		}
	}


	static class win32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
			public static implicit operator System.Drawing.Point(POINT point)
			{
				return new System.Drawing.Point(point.x, point.y);
			}
		}
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr WindowFromPoint(POINT point);

		[DllImport("User32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("user32")]
		public extern static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

	}
}
