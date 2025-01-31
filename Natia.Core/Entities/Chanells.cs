using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Natia.Core.Entities
{
    [Table("Chanells")]
    public class Chanells
    {
        [Key]
        public int Id { get; set; }

        [Column("ChanellFormat")]
        public string ChanellFormat { get; set; } //MPG4 vs MPG2

        [Column("Port_In_250")]
        public int PortIn250 { get; set; }

        [Column("Is_From_Optic")]
        public bool FromOptic { get; set; }

        [Column("Name_Of_Chanell")]
        public string Name { get; set; }
        public string NameForSPeak { get; set; }
        public List<Infos> Infos { get; set; }
        public List<Transcoder> Transcoder { get; set; }
        public List<Desclamblers> Desclambler { get; set; }
        public List<Reciever> Recievers { get; set; }
    }
}
