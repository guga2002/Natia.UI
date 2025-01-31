using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    [Table("emr200info")]
    public class Emr200Info : AbstractEntity
    {
        public string? Port { get; set; }
        public int SourceEmr { get; set; }
        public string? Text { get; set; }
    }
}
