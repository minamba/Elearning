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
    public class Comment_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();

        // GET: Comment_
        public ActionResult Index()
        {
            var comment_ = db.Comment_.Include(c => c.User_).Include(c => c.Subchapter_);
            return View(comment_.ToList());
        }

        // GET: Comment_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment_ comment_ = db.Comment_.Find(id);
            if (comment_ == null)
            {
                return HttpNotFound();
            }
            return View(comment_);
        }

        // GET: Comment_/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.User_, "id", "last_name");
            ViewBag.sub_chapter_id = new SelectList(db.Subchapter_, "id", "name");
            return View();
        }

        // POST: Comment_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,message,date_message,class_id,user_id,sub_chapter_id")] Comment_ comment_)
        {
            if (ModelState.IsValid)
            {
                db.Comment_.Add(comment_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.User_, "id", "last_name", comment_.user_id);
            ViewBag.sub_chapter_id = new SelectList(db.Subchapter_, "id", "name", comment_.sub_chapter_id);
            return View(comment_);
        }

        // GET: Comment_/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment_ comment_ = db.Comment_.Find(id);
            if (comment_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.User_, "id", "last_name", comment_.user_id);
            ViewBag.sub_chapter_id = new SelectList(db.Subchapter_, "id", "name", comment_.sub_chapter_id);
            return View(comment_);
        }

        // POST: Comment_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,message,date_message,class_id,user_id,sub_chapter_id")] Comment_ comment_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.User_, "id", "last_name", comment_.user_id);
            ViewBag.sub_chapter_id = new SelectList(db.Subchapter_, "id", "name", comment_.sub_chapter_id);
            return View(comment_);
        }

        // GET: Comment_/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment_ comment_ = db.Comment_.Find(id);
            if (comment_ == null)
            {
                return HttpNotFound();
            }
            return View(comment_);
        }

        // POST: Comment_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment_ comment_ = db.Comment_.Find(id);
            db.Comment_.Remove(comment_);
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

        [HttpPost]
        public ActionResult SaveComment(string msg, int uid,int subchapter_id)
        {
            Comment_ newComment = new Comment_();
            newComment.date_message = DateTime.Now;
            newComment.sub_chapter_id = subchapter_id;
            newComment.user_id= uid;
            newComment.message = msg;

            using (db)
            {
                db.Comment_.Add(newComment);
                db.SaveChanges();             
            }
            var result = "Merci, votre commentaire a bien été ajouté !";
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}
