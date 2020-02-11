namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Kullanicilar")]
    public partial class Kullanicilar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kullanicilar()
        {
            KullaniciNotlars = new HashSet<KullaniciNotlar>();
            Yorumlars = new HashSet<Yorumlar>();
        }

        public int KullanicilarId { get; set; }

        [StringLength(500)]
        public string İsim { get; set; }

        [StringLength(500)]
        public string Soyisim { get; set; }

        [StringLength(500)]
        public string KullaniciAdi { get; set; }

        public int? UserId { get; set; }

        public int? Blocked { get; set; }

        public int? Admin { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KullaniciNotlar> KullaniciNotlars { get; set; }

        public virtual Oyun Oyun { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Yorumlar> Yorumlars { get; set; }
    }
}
