using Prism.Mvvm;
using System;
using System.Reactive.Disposables;

namespace PlayerSetter.ViewModels
{
	public class BindableViewModel : BindableBase, IDisposable
	{
		protected CompositeDisposable Disposable { get; } = new CompositeDisposable();
		private bool disposedValue = false;
		~BindableViewModel()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// まとめてDisposeする				
					Disposable.Dispose();
				}
			}
		}
	}
}
