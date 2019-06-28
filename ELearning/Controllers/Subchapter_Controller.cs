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
using System.IO;
using ELearning.Utilities;

namespace ELearning.Controllers
{
    public class Subchapter_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();
        private static string uv;
        private static string uf;
        private static DateTime datesub;

        // GET: Subchapter_
        [Authorize]
        public ActionResult Index(int sub_chapter_id)
        {
            /**************************************GET CURRENT PAGE********************************/
            ViewBag.currentPage = null;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var  usr = (from u in db.User_
                        where u.user_asp_net_id == uid
                        select u).First();

            ViewBag.Role = usr.type;
            /****************************************************************/


            var vm = new SubchapterWithCommentsAndReply();

            var subchapter_ = (from sch in db.Subchapter_
                               where sch.id == sub_chapter_id
                               select sch).First();


            /*****travail de chaine sur le nom du cour pour l'afficher dans le header ********/
            int nb = subchapter_.name.Length;
            string firstLetterSubChapName_ = subchapter_.name.Substring(0, 1);
            ViewBag.firstLetterSubChapName = firstLetterSubChapName_;
            ViewBag.theme_name = subchapter_.name.Substring(1, nb - 1);
            /*****fin travail de chaine sur le nom du cour pour l'afficher dans le header ********/


            var userIdIdentity = User.Identity.GetUserId();

           var user = (from u in db.User_
                          where u.user_asp_net_id == userIdIdentity
                        select u).First();

            var cheikh = (from u in db.User_
                          where u.type == 3
                          select u).First();

            ViewBag.cheikh = cheikh.id;

            ViewBag.compteur = 0;
            ViewBag.userID = user.id; /*user.GetUserName(subchapter_.) */
            ViewBag.subchapterID = subchapter_.id;

            var subchapterID2 = subchapter_.id;
            List<Comment_> comment = (from c in db.Comment_
                           where c.sub_chapter_id == subchapterID2
                           select c).ToList();

            var reply = (from r in db.Reply_
                         select r).ToList();


            var alluser = (from u in db.User_
                        select u).ToList();

            //je crée une liste pour récuperer toutes les dates time et extraire les informations qui m'interesse
            List<string> d = new List<string>();
            
            foreach(var date in comment)
            {
                DateTime dateValue = (DateTime)date.date_message;
                var day = dateValue.Day;
                var month = dateValue.Month;
                var year = dateValue.Year;
                var hour = dateValue.Hour;
                var minute = dateValue.Minute;


                if (day < 10)
                {
                    day = 0+day;
                }

                var concatdate = day + "/" + month + "/" + year + " à " +hour + ":" + minute;
                d.Add(concatdate);
            }
            vm.ListDate = d;

            //je crée une list de string qui va me permettre d'avoir des id unique pour le spoiler
            var l = new List<string>();
            var l_ = new List<string>();
            foreach (var lst in comment)
            {
                l.Add("tag" + lst.id);
                l_.Add("tag2" + lst.id);
            }
            vm.TagComment = l;
            vm.TagComment2 = l_;

            var l2 = new List<string>();
            foreach (var lst in reply)
            {
                l2.Add("tagreply" + lst.id);
            }
            vm.TagReply = l2;

            var l3 = new List<string>();
            foreach (var lst in reply)
            {
                l3.Add("tagreplyagain" + lst.id);
            }
            vm.TagReply2 = l3;

            ViewBag.subchapterTitle = "";

            vm.CurrentSubchapter = subchapter_;
            vm.ListComment = comment;
            vm.ListReply = reply;
            vm.CurrrentUsr = user;
            vm.ListUser = alluser;
            return View(vm);
        }


