using System;
namespace MyFormsLibrary.Services
{
	public interface IMessagingService
	{
		void Subscribe<T>(object subscriber, Action<T> callback) where T : class;

		void Send<T>(T sender) where T : class;

		void UnSubscribe<T>(object subscriber) where T : class;
	}
}

