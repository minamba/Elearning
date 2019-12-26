using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ELearning.Models;

namespace ELearning.Controllers
{
    public class VideoTable_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();

        // GET: VideoTable_
        public ActionResult Index()
        {
            return View(db.Video_.ToList());
        }

        // GET: VideoTable_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video_ video_ = db.Video_.Find(id);
            if (video_ == null)
            {
                return HttpNotFound();
            }
            return View(video_);
        }

        // GET: VideoTable_/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VideoTable_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,url_video,time_start,user_id,chapter_id")] Video_ video_)
        {
            if (ModelState.IsValid)
            {
                db.Video_.Add(video_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(video_);
        }

        // GET: VideoTable_/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video_ video_ = db.Video_.Find(id);
            if (video_ == null)
            {
                return HttpNotFound();
            }
            return View(video_);
        }

        // POST: VideoTable_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,url_video,time_start,user_id,chapter_id")] Video_ video_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(video_);
        }

        // GET: VideoTable_/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video_ video_ = db.Video_.Find(id);
            if (video_ == null)
            {
                return HttpNotFound();
            }
            return View(video_);
        }

        // POST: VideoTable_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video_ video_ = db.Video_.Find(id);
            db.Video_.Remove(video_);
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
