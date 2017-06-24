using PCLStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace StoryCanvas.Shared.Models.IO
{
    /// <summary>
    /// ローカルのストレージ
    /// </summary>
    class LocalStorage : IStorage, INotifyPropertyChanged
    {
        public ObservableCollection<IWorkspace> Workspaces { get; } = new ObservableCollection<IWorkspace>();

        public IWorkspace SelectedWorkspace
        {
            get => this._selectedWorkspace;
            set
            {
                if (this._selectedWorkspace != value)
                {
                    this._selectedWorkspace = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private IWorkspace _selectedWorkspace;

        public ISlot UsingSlot
        {
            get => this._usingSlot;
            set
            {
                if (this._usingSlot != value)
                {
                    this._usingSlot = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private ISlot _usingSlot;

        public bool IsLogining
        {
            get => this._isLogining;
            protected set
            {
                if (this._isLogining != value)
                {
                    this._isLogining = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isLogining;

        public bool HasLogined
        {
            get => this._hasLogined;
            protected set
            {
                if (this._hasLogined != value)
                {
                    this._hasLogined = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _hasLogined;

        public LocalStorage()
        {
            for (int i = 0; i < 4; i++)
            {
                var workspace = new LocalWorkspace
                {
                    Id = i + 1,
                    Name = "Workspace " + (i + 1),
                };
                this.Workspaces.Add(workspace);
            }
        }

        public async Task InitializeAsync()
        {
            foreach (var workspace in this.Workspaces)
            {
                await workspace.LoadAsync();
            }
            this.SelectedWorkspace = this.Workspaces[0];
        }

        public void ResetUsingSlot()
        {
            this.UsingSlot = null;
        }

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        public async Task StartLoginAsync()
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            this.HasLogined = true;
        }

        public override string ToString() => "Local";

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
