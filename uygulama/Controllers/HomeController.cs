using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using uygulama.Models.Entity;
using uygulama.Models.Context;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Data.Entity;

namespace uygulama.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: Home
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GirisYap(uye uye)
        {
            return View();
        }

        public ActionResult Anasayfa()
        {
            DatabaseContext db = new DatabaseContext();
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




    }
}