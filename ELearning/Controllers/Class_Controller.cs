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
    public class Class_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();
        private static string ci;
        private static DateTime cdate;
        // GET: Class_
        [Authorize]
        public ActionResult Index(int theme_id)
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

            ViewBag.cactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            //si l'utilisateur est administrateur ou si c'est cheikh, pas de filtre
            if (user.type == 1 || user.type == 3)
            {
                var vm = new ClassViewModel();
                List<string> lfade = new List<string>();

                //lfade.Add("fade-up");
                lfade.Add("fade-right");
                lfade.Add("fade-left");
                //lfade.Add("flip-left");
                //lfade.Add("flip-right");
                //lfade.Add("fade-up");


                var class_ = (from c in db.Class_
                              where c.theme_id == theme_id
                              select c).ToList();


                var theme = (from t in db.Theme_
                             where t.id == theme_id
                             select t).First();


                /*****travail de chaine sur le nom du cour pour l'afficher dans le header ********/
                int nb = theme.name.Length;
                string firstLetterClassName_ = theme.name.Substring(0, 1);
                ViewBag.firstLetterThemeName = firstLetterClassName_;
                ViewBag.theme_name = theme.name.Substring(1, nb - 1);
                /*****fin travail de chaine sur le nom du cour pour l'afficher dans le header ********/


                vm.ListFade = lfade;
                vm.ListClass = class_;

                return View(vm);
            }
            //sinon je filtre l'affichage des themes
            else
            {


                var lusergroupe = (from g in db.Group_
                                   select g).ToList();

                var ugid = (from u in db.User_
                            where u.id == user.id
                            select u.group_id).First();


                var class_ = (from c in db.Class_
                              where c.theme_id == theme_id
                              && c.group_id == ugid
                              select c).ToList();

                var theme = (from t in db.Theme_
                             where t.id == theme_id
                             select t).First();


                /*****travail de chaine sur le nom du cour pour l'afficher dans le header ********/
                int nb = theme.name.Length;
                string firstLetterClassName_ = theme.name.Substring(0, 1);
                ViewBag.firstLetterThemeName = firstLetterClassName_;
                ViewBag.theme_name = theme.name.Substring(1, nb - 1);
                /*****fin travail de chaine sur le nom du cour pour l'afficher dans le header ********/


                var vm = new ClassViewModel();


                List<string> lfade = new List<string>();

                //lfade.Add("fade-up");
                lfade.Add("fade-right");
                lfade.Add("fade-left");
                //lfade.Add("flip-left");
                //lfade.Add("flip-right");
                //lfade.Add("fade-up");

                vm.ListFade = lfade;
                vm.ListClass = class_;

                return View(vm);
            }
        }

        // GET: Class_/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ class_ = db.Class_.Find(id);
            if (class_ == null)
            {
                return HttpNotFound();
            }
            return View(class_);
        }

        // GET: Class_/Create
        [Authorize]
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

            ViewBag.cactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            ViewBag.group_id = new SelectList(db.Group_, "id", "name");
            ViewBag.theme_id = new SelectList(db.Theme_, "id", "name");
            return View();
        }

        // POST: Class_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,theme_id,group_id")] Class_ class_, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if (ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    class_.url_img = fileName;
                    //ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Img/"), fileName));
                    //----------------------------FTP VPS UPLOAD-----------------------------------------//
                    var uploadurl = @"ftp://vps64363.lws-hosting.com//web//Img/";
                    var uploadfilename = fileName;
                    var username = "defaultminamba";
                    var password = "elearning2019@";
                    Stream streamObj = ImageUpload.InputStream;
                    byte[] buffer = new byte[ImageUpload.ContentLength];
                    streamObj.Read(buffer, 0, buffer.Length);
                    streamObj.Close();
                    streamObj = null;
                    string ftpurl = String.Format("{0}/{1}", uploadurl, uploadfilename);
                    var requestObj = FtpWebRequest.Create(ftpurl) as FtpWebRequest;
                    requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                    requestObj.Credentials = new NetworkCredential(username, password);
                    Stream requestStream = requestObj.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    requestObj = null;
                    //------------------------------ftp VPS upload----------------------------------------------//

                }
                else
                {
                    class_.url_img = "card1.jpg";
                }

                class_.create_date = DateTime.Now;
                db.Class_.Add(class_);
                db.SaveChanges();
                return RedirectToAction("AdminClass");
            }

            ViewBag.group_id = new SelectList(db.Group_, "id", "name", class_.group_id);
            ViewBag.theme_id = new SelectList(db.Theme_, "id", "name", class_.theme_id);
            return View(class_);
        }

        // GET: Class_/Edit/5
        [Authorize]
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

            ViewBag.cactivity = sa.Activesection(controllerName);

            /***************************************************************************************/


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ class_ = db.Class_.Find(id);
            if (class_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.group_id = new SelectList(db.Group_, "id", "name", class_.group_id);
            ViewBag.theme_id = new SelectList(db.Theme_, "id", "name", class_.theme_id);
            ci = class_.url_img;
            cdate = (DateTime)class_.create_date;
            return View(class_);
        }

        // POST: Class_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,theme_id,group_id")] Class_ class_, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if (ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    class_.url_img = fileName;
                    //ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Img/"), fileName));

                    //----------------------------FTP UPLOAD-----------------------------------------//
                    var uploadurl = @"ftp://vps64363.lws-hosting.com//web//Img/";
                    var uploadfilename = fileName;
                    var username = "defaultminamba";
                    var password = "elearning2019@";
                    Stream streamObj = ImageUpload.InputStream;
                    byte[] buffer = new byte[ImageUpload.ContentLength];
                    streamObj.Read(buffer, 0, buffer.Length);
                    streamObj.Close();
                    streamObj = null;
                    string ftpurl = String.Format("{0}/{1}", uploadurl, uploadfilename);
                    var requestObj = FtpWebRequest.Create(ftpurl) as FtpWebRequest;
                    requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                    requestObj.Credentials = new NetworkCredential(username, password);
                    Stream requestStream = requestObj.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    requestObj = null;
                    //------------------------------ftp upload----------------------------------------------//

                    class_.create_date = DateTime.Now;
                }
                else
                {
                    class_.url_img = ci;
                    class_.create_date = cdate;
                }

                db.Entry(class_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminClass");
            }
            ViewBag.group_id = new SelectList(db.Group_, "id", "name", class_.group_id);
            ViewBag.theme_id = new SelectList(db.Theme_, "id", "name", class_.theme_id);
            return View(class_);
        }

        // GET: Class_/Delete/5
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

            ViewBag.cactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class_ class_ = db.Class_.Find(id);
            if (class_ == null)
            {
                return HttpNotFound();
            }
            return View(class_);
        }

        // POST: Class_/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class_ class_ = db.Class_.Find(id);
            db.Class_.Remove(class_);
            db.SaveChanges();


            return RedirectToAction("AdminClass");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        // GET: Class_
        public ActionResult AdminClass()
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

            ViewBag.cactivity = sa.Activesection(controllerName);

            /***************************************************************************************/


            var class_ = (from c in db.Class_
                          select c).ToList();
            return View(class_);
        }
    }
}
