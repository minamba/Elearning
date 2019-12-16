using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class UserRegister
    {
        elearningEntities db = new elearningEntities();

        public void Insert_LastName_FirstName(string userid,string LastName,string FirstName,string Email,string Group,string sexe,string type,bool activate)
        {
            using (db)
            {
                var userEntity = db.User_.FirstOrDefault(p => p.user_asp_net_id == userid);
                if(userEntity == null)
                {
                    var u = new User_();
                    u.last_name = LastName;
                    u.first_name = FirstName;
                    u.mail = Email;
                    u.type = 2;
                    u.user_asp_net_id = userid;
                    u.sexe = sexe;

                    if(type == "Administrateur")
                    {
                        u.type = 1;
                    }
                    else if(type == "Cheikh")
                    {
                        u.type = 3;
                    }
                    else
                    {
                        u.type = 2;
                    }
                    u.user_validate = activate;

                    var groupeId = (from gid in db.Group_
                                    where gid.name == Group
                                    select gid.id).First();
                    u.group_id = groupeId;
                    db.User_.Add(u);
                    db.SaveChanges();
                }
            }
        }


        public string GetUserName(string userID)
        {
            var us = db.User_.FirstOrDefault(u => u.user_asp_net_id == userID);

            if (us == null)
            {
                return null;
            }
            else
                return us.first_name;

        }
    }
}