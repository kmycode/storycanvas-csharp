using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.ViewTools.BehaviorHelpers;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.ViewTools;
using System.Collections.ObjectModel;

namespace StoryCanvas.Shared.ViewModels.EntityEditors
{
    /// <summary>
    /// 人物編集のビューモデル
    /// </summary>
    public class PersonEditorViewModel : EntityEditorWithEachRelationViewModelBase<PersonEntity>
    {
        /// <summary>
        /// 既存のエンティティをマップに追加する時の選択ヘルパ
        /// </summary>
        public SelectEntityHelper<PersonEntity> AddExistsEntitySelector { get; } = new SelectEntityHelper<PersonEntity>();

        public PersonEditorViewModel() : base(StoryModel.Current.PersonEditorModel, StoryModel.Current.People)
        {
            this.Editor.EntitySelectionChanged += (sender, e) =>
            {
                this.SelectedEntity?.UpdateRelations();
            };

            // 既存のエンティティをマップに追加
            this.AddExistsEntitySelector.EntitySelected += (sender, e) =>
            {
                var editor = (PersonEditorModel)this.Editor;
                editor.Canvas.AddEntity(e.Entity);
            };
        }

        public RelayCommand AddExistsEntityCommand
        {
            get
            {
                return this._addExistsEntityCommand = this._addExistsEntityCommand ?? new RelayCommand((obj) =>
                {
                    this.AddExistsEntitySelector.OpenSelection();
                });
            }
        }
        private RelayCommand _addExistsEntityCommand;
    }
}
