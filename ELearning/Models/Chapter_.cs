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
    
    public partial class Chapter_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Chapter_()
        {
            this.Subchapter_ = new HashSet<Subchapter_>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> class_id { get; set; }
    
        public virtual Class_ Class_ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subchapter_> Subchapter_ { get; set; }
    }
}
