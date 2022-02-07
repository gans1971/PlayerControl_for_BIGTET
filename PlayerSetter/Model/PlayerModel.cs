using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSetter.Model
{
    public class PlayerModel : BindableBase
    {
        private String _name = String.Empty;
        public String Name { get => _name; set => SetProperty(ref _name, value); }

    }
}