        // GET: Subchapter_/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subchapter_ subchapter_ = db.Subchapter_.Find(id);
            if (subchapter_ == null)
            {
                return HttpNotFound();
            }
            return View(subchapter_);
        }

        // GET: Subchapter_/Create
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

            ViewBag.sactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            var class_ = (from c in db.Class_
                          select c).ToList();

            List<SelectListItem> lclass = new List<SelectListItem>();

            foreach(var c in class_)
            {
                lclass.Add(new SelectListItem() { Text = c.name, Value = c.name });
            }

            var vm = new Subchapter_();
            vm.ListClass = GetAllClass();
            vm.ListChapter = GetAllChapter();


            ViewBag.chapter_id = new SelectList(db.Chapter_, "id", "name");
            return View(vm);
        }

        // POST: Subchapter_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,content,description,chapter_id,url_file")] Subchapter_ subchapter_, HttpPostedFileBase VideoUpload)
        {
            if (ModelState.IsValid)
            {

                if (VideoUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(VideoUpload.FileName);
                    string extension = Path.GetExtension(VideoUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    subchapter_.url_video = fileName;
                    VideoUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Video/"), fileName));
                }


                subchapter_.date_creation = DateTime.Now;
                db.Subchapter_.Add(subchapter_);
                db.SaveChanges();
                return RedirectToAction("AdminSubchapter");
            }

            ViewBag.chapter_id = new SelectList(db.Chapter_, "id", "name", subchapter_.chapter_id);
            return View(subchapter_);
        }

        //Action result for ajax call
        [HttpPost]
        public ActionResult GetChapterByClassId(int classid)
        {
            List<Chapter_> objchapter = new List<Chapter_>();
            objchapter = GetAllChapter().Where(m => m.class_id == classid).ToList();
            SelectList obgchapter = new SelectList(objchapter, "id", "name", 0);
            return Json(obgchapter);
        }



        //collection for class
        public List<Class_> GetAllClass()
        {
            List<Class_> objclass = new List<Class_>();
            var lclass = (from cl in db.Class_
                            select cl).ToList();

            foreach (var c in lclass)
            {
                objclass.Add(new Class_ { id = c.id, name = c.name });
            }

            return objclass;
        }


        //collection for chapter
        public List<Chapter_> GetAllChapter()
        {
            List<Chapter_> objchapter = new List<Chapter_>();
            var lchapter = (from c in db.Chapter_
                           select c).ToList();
            
            foreach(var c in lchapter)
            {
                objchapter.Add(new Chapter_ { id = c.id, class_id =c.class_id, name = c.name });
            }

            return objchapter;
        }


        // GET: Subchapter_/Edit/5
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

            ViewBag.sactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subchapter_ subchapter_ = db.Subchapter_.Find(id);
            if (subchapter_ == null)
            {
                return HttpNotFound();
            }

            var lclass_ = (from c in db.Class_
                           select c).ToList();

            ViewBag.urlvideo = subchapter_.url_video;
            uv = subchapter_.url_video;
            uf = subchapter_.url_file;
            datesub = (DateTime)subchapter_.date_creation;
            subchapter_.ListClass = lclass_;
            ViewBag.chapter_id = new SelectList(db.Chapter_, "id", "name", subchapter_.chapter_id);
            return View(subchapter_);
        }

        // POST: Subchapter_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,content,description,chapter_id,url_video,url_file")] Subchapter_ subchapter_, Subchapter_ model,HttpPostedFileBase VideoUpload)
        {
            if (ModelState.IsValid)
            {
                var urlvideo = subchapter_.url_video;
                if (VideoUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(VideoUpload.FileName);
                    string extension = Path.GetExtension(VideoUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    subchapter_.url_video = fileName;
                    VideoUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Video/"), fileName));
                    subchapter_.date_creation = DateTime.Now;
                }
                else
                {
                    subchapter_.url_video = uv;
                    subchapter_.date_creation = datesub;
                }

       
                

                db.Entry(subchapter_).State = EntityState.Modified;
                db.SaveChanges();



                    //subchapter_.VideoUpload = su.VideoUpload;

                    //db.SaveChanges();
                

                


                return RedirectToAction("AdminSubchapter");


            }
            var lclass_ = (from c in db.Class_
                           select c).ToList();
            subchapter_.ListClass = lclass_;
            ViewBag.chapter_id = new SelectList(db.Chapter_, "id", "name", subchapter_.chapter_id);
            return View(subchapter_);
        }

        // GET: Subchapter_/Delete/5
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

            ViewBag.sactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subchapter_ subchapter_ = db.Subchapter_.Find(id);
            if (subchapter_ == null)
            {
                return HttpNotFound();
            }
            return View(subchapter_);
        }

        // POST: Subchapter_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subchapter_ subchapter_ = db.Subchapter_.Find(id);
            db.Subchapter_.Remove(subchapter_);
            db.SaveChanges();
            return RedirectToAction("AdminSubchapter");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult AdminSubchapter()

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

            ViewBag.sactivity = sa.Activesection(controllerName);

            /***************************************************************************************/



            var subchapter_ = (from sch in db.Subchapter_
                               select sch).ToList();
            return View(subchapter_);
        }


        public string RecupSexeUserFromId(int id)
        {
            var urs = (from uu in db.User_
                       where uu.id == id
                       select uu).First();

            return urs.sexe;
        }


        public string RecupTypeUserFromId(int id)
        {
            var urs = (from uu in db.User_
                       where uu.id == id
                       select uu).First();

            return urs.type.ToString();
        }


        public void SendMailResponse(string mailWhoWriteTheComment,int idFromWhoRespond, string message)
        {
            var usr = (from u in db.User_
                       where u.id == idFromWhoRespond
                       select u).First();


            var sm = new SendMail();

            try
            {
                //mettre l'adresse du destinataire
                sm.SenEmail("webmaster@elearning-malik-ibn-anas.fr","minamba.c@gmail.com","Réponse à votre message", "As salamou 3alaykoum " +usr.first_name+ "a répondu à votre message ("+message+")");
            }
            catch{

            }
        }

    }
}
