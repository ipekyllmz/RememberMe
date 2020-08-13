using System.Data.Entity;
using uygulama.Models.Entity;

namespace uygulama.Models.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<uye> Kullanıcı { get; set; }
        public DbSet<etkinlik> Etkinlik { get; set; }
        public DbSet<facebook> Facebook { get; set; }


    }
}