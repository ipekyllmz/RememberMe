using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using uygulama.Helpers;
using uygulama.Models.Context;
using uygulama.Models.Entity;

namespace uygulama.Controllers
{
    public class UyeController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: üye
        public ActionResult UyeOl()
        {


            return View();
        }
        [HttpPost]
        public ActionResult UyeOl(uye uye)
        {
            try
            {

                var x = uye.UyeAdi.ToString().Split();
                uye.UyeSoyadi = x[x.Length - 1];
                var uyeadimiz = "";
                var sifrelipass = MD5Sifrele(uye.Sifre);
                uye.Sifre = sifrelipass;
                uye.Sifre2 = sifrelipass;


                foreach (var item in x)
                {
                    if (item != x[x.Length - 1])
                    {
                        uyeadimiz += item;
                    }
                }
                uye.UyeAdi = uyeadimiz;


                var gelenuye = db.Kullanıcı.Where(a => a.UyeMail == uye.UyeMail);

                //= db.Kullanıcı.Where(a=>a.KullaniciAdi == uye.KullaniciAdi);
                //gelenuye = db.Kullanıcı.Where(a => a.Sifre == uye.Sifre);
                //gelenuye = db.Kullanıcı.Where(a => a.UyeAdi == uye.UyeAdi);
                //gelenuye = db.Kullanıcı.Where(a => a.UyeSoyadi == uye.UyeSoyadi);



                if (gelenuye.Count() > 0)
                {
                    ViewBag.Result = "Bu kullanıcı zaten kayıtlı";
                    ViewBag.Status = "error";
                }
                else
                {
                    uye.ActivateGuid = Guid.NewGuid();
                    var result = db.Kullanıcı.Add(uye);

                    if (result.ID == 0)
                    {
                        int sonuc = db.SaveChanges();


                        TempData["Veri"] = uye.KullaniciAdi;


                        ViewBag.Result = "Kişi Kaydedilmiştir.";
                        ViewBag.Status = "ok";


                        gelenuye = db.Kullanıcı.Where(a => a.ActivateGuid == Guid.NewGuid());
                        string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                        string activateUri = string.Format("{0}/Home/GirisYap/{1}", siteUri, uye.ActivateGuid);
                        string body = string.Format("Merhaba {0};<br><br>Hesabınızı aktifleştirmek için <a href='{1}' target='_blank'>tıklayınız</a>", uye.KullaniciAdi, activateUri);

                        MailHelper.SendMail(body, uye.UyeMail, "RememberMe Hesap Aktifleştirme");



                        return RedirectToAction("UyeAktivasyonuOk", "Uye");
                    }
                    else
                    {
                        ViewBag.Result = "Kişi kaydedilememiştir.";
                        ViewBag.Status = "error";

                        return View();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();

        }





        public static string MD5Sifrele(string Sifre)
        {

            // MD5CryptoServiceProvider sınıfının bir örneğini oluşturduk.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüştürdük.
            byte[] dizi = Encoding.UTF8.GetBytes(Sifre);
            //dizinin hash'ini hesaplattık.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiş verileri depolamak için StringBuilder nesnesi oluşturduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüştürdük.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString().ToLower());
            }

            //hexadecimal(onaltılık) stringi geri döndürdük.
            return sb.ToString();
        }

        public ActionResult UyeAktivasyonu(Guid id)
        {


            var selectedUser = db.Kullanıcı.Where(c => c.ActivateGuid == id).FirstOrDefault();


            if (selectedUser != null && selectedUser.isactivate)

            {
                db.SaveChanges();

                return RedirectToAction("Anasayfa", "Home");

            }

            else
            {

                string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                string activateUri = string.Format("{0}/Home/GirisYap{1}", siteUri, selectedUser.ActivateGuid);
                string body = string.Format("Merhaba {0};<br><br>Hesabınızı aktifleştirmek için <a href='{1}' target='_blank'>tıklayınız</a>", selectedUser.KullaniciAdi, activateUri);

                MailHelper.SendMail(body, selectedUser.UyeMail, "RememberMe Hesap Aktifleştirme");

                return RedirectToAction("UyeAktivasyonuOk", "Uye");


            }

        }

        public ActionResult UyeAktivasyonuOk()
        {
            var TempDataVeri = TempData["Veri"];

            return View();
        }

    }
}