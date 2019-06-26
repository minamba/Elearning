using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ELearning.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using ELearning.Utilities;

namespace ELearning.Controllers
{
    public class Theme_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();
        private static string ti;
        // GET: Theme_
        [Authorize]
        public ActionResult Index()
        {
                           /**************************************GET CURRENT PAGE********************************/
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.currentPage = controllerName;
                /**************************************GET CURRENT PAGE********************************/
                /**************************************GET CURRENT PAGE********************************/
                ViewBag.currentPage = null;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var user = (from u in db.User_
                        where u.user_asp_net_id == uid
                        select u).First();

                ViewBag.Role = user.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.tactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            var vm = new ThemeViewModel();

            var ltheme = (from t in db.Theme_
                          select t).ToList();


            List<string> lfade = new List<string>();

            //lfade.Add("fade-up");
            lfade.Add("fade-right");
            lfade.Add("fade-left");         
            //lfade.Add("flip-left");
            //lfade.Add("flip-right");
            //lfade.Add("fade-up");


            vm.ListFade = lfade;
            vm.ListTheme = ltheme;
            return View(vm);
        }

        // GET: Theme_/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme_ theme_ = db.Theme_.Find(id);
            if (theme_ == null)
            {
                return HttpNotFound();
            }
            return View(theme_);
        }

        // GET: Theme_/Create
        [Authorize]
        public ActionResult Create()
        {               /**************************************GET CURRENT PAGE********************************/
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

            ViewBag.tactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            return View();
        }

        // POST: Theme_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] Theme_ theme_, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if (ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    theme_.url_img = fileName;
                    ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Img/"), fileName)); 
                }
                else
                {
                    theme_.url_img = "card1.jpg";
                }
                db.Theme_.Add(theme_);
                db.SaveChanges();

                return RedirectToAction("AdminTheme");
            }

            return View(theme_);
        }
        [Authorize]
        // GET: Theme_/Edit/5
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

            ViewBag.tactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme_ theme_ = db.Theme_.Find(id);
            if (theme_ == null)
            {
                return HttpNotFound();
            }
            ti = theme_.url_img;
            return View(theme_);
        }

        // POST: Theme_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] Theme_ theme_, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if(ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    theme_.url_img = fileName;
                    ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Img/"), fileName));
                }
                else
                {
                    theme_.url_img = ti;
                }

                db.Entry(theme_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminTheme");
            }
            return View(theme_);
        }

        // GET: Theme_/Delete/5
        [Authorize]
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

            ViewBag.tactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Theme_ theme_ = db.Theme_.Find(id);
            if (theme_ == null)
            {
                return HttpNotFound();
            }
            return View(theme_);
        }

        // POST: Theme_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Theme_ theme_ = db.Theme_.Find(id);
            db.Theme_.Remove(theme_);
            db.SaveChanges();
            return RedirectToAction("AdminTheme");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult AdminTheme()
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

            ViewBag.tactivity = sa.Activesection(controllerName);

            /***************************************************************************************/


            var theme_ = (from t in db.Theme_
                         select t).ToList();

            var vm = new ThemeViewModel();
            vm.ListTheme = theme_.ToList();

            return View(vm);
        }


    }
}
