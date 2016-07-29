/**
 * Copy From
 * http://qiita.com/yu_ka1984/items/5cef0fcb167febbb3e8b
 */
using System;
using Android.App;
using MyFormsLibrary.DependencyServices;
using MyFormsLibrary.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoadingMessage))]
namespace MyFormsLibrary.Droid.DependencyServices
{
	public class LoadingMessage:ILoadingMessage
	{
		private ProgressDialog progress;
		/// <summary>ローディングを開始する</summary>
		/// <param name="message"></param>
		public void Show(string message) {
			Device.BeginInvokeOnMainThread(() => {
				progress = new ProgressDialog(Forms.Context);
				progress.Indeterminate = true;
				progress.SetProgressStyle(ProgressDialogStyle.Spinner);
				progress.SetCancelable(false);
				progress.SetMessage(message);
				progress.Show();
				ishow = true;
			});
		}

		/// <summary>ローディングを終了する</summary>
		public void Hide() {
			Device.BeginInvokeOnMainThread(() => {
				progress?.Dismiss();
				ishow = false;
			});
		}

		/// <summary>状態</summary>
		public bool IsShow => ishow;

		public void SetMessage(string message) {
			Device.BeginInvokeOnMainThread(() => {
				progress.SetMessage(message);
			});
		}

		private bool ishow = false;
	}
}

