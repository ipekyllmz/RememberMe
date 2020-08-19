using Facebook;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using uygulama.Helpers;
using uygulama.Models.Context;
using uygulama.Models.Entity;

namespace uygulama.Controllers
{
    [RequireHttps]
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
                Session["ID"] = kayitlicerez.Values["ID"];
                Session["UyeAdi"] = kayitlicerez.Values["UyeAdi"];
                Session["UyeSoyadi"] = kayitlicerez.Values["UyeSoyadi"];

                return RedirectToAction("Anasayfa", "Home");
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

                        return Json(kayitli, JsonRequestBehavior.AllowGet);
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

        public ActionResult SifremiUnuttum(string KullaniciAdi)
        {
            var kayitliuye = db.Kullanıcı.Where(a => a.KullaniciAdi == KullaniciAdi).FirstOrDefault();

            var otoSifre = OtoSifre();
            kayitliuye.Sifre = otoSifre;
            kayitliuye.Sifre2 = otoSifre;
            var kayitlisifre = kayitliuye.Sifre;

            string siteUri = ConfigHelper.Get<string>("SiteRootUri");
            string sifreUri = string.Format("{0}/Home/GirisYap", siteUri);
            string body = string.Format("Merhaba {0};<br><br>Şifreniz:{1};<br>Giriş yapmak için <a href='{2}' target='_blank'>tıklayınız</a>", kayitliuye.KullaniciAdi, kayitlisifre, sifreUri);

            MailHelper.SendMail(body, kayitliuye.UyeMail, "RememberMe Şifremi Unuttum");

            kayitlisifre = MD5Sifrele(kayitliuye.Sifre);

            int sonuc = db.SaveChanges();

            return View();
        }
        public ActionResult SifremiUnuttumOk()
        {
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
        public string Decrypt(string Sifre)
        {
            byte[] data = Convert.FromBase64String(Sifre);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Sifre));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }
        public static string OtoSifre()
        {
            Random Rnd = new Random();
            StringBuilder StrBuild = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                int ASCII = Rnd.Next(32, 127);
                char Karakter = Convert.ToChar(ASCII);
                StrBuild.Append(Karakter);

            }

            return StrBuild.ToString();

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

        private Uri RediredtUri

        {

            get

            {

                var uriBuilder = new UriBuilder(Request.Url);

                uriBuilder.Query = null;

                uriBuilder.Fragment = null;

                uriBuilder.Path = Url.Action("FacebookCallback");

                return uriBuilder.Uri;

            }

        }




        [AllowAnonymous]

        public ActionResult Facebook()

        {

            var fb = new FacebookClient();

            var loginUrl = fb.GetLoginUrl(new

            {


                client_id = "226800241892944",

                client_secret = "bcc29a43565a0deedbf6576cd8c1512a",

                redirect_uri = RediredtUri.AbsoluteUri,

                response_type = "code",

                scope = "email"

            });

            return Redirect(loginUrl.AbsoluteUri);

        }




        public ActionResult FacebookCallback(string code)

        {

            var fb = new FacebookClient();

            dynamic result = fb.Post("oauth/access_token", new

            {

                client_id = "226800241892944",

                client_secret = "bcc29a43565a0deedbf6576cd8c1512a",

                redirect_uri = RediredtUri.AbsoluteUri,

                code = code




            });

            var accessToken = result.access_token;

            Session["AccessToken"] = accessToken;

            fb.AccessToken = accessToken;

            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");

            string email = me.email;

            TempData["email"] = me.email;

            TempData["first_name"] = me.first_name;

            TempData["lastname"] = me.last_name;

            TempData["picture"] = me.picture.data.url;

            FormsAuthentication.SetAuthCookie(email, false);

            return RedirectToAction("Anasayfa", "Home");

        }




    }

}
