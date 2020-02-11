namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sayfalar")]
    public partial class Sayfalar
    {
        [Key]
        public int SayfaId { get; set; }

        [StringLength(500)]
        public string SayfaStringi { get; set; }

        public int? KategoriId { get; set; }

        [StringLength(500)]
        public string Caption { get; set; }

        public virtual Kategoriler Kategoriler { get; set; }
    }
}
