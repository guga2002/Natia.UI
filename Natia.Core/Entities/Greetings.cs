using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    [Table("Greetings")]
    public class Greetings
    {
        [Key]
        public int GreetingId { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
    }
}
