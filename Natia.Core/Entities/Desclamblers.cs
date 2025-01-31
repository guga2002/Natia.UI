using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    public class Desclamblers : AbstractEntity
    {
        [Column("Emr_Number")]
        public int EmrNumber { get; set; }

        [Column("Card_In_Desclambler")]

        public int Card { get; set; }

        [Column("Port_In_Desclambler")]
        public int Port { get; set; }

        [Column("Chanell_Id")]
        [ForeignKey("Chanell")]
        public int ChanellId { get; set; }

        public Chanells Chanell { get; set; }
    }
}
