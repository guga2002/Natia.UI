using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Natia.Core.Entities;


[Table("Infos")]
public class Infos
{
    [Key]
    public int Id { get; set; }

    [Column("Alarm_For_Display")]
    public string? AlarmMessage { get; set; }

    [ForeignKey("chanell")]
    [Column("CHanell_Id")]
    public int CHanellId { get; set; }

    public Chanells? chanell { get; set; }
}
