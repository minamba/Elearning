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
    public class Reply_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();

        // GET: Reply_
        public ActionResult Index()
        {
            var reply_ = db.Reply_.Include(r => r.Comment_);
            return View(reply_.ToList());
        }

        // GET: Reply_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply_ reply_ = db.Reply_.Find(id);
            if (reply_ == null)
            {
                return HttpNotFound();
            }
            return View(reply_);
        }

        // GET: Reply_/Create
        public ActionResult Create()
        {
            ViewBag.comment_id = new SelectList(db.Comment_, "id", "message");
            return View();
        }

        // POST: Reply_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,comment_id,date_reply,message")] Reply_ reply_)
        {
            if (ModelState.IsValid)
            {
                db.Reply_.Add(reply_);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.comment_id = new SelectList(db.Comment_, "id", "message", reply_.comment_id);
            return View(reply_);
        }

        // GET: Reply_/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply_ reply_ = db.Reply_.Find(id);
            if (reply_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.comment_id = new SelectList(db.Comment_, "id", "message", reply_.comment_id);
            return View(reply_);
        }

        // POST: Reply_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,comment_id,date_reply,message")] Reply_ reply_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reply_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.comment_id = new SelectList(db.Comment_, "id", "message", reply_.comment_id);
            return View(reply_);
        }

        // GET: Reply_/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply_ reply_ = db.Reply_.Find(id);
            if (reply_ == null)
            {
                return HttpNotFound();
            }
            return View(reply_);
        }

        // POST: Reply_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reply_ reply_ = db.Reply_.Find(id);
            db.Reply_.Remove(reply_);
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
        public ActionResult SaveReply(string msgr, int uid, int cid, int subchapter_id)
        {
            Reply_ newReply = new Reply_();
            newReply.date_reply = DateTime.Now;
            newReply.comment_id = cid;
            newReply.user_id = uid;
            newReply.message = msgr;
            

            using (db)
            {
                db.Reply_.Add(newReply);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Subchapter_", new { sub_chapter_id = subchapter_id });
        }

        [HttpPost]
        public ActionResult SaveReply2(string msgr2, int uid, int cid, int subchapter_id)
        {
            Reply_ newReply = new Reply_();
            newReply.date_reply = DateTime.Now;
            newReply.comment_id = cid;
            newReply.user_id = uid;
            newReply.message = msgr2;


            using (db)
            {
                db.Reply_.Add(newReply);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Subchapter_", new { sub_chapter_id = subchapter_id });
        }
    }
}
