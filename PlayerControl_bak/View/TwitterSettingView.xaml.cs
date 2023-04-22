using System;
using System.Windows;
using System.Windows.Controls;

namespace PlayerControl.View
{
    /// <summary>
    /// TwitterSettingView.xaml の相互作用ロジック
    /// </summary>
    public partial class TwitterSettingView : UserControl
    {
        public static readonly DependencyProperty EditTwitterProperty =
            DependencyProperty.Register("EditTwitter", typeof(String),
            typeof(TwitterSettingView), new PropertyMetadata(String.Empty));
        public String EditTwitter
        {
            get { return (String)GetValue(EditTwitterProperty); }
            set { SetValue(EditTwitterProperty, value); }
        }


        public TwitterSettingView()
        {
            InitializeComponent();
        }
    }
}
