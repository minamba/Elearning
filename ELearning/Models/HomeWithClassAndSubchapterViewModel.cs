using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class HomeWithClassAndSubchapterViewModel
    {
        public List<Class_> ListClass { get; set; }
        public List<Subchapter_> ListSubChapter { get; set; }
        public List<Subchapter_> ListSubChapter2 { get; set; }
        public List<Chapter_> ListChapter {get; set;}
       
    }
}