using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Student
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "Date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = (IQueryable<Student>)db.Students;
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.ToLower().Contains(searchString) ||
                                               s.FirstMidName.ToLower().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Name_desc":
                    students = students.OrderByDescending(s => s.LastName).ThenBy(s => s.FirstMidName).ThenBy(s => s.EnrollmentDate);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate).ThenBy(s => s.LastName).ThenBy(s => s.FirstMidName);
                    break;
                case "Date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate).ThenBy(s => s.LastName).ThenBy(s => s.FirstMidName);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName).ThenBy(s => s.FirstMidName).ThenBy(s => s.EnrollmentDate);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                //log the error here
                ModelState.AddModelError("", "Unable to save changes. Please try again, and if the problem persists, see your system administrator.");
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var student = db.Students.Find(ID);
            if (TryUpdateModel(student, "", new string[] { "LastName", "FirstName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    //log the error
                    ModelState.AddModelError("", "Unable to save changes. Please try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(student);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again, and if the problem persists, see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = new Student() { ID = id };
                db.Entry(student).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {
                //log the error
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
