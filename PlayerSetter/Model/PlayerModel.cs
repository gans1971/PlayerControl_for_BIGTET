using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerControl.Model
{
    public class PlayerModel : BindableBase
    {
        public ReactivePropertySlim<String> Name { get; } = new ReactivePropertySlim<string>(String.Empty);
        public ReactivePropertySlim<int> PersonalBest { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> TodayBest { get; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<DateTime> LastAccess { get; } = new ReactivePropertySlim<DateTime>(DateTime.MinValue);

        public PlayerModel()
        {

        }
        public PlayerModel(String name, int personalbest, int todaybest )
        {
            Name.Value = name;
            PersonalBest.Value = personalbest;
            TodayBest.Value = todaybest;
            LastAccess.Value = DateTime.Now;
        }
    }
}
