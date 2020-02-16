using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class BaseEntity
    {
        [Required]
        public string ID { get; set; }
    }
}