using StoryCanvas.Shared.Models.Story;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    public interface ISlot
    {
        /// <summary>
        /// スロットの番号
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// スロットの名前
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// スロットのコメント
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// スロットが存在するか
        /// </summary>
        bool IsExists { get; set; }

        /// <summary>
        /// スロットの最終更新日時
        /// </summary>
        DateTime LastModified { get; set; }

        /// <summary>
        /// ファイルオブジェクトを取得する
        /// </summary>
        IFileObject File { get; }

        /// <summary>
        /// ファイルが存在するか、最新情報を取得する
        /// </summary>
        /// <returns></returns>
        Task UpdateExistsAsync();
    }
}
