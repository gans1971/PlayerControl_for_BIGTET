using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Diagnostics;

namespace PlayerControl.Model
{
	public class PlayerModel : BindableBase
    {
        public ReactivePropertySlim<String> Name { get; } = new ReactivePropertySlim<string>(String.Empty);
        public ReactivePropertySlim<int> PersonalBest { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Score { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<DateTime> LastAccess { get; } = new ReactivePropertySlim<DateTime>(DateTime.MinValue);
		public ReactivePropertySlim<bool> IsPlayerNameInEditMode { get; } = new ReactivePropertySlim<bool>(false);


		public PlayerModel()
        {
			// ※ 引数なしの場合、LastAccess は初期値（MinValue）のままにしておく
        }
        public PlayerModel(String name, int personalbest, int score )
        {
            Name.Value = name;
            PersonalBest.Value = personalbest;
            Score.Value = score;
            LastAccess.Value = DateTime.Now;
        }


	}

	public class StreamControlParam
	{
		public StreamControlParam()
		{
			timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
			Debug.WriteLine($"TimeStump:{timestamp}");
		}
		public String pCountry1 { get; set; } = "blk";
		public String pCountry2 { get; set; } = "blk";
		public String pName1 { get; set; } = String.Empty;
		public String pName2 { get; set; } = String.Empty;
		public String pScore1 { get; set; } = String.Empty;
		public String pScore2 { get; set; } = String.Empty;
		public String stage { get; set; } = String.Empty;
		public String timestamp { get; set; } = "0";
	}
}
