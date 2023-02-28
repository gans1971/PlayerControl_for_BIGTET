using Converters;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace PlayerControl.Model
{
	[TypeConverter(typeof(EnumDisplayTypeConverter))]
	public enum ScoreMode
	{
		[Display(Name = "シングル")]
		Single = 0,
		[Display(Name = "ノーマル + 20G")]
		Mixture = 1,
	}

	public class PlayerModel : BindableBase
    {
        public ReactivePropertySlim<String> Name { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<String> Twitter { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<int> PersonalBest { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Score { get; } = new ReactivePropertySlim<int>(0);
		public ReactivePropertySlim<int> Score_Second { get; } = new ReactivePropertySlim<int>(0);
		public ReactivePropertySlim<DateTime> LastAccess { get; } = new ReactivePropertySlim<DateTime>(DateTime.MinValue);
		public ReactivePropertySlim<bool> IsPlayerNameInEditMode { get; } = new ReactivePropertySlim<bool>(false);

		public PlayerModel()
        {
			// ※ 引数なしの場合、LastAccess は初期値（MinValue）のままにしておく
        }
        public PlayerModel(String name, String twitter, int personalbest, int score )
        {
            Name.Value = name;
			Twitter.Value = twitter;
            PersonalBest.Value = personalbest;
            Score.Value = score;
            LastAccess.Value = DateTime.Now;
        }
	}
}
