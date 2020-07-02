using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using uygulama.Models.Context;
using uygulama.Models.Entity;

namespace uygulama.Controllers
{
    public class UyeController : Controller
    {

        // GET: üye
        public ActionResult UyeOl()
        {


            return View(new uye());
        }
        [HttpPost]
        public ActionResult UyeOl(uye uye)
        {
            try
            {


                DatabaseContext db = new DatabaseContext();
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

                var gelenuye = db.Kullanıcı.Where(a=>a.KullaniciAdi == uye.KullaniciAdi);
                gelenuye = db.Kullanıcı.Where(a => a.Sifre == uye.Sifre);
                gelenuye = db.Kullanıcı.Where(a => a.UyeAdi == uye.UyeAdi);
                gelenuye = db.Kullanıcı.Where(a => a.UyeSoyadi == uye.UyeSoyadi);
                gelenuye = db.Kullanıcı.Where(a => a.UyeMail == uye.UyeMail);


                if (gelenuye.Count() > 0 )
                {
                    ViewBag.Result = "Bu kullanıcı zaten kayıtlı";
                    ViewBag.Status = "error";
                }
                else
                {

                    var result = db.Kullanıcı.Add(uye);

                    if (result.ID == 0)
                    {
                        int sonuc = db.SaveChanges();
                        ViewBag.Result = "Kişi kaydedilmiştir.";
                        ViewBag.Status = "ok";
                    }
                    else
                    {
                        ViewBag.Result = "Kişi kaydedilememiştir.";
                        ViewBag.Status = "error";
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
    }
}