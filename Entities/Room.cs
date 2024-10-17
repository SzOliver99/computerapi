using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace computerapi.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? UserId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser? User { get; set; }
    }
}
