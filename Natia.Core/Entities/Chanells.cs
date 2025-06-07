using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Natia.Core.Entities;

[Table("Chanells")]
[Index(nameof(PortIn250))]
[Index(nameof(FromOptic))]
public class Chanells
{
    [Key]
    public int Id { get; set; }

    [Column("ChanellFormat")]
    public string? ChanellFormat { get; set; } //MPG4 vs MPG2

    [Column("Port_In_250")]
    public int PortIn250 { get; set; }

    [Column("Is_From_Optic")]
    public bool FromOptic { get; set; }

    [Column("Name_Of_Chanell")]
    public string? Name { get; set; }
    public string? NameForSPeak { get; set; }
    public virtual List<Infos>? Infos { get; set; }
    public virtual List<Transcoder>? Transcoder { get; set; }
    public virtual List<Desclamblers>? Desclambler { get; set; }
    public virtual List<Reciever>? Recievers { get; set; }
}
