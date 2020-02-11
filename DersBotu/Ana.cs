namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ana")]
    public partial class Ana
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ana()
        {
            Kategorilers = new HashSet<Kategoriler>();
        }

        [Key]
        public int DersId { get; set; }

        [StringLength(20)]
        public string Dersler { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kategoriler> Kategorilers { get; set; }
    }
}
