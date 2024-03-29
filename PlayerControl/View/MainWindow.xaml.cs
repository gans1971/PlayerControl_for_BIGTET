﻿using MahApps.Metro.Controls;
using PlayerControl.Model;
using PlayerControl.ViewModels;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PlayerControl.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Array ScoreModeEnumValues { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            ScoreModeEnumValues = Enum.GetValues(typeof(ScoreMode));
        }

        /// <summary>
        /// アイテム選択中のキーボード操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is ListBoxItem item)
            {
                switch (e.Key)
                {
                    // JSON保存
                    case Key.Enter:
                        vm.SaveStreamControlJson();
                        item.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate ()
                        {
                            item.Focus();
                        }));

                        e.Handled = true;
                        break;

                    // スコア入力ダイアログを表示
                    case Key.Space:
                        vm.PlayerScoreSetting(item.DataContext);
                        e.Handled = true;
                        break;

                    // 1Pに設定
                    case Key.Left:
                        vm.ChangePlayer(item.DataContext, true);
                        e.Handled = true;
                        break;

                    // 2Pに設定
                    case Key.Right:
                        vm.ChangePlayer(item.DataContext, false);
                        e.Handled = true;
                        break;

                    // クリップボードにユーザー名とスコアをコピー
                    case Key.C:
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            vm.SetPlayerModelNameAndScoreToClipboard(item.DataContext);
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// スコアテキストボックスにキーボードフォーカスが当たったときの処理
        /// IMEをOFFにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScoreTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            InputMethod.Current.ImeState = InputMethodState.Off;
        }

        /// <summary>
        /// スコアテキストボックスの文字入力処理（Preview）
        /// 数字のみ入力できるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScoreTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        /// <summary>
        /// スコアテキストボックスへのコマンド処理(Preview)
        /// 外部から文字をペーストできないようにする（数字のみを許容するため）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScoreTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        /// <summary>
        /// ステージ文字列入力テキストボックのキー入力前処理
        /// Alt + Enter で改行を可能にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StageName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter))
                {
                    var caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);
                    textBox.CaretIndex = caretIndex + Environment.NewLine.Length;
                    e.Handled = true;
                }
            }
        }

		private void BottomMenuItem_MouseEnter(object sender, MouseEventArgs e)
		{
			if (sender is MenuItem menuItem)
			{
				menuItem.IsSubmenuOpen = true;
			}
		}
	}
}
