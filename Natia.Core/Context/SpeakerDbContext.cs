using Microsoft.EntityFrameworkCore;
using Natia.Core.Entities;

namespace Natia.Core.Context
{
    public class SpeakerDbContext : DbContext
    {

        public SpeakerDbContext(DbContextOptions<SpeakerDbContext> ops) : base(ops)
        {

        }

        public virtual DbSet<Chanells> Chanells { get; set; }
        public virtual DbSet<Infos> Infos { get; set; }
        public virtual DbSet<Transcoder> Transcoders { get; set; }
        public virtual DbSet<Desclamblers> Desclamblers { get; set; }
        public virtual DbSet<Emr60Info> Emr60Info { get; set; }
        public virtual DbSet<Reciever> Recievers { get; set; }
        public virtual DbSet<Emr100Info> Emr100Info { get; set; }
        public virtual DbSet<Emr110info> Emr110Info { get; set; }
        public virtual DbSet<Emr120Info> Emr120Info { get; set; }
        public virtual DbSet<Emr130Info> Emr130Info { get; set; }
        public virtual DbSet<Emr200Info> Emr200Info { get; set; }
        public virtual DbSet<Neurall> Neuralls { get; set; }
        public virtual DbSet<Greetings> Greetings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=192.168.1.102;Database=JandagBase;User Id=Guga13guga;Password=Guga13gagno!;TrustServerCertificate=True;");
        }

    }
}
