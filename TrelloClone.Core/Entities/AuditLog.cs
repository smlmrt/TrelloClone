namespace TrelloClone.Core.Entities
{
    public class AuditLog : BaseEntity
    {
        public string Action { get; set; } // Örn: "Kart Silindi"
        public string Details { get; set; } // Örn: "Ahmet 'Arayüz' görevini sildi."
        public string Username { get; set; }
        public int? CardId { get; set; }
    }
}