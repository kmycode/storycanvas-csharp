using PCLStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    /// <summary>
    /// ローカルのスロット
    /// </summary>
    class LocalSlot : ISlot, INotifyPropertyChanged
    {
        private IFolder folder;

        public int Id { get; set; }

        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _name;

        public string Comment
        {
            get => this._comment;
            set
            {
                if (this._comment != value)
                {
                    this._comment = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _comment;

        public bool IsExists
        {
            get => this._isExists;
            set
            {
                if (this._isExists != value)
                {
                    this._isExists = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isExists;

        public DateTime LastModified
        {
            get => this._lastModified;
            set
            {
                if (this._lastModified != value)
                {
                    this._lastModified = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private DateTime _lastModified;

        public IFileObject File
        {
            get
            {
                if (this._file == null)
                {
                    this._file = new LocalFile(this.folder, "slot" + this.Id + ".dat");
                    this._file.Created += (sender, e) =>
                    {
                        this.IsExists = true;
                    };
                    this._file.Modified += (sender, e) =>
                    {
                        this.IsExists = true;
                    };
                    this._file.Deleted += (sender, e) =>
                    {
                        this.IsExists = false;
                    };
                }
                return this._file;
            }
        }
        private LocalFile _file;

        public LocalSlot(IFolder folder)
        {
            this.folder = folder;
        }

        public async Task UpdateExistsAsync()
        {
            this.IsExists = await this.File.IsExistsAsync();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
