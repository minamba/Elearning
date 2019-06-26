using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ELearning.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ELearning.Utilities;
using PagedList;

namespace ELearning.Controllers
{
    public class User_Controller : Controller
    {
        private ApplicationUserManager _userManager;
        private elearningEntities db = new elearningEntities();

        // GET: User_
        [Authorize]
        public ActionResult Index()
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/
            /**************************************GET CURRENT PAGE********************************/
            ViewBag.currentPage = "";
            /**************************************GET CURRENT PAGE********************************/

            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.uactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            var user_ = (from u in db.User_
                         select u).ToList();

            //var vm = new UserViewModel();
            //vm.ListUser = user_.ToList();

            return View(user_);
        }

        // GET: User_/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_ user_ = db.User_.Find(id);
            if (user_ == null)
            {
                return HttpNotFound();
            }
            return View(user_);
        }

        // GET: User_/Create
        [Authorize]
        public ActionResult Create()
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/

            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from u in db.User_
                       where u.user_asp_net_id == uid
                       select u).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.uactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            var group = (from g in db.Group_
                         select g).ToList();

            var sitems = new SelectListItem();
            var vm = new RegisterViewModel();
            List<SelectListItem> lgp = new List<SelectListItem>();
            foreach (var i in group)
            {
                lgp.Add(new SelectListItem() { Text = i.name, Value = i.name });
            }

            List<SelectListItem> lsexe = new List<SelectListItem>();
            lsexe.Add(new SelectListItem() { Text = "Homme", Value = "Homme" });
            lsexe.Add(new SelectListItem() { Text = "Femme", Value = "Femme" });

            List<SelectListItem> ltype = new List<SelectListItem>();
            ltype.Add(new SelectListItem() { Text = "Utilisateur", Value = "Utilisateur" });
            ltype.Add(new SelectListItem() { Text = "Administrateur", Value = "Administrateur" });
            ltype.Add(new SelectListItem() { Text = "Cheikh", Value = "Cheikh" });

            vm.ListType = ltype;
            vm.ListSexe = lsexe;
            vm.ListGroupes = lgp;

            ViewBag.group_id = new SelectList(db.Group_, "id", "name");
            return View(vm);
        }

        // POST: User_/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    var userRegister = new UserRegister();
                    userRegister.Insert_LastName_FirstName(user.Id, model.LastName, model.FirstName, user.Email, model.SelectedGroupe, model.SelectedSexe,model.SelectedType,(bool)model.user_validate);

                    var sm = new SendMail();
                    if((bool)model.user_validate == true)
                        sm.SenEmail("minamba.c@gmail.com", "Bienvenue", "As salamou 3alaykoum wa rahmatulLah " + model.FirstName + ", votre compte à bien été crée et activé, vous pouvez des à present vous connecter sur la plateforme.");
                    else
                        sm.SenEmail("minamba.c@gmail.com", "Bienvenue", "As salamou 3alaykoum wa rahmatulLah " + model.FirstName + ", votre compte à bien été crée, vous recevrez un mail lors de l'activation de votre compte.");


                    return RedirectToAction("AdminUser", "User_");
                }                    
                AddErrors(result);
            }

            //si on arrive ici c'est qu'il y a un probleme avec le formulaire 

            var group = (from g in db.Group_
                         select g).ToList();

            var sitems = new SelectListItem();
            List<SelectListItem> lgp = new List<SelectListItem>();
            foreach (var i in group)
            {
                lgp.Add(new SelectListItem() { Text = i.name, Value = i.name });
            }

            List<SelectListItem> lsexe = new List<SelectListItem>();
            lsexe.Add(new SelectListItem() { Text = "Homme", Value = "Homme" });
            lsexe.Add(new SelectListItem() { Text = "Femme", Value = "Femme" });

            List<SelectListItem> ltype = new List<SelectListItem>();
            ltype.Add(new SelectListItem() { Text = "Utilisateur", Value = "Utilisateur" });
            ltype.Add(new SelectListItem() { Text = "Administrateur", Value = "Administrateur" });
            ltype.Add(new SelectListItem() { Text = "Cheikh", Value = "Cheikh" });

            var m = new RegisterViewModel();

            m.ListSexe = lsexe;
            m.ListGroupes = lgp;
            m.ListType = ltype;



            return View(m);
        }

        // GET: User_/Edit/5
        [Authorize]
        public ActionResult Edit(int? id, User_ u)
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

            ViewBag.uactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_ user_ = db.User_.Find(id);

            List<SelectListItem> ltype = new List<SelectListItem>();
            ltype.Add(new SelectListItem() { Text = "Utilisateur", Value = "Utilisateur" });
            ltype.Add(new SelectListItem() { Text = "Administrateur", Value = "Administrateur" });
            ltype.Add(new SelectListItem() { Text = "Cheikh", Value = "Cheikh" });

            List<SelectListItem> lsexe = new List<SelectListItem>();
            lsexe.Add(new SelectListItem() { Text = "Homme", Value = "Homme" });
            lsexe.Add(new SelectListItem() { Text = "Femme", Value = "Femme" });

            user_.ListType = ltype;
            user_.ListSexe = lsexe;

            if(user_.type == 3)
            {
                user_.SelectedType = "Cheikh";
            }
            else if(user_.type == 2)
            {
                user_.SelectedType = "Utilisateur";
            }
            else
            {
                user_.SelectedType = "Administrateur";
            }

            
            if(user_.sexe == "Homme")
            {
                user_.SelectedSexe = "Homme";
            }
            else
            {
                user_.SelectedSexe = "Femme";
            }


            if (user_ == null)
            {
                return HttpNotFound();
            }
            ViewBag.group_id = new SelectList(db.Group_, "id", "name", user_.group_id);
            return View(user_ );
        }

        // POST: User_/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,last_name,first_name,mail,type,group_id,sexe,user_validate")] User_ user_, User_ model)
        {
            if (ModelState.IsValid)
            {


                if (model.SelectedType == "Administrateur")
                {
                    user_.type = 1;
                }
                else if (model.SelectedType == "Cheikh")
                {
                    user_.type = 3;
                }
                else
                {
                    user_.type = 2;
                }
                user_.sexe = model.SelectedSexe;

                db.Entry(user_).State = EntityState.Modified;
                db.SaveChanges();


                var uid = (from u in db.AspNetUsers
                           where u.Email == user_.mail
                           select u).First();


                var usr = (from us in db.User_
                           where us.mail == user_.mail
                           select us).First();

                if (usr.user_validate == false && model.user_validate == true)
                {
                    var sm = new SendMail();
                    sm.SenEmail("minamba.c@gmail.com", "Activation de votre Compte", "As salamou 3alaykoum wa rahmatulLah " + usr.first_name + ", votre compte à été activé par un administrateur, vous pouvez des à present vous connecter sur la plateforme.");
                }

                user_.user_asp_net_id = uid.Id;
                db.SaveChanges();


                return RedirectToAction("AdminUser");
            }
            return View(user_);
        }

        // GET: User_/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/

            /***************Affichage de l'administration*******************/
            var uid = User.Identity.GetUserId();
            var urs = (from u in db.User_
                        where u.user_asp_net_id == uid
                        select u).First();

            ViewBag.Role = urs.type;
            /****************************************************************/
            /***************************Sidebar activity********************************************/
            var sa = new SidebarActivity();

            ViewBag.uactivity = sa.Activesection(controllerName);

            /***************************************************************************************/

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_ user_ = db.User_.Find(id);
            if (user_ == null)
            {
                return HttpNotFound();
            }
            return View(user_);
        }

        // POST: User_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_ user_ = db.User_.Find(id);
            db.User_.Remove(user_);
            db.SaveChanges();
            return RedirectToAction("AdminUser");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ViewResult AdminUser(string sortOrder, string currentFilter, string searchString,int? page)
        {

            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;

            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.currentAction = controllerName;

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

            ViewBag.uactivity = sa.Activesection(controllerName); 

            /***************************************************************************************/



            var user_ = (from u in db.User_
                         select u);

            //var vm = new UserViewModel();
            //vm.ListUser = user_.ToList();


            ViewBag.CurrenSort = sortOrder; //permet de fournir une vue avec l'ordre de tri actuelle, obligé de l'utiliser sinon en changeant de page, le filtre ne s'activera pas
            ViewBag.lastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.firstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "first_name_desc" : "";


            /***controle du champ de recherche***/
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; //cette variable prends la recherche effecter afin de l'appliquer sur les autres pages (pagination)


            if (!String.IsNullOrEmpty(searchString))
            {
                user_ = user_.Where(s => s.last_name.Contains(searchString)
                                       || s.first_name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "last_name_desc":
                    user_ = user_.OrderByDescending(s => s.last_name);
                    //vm.ListUser = user_;
                    break;

                case "first_name_desc":
                    user_ = user_.OrderByDescending(s => s.first_name);
                    //vm.ListUser = user_;
                    break;

                default:
                    user_ = user_.OrderBy(s => s.last_name);
                    //vm.ListUser = user_;
                    break;
            }

            //vm.ListUser = await user_.AsNoTracking().ToListAsync();

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            

            //vm.ListUser = user_;
            return View(user_.ToPagedList(pageNumber, pageSize));
        }



        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public ActionResult GeneralActivation(string mess)
        {
            /**************************************GET CURRENT PAGE********************************/
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.currentPage = controllerName;
            /**************************************GET CURRENT PAGE********************************/

            var vm = new User_();

            vm.MessageActivGlobal = mess;

            return View(vm);
        }


        [HttpPost]
        public ActionResult GeneralActivation(bool? CheckActive, bool? CheckDesactive)
        {
            var users = (from u in db.User_
                         select u).ToList();

            if(CheckActive == true)
            {
                foreach( var u in users)
                {
                    if (u.type != 1)
                    {
                        u.user_validate = true;

                        var sm = new SendMail();
                        sm.SenEmail("minamba.c@gmail.com", "Activation de votre Compte", "As salamou 3alaykoum wa rahmatulLah " + u.first_name + ", votre compte à été activé par un administrateur, vous pouvez des à present vous connecter sur la plateforme.");
                        ViewBag.activate = "Votre action à bien été pris en compte, les comptes utilisateurs à bien été activé !";
                    }
                }
            }
            else if (CheckDesactive == true)
            {
                foreach (var u in users)
                {
                    if (u.type != 1)
                    {
                        u.user_validate = false;
                        ViewBag.activate = "Votre action à bien été pris en compte, le comptes utilisateurs à bien été desactivé !";
                    }
                }             
            }

            db.SaveChanges();

            return RedirectToAction("GeneralActivation", new { mess = ViewBag.activate });
        }

    }
}
