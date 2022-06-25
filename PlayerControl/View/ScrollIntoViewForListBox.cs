using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlayerControl.View
{
	public class ScrollIntoViewForListBox : Behavior<ListBox>
	{
		/// <summary>
		///  When Beahvior is attached
		/// </summary>
		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
		}

		/// <summary>
		/// On Selection Changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox)
			{
				if (listBox.SelectedItem != null)
				{
					listBox.Dispatcher.BeginInvoke(
						(Action)(() =>
						{
							listBox.UpdateLayout();
							if (listBox.SelectedItem != null)
							{
								listBox.ScrollIntoView(listBox.SelectedItem);
							}
						}));
				}
			}
		}
		/// <summary>
		/// When behavior is detached
		/// </summary>
		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.AssociatedObject.SelectionChanged -=
				AssociatedObject_SelectionChanged;
		}
	}
}
