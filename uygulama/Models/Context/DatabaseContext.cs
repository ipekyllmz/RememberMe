using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using uygulama.Models.Entity;

namespace uygulama.Models.Context
{
    public class DatabaseContext : DbContext
    {
      public DbSet<uye> Kullanıcı { get; set; }
        public DbSet<etkinlik> Etkinlik { get; set; }

        
    }
}