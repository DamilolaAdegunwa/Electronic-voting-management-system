using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDMS.Models;
using System.IO;
using System.Data.Entity;
namespace EDMS.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private EDMSEntities db = new EDMSEntities();
        public ActionResult password()
        {
            return View();
        }
        public ActionResult login()
        {
            var model = new login();
            return View(model);
        }
        [HttpPost]
        public ActionResult login([Bind(Include = "username,password")]login model)
        {
            if (ModelState.IsValid == true)
            {
                var usernamecount = db.people.Where(dbl => dbl.email.Trim() == model.username.Trim()).Count<person>();
                if(usernamecount == 0)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "The username does not exist!!";
                    return View(model);
                }
                var username = model.username.Trim().ToLower(); var hash = Crypto.Hash(model.password);
                var credentials = db.people.Where(dbl => dbl.email.Trim().ToLower() == username && dbl.hash == hash).Count<person>();
                if(credentials == 1)
                {
                    Response.Cookies["EDMSuser"].Value = model.username.Trim();
                    Response.Cookies["EDMSuser"].Path = "~/Home/";
                    Response.Cookies["EDMSuser"].Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(Response.Cookies["EDMSuser"]);

                    var person = db.people.Where(dbl => dbl.email.Trim().ToLower() == username && dbl.hash == hash).FirstOrDefault<person>();

                    TempData["MessageType"] = "success";
                    TempData["Message"] = person.fullname + ", you are successfully logged in!!";
                    return RedirectToAction("Index");
                }
                if(usernamecount > 1)
                {
                    //Register into the error log!!
                }
            }
            TempData["MessageType"] = "danger";
            TempData["Message"] = "The username or password is invalid!!";
            return View(model);
        }
        public ActionResult Index()
        {
            var username = Request.Cookies["EDMSuser"].Value.Trim().ToLower();
            //var y = Response.Cookies["EDMSuser"].Value;
            //var z = 0;
            if(username != null)
            {
                var usernamecount = db.people.Where(dbl => dbl.email.Trim().ToLower() == username).Count<person>();
                var person = db.people.Where(dbl => dbl.email.Trim().ToLower() == username).FirstOrDefault<person>();
                if (usernamecount > 1 || usernamecount < 0)
                {
                    //Register into the error log!!
                }
                if (usernamecount == 0)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "This username does not exist, please input your correct username or register if you have not!!";
                    return RedirectToAction("login");
                }
                TempData["MessageType"] = "success";
                TempData["Message"] = person.fullname + ", you are successfully logged in!!";
                return View();
            }
            TempData["MessageType"] = "danger";
            TempData["Message"] = "Please input your login credentials!!";
            return RedirectToAction("login");
        }
        
        //[HttpGet]
        public ActionResult ViewAll()
        {
            //got to figure out how to make this page unbrowseable
            return View(GetAllFiles());
        }

        IEnumerable<file> GetAllFiles()
        {
            var user = Request.Cookies["EDMSuser"].Value.Trim().ToLower();
            var person = db.people.Where(dbl => dbl.email.Trim().ToLower() == user).FirstOrDefault<person>();
            return db.files.Where(dbl => dbl.person.Id == person.Id).ToList<file>();
        }
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            //got to figure out how to make this page unbrowseable
            var user = Request.Cookies["EDMSuser"].Value.Trim().ToLower();
            var person = db.people.Where(dbl => dbl.email.Trim().ToLower() == user).FirstOrDefault<person>();
            file emp = new file();
            if (id != 0)
            {
                emp = db.files.Where(dbl => dbl.person.Id == person.Id && dbl.Id == id).FirstOrDefault();
            }
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddOrEdit(file emp)
        {
            //got to figure out how to make this page unbrowseable
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.FileImage = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                if (emp.Id == 0)
                {
                    db.files.Add(emp);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(emp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                return Json(new
                {
                    success = true,
                    html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllFiles()),
                    message = "Submitted Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet
                );
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = Request.Cookies["EDMSuser"].Value.Trim().ToLower();
                var person = db.people.Where(dbl => dbl.email.Trim().ToLower() == user).FirstOrDefault<person>();
                file emp = db.files.Where(dbl => dbl.person.Id == person.Id && dbl.Id == id).FirstOrDefault();
                db.files.Remove(emp);
                db.SaveChanges();
                
                return Json(new
                {
                    success = true,
                    html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllFiles()),
                    message = "Deleted Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //////////////////////////////
        public ActionResult Userpage()
        {
            return View();
        }
        public ActionResult files()
        {
            var model = db.files;
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult homepage()
        {
            return View();
        }
        public ActionResult test()
        {
            return View();
        }
    }
}