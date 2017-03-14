using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gymbokning.Models;
using System.Data.Entity.Core.Objects;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GymClasses
        public ActionResult Index(string showHistory = "off")
        {
            if (showHistory == "checked") showHistory = "on";

            var model = new IndexGymClassesViewModel();

            model.Checked = (showHistory == "on") ? "checked" : "";
            model.GymClasses = (showHistory == "on") ?
                db.GymClasses
                .Select(x => new ClassAttendedViewModel
                {
                    Attended = x.AttendingMembers.Any(y => y.Email == User.Identity.Name),
                    GymClass = x,
                    IsOldClass = x.StartTime < DateTime.Now
                }).ToList()
                :
                db.GymClasses
                .Where(c => c.StartTime >= DateTime.Now)
                .Select(x => new ClassAttendedViewModel
                {
                    Attended = x.AttendingMembers.Any(y => y.Email == User.Identity.Name),
                    GymClass = x,
                    IsOldClass = x.StartTime < DateTime.Now
                }).ToList();

            return View(model);
        }

        [Authorize]
        public ActionResult BookedClasses()
        {
            var user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null) return View(user.AttendedClasses.Where(x => x.StartTime >= DateTime.Now).ToList());

            return View();
        }

        [Authorize]
        public ActionResult ClassHistory()
        {
            var user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null) return View(user.AttendedClasses.Where(x => x.StartTime < DateTime.Now).ToList());

            return View();
        }

        // GET: GymClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        public ActionResult BookingToggle(int id, string showHistory = "")
        {
            GymClass CurrentClass = db.GymClasses.Where(g => g.Id == id).FirstOrDefault();
            ApplicationUser CurrentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (CurrentClass.AttendingMembers.Contains(CurrentUser))
            {
                CurrentClass.AttendingMembers.Remove(CurrentUser);
                db.SaveChanges();
            }
            else
            {
                CurrentClass.AttendingMembers.Add(CurrentUser);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { showHistory = showHistory });
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.GymClasses.Add(gymClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gymClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GymClass gymClass = db.GymClasses.Find(id);
            db.GymClasses.Remove(gymClass);
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
