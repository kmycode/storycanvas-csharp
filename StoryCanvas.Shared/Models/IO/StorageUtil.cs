using StoryCanvas.Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.IO
{
    static class StorageUtil
    {
        /// <summary>
        /// 現在アクティブになっているストレージ。または、最後に使用されたストレージ
        /// </summary>
        public static IStorage CurrentStorage
        {
            get => _currentStorage;
            set
            {
                if (_currentStorage != value)
                {
                    var old = _currentStorage;
                    _currentStorage = value;
                    CurrentStorageChanged?.Invoke(null, new ValueChangedEventArgs<IStorage>
                    {
                        OldValue = old,
                        NewValue = value,
                    });
                }
            }
        }
        private static IStorage _currentStorage = LocalStorage;

        public static IStorage LocalStorage
        {
            get
            {
                if (_localStorage == null)
                {
                    _localStorage = new LocalStorage();
                    _localStorage.InitializeAsync().ConfigureAwait(false);
                }
                return _localStorage;
            }
        }
        private static IStorage _localStorage;

        /// <summary>
        /// ストレージを初期化する
        /// </summary>
        public static void InitializeStorages()
        {
            // ローカルストレージ
            LocalStorage.StartLoginAsync().ConfigureAwait(false);
            LocalStorage.InitializeAsync().ConfigureAwait(false);
        }

        public static event EventHandler<ValueChangedEventArgs<IStorage>> CurrentStorageChanged;
    }
}
