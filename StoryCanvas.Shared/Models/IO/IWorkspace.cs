using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    public interface IWorkspace
    {
        /// <summary>
        /// スロットの一覧
        /// </summary>
        ObservableCollection<ISlot> Slots { get; }

        /// <summary>
        /// 現在選択されているスロット
        /// </summary>
        ISlot SelectedSlot { get; set; }

        /// <summary>
        /// ワークスペースの番号
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// ワークスペースの名前
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// ワークスペース情報を読み込む
        /// </summary>
        Task LoadAsync();
    }
}
