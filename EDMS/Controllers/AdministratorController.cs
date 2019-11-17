using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDMS.Models;
using System.Net;
using System.Data.Entity;

namespace EDMS.Controllers
{
    public class AdministratorController : Controller
    {
        private EDMSEntities db = new EDMSEntities();
        [HttpGet]
        public ActionResult login()
        {
            var model = new login(); 
            return View(model);
        }
        [HttpPost]
        public ActionResult login(login model)
        {
            if(ModelState.IsValid == true)
            {
                //Code goes here
            }
            return View(model);
        }
        public ActionResult password()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        //////////////////////////////////// Start Dashboard functionality
        // GET: Administrator/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Administrator/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Administrator/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Administrator/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Administrator/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Administrator/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //////////////////////////////////// End Dashboard functionality
        //////////////////////////////////// Start department functionality
        // GET: departments
        public ActionResult departmentList()//Index
        {
            return View(db.departments.ToList());
        }

        // GET: departments/Details
        public ActionResult departmentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            department department = db.departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: departments/Create
        public ActionResult departmentCreate()
        {
            return View();
        }

        // POST: departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult departmentCreate([Bind(Include = "Id,deptName")] department department)
        {
            if (ModelState.IsValid)
            {
                db.departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("departmentList");
            }
            return View(department);
        }

        // GET: departments/Edit
        public ActionResult departmentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            department department = db.departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: departments/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult departmentEdit([Bind(Include = "Id,deptName")] department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("departmentList");
            }
            return View(department);
        }

        // GET: departments/Delete
        public ActionResult departmentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            department department = db.departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: departments/Delete
        [HttpPost, ActionName("departmentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult departmentDeleteConfirmed(int id)
        {
            department department = db.departments.Find(id);
            db.departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("departmentList");
        }
        //////////////////////////////////// End department functionality
        //////////////////////////////////// Start people functionality
        // GET: people
        public ActionResult peopleList()//index
        {
            var people = db.people.Include(p => p.department).Include(p => p.role);
            return View(people.ToList());
        }

        // GET: people/Details
        public ActionResult peopleDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            person person = db.people.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: people/Create
        public ActionResult peopleCreate()
        {
            ViewBag.deptId = new SelectList(db.departments, "Id", "deptName");
            ViewBag.roleId = new SelectList(db.roles, "Id", "name");
            return View();
        }

        // POST: people/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult peopleCreate([Bind(Include = "fullname,email,roleId,deptId")] person person)
        {//Id,fullname,email,roleId,deptId,password,hash,identifier
            #region checks
            var emailcount = db.people.Where(dbl => dbl.email == person.email).Count();
            if(emailcount != 0)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "The email already exist!!";
                return View(person);
            }
            #endregion
            if (ModelState.IsValid)
            {
                var password = Guid.NewGuid().ToString().Substring(0,8);
                Guid identifier = Guid.NewGuid();
                person p = new person
                {
                    fullname = person.fullname.Trim(),
                    email = person.email.Trim(),
                    roleId = person.roleId,
                    deptId = person.deptId,
                    password = password,
                    hash = Crypto.Hash(password),
                    identifier = identifier,
                };
                db.people.Add(p);
                db.SaveChanges();
                return RedirectToAction("peopleList");
            }

            ViewBag.deptId = new SelectList(db.departments, "Id", "deptName", person.deptId);
            ViewBag.roleId = new SelectList(db.roles, "Id", "name", person.roleId);
            return View(person);
        }

        // GET: people/Edit
        public ActionResult peopleEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            person person = db.people.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.deptId = new SelectList(db.departments, "Id", "deptName", person.deptId);
            ViewBag.roleId = new SelectList(db.roles, "Id", "name", person.roleId);
            return View(person);
        }

