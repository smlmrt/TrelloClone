namespace TrelloClone.Core.Entities
{
    public class Card : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public int ColumnId { get; set; }
        public Column Column { get; set; }
    }
}