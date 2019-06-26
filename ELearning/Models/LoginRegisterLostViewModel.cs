using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Models
{
    public class LoginRegisterLostViewModel
    {
        [Required]
        public RegisterViewModel RegisterViewMdl { get; set; }
        public LoginViewModel LoginViewMdl { get; set; }
        public string SelectedGroupe { get; set; }
        public string SelectedSexe { get; set; }
        [Required]
        public List<SelectListItem> ListGroupes { get; set; }
        [Required]
        public List<SelectListItem> ListSexe { get; set; }
        [Required]
        [Display(Name = "Activé")]
        public Nullable<bool> user_validate { get; set; }
        public string SelectedType { get; set; }
        [Required]
        public List<SelectListItem> ListType { get; set; }
    }
}