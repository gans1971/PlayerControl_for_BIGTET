using Converters;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlayerControl.Model
{
	/// <summary>
	/// ステージおよびユーザー情報に関するModel
	/// ※　StreamControl.json保存時にバックアップする 強制終了時にはこのファイルから状態を復元する
	/// </summary>
	public class OperationModel : BindableBase
	{
		public ReactivePropertySlim<String> Stage { get; } = new ReactivePropertySlim<String>(String.Empty);
		public ReactivePropertySlim<String> ScoreLabel { get; } = new ReactivePropertySlim<String>(String.Empty);
		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();
		public OperationModel()
		{
			Clear();
		}
		public void Clear()
		{
			Stage.Value = String.Empty;
			ScoreLabel.Value = String.Empty;
			Players.Clear();
		}
	}
}
