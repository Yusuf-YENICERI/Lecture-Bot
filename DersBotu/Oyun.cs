namespace DersBotu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Oyun")]
    public partial class Oyun
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OyunId { get; set; }

        public bool Oynuyor { get; set; }

        public int? Kazandı { get; set; }

        public int? Kaybetti { get; set; }

        public int? Puan { get; set; }

        public virtual Kullanicilar Kullanicilar { get; set; }
    }
}
