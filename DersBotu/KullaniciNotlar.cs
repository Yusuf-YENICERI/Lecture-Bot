namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KullaniciNotlar")]
    public partial class KullaniciNotlar
    {
        public int KullaniciNotlarId { get; set; }

        public int? KullaniciId { get; set; }

        [StringLength(500)]
        public string NotStringi { get; set; }

        public virtual Kullanicilar Kullanicilar { get; set; }
    }
}
