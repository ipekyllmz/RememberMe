using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using uygulama.Models.Entity;
using uygulama.Models.Context;
using System.Data.Entity.Validation;
using System.Web.UI.WebControls;
using uygulama.Helpers;
using System.Web;

namespace uygulama.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: Home

        public ActionResult GirisYap()
        {
            if (Request.Cookies["cerezim"] != null)
            {
                HttpCookie kayitlicerez = Request.Cookies["cerezim"];

                Session["UyeMail"] = kayitlicerez.Values["UyeMail"];
                Session["ID"] = kayitlicerez.Values["ID"].ToString();
                Session["adsoyad"] = kayitlicerez.Values["UyeAdi"];

                return RedirectToAction("GirisYap", "Home");
            }

            return View();

        }
        [HttpPost]
        public ActionResult GirisYap(string KullaniciAdi, string Sifre, bool? Benihatirla)
        {

            try
            {

                var md5Sifre = MD5Sifrele(Sifre);
                Sifre = md5Sifre;
                var kayitli = db.Kullanıcı.Where(a => a.KullaniciAdi == KullaniciAdi && a.Sifre == Sifre).FirstOrDefault();

                if (kayitli != null && kayitli.isactivate)
                {

                    if (Benihatirla == true)
                    {
                        HttpCookie cerez = new HttpCookie("cerezim");
                        cerez.Values.Add("UyeMail", kayitli.UyeMail);
                        cerez.Values.Add("Sifre", kayitli.Sifre);
                        cerez.Values.Add("ID", kayitli.ID.ToString());
                        cerez.Values.Add("UyeAdi", kayitli.UyeAdi);
                        cerez.Values.Add("UyeSoyadi", kayitli.UyeSoyadi);
                        cerez.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cerez);


                        Session["UyeAdi"] = kayitli.UyeAdi;
                        Session["UyeSoyadi"] = kayitli.UyeSoyadi;

                        return Json(kayitli, JsonRequestBehavior.AllowGet);
                    }


                    else
                    {
                        ViewBag.uyari = "E-Mail Adresinizi veya Şifrenizi Kontrol Ediniz";

                        return View();
                    }
                }
                else
                {

                    Session["noActive"] = kayitli;

                    ViewBag.Message = "kullanıcı kayıtlı değil.";

                    return Json(kayitli, JsonRequestBehavior.DenyGet);



                }
            }

            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
        public ActionResult CikisYap()
        {
            Session["ID"] = null;
            Session.Abandon();
            if (Request.Cookies["cerezim"] != null)
            {
                Response.Cookies["cerezim"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("GirisYap", "Home");
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

        public ActionResult Anasayfa()

        {
            ViewBag.Message = Session["kayitli"];

            List<etkinlik> Etkinlik = db.Etkinlik.ToList();


            return View();
        }

        public ActionResult Profil()
        {
            return View();
        }

        public ActionResult Arkadaslar()
        {
            return View();
        }

        public ActionResult KayitliNotlar()
        {
            return View();
        }

        public ActionResult DogumGunleri()
        {
            return View();
        }

        [HttpGet]
        public JsonResult EtkinlikGetir()
        {

            List<etkinlik> Etkinlik = db.Etkinlik.ToList();



            return Json(Etkinlik, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EtkinlikGetir(string date)
        {
            try
            {


                List<etkinlik> etkinlikler = db.Etkinlik.Where(q => q.DogumTarihi == date).ToList();

                return Json(etkinlikler, JsonRequestBehavior.AllowGet);

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


        }

        [HttpGet]
        public ActionResult EtkinlikEkle()
        {


            return View();
        }

        [HttpPost]
        public JsonResult EtkinlikEkle(etkinlik etkinlik)
        {
            try
            {
                db.Etkinlik.Add(etkinlik);
                var a = db.SaveChanges();

                return Json(etkinlik, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }




        }

        public ActionResult Index()
        {
            if (Request.Cookies["cerezim"] != null)
            {
                HttpCookie kayitlicerez = Request.Cookies["cerezim"];
                Session["eposta"] = kayitlicerez.Values["eposta"];
                Session["yetkiid"] = kayitlicerez.Values["yetkiid"];
                Session["adsoyad"] = kayitlicerez.Values["adsoyad"];
                return RedirectToAction("Panel", "Ana");
            }
            return View();
        }

    }
}