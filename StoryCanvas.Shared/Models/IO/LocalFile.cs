using PCLStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    /// <summary>
    /// ローカルのファイル
    /// </summary>
    class LocalFile : IFileObject
    {
        private bool isLock;

        private IFolder folder;
        private IFile file;
        private string fileName;

        public LocalFile(IFolder folder, string name)
        {
            this.folder = folder;
            this.fileName = name;
        }

        public async Task DeleteAsync()
        {
            await this.OpenAsync();
            await this.file.DeleteAsync();
            this.Deleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task<DateTime> GetLastModifiedAsync()
        {
            await this.OpenAsync();
#if WINDOWS_UWP
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(this.fileName);
            var property = await file.GetBasicPropertiesAsync();
            return property.DateModified.LocalDateTime;
#else
            throw new NotImplementedException();
#endif
        }

        public async Task<bool> IsExistsAsync()
        {
            return await this.folder.CheckExistsAsync(this.fileName) == ExistenceCheckResult.FileExists;
        }

        private async Task CreateOrOpenAsync()
        {
            if (this.file == null)
            {
                bool isOpen = await this.IsExistsAsync();
                this.file = await this.folder.CreateFileAsync(this.fileName, CreationCollisionOption.OpenIfExists);
                if (!isOpen)
                {
                    this.Created?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private async Task OpenAsync()
        {
            if (this.file == null)
            {
                this.file = await this.folder.GetFileAsync(this.fileName);
            }
        }

        public async Task<T> LoadAsync<T>()
        {
            while (this.isLock);
            this.isLock = true;

            T result;
            await this.OpenAsync();
            using (var stream = await this.file.OpenAsync(FileAccess.Read))
            {
                result = SerializationUtil.Deserialize<T>(stream);
            }

            this.isLock = false;
            return result;
        }

        public async Task SaveAsync(object obj)
        {
            while (this.isLock);
            this.isLock = true;

            await this.CreateOrOpenAsync();
            using (var stream = await this.file.OpenAsync(FileAccess.ReadAndWrite))
            {
                SerializationUtil.Serialize(stream, obj);
            }

            this.isLock = false;
            this.Modified?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Created;
        public event EventHandler Modified;
        public event EventHandler Deleted;
    }
}
