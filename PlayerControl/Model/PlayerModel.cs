using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;

namespace PlayerControl.Model
{
	public class PlayerModel : BindableBase, INotifyPropertyChanged
	{
		private String _name = String.Empty;
		public String Name { get => _name; set => SetProperty(ref _name, value); }

		private String _twitter = String.Empty;
		public String Twitter { get => _twitter; set => SetProperty(ref _twitter, value); }

		private int _score = 0;
		public int Score { get => _score; set => SetProperty(ref _score, value); }

		private int _score_Second = 0;
		public int Score_Second { get => _score_Second; set => SetProperty(ref _score_Second, value); }

		private DateTime _lastAccess = DateTime.MinValue;
		public DateTime LastAccess { get => _lastAccess; set => SetProperty(ref _lastAccess, value); }

		private String _country = String.Empty;
		public String Country { get => _country; set => SetProperty(ref _country, value); }

		private bool _isSelectedUser = false;
		[JsonIgnore]
		public bool IsSelectedUser { get => _isSelectedUser; set => SetProperty(ref _isSelectedUser, value); }

		public PlayerModel()
		{
			// ※ 引数なしの場合、LastAccess は初期値（MinValue）のままにしておく
		}
		public PlayerModel(String name, String twitter, int score, int score_second, String country = "")
		{
			Name = name;
			Twitter = twitter;
			Score = score;
			Score_Second = score_second;
			LastAccess = DateTime.Now;
			Country = country;
		}
	}
}
