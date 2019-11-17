using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDMS;
using System.IO;

namespace EDMS.Controllers
{
    public class UserController : Controller
    {
        private EDMSEntities db = new EDMSEntities();

        // GET: User
        public ActionResult Index()
        {
            Session["usr"] = "damee1993@gmail.com";
            var usr = Session["usr"].ToString();
            var files = db.files.Include(f => f.filetype).Include(f => f.person);
            return View(files.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            var usr = Session["usr"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            Session["usr"] = "damee1993@gmail.com";
            var usr = Session["usr"].ToString();
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name");
            ViewBag.personId = new SelectList(db.people, "Id", "fullname");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(file file)
        {//[Bind(Include = "Id,name,filepath,filetypeId,personId")]
            Session["usr"] = "damee1993@gmail.com";
            var usr = Session["usr"].ToString();
            person p = db.people.Where(dbl => dbl.email.ToLower().Trim() == usr.ToLower().Trim()).FirstOrDefault<person>();
            if(file.ImageUpload.ContentLength != 0 || file.ImageUpload != null)
            {
                var doc = file.ImageUpload;
                string _FileName = Path.GetFileName(file.ImageUpload.FileName);
                string _path = Path.Combine(Server.MapPath("~/Files"), _FileName);
                file.ImageUpload.SaveAs(_path);
                file f = new file
                {
                    name = file.ImageUpload.FileName,
                    filepath = _path,
                    filetypeId = 10,
                    personId = p.Id,
                    contentType = file.ImageUpload.ContentType,
                    size = file.ImageUpload.ContentLength
                };
                db.files.Add(f);
                db.SaveChanges();
                TempData["MaessageTYpe"] = "success";
                TempData["Message"] = "successfully added"; 
                return RedirectToAction("Index");
            }
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name", file.filetypeId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", file.personId);
            return View(file);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            var usr = Session["usr"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name", file.filetypeId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", file.personId);
            return View(file);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,filepath,filetypeId,personId")] file file)
        {
            var usr = Session["usr"].ToString();
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name", file.filetypeId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", file.personId);
            return View(file);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            var usr = Session["usr"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var usr = Session["usr"].ToString();
            file file = db.files.Find(id);
            db.files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
