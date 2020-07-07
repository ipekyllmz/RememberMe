using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace uygulama.Models.Entity
{
    [Table("Etkinlik")]
    public class etkinlik
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required()]
        public string EtkinlikAdi { get; set; }

        [Required()]
        public string Not { get; set; }

        [Required()]
        public int DogumGunu { get; set; }
        [Required()]
        public string DogumTarihi { get; set; }

        [Required()]
        public string Kisi { get; set; }
        
    }
}