using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class UserViewModel
    {
        public IQueryable<User_> ListUser { get; set; }
        public User_ usr { get; set; }
    }
}