namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Yorumlar")]
    public partial class Yorumlar
    {
        [Key]
        public int YorumId { get; set; }

        public int? KullaniciId { get; set; }

        [Column("Yorumlar")]
        [StringLength(500)]
        public string Yorumlar1 { get; set; }

        public virtual Kullanicilar Kullanicilar { get; set; }
    }
}
