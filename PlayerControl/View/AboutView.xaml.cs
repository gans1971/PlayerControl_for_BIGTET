using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayerControl.View
{
	/// <summary>
	/// AboutView.xaml の相互作用ロジック
	/// </summary>
	public partial class AboutView : UserControl
	{
		public AboutView()
		{
			InitializeComponent();

			var asm = Assembly.GetExecutingAssembly();
			try
			{
				if (asm != null)
				{
					var asmName = asm.GetName();
					if (asmName != null)
					{
						// 製品名
						var AppName = asmName.Name;
						object[] productarray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
						if (productarray != null && productarray.Length > 0)
						{
							AppName = ((AssemblyProductAttribute)productarray[0]).Product;
						}

						// Creator
						object[] companyarray = asm.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
						if (companyarray != null && companyarray.Length > 0)
						{
							Creator.Value = ((AssemblyCompanyAttribute)companyarray[0]).Company;
						}

						// 
						object[] metadataarray = asm.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false);
						if (metadataarray != null && metadataarray.Length > 0)
						{
							MetaDataCaption.Value = ((AssemblyMetadataAttribute)metadataarray[0]).Key;
							var metaValue = ((AssemblyMetadataAttribute)metadataarray[0]).Value;
							if (metaValue != null)
							{
								MetaData.Value = metaValue.ToString();
							}
						}
					}
				}
			}
			catch
			{
				// アセンブリ情報（タイトルバー表示）が取れない場合はなにもせずデフォルト名を表示する
			}
		}
		private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
				e.Handled = true;
			}
			catch { }
		}

		public ReactivePropertySlim<String> Creator { get; } = new ReactivePropertySlim<String>("---");
		public ReactivePropertySlim<String> MetaDataCaption { get; } = new ReactivePropertySlim<String>("MetaData");
		public ReactivePropertySlim<String> MetaData{ get; } = new ReactivePropertySlim<String>("---");


	}
}
