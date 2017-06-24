using System;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    public interface IFileObject
    {
        /// <summary>
        /// ファイルが存在するか調べる
        /// </summary>
        /// <returns>存在すればtrue</returns>
        Task<bool> IsExistsAsync();

        /// <summary>
        /// ファイルの最終更新日時を取得する
        /// </summary>
        /// <returns>最終更新日時</returns>
        Task<DateTime> GetLastModifiedAsync();

        /// <summary>
        /// データをロードする
        /// </summary>
        Task<T> LoadAsync<T>();

        /// <summary>
        /// データを保存する
        /// </summary>
        /// <param name="obj">保存するデータ</param>
        Task SaveAsync(object obj);

        /// <summary>
        /// データを削除する
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync();

        /// <summary>
        /// ファイルが新規作成された時に発行
        /// </summary>
        event EventHandler Created;

        /// <summary>
        /// ファイルが更新された時に発行
        /// </summary>
        event EventHandler Modified;

        /// <summary>
        /// ファイルが削除された時に発行
        /// </summary>
        event EventHandler Deleted;
    }
}
