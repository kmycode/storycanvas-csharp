using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.EntityRelate
{
    public class EntityRelationEditorModel
    {
		public enum RelationDirection
		{
			Entity1,
			Entity2,
			Focused,
			NotFocused,
		}

		public RelationDirection Direction { get; set; } = RelationDirection.Entity1;
		public RelayCommand AddCommand { get; set; }
		public RelayCommand RemoveCommand { get; set; }
		public Func<IEnumerable<IRelation>> RelatedEntityItemsGetter { get; set; }
		public Func<IEnumerable<Entity>> NotRelatedEntitiesGetter { get; set; }
		public IEntitySelection<Entity> EntityForRelateSelection { get; set; }
		public IEntitySelection<IRelationEntity> RelatedEntityItemSelection { get; set; }
	}
}
