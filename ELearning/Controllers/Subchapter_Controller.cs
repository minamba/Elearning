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
using Microsoft.WindowsAPICodePack.Shell;
using System.Web.Hosting;

namespace ELearning.Controllers
{
    public class Subchapter_Controller : Controller
    {
        private elearningEntities db = new elearningEntities();
        private static string uv;
        private static string uf;
        private static TimeSpan time;
        private static int time_vid;
        private static DateTime datesub;

        // GET: Subchapter_
        [Authorize]
        public ActionResult Index(int sub_chapter_id, int class_id)
        {
            /**************************************GET CURRENT PAGE********************************/
            ViewBag.currentPage = null;
            /**************************************GET CURRENT PAGE********************************/
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var usr = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            ViewBag.Role = usr.type;
            /****************************************************************/

            var vm = new SubchapterWithCommentsAndReply();

            var chapterList = (from cl in db.Chapter_
                               where cl.class_id == class_id
                               select cl).ToList();

            var subchapterList = (from sl in db.Subchapter_
                                  select sl).ToList();

            var subchapter_ = (from sch in db.Subchapter_
                               where sch.id == sub_chapter_id
                               select sch).First();

            //compte le nombre de sous chapitre
            var numberVideo = (from nv in db.Subchapter_
                               from ch in db.Chapter_
                               from cl in db.Class_
                               where cl.id == class_id
                               && ch.class_id == cl.id
                               && nv.chapter_id == ch.id
                               select nv).Count();

            var listSeen = (from s in db.Seen_
                            where s.user_asp_net_id == uid
                            select s).ToList();

            var numberVideoSeenByUser = (from nsu in db.Seen_
                                         where nsu.user_asp_net_id == uid
                                         && nsu.seen == true
                                         && nsu.chapter_id == subchapter_.chapter_id
                                         select nsu).Count();

            //selection de tous les sous chapitre en fonction du chapitre en parametre (j'utilise ça pour initialiser les videos du chap courant pour l'utilisateur courant)

            //List<Subchapter_> lsbch = new List<Subchapter_>();
            //foreach(var ch in chapterList)
            //{
            //    try
            //    {
            //        var subchapter = (from sbc in db.Subchapter_
            //                                       where sbc.chapter_id == ch.id
            //                                       select sbc).First();
            //        lsbch.Add(subchapter);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            
            //}

            //foreach(var subch in lsbch)
            //{
            //    CreateVideoUser(subch.url_video, usr.id, (int)subch.chapter_id);
            //}

          

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
            ViewBag.uid = user.id;
            ViewBag.compteur = 0;
            ViewBag.userID = user.id; /*user.GetUserName(subchapter_.) */
            ViewBag.subchapterID = subchapter_.id;

            var subchapterID2 = subchapter_.id;

            List<Comment_> comment = ListCommentsss(subchapterID2);
            
            var reply = (from r in db.Reply_
                         select r).ToList();


            var alluser = (from u in db.User_
                           select u).ToList();

            //je crée une liste pour récuperer toutes les dates time et extraire les informations qui m'interesse
            List<string> d = new List<string>();

            foreach (var date in comment)
            {
                DateTime dateValue = (DateTime)date.date_message;
                var day = dateValue.Day;
                var month = dateValue.Month;
                var year = dateValue.Year;
                var hour = dateValue.Hour;
                var minute = dateValue.Minute;


                if (day < 10)
                {
                    day = 0 + day;
                }

                var concatdate = day + "/" + month + "/" + year + " à " + hour + ":" + minute;
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

            ViewBag.subchapterTitle = subchapter_.name;


            var listTag = new List<string>();
            for(int x=0; x<chapterList.Count; x++)
            {
                listTag.Add("tag" + chapterList[x].id + x);
            }
            vm.tag = listTag;


            try
            {
                var video = (from v in db.Video_
                             where v.url_video == subchapter_.url_video
                             select v).First();
                vm.video = video;
            }
            catch
            {
                Video_ v = new Video_();
                v.time_start = 0;
                v.user_id = usr.id;
                v.url_video = subchapter_.url_video;
                db.Video_.Add(v);
                db.SaveChanges();

                vm.video = v;
            }


            //Calcul du pourcentage par rapport à la progression
            decimal totalPurcent = numberVideo;
            decimal userPurcent = numberVideoSeenByUser;
            decimal currentPurcent;
            if (totalPurcent < userPurcent)
                currentPurcent = 100;
            else
                currentPurcent = (userPurcent/totalPurcent)*100;

            vm.CurrentSubchapter = subchapter_;
            vm.ListComment = comment;
            vm.ListReply = reply;
            vm.CurrrentUsr = user;
            vm.ListUser = alluser;
            vm.ListChapter = chapterList;
            vm.ListSubchapter = subchapterList;
            vm.ListSeen = listSeen;
            vm.UserPurcentage = (int)currentPurcent;
            vm.TotalVideoSection = numberVideo;
            vm.VideoSeenUser = numberVideoSeenByUser;
            vm.UID = uid;


            return View(vm);
        }


        //methode qui me renvoie la liste des commentaires en fonction du subchapter_id
        public List<Comment_> ListCommentsss(int sub_chapter_id)
        {
            var lstComment = (from lc in db.Comment_
                              where lc.sub_chapter_id == sub_chapter_id
                              select lc).ToList();

            return lstComment;
        }

        [Authorize]
        public ActionResult GetComments(int sub_chapter_id)
        {
            var vm = new SubchapterWithCommentsAndReply();

            try
            {
                var lstComment = (from lc in db.Comment_
                                  where lc.sub_chapter_id == sub_chapter_id
                                  select lc).ToList();

                var reply = (from r in db.Reply_
                             select r).ToList();

                //je crée une list de string qui va me permettre d'avoir des id unique pour le spoiler
                var l = new List<string>();
                var l_ = new List<string>();
                foreach (var lst in lstComment)
                {
                    l.Add("tag" + lst.id);
                    l_.Add("tag2" + lst.id);
                }


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


                vm.TagComment = l;
                vm.TagComment2 = l_;

                vm.ListComment = lstComment;
                vm.ListReply = reply;
            }
            catch
            {
                return null;
            }



           
            return PartialView("_CommentPartial", vm);
        }


        //methode qui permet de convertir des nanoseconde en milliseconde
        public static double Convert100NanosecondsToMilliseconds(double nanoseconds)
        {
            return nanoseconds * 0.0001;
        }


        public void CreateVideoUser(string url_video,int uid, int chapter_id)
        {

            try
            {
                var uvideo = (from uv in db.Video_
                              where uv.url_video == url_video
                              && uv.user_id == uid
                              select uv).First();
            }
            catch
            {
                var video = new Video_();
                video.user_id = uid;
                video.chapter_id = chapter_id;
                video.url_video = url_video;
                db.Video_.Add(video);
                db.SaveChanges();
            }

        }


        [Authorize]
        public string PushCurrentVideoTime(int currentTime, string url_video, int duration)
        {
            var uid = User.Identity.GetUserId();
            var usr = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            try
            {
                var video = (from v in db.Video_
                             where v.url_video == url_video
                             && v.user_id == usr.id
                             select v).First();

                if (video.duration == null)
                    video.duration = duration;

                video.time_start = currentTime;
                db.SaveChanges();


                return "sauvegarde reussi";
            }
            catch(Exception e)
            {
                return e.Message;
            }          
        }

        [Authorize]
        public string PushCurrentVideoFinished(int currentTime, string url_video, int duration, int subchapter_id, int chapter_id)
        {
            var uid = User.Identity.GetUserId();
            var usr = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            try
            {
                var video = (from v in db.Video_
                             where v.url_video == url_video
                             && v.user_id == usr.id
                             select v).First();

                if (video.duration == null)
                    video.duration = duration;

                video.time_start = currentTime;

                if(currentTime == video.duration)
                {
                    try
                    {
                        var validateVideo = (from vv in db.Seen_
                                             where vv.user_asp_net_id == uid
                                             && vv.subchapter_id == subchapter_id
                                             && vv.chapter_id == chapter_id
                                             select vv).First();

                        if(validateVideo.seen == false)
                            validateVideo.seen = true;

                        db.SaveChanges();
                        return "sauvegarde reussi";
                    }
                    catch
                    {
                        return "savegarde échoué";
                    }
                }

                return "succès";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Authorize]
        public ActionResult GetSubchapterVideo(int sub_chapter_id)
        {
            var vm = new SubchapterWithCommentsAndReply();

            var uid = User.Identity.GetUserId();
            var usr = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            var subchapter_ = (from sc in db.Subchapter_
                               where sc.id == sub_chapter_id
                               select sc).First();


            try
            {
                var video = (from v in db.Video_
                             where v.url_video == subchapter_.url_video
                             && v.user_id == usr.id
                             select v).First();

                vm.video = video;
            }
            catch
            {
                Video_ v = new Video_();
                v.time_start = 0;
                v.user_id = usr.id;
                v.url_video = subchapter_.url_video;
                v.chapter_id = subchapter_.chapter_id;
                db.Video_.Add(v);
                db.SaveChanges();

                vm.video = v;
            }


           
            vm.CurrentSubchapter = subchapter_;



            return PartialView("_PartialVideo", vm);
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

            foreach (var c in class_)
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
                    string fullPath = Path.GetFullPath(VideoUpload.ToString());
                    string fileName = Path.GetFileNameWithoutExtension(VideoUpload.FileName);
                    string extension = Path.GetExtension(VideoUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                    subchapter_.url_video = fileName;           

                    
                    //VideoUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Video/"), fileName));
                    //----------------------------FTP VPS UPLOAD-----------------------------------------//
                    var uploadurl = @"ftp://vps64363.lws-hosting.com//web//Video/";
                    var uploadfilename = fileName;
                    var username = "defaultminamba";
                    var password = "elearning2019@";
                    Stream streamObj = VideoUpload.InputStream;
                    byte[] buffer = new byte[VideoUpload.ContentLength];
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

            foreach (var c in lchapter)
            {
                objchapter.Add(new Chapter_ { id = c.id, class_id = c.class_id, name = c.name });
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
            if(subchapter_.time_video != null)
            time_vid = (int)subchapter_.time_video;
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
        public ActionResult Edit([Bind(Include = "id,name,content,description,chapter_id,url_video,url_file")] Subchapter_ subchapter_, Subchapter_ model, HttpPostedFileBase VideoUpload, int chapter_id )
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
                    string fullFilePath = model.url_video_base_path + VideoUpload.FileName;

                    try
                    {
                        ShellFile so = ShellFile.FromFilePath(fullFilePath);
                        double nanoseconds;
                        double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds);

                        if (nanoseconds > 0)
                        {
                            double seconds = Convert100NanosecondsToMilliseconds(nanoseconds) / 1000;
                            int ttl_seconds = Convert.ToInt32(seconds);
                            time = TimeSpan.FromSeconds(ttl_seconds);

                        }


                        //VideoUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Video/"), fileName));
                        //----------------------------FTP VPS UPLOAD-----------------------------------------//
                        var uploadurl = @"ftp://vps64363.lws-hosting.com//web//Video/";
                        var uploadfilename = fileName;
                        var username = "defaultminamba";
                        var password = "elearning2019@";
                        Stream streamObj = VideoUpload.InputStream;
                        byte[] buffer = new byte[VideoUpload.ContentLength];
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
                        subchapter_.date_creation = DateTime.Now;
                        subchapter_.time_video = time.Minutes;
                        if (VideoUpload.FileName != null)
                            subchapter_.url_video = fileName;
                        else
                            subchapter_.url_video = uv;
                        db.SaveChanges();
                    }
                    catch
                    {
                        Console.WriteLine("editing error !");
                    }

                }
                else
                {
                    subchapter_.time_video = time_vid;
                    subchapter_.url_video = uv;
                    subchapter_.date_creation = datesub;
                }



                subchapter_.chapter_id = chapter_id;
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


        public void SendMailResponse(string mailWhoWriteTheComment, int idFromWhoRespond, string message)
        {
            var usr = (from u in db.User_
                       where u.id == idFromWhoRespond
                       select u).First();


            var sm = new SendMail();

            try
            {
                //mettre l'adresse du destinataire
                sm.SenEmail("webmaster@elearning-malik-ibn-anas.fr", mailWhoWriteTheComment, "Réponse à votre message", "As salamou 3alaykoum " + usr.first_name + "a répondu à votre message (" + message + ")");
            }
            catch
            {

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

                for (int i = 0; i < seen.Count; i++)
                {
                    if (seen[i].subchapter_id == subchapterID)
                    {
                        try
                        {
                            var user_seen = seen[i - 1].seen;

                            if (user_seen != null)
                            {
                                if (user_seen == false)
                                {
                                    var result = "Vous devez regarder la vidéo précedente avant de pouvoir regarder celle - ci";
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                    //ViewBag.Message = "Vous devez regarder la vidéo précedente avant de pouvoir regarder celle - ci";
                                    return RedirectToAction("Index", "Chapter_", new { class_id = class_.id, class_description = description });
                               
                                }
                                else
                                {
                                    //seen[i].seen = true;
                                    //db.SaveChanges();
                                    
                                    return RedirectToAction("Index", "Subchapter_", new { sub_chapter_id = (int)seen[i].subchapter_id });

                                }
                            }
                        }
                        catch
                        {
                            //seen[i].seen = true;
                            //db.SaveChanges();

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
