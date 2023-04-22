using Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlayerControl.Model
{
	/// <summary>
	/// スコアモード
	/// 将来機能（Ver0.3.x では使用しない）
	/// </summary>
	[TypeConverter(typeof(EnumDisplayTypeConverter))]
	public enum ScoreMode
	{
		[Display(Name = "シングル")]
		Single = 0,
		[Display(Name = "自己/本日 Best)")]
		BestChallenge = 1,
		[Display(Name = "ノーマル + 20G")]
		Mixture = 2,
	}
}
