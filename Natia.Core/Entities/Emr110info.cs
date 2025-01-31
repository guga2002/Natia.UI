using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natia.Core.Entities
{
    [Table("emr110info")]
    public class Emr110info : AbstractEntity
    {
        public string? Port { get; set; }
        public int SourceEmr { get; set; }
        public string? Text { get; set; }
    }
}
