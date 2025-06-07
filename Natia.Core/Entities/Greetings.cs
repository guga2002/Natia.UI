using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Natia.Core.Entities;

[Table("Greetings")]
[Index(nameof(Category))]
[Index(nameof(Text))]
public class Greetings
{
    [Key]
    public int GreetingId { get; set; }

    public string? Text { get; set; }

    public string? Category { get; set; }
}
