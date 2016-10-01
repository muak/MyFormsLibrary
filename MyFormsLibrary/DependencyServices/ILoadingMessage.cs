/**
 * Copy From
 * http://qiita.com/yu_ka1984/items/5cef0fcb167febbb3e8b
 */
using System;

namespace MyFormsLibrary.DependencyServices
{
	public interface ILoadingMessage
	{
		/// <summary>ローディングを開始する</summary>
		/// <param name="message"></param>
		void Show(string message);

		/// <summary>ローディングを終了する</summary>
		void Hide();

		/// <summary>状態</summary>
		bool IsShow { get; }

		void SetMessage(string message);
	}
}

