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

            try
            {
                var test = (from d in db.User_
                            select d).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


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


                    //si l'utilisateur est un admin ou si c'est cheikh, je filtre pas l'affichage des cours, theme en fonction de l'user
                    if (user.type == 1 || user.type == 3)
                    {
                        var glclass = (from c in db.Class_
                                       select c).OrderByDescending(c => c.create_date).Take(3).ToList();

                        var generalchapter = (from ch in db.Chapter_
                                              select ch).ToList();


                        List<Chapter_> glchapter = new List<Chapter_>();

                        for (int i = 0; i < glclass.Count; i++)
                        {
                            for (int j = 0; j < generalchapter.Count; j++)
                            {
                                if (glclass[i].id == generalchapter[j].class_id)
                                {
                                    glchapter.Add(generalchapter[i]);
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




                        vm.ListClass = glclass;
                        vm.ListSubChapter = glsubchapter_;
                        vm.ListSubChapter2 = glsubchapter2_;
                        ViewBag.classidzero = glclass[0].id;
                        ViewBag.classdescriptionzero = glclass[0].description;
                    }
                    //sinon c'est un utilisateur normal et j'affiche les cours et thème en fonction des droits
                    else
                    {
                        var glclass = (from c in db.Class_
                                       where c.group_id == groupUser.id
                                       select c).OrderByDescending(c => c.create_date).Take(3).ToList();

                        var generalchapter = (from ch in db.Chapter_
                                              select ch).ToList();


                        List<Chapter_> glchapter = new List<Chapter_>();

                        for (int i = 0; i < glclass.Count; i++)
                        {
                            for (int j = 0; j < generalchapter.Count; j++)
                            {
                                if (glclass[i].id == generalchapter[j].class_id)
                                {
                                    glchapter.Add(generalchapter[i]);
                                }
                            }
                        }



                        /*pour les ppt*/
                        var glsubchapter_ = (from sb in db.Subchapter_
                                             where sb.url_file != null
                                             && sb.url_file != ""
                                             && sb.Chapter_.Class_.group_id == groupUser.id
                                             select sb).OrderByDescending(sb => sb.date_creation).Take(3).ToList();
                        /*pour les videos*/
                        var glsubchapter2_ = (from sb in db.Subchapter_
                                              where sb.url_video != null
                                              && sb.url_video != ""
                                              && sb.Chapter_.Class_.group_id == groupUser.id
                                              select sb).OrderByDescending(sb => sb.date_creation).Take(3).ToList();

                        List<Chapter_> listChapterAfterFilter = new List<Chapter_>();

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

                        vm.ListClass = glclass;
                        vm.ListSubChapter = glsubchapter_;
                        vm.ListSubChapter2 = glsubchapter2_;
                        ViewBag.classidzero = glclass[0].id;
                        ViewBag.classdescriptionzero = glclass[0].description;
                    }



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
                mail.SenEmail(user.mail,"webmaster@elearning-malik-ibn-anas.fr", title, message);
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