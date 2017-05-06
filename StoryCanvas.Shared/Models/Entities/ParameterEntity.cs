using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Entities
{
	/// <summary>
	/// 要素のカスタムパラメータ
	/// </summary>
	[DataContract]
	public class ParameterEntity : Entity
	{
		/// <summary>
		/// パラメータの種類
		/// </summary>
		public enum ParameterType
		{
			Boolean,
			String,
			Select,
		}

		/// <summary>
		/// 全ての人物につけるべきパラメータであるか
		/// </summary>
		[DataMember]
		private bool _isForAllPeople;
		public bool IsForAllPeople
		{
			get
			{
				return this._isForAllPeople;
			}
			set
			{
				this._isForAllPeople = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// パラメータの種類
		/// </summary>
		[DataMember]
		private ParameterType _type;
		public ParameterType Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 選択肢のリスト
		/// </summary>
		[DataMember]
		private ObservableCollection<SelectCell> _selectCells;
		public ObservableCollection<SelectCell> SelectCells
		{
			get
			{
				if (this._selectCells == null)
				{
					this._selectCells = new ObservableCollection<SelectCell>();
				}
				return this._selectCells;
			}
		}

		/// <summary>
		/// 選択肢の項目
		/// </summary>
		[DataContract]
		public class SelectCell : INotifyPropertyChanged
		{
			public static long LastId
			{
				get;
				set;
			} = 0;

			public SelectCell()
			{
				this._id = LastId++;
			}

			[DataMember]
			private long _id;
			public long Id
			{
				get
				{
					return this._id;
				}
				private set
				{
					this._id = value;
					this.OnPropertyChanged();
				}
			}

			[DataMember]
			private string _name;
			public string Name
			{
				get
				{
					return this._name;
				}
				set
				{
					this._name = value;
					this.OnPropertyChanged();
				}
			}

			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;
			protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
			{
				if (PropertyChanged == null) return;

				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}

			#endregion
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "parameter";
			}
		}
	}
}
