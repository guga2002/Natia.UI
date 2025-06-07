using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities;

[Table("emr60info")]
public class Emr60Info : AbstractEntity
{
    public string? Port { get; set; }

    public int SourceEmr { get; set; }

    public string? Text { get; set; }
}
