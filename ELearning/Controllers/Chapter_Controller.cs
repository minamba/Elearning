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
    public class Chapter_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();

        // GET: Chapter_
        [Authorize]
        public ActionResult Index(int class_id,string class_description)
        {
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

            var vm = new ClassViewModelsWithChapter();

            var listSeen = (from s in db.Seen_
                            where s.user_asp_net_id == uid
                            select s).ToList();

            var class_name = (from cn in db.Class_
                              where cn.id == class_id
                              select cn.name).First();

            /*****travail de chaine sur le nom du cour pour l'afficher dans le header ********/
            int nb = class_name.Length;
            string firstLetterClassName_ = class_name.Substring(0,1);
            ViewBag.firstLetterClassName = firstLetterClassName_;
            ViewBag.class_name = class_name.Substring(1, nb-1);
            /*****fin travail de chaine sur le nom du cour pour l'afficher dans le header ********/


            var lstchapter_ = (from ch in db.Chapter_
                            where ch.class_id == class_id
                            select ch).ToList();

            var lstsubchapter = (from ch in db.Subchapter_
                               select ch).ToList();
            vm.ClassDescription = class_description;
            vm.ListChapter_ = lstchapter_;
            vm.ListSubChapter_ = lstsubchapter;
            vm.ListSeen = listSeen;

            //je crée une list de string qui va me permettre d'avoir des id unique pour le spoiler
            var l = new List<string>();
            for(int i=0; i< lstsubchapter.Count; i++)
            {
                l.Add("tag" + lstsubchapter[i].id);
            }
            vm.tag = l;


            for(int i=0; i<lstchapter_.Count; i++)
            {
                for(int j=0; j<lstsubchapter.Count; j++)
                {
                    if(lstsubchapter[j].chapter_id == lstchapter_[i].id)
                    {
                        CreateLigneSeen(lstchapter_[i].id, (int)lstsubchapter[j].id);
                    }
                }
            }


            return View(vm);
        }

        // GET: Chapter_/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter_ chapter_ = db.Chapter_.Find(id);
            if (chapter_ == null)
            {
                return HttpNotFound();
            }
            return View(chapter_);
        }

        // GET: Chapter_/Create
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

            ViewBag.chactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            ViewBag.class_id = new SelectList(db.Class_, "id", "name");
            return View();
        }

        // POST: Chapter_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,class_id")] Chapter_ chapter_)
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

            ViewBag.chactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (ModelState.IsValid)
            {
                db.Chapter_.Add(chapter_);
                db.SaveChanges();
                return RedirectToAction("AdminChapter");
            }

            ViewBag.class_id = new SelectList(db.Class_, "id", "name", chapter_.class_id);
            return View(chapter_);
        }

        // GET: Chapter_/Edit/5
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

            ViewBag.chactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter_ chapter_ = db.Chapter_.Find(id);
            if (chapter_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.class_id = new SelectList(db.Class_, "id", "name", chapter_.class_id);
            return View(chapter_);
        }

        // POST: Chapter_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,class_id")] Chapter_ chapter_)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter_).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminChapter");
            }
            ViewBag.class_id = new SelectList(db.Class_, "id", "name", chapter_.class_id);
            return View(chapter_);
        }

        // GET: Chapter_/Delete/5
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

            ViewBag.chactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter_ chapter_ = db.Chapter_.Find(id);
            if (chapter_ == null)
            {
                return HttpNotFound();
            }
            return View(chapter_);
        }

        // POST: Chapter_/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chapter_ chapter_ = db.Chapter_.Find(id);
            db.Chapter_.Remove(chapter_);
            db.SaveChanges();
            return RedirectToAction("AdminChapter");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        // GET: Chapter_
        public ActionResult Adminchapter()
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

            ViewBag.chactivity = sa.Activesection(controllerName);

            /***************************************************************************************/


            var lstchapter_ = (from ch in db.Chapter_
                               select ch).ToList();

            return View(lstchapter_);
        }


        public void CreateLigneSeen(int chapterid, int subchapterID)
        {

                var uid = User.Identity.GetUserId();
                try
                {
                    var seen = (from s in db.Seen_
                                where s.user_asp_net_id == uid
                                && s.subchapter_id == subchapterID
                                && s.chapter_id == chapterid
                                select s).First();
                return;
                }
                catch
                {
                    Seen_ newSeen = new Seen_();
                    newSeen.user_asp_net_id = uid;
                    newSeen.subchapter_id = subchapterID;
                    newSeen.chapter_id = chapterid;
                    newSeen.seen = false;
                    db.Seen_.Add(newSeen);
                    db.SaveChanges();
                }

            

        }

        public ActionResult VerifySeen(int subchapterID, int chapterID)
        {
            var uid = User.Identity.GetUserId();

            var class_ = (from cl in db.Chapter_
                          where cl.id == chapterID
                          select cl).First();

            ViewBag.class_id = class_.id;

            var description = (from d in db.Class_
                               where d.id == class_.class_id
                               select d.description).First();

            ViewBag.description = description;
            try
            {
                var seen = (from s in db.Seen_
                            where s.user_asp_net_id == uid
                            && s.chapter_id == chapterID
                            select s).ToList();

                for(int i=0;i< seen.Count; i++)
                {
                    if(seen[i].subchapter_id == subchapterID)
                    {
                        try
                        {
                            var user_seen = seen[i - 1].seen;

                            if(user_seen != null)
                            {
                                if(user_seen == false)
                                {
                                    var result = "Vous devez regarder la vidéo précedente avant de pouvoir regarder celle - ci";
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                    //ViewBag.Message = "Vous devez regarder la vidéo précedente avant de pouvoir regarder celle - ci";
                                    //return RedirectToAction("Index", "Chapter_", new { class_id = class_.id, class_description = description });
                                }
                                else
                                {
                                    seen[i].seen = true;
                                    db.SaveChanges();
                                    return RedirectToAction("Index", "Subchapter_", new { sub_chapter_id = (int)seen[i].subchapter_id });
            
                                }
                            }
                        }
                        catch
                        {
                            seen[i].seen = true;
                            db.SaveChanges();

                            return RedirectToAction("Index", "Subchapter_", new { sub_chapter_id = (int)seen[i].subchapter_id });
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

    }
}
