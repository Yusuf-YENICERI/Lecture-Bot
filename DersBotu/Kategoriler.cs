namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Kategoriler")]
    public partial class Kategoriler
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kategoriler()
        {
            Sayfalars = new HashSet<Sayfalar>();
        }

        [Key]
        public int KategoriId { get; set; }

        [StringLength(300)]
        public string KategoriIsmi { get; set; }

        public int? DersId { get; set; }

        public virtual Ana Ana { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sayfalar> Sayfalars { get; set; }
    }
}
