﻿using System;
using System.Reflection;

namespace Servicies
{
    // サポート用：各種実行環境情報
    public class EnvironmentInfo
    {
        public EnvironmentInfo()
        {
            try
            {
                // Windowsのバージョン
                System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_OperatingSystem");
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc)
                {
                    //簡単な説明（Windows 8.1では「Microsoft Windows 8.1 Pro」等）
                    _WindowsCaption += (mo["Caption"]);
                    //バージョン（Windows 8.1では、「6.3.9600」）
                    _WindowsVersion += (mo["Version"]);
                }

                // OSとアプリのビット数
                _ProcBit = (System.Environment.Is64BitProcess) ? "(64bit)" : "(32bit)";
                _OsBit = (System.Environment.Is64BitOperatingSystem) ? "(64bit)" : "(32bit)";
            }
            catch { }
        }

        private string _WindowsCaption = string.Empty;
        public string WindowsCaption { get { return _WindowsCaption; } }

        private string _WindowsVersion = string.Empty;
        public string WindowsVersion { get { return _WindowsVersion; } }

        private string _ClrVersionRuntime = string.Empty;
        public string ClrVersionRuntime { get { return _ClrVersionRuntime; } }

        private string _ProcBit = string.Empty;
        public string ProcBit { get { return _ProcBit; } }

        private string _OsBit = string.Empty;
        public string OsBit { get { return _OsBit; } }
    }

    // アセンブリ情報
    public class AssemblyAttribute
    {
        public AssemblyAttribute()
        {
            try
            {
				//AssemblyTitleの取得
				if (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute)) is AssemblyTitleAttribute asmttl)
				{
					_Title = asmttl.Title;
				}

				//AssemblyCompanyの取得
				if (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute)) is AssemblyCompanyAttribute asmcmp)
				{
					_Company = asmcmp.Company;
				}

				//AssemblyProductの取得
				if (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),typeof(AssemblyProductAttribute)) is AssemblyProductAttribute asmprd)
				{
					_Product = asmprd.Product;
				}

				//AssemblyCopyrightの取得
				if (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),typeof(AssemblyCopyrightAttribute)) is AssemblyCopyrightAttribute asmcpy)
				{
					_Copyright = asmcpy.Copyright;
				}
            }
            catch { }
        }

        private string _Title = string.Empty;
        public string Title { get { return _Title; } }

        private string _Company = string.Empty;
        public string Company { get { return _Company; } }

        private string _Product = string.Empty;
        public string Product { get { return _Product; } }

        private string _Copyright = string.Empty;
        public string Copyright { get { return _Copyright; } }
    }

}
