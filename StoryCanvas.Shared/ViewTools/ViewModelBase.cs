using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StoryCanvas.Shared.ViewTools
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		private List<ModelMessageData> _modelMessageData = new List<ModelMessageData>();

		protected void StoreModel(INotifyPropertyChanged model)
		{
			model.PropertyChanged += this.OnPropertyChanged;
		}

		protected void SpendModel(INotifyPropertyChanged model)
		{
			model.PropertyChanged -= this.OnPropertyChanged;
		}

		// 特定の種類のメッセージをあるモデルと結びつけることを許可する
		protected void PermitStoreMessage(INotifyPropertyChanged model, Type messageType)
		{
			this._modelMessageData.Add(new ModelMessageData
			{
				model = model,
				messageType = messageType,
			});
		}

		public void RegisterMessage<TMessage>(Action<TMessage> action)
		{
			var query = _modelMessageData.Where(o => o.messageType == typeof(TMessage))
							.Select(o => o.model);
			foreach (var model in query)
			{
				Messenger.Default.Register(model, action);
			}
		}

		private struct ModelMessageData
		{
			public INotifyPropertyChanged model;
			public Type messageType;
		}


		// [INotifyPropertyChanged]

		// プロパティ変更イベント（受信用）
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		// プロパティ変更イベント（送信用）
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}
	}
}
