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
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Subchapter_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subchapter_()
        {
            this.Comment_ = new HashSet<Comment_>();
            this.Video_ = new HashSet<Video_>();
        }

        public int id { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string name { get; set; }
        [Display(Name = "Fichier")]
        public byte[] content { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }
        [Required]
        public Nullable<int> chapter_id { get; set; }
        [Display(Name = "Vidéo")]
        public string url_video { get; set; }
        [Display(Name = "Fichier")]
        public string url_file { get; set; }
        public Nullable<System.DateTime> date_creation { get; set; }
        public System.Web.HttpPostedFileBase VideoUpload { get; set; }
        public virtual Chapter_ Chapter_ { get; set; }
        public string ClassName { get; set; }
        public List<Class_> ListClass { get; set; }
        public List<Chapter_> ListChapter { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment_> Comment_ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Video_> Video_ { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Class_> Class_ { get; set; }
    }



}
