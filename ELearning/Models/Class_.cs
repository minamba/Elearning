//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ELearning.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Class_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Class_()
        {
            this.Chapter_ = new HashSet<Chapter_>();
            this.Comment_ = new HashSet<Comment_>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<int> theme_id { get; set; }
        public Nullable<int> group_id { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public string url_img { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chapter_> Chapter_ { get; set; }
        public virtual Group_ Group_ { get; set; }
        public virtual Theme_ Theme_ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment_> Comment_ { get; set; }
    }
}
