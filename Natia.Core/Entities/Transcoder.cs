using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Natia.Core.Entities;

[Table("Transcoders")]
[Index(nameof(EmrNumber))]
[Index(nameof(Card))]
[Index(nameof(Port))]
public class Transcoder : AbstractEntity
{
    [Column("Emr_Number")]
    public int EmrNumber { get; set; }


    [Column("Card_In_Transcoder")]
    public int Card { get; set; }

    [Column("Port_In_Transcoder")]
    public int Port { get; set; }

    [Column("Chanell_Id")]
    [ForeignKey("Chanell")]
    public int ChanellId { get; set; }

    public Chanells? Chanell { get; set; }
}
