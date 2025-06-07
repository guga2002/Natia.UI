using System.ComponentModel.DataAnnotations.Schema;

namespace Natia.Core.Entities;

[Table("emr110info")]
public class Emr110info : AbstractEntity
{
    public string? Port { get; set; }

    public int SourceEmr { get; set; }

    public string? Text { get; set; }
}
