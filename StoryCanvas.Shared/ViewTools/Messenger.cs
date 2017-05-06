using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace StoryCanvas.Shared.ViewTools
{
	public class Messenger
	{

		private static Messenger _instance = new Messenger();

		public static Messenger Default
		{
			get { return _instance; }
		}

		private List<ActionInfo> list = new List<ActionInfo>();

		public void Register<TMessage>(ViewModelBase recipient, Action<TMessage> action)
		{
			//recipient.RegisterMessage(action);
			this.Register((INotifyPropertyChanged)recipient, action);
		}

		public void Register<TMessage>(IDataContextHolder recipient, Action<TMessage> action)
		{
			ViewModelBase viewModel = recipient.DataContext as ViewModelBase;
			if (viewModel != null)
			{
				this.Register(viewModel, action);
			}
			else
			{
				this.Register(action);
				//throw new ArgumentException("This is not ViewModelBase object!");
			}
		}

		public void Register<TMessage>(INotifyPropertyChanged recipient, Action<TMessage> action)
		{
			/*if (recipient is ViewModelBase)
			{
				this.Register((ViewModelBase)recipient, action);
			}
			else */
			{
				list.Add(new ActionInfo
				{
					Type = typeof(TMessage),
					sender = recipient,
					action = action,
				});
			}
		}

		public void Register<TMessage>(Action<TMessage> action)
		{
			{
				list.Add(new ActionInfo
				{
					Type = typeof(TMessage),
					sender = null,
					action = action,
				});
			}
		}

		public void Send<TMessage>(INotifyPropertyChanged sender, TMessage message)
		{
		//	var query = list.Where(o => o.sender == sender && o.Type == message.GetType())
		//					.Select(o => o.action as Action<TMessage>);
			var query = list.Where(o => o.Type == message.GetType())
							.Select(o => o.action as Action<TMessage>).ToList();
			foreach (var action in query)
			{
				action(message);
			}
		}

		private class ActionInfo
		{
			public Type Type { get; set; }
			public INotifyPropertyChanged sender { get; set; }
			public Delegate action { get; set; }

		}
	}
}
