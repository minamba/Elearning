using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Utilities
{
    public class SidebarActivity
    {
        public string Name { get; set; }

        public SidebarActivity()
        {

        }

        public SidebarActivity(string name)
        {
            this.Name = name;
        }

        public string Activesection(string name)
        {
            string active;

            if (name == "User_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }


            if (name == "Theme_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }


            if (name == "Class_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }

            if (name == "Chapter_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }


            if (name == "Subchapter_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }


            if (name == "Group_")
            {
                active = "active";
                return active;
            }
            else
            {
                active = "";
            }



            return active;
        }


    }
}