namespace TrelloClone.Core.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // OneToMany ilişki -> bir projenin birden fazla sütunu olabilir
        public ICollection<Column> Columns { get; set; }
    }
}