using System.ComponentModel.DataAnnotations;

namespace Natia.Core.Entities
{
    public abstract class AbstractEntity
    {
        [Key]
        public int Id { get; set; }

        protected AbstractEntity()
        {

        }

        public AbstractEntity(int Id)
        {
            this.Id = Id;
        }
    }
}
