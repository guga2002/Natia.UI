using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities
{
    [Table("emr100info")]
    public class Emr100Info : AbstractEntity
    {
        public string? Port { get; set; }
        public int SourceEmr { get; set; }
        public string? Text { get; set; }
    }
}
