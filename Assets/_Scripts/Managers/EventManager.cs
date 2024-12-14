using System;
using System.Collections.Generic;

namespace Managers
{
	public static class EventManager
	{
		public static void Subscribe<TEvent>(Action<TEvent> callback) where TEvent : EventArgs
		{
			EventHandlers<TEvent>.Subscribe(callback);
		}

		public static void UnSubscribe<TEventType>(Action<TEventType> callback) where TEventType : EventArgs
		{
			EventHandlers<TEventType>.UnSubscribe(callback);
		}

		public static void RaiseEvent<TEventType>(TEventType eventArgs) where TEventType : EventArgs, new()
		{
			EventHandlers<TEventType>.RaiseEvent(eventArgs);
		}
	}

	public static class EventHandlers<TEvent> where TEvent : EventArgs
	{
		private static readonly List<Action<TEvent>> callbacks = new List<Action<TEvent>>();

		public static void Subscribe(Action<TEvent> callback)
		{
			if (!callbacks.Contains(callback))
			{
				callbacks.Add(callback);
			}
		}

		public static void UnSubscribe(Action<TEvent> callback)
		{
			if (callbacks.Contains(callback))
			{
				callbacks.Remove(callback);
			}
		}

		public static void RaiseEvent(TEvent eventArgs)
		{
			foreach (Action<TEvent> callback in callbacks)
			{
				callback.Invoke(eventArgs);
			}
		}
	}
}
