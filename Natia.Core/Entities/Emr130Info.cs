using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities;

[Table("emr130info")]
public class Emr130Info : AbstractEntity
{
    public string? Port { get; set; }

    public int SourceEmr { get; set; }

    public string? Text { get; set; }
}