        // POST: people/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult peopleEdit([Bind(Include = "fullname,email,roleId,deptId")] person person)
        {//Id,fullname,email,roleId,deptId,password,hash,identifier
            #region checks
            var emailcount = db.people.Where(dbl => dbl.email == person.email).Count();
            if (emailcount != 0)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "The email already exist!!";
                return View(person);
            }
            #endregion
            if (ModelState.IsValid)
            {
                var password = Guid.NewGuid().ToString().Substring(0, 8);
                Guid identifier = Guid.NewGuid();
                person p = new person
                {
                    fullname = person.fullname,
                    email = person.email,
                    roleId = person.roleId,
                    deptId = person.deptId,
                    //password = db.people.Find(person.Id).password,
                    //hash = db.people.Find(person.Id).hash,
                    //identifier = db.people.Find(person.Id).identifier,
                };
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("peopleList");
            }
            ViewBag.deptId = new SelectList(db.departments, "Id", "deptName", person.deptId);
            ViewBag.roleId = new SelectList(db.roles, "Id", "name", person.roleId);
            return View(person);
        }

        // GET: people/Delete
        public ActionResult peopleDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            person person = db.people.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: people/Delete
        [HttpPost, ActionName("peopleDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult peopleDeleteConfirmed(int id)
        {
            person person = db.people.Find(id);
            db.people.Remove(person);
            db.SaveChanges();
            return RedirectToAction("people");
        }
        //////////////////////////////////// End people functionality
        //////////////////////////////////// Start role functionality
        // GET: roles
        public ActionResult roleList()
        {
            return View(db.roles.ToList());
        }

        // GET: roles/Details/5
        public ActionResult roleDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role role = db.roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // GET: roles/Create
        public ActionResult roleCreate()
        {
            return View();
        }

        // POST: roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult roleCreate([Bind(Include = "Id,name")] role role)
        {
            if (ModelState.IsValid)
            {
                db.roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("roleList");
            }

            return View(role);
        }

        // GET: roles/Edit/5
        public ActionResult roleEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role role = db.roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult roleEdit([Bind(Include = "Id,name")] role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("roleList");
            }
            return View(role);
        }

        // GET: roles/Delete/5
        public ActionResult roleDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role role = db.roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: roles/Delete/5
        [HttpPost, ActionName("roleDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult roleDeleteConfirmed(int id)
        {
            role role = db.roles.Find(id);
            db.roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("roleList");
        }
        //////////////////////////////////// End role functionality
        //////////////////////////////////// Start activities functionality
        // GET: activities
        public ActionResult activitiesList()
        {
            return View(db.activities.ToList());
        }

        // GET: activities/Details
        public ActionResult activitiesDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: activities/Create
        public ActionResult activitiesCreate()
        {
            return View();
        }

        // POST: activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult activitiesCreate([Bind(Include = "Id,name")] activity activity)
        {
            if (ModelState.IsValid)
            {
                db.activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("activitiesList");
            }

            return View(activity);
        }

        // GET: activities/Edit/5
        public ActionResult activitiesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult activitiesEdit([Bind(Include = "Id,name")] activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("activitiesList");
            }
            return View(activity);
        }

        // GET: activities/Delete/5
        public ActionResult activitiesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: activities/Delete/5
        [HttpPost, ActionName("activitiesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult activitiesDeleteConfirmed(int id)
        {
            activity activity = db.activities.Find(id);
            db.activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("activitiesList");
        }
        //////////////////////////////////// End activities functionality
        //////////////////////////////////// Start admins functionality
        // GET: admins
        public ActionResult adminsList()
        {
            return View(db.admins.ToList());
        }

        // GET: admins/Details/5
        public ActionResult adminsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            admin admin = db.admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: admins/Create
        public ActionResult adminsCreate()
        {
            return View();
        }

        // POST: admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult adminsCreate([Bind(Include = "Id,username,password,hash,personId")] admin admin)
        {
            if (ModelState.IsValid)
            {
                db.admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("adminsList");
            }

            return View(admin);
        }

        // GET: admins/Edit/5
        public ActionResult adminsEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            admin admin = db.admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult adminsEdit([Bind(Include = "Id,username,password,hash,personId")] admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("adminsList");
            }
            return View(admin);
        }

        // GET: admins/Delete/5
        public ActionResult adminsDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            admin admin = db.admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: admins/Delete/5
        [HttpPost, ActionName("adminsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult adminsDeleteConfirmed(int id)
        {
            admin admin = db.admins.Find(id);
            db.admins.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("adminsList");
        }
        //////////////////////////////////// End admins functionality
        //////////////////////////////////// Start filetype functionality
        // GET: filetypes
        public ActionResult filetypeList()
        {
            return View(db.filetypes.ToList());
        }

        // GET: filetypes/Details/5
        public ActionResult filetypeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            filetype filetype = db.filetypes.Find(id);
            if (filetype == null)
            {
                return HttpNotFound();
            }
            return View(filetype);
        }

        // GET: filetypes/Create
        public ActionResult filetypeCreate()
        {
            return View();
        }

        // POST: filetypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult filetypeCreate([Bind(Include = "Id,name")] filetype filetype)
        {
            if (ModelState.IsValid)
            {
                db.filetypes.Add(filetype);
                db.SaveChanges();
                return RedirectToAction("filetypeList");
            }

            return View(filetype);
        }

        // GET: filetypes/Edit/5
        public ActionResult filetypeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            filetype filetype = db.filetypes.Find(id);
            if (filetype == null)
            {
                return HttpNotFound();
            }
            return View(filetype);
        }

        // POST: filetypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult filetypeEdit([Bind(Include = "Id,name")] filetype filetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("filetypeList");
            }
            return View(filetype);
        }

        // GET: filetypes/Delete/5
        public ActionResult filetypeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            filetype filetype = db.filetypes.Find(id);
            if (filetype == null)
            {
                return HttpNotFound();
            }
            return View(filetype);
        }

        // POST: filetypes/Delete/5
        [HttpPost, ActionName("filetypeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult filetypeDeleteConfirmed(int id)
        {
            filetype filetype = db.filetypes.Find(id);
            db.filetypes.Remove(filetype);
            db.SaveChanges();
            return RedirectToAction("filetypeList");
        }
        //////////////////////////////////// End filetype functionality
        //////////////////////////////////// Start file functionality
        // GET: files
        public ActionResult fileList()
        {
            var files = db.files.Include(f => f.filetype).Include(f => f.person);
            return View(files.ToList());
        }

        // GET: files/Details/5
        public ActionResult fileDetails(int? id)
        {
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

        // GET: files/Create
        public ActionResult fileCreate()
        {
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name");
            ViewBag.personId = new SelectList(db.people, "Id", "fullname");
            return View();
        }

        // POST: files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult fileCreate([Bind(Include = "Id,name,filepath,filetypeId,personId")] file file)
        {
            if (ModelState.IsValid)
            {
                db.files.Add(file);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name", file.filetypeId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", file.personId);
            return View(file);
        }

        // GET: files/Edit/5
        public ActionResult fileEdit(int? id)
        {
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

        // POST: files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult fileEdit([Bind(Include = "Id,name,filepath,filetypeId,personId")] file file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("fileList");
            }
            ViewBag.filetypeId = new SelectList(db.filetypes, "Id", "name", file.filetypeId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", file.personId);
            return View(file);
        }

        // GET: files/Delete/5
        public ActionResult fileDelete(int? id)
        {
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

        // POST: files/Delete/5
        [HttpPost, ActionName("fileDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult fileDeleteConfirmed(int id)
        {
            file file = db.files.Find(id);
            db.files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("fileList");
        }
        //////////////////////////////////// End file functionality
        //////////////////////////////////// Start audit functionality
        // GET: audits
        public ActionResult auditList()
        {
            var audits = db.audits.Include(a => a.activity).Include(a => a.person);
            return View(audits.ToList());
        }

        // GET: audits/Details/5
        public ActionResult auditDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            audit audit = db.audits.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        // GET: audits/Create
        public ActionResult auditCreate()
        {
            ViewBag.activityId = new SelectList(db.activities, "Id", "name");
            ViewBag.personId = new SelectList(db.people, "Id", "fullname");
            return View();
        }

        // POST: audits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult auditCreate([Bind(Include = "Id,personId,datetime,activityId,auditObject")] audit audit)
        {
            if (ModelState.IsValid)
            {
                db.audits.Add(audit);
                db.SaveChanges();
                return RedirectToAction("auditList");
            }

            ViewBag.activityId = new SelectList(db.activities, "Id", "name", audit.activityId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", audit.personId);
            return View(audit);
        }

        // GET: audits/Edit/5
        public ActionResult auditEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            audit audit = db.audits.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            ViewBag.activityId = new SelectList(db.activities, "Id", "name", audit.activityId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", audit.personId);
            return View(audit);
        }

        // POST: audits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult auditEdit([Bind(Include = "Id,personId,datetime,activityId,auditObject")] audit audit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(audit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("auditList");
            }
            ViewBag.activityId = new SelectList(db.activities, "Id", "name", audit.activityId);
            ViewBag.personId = new SelectList(db.people, "Id", "fullname", audit.personId);
            return View(audit);
        }

        // GET: audits/Delete/5
        public ActionResult auditDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            audit audit = db.audits.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        // POST: audits/Delete/5
        [HttpPost, ActionName("auditDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult auditDeleteConfirmed(int id)
        {
            audit audit = db.audits.Find(id);
            db.audits.Remove(audit);
            db.SaveChanges();
            return RedirectToAction("auditList");
        }
        //////////////////////////////////// End audit functionality

    }
}