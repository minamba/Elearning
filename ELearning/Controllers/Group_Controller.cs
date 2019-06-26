using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ELearning.Models;
using Microsoft.AspNet.Identity;
using ELearning.Utilities;

namespace ELearning.Controllers
{
    public class Group_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();

        // GET: Group_
        public ActionResult Index()
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from uu in db.User_
                       where uu.user_asp_net_id == uid
                       select uu).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.gactivity = sa.Activesection(controllerName);

            /***************************************************************************************/
            return View(db.Group_.ToList());
        }

        // GET: Group_/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group_ group_ = db.Group_.Find(id);
            if (group_ == null)
            {
                return HttpNotFound();
            }
            return View(group_);
        }

        // GET: Group_/Create
        public ActionResult Create()
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from uu in db.User_
                       where uu.user_asp_net_id == uid
                       select uu).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.gactivity = sa.Activesection(controllerName);

            /***************************************************************************************/
            return View();
        }

        // POST: Group_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] Group_ group_)
        {
            if (ModelState.IsValid)
            {
                db.Group_.Add(group_);
                db.SaveChanges();
                return RedirectToAction("AdminGroup");
            }

            return View(group_);
        }

        // GET: Group_/Edit/5
        public ActionResult Edit(int? id)
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from uu in db.User_
                       where uu.user_asp_net_id == uid
                       select uu).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.gactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group_ group_ = db.Group_.Find(id);
            if (group_ == null)
            {
                return HttpNotFound();
            }
            return View(group_);
        }

        // POST: Group_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] Group_ group_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminGroup");
            }
            return View(group_);
        }

        // GET: Group_/Delete/5
        public ActionResult Delete(int? id)
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from uu in db.User_
                       where uu.user_asp_net_id == uid
                       select uu).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.gactivity = sa.Activesection(controllerName);

            /***************************************************************************************/
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group_ group_ = db.Group_.Find(id);
            if (group_ == null)
            {
                return HttpNotFound();
            }
            return View(group_);
        }

        // POST: Group_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group_ group_ = db.Group_.Find(id);
            db.Group_.Remove(group_);
            db.SaveChanges();
            return RedirectToAction("AdminGroup");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult AdminGroup()
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from uu in db.User_
                       where uu.user_asp_net_id == uid
                       select uu).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.gactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            var group = (from g in db.Group_
                          select g).ToList();

            return View(group);
        }

    }
}
