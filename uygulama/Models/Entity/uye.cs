using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uygulama.Models.Entity
{
    [Table("Uye")]
    public class uye
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Ad"), Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        public string UyeAdi { get; set; }

        [DisplayName("Soyad"), Required()]
        public string UyeSoyadi { get; set; }

        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        public string KullaniciAdi { get; set; }

        [DisplayName("E-Mail"), EmailAddress(ErrorMessage = "Lüften geçerli bir E-mail adresi giriniz"), Required(ErrorMessage = "Bu alanı boş bırakılamaz"),]

        public string UyeMail { get; set; }

        [DisplayName("E-Mail Doğrula"), EmailAddress(ErrorMessage = "E-Mail ile uyuşmamaktadır"), Required(ErrorMessage = "Bu alanı boş bırakılamaz"), Compare(nameof(UyeMail))]
        public string UyeMail2 { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "Bu alan boş bırakılamaz"), MinLength(6, ErrorMessage = "Şifre minimum 6 karakter olmalıdır"), DataType(DataType.Password)]
        public string Sifre { get; set; }

        [DisplayName("Şifre Doğrula"), Required(ErrorMessage = "Bu alan boş bırakılamaz"), MinLength(6), DataType(DataType.Password, ErrorMessage = "Şifre ile uyuşmamaktadır"), Compare(nameof(Sifre))]
        public string Sifre2 { get; set; }

        public Guid ActivateGuid { get; set; }
        public bool isactivate { get; set; }
        public bool Benihatirla { get; set; }


    }
}