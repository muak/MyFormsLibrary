using System;
using Xamarin.Forms;
namespace MyFormsLibrary.Services
{
	public class MessagingService:IMessagingService
	{
		
		public void Subscribe<T>(object subscriber, Action<T> callback) where T :class {
			MessagingCenter.Subscribe<T>(subscriber, "", callback);
		}

		public void Send<T>(T sender) where T :class {
			MessagingCenter.Send<T>(sender, "");		
		}

		public void UnSubscribe<T>(object subscriber) where T : class{
			MessagingCenter.Unsubscribe<T>(subscriber,"");
		}
	}
}

