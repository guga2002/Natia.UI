using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    [Table("emr120info")]
    public class Emr120Info : AbstractEntity
    {
        public string? Port { get; set; }
        public int SourceEmr { get; set; }
        public string? Text { get; set; }
    }
}
