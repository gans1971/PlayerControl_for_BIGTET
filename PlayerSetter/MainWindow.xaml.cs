﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static PlayerSetter.win32;

namespace PlayerSetter
{
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

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public const int WM_PASTE = 0x0302;

		public MainWindow()
		{

			InitializeComponent();
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			captureRectangle.CaptureMouse();
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
			MessageBox.Show($"hwnd:[{hWnd}]");
		}
	}
}
