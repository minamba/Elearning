using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class ClassViewModelsWithChapter
    {
        public string ClassDescription { get; internal set; }
        public List<string> tag { get; set; }
        public virtual List<Chapter_> ListChapter_ { get; set; }
        public virtual List<Subchapter_> ListSubChapter_ { get; set; }
        public List<Seen_> ListSeen { get; set; }
    }
}