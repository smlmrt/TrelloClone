namespace TrelloClone.Core.Entities
{
    public class Column : BaseEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }

        // ilişkiler
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}