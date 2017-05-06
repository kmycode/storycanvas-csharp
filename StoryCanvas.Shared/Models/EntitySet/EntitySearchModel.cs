using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StoryCanvas.Shared.Models.EntitySet
{
	public delegate void EntitySearchRequestedEventHandler(object sender, EntitySearchRequestedEventArgs e);
	public class EntitySearchRequestedEventArgs : EventArgs
	{
		public EntitySearchRequestedEventArgs(EntitySearchQuery query)
		{
			this.Query = query;
		}
		public EntitySearchQuery Query { get; private set; }
	}

	public enum SearchMode
	{
		And,
		Or,
	}

	public struct EntitySearchQuery
	{
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.Keyword);
			}
		}
		private string _keyword;
		public string Keyword
		{
			get
			{
				return this._keyword;
			}
			set
			{
				this._keyword = value ?? "";

				// 半角スペースで区切る
				this.Keywords.Clear();
				var words = this._keyword.Split(' ');
				foreach (var word in words)
				{
					this.Keywords.Add(word);
				}
			}
		}

		private Collection<string> _keywords;
		public Collection<string> Keywords
		{
			get
			{
				return this._keywords = this._keywords ?? new Collection<string>();
			}
		}

		public SearchMode Mode { get; set; }
	}

    public class EntitySearchModel : INotifyPropertyChanged
	{
		public event EntitySearchRequestedEventHandler EntitySearchRequested = delegate { };

		//private string _keyword;
		public string Keyword
		{
			get
			{
				return this.Query.Keyword;
			}
			set
			{
				this.Query.Keyword = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("IsValid");
				this.Search();
			}
		}

		private EntitySearchQuery Query = new EntitySearchQuery
		{
			Mode = SearchMode.And,
		};

		public bool IsValid
		{
			get
			{
				return this.Query.IsValid;
			}
		}

		public void Search()
		{
			this.EntitySearchRequested(this, new EntitySearchRequestedEventArgs(this.Query));
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
}
