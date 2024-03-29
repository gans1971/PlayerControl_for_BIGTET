﻿using System;
using System.Diagnostics;

namespace PlayerControl.Model
{
	public class StreamControlParam
	{
		public StreamControlParam(String _defaultCountry)
		{
			timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
			pCountry1 = _defaultCountry;
			pCountry2 = _defaultCountry;
			Debug.WriteLine($"TimeStump:{timestamp}");
		}
		public String pCountry1 { get; set; } = String.Empty;
		public String pCountry2 { get; set; } = String.Empty;
		public String pName1 { get; set; } = String.Empty;
		public String pName2 { get; set; } = String.Empty;
		public String pScore1 { get; set; } = String.Empty;
		public String pScore2 { get; set; } = String.Empty;
		public String pTwitter1 { get; set; } = String.Empty;
		public String pTwitter2 { get; set; } = String.Empty;
		public String stage { get; set; } = String.Empty;
		public String timestamp { get; set; } = "0";
	}
}
