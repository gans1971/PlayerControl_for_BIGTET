using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PlayerControl.Model
{
	/// <summary>
	/// ステージおよびユーザー情報に関するModel
	/// ※　StreamControl.json保存時にバックアップする 強制終了時にはこのファイルから状態を復元する
	/// </summary>
	public class OperationModel : BindableBase, INotifyPropertyChanged
	{
		// publicプロパティがJSONで保存される
		private String _stage = String.Empty;
		public String Stage { get => _stage; set => SetProperty(ref _stage, value); }

		private String _scoreLabel = String.Empty;
		public String ScoreLabel { get => _scoreLabel; set => SetProperty(ref _scoreLabel, value); }

		private ObservableCollection<PlayerModel> _players = new ObservableCollection<PlayerModel>();
		public OperationModel()
		{
			Clear();
		}
		public void Clear()
		{
			Stage = String.Empty;
			ScoreLabel = String.Empty;
			_players.Clear();
		}

		public ObservableCollection<PlayerModel> GetPlayers()
		{
			return _players;
		}

		public bool RemovePlayer(PlayerModel removeTarget)
		{
			return _players.Remove(removeTarget);
		}
		public void AddPlayer(PlayerModel addPlayer)
		{
			if (addPlayer != null)
			{
				_players.Add(addPlayer);
			}
		}
	}
}
