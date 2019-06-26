using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ELearning.Models;
using Microsoft.AspNet.Identity;
using ELearning.Utilities;

namespace ELearning.Controllers
{
    public class HomeController : Controller
    {
        elearningEntities db = new elearningEntities();


        [Authorize]
        public ActionResult Index()
        {
            /**************************************GET CURRENT PAGE********************************/
            ViewBag.currentPage = null;
            /**************************************GET CURRENT PAGE********************************/

            var vm = new HomeWithClassAndSubchapterViewModel();
            var uid="";
            try
            {
                uid = User.Identity.GetUserId();        
          
                var user = (from u in db.User_
                            where u.user_asp_net_id == uid
                            select u).First();

                if (user.user_validate == false)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.uname = user.first_name;
                    ViewBag.Role = user.type;

                var groupUser = (from g in db.Group_
                                 where g.id == user.group_id
                                 select g).First();

                var glclass = (from c in db.Class_
                              where c.group_id == groupUser.id
                              select c).OrderByDescending(c => c.create_date).Take(3).ToList();

                var glchapter = (from ch in db.Chapter_
                                 select ch).ToList();


            List<Chapter_> lchapter = new List<Chapter_>();
                for (int i = 0; i < glchapter.Count; i++)
                {
                    for (int j = 0; j < glclass.Count; j++)
                    {
                        if (glchapter[i].class_id == glclass[j].id)
                        {
                            lchapter.Add(glchapter[i]);
                        }
                    }
                }

                /*pour les ppt*/
                var glsubchapter_ = (from sb in db.Subchapter_
                                     where sb.url_file != null
                                     && sb.url_file != ""
                                     select sb).OrderByDescending(sb => sb.date_creation).Take(3).ToList();
                /*pour les videos*/
                var glsubchapter2_ = (from sb in db.Subchapter_
                                     where sb.url_video != null
                                     && sb.url_video != ""
                                     select sb).OrderByDescending(sb => sb.date_creation).Take(3).ToList();

                    List<Chapter_> listChapterAfterFilter = new List<Chapter_>();

                List<Subchapter_> lsubchapter_ = new List<Subchapter_>();
                List<Subchapter_> lsubchapter2_ = new List<Subchapter_>();
                    for (int i = 0; i < glclass.Count; i++)
                {
                    for (int j = 0; j < glchapter.Count; j++)
                    {
                        if (glclass[i].id == glchapter[j].class_id)
                        {
                            listChapterAfterFilter.Add(glchapter[j]);
                        }
                    }
                }


               for(int i=0; i< glchapter.Count; i++)
            {
                for (int j = 0; j < glsubchapter_.Count; j++)
                {
                    if (glchapter[i].id == glsubchapter_[j].chapter_id)
                    {
                        lsubchapter_.Add(glsubchapter_[j]);
                    }
                }
            }


                    for (int i = 0; i < glchapter.Count; i++)
                    {
                        for (int j = 0; j < glsubchapter2_.Count; j++)
                        {
                            if (glchapter[i].id == glsubchapter2_[j].chapter_id)
                            {
                                lsubchapter2_.Add(glsubchapter2_[j]);
                            }
                        }
                    }


                    vm.ListClass = glclass;
                vm.ListSubChapter = lsubchapter_;
                vm.ListSubChapter2 = lsubchapter2_;
                ViewBag.classidzero = glclass[0].id;
                ViewBag.classdescriptionzero = glclass[0].description;

                return View(vm);
                }
                return null;
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact(string mess)
        {
            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var user = (from u in db.User_
                        where u.user_asp_net_id == uid
                        select u).First();

            ViewBag.Role = user.type;
            /****************************************************************/

            ViewBag.Message = "Your contact page.";
            var vm = new ContactViewModel();
            vm.msg = mess;

            return View(vm);
        }

        public ActionResult EnvoieMail(string email,string title,string message)
        {
            var mail = new SendMail();
            var uid = User.Identity.GetUserId();

            var user = (from u in db.User_
                        where u.user_asp_net_id == uid
                        select u).First();

            var vm = new ContactViewModel();

            try
            {
                mail.SenEmail(user.mail, title, message);
                vm.msg = "Merci, votre e-mail a bien été envoyer à notre équipe";
                return RedirectToAction("Contact", "Home", new { mess = vm.msg });
            }
            catch
            {
                vm.msg = "Un problème est survenu lors de l'envoie de l'e-mail, veuillez réessayer plus tard...";
                return RedirectToAction("Contact", "Home", new { mess = vm.msg}); ;
            }

           
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}