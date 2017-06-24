using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    public interface IStorage
    {
        /// <summary>
        /// ワークスペース
        /// </summary>
        ObservableCollection<IWorkspace> Workspaces { get; }

        /// <summary>
        /// 選択されているワークスペース
        /// </summary>
        IWorkspace SelectedWorkspace { get; set; }

        /// <summary>
        /// 現在使用中のスロット
        /// </summary>
        ISlot UsingSlot { get; }

        /// <summary>
        /// ログイン処理の途中であるか
        /// </summary>
        bool IsLogining { get; }

        /// <summary>
        /// ログイン処理が完了し、読み込み可能な状態になっているか
        /// </summary>
        bool HasLogined { get; }

        /// <summary>
        /// 初期化を行う。ログイン後に行う必要がある
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// ログイン画面を表示する
        /// </summary>
        Task StartLoginAsync();

        /// <summary>
        /// 現在使用中のスロットの利用を解除する
        /// </summary>
        void ResetUsingSlot();
    }
}
