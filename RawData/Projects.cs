namespace RawData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Projects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Projects()
        {
            Project_Address = new HashSet<Project_Address>();
            Projects_Project_Type = new HashSet<Projects_Project_Type>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int district_id { get; set; }

        public int? church_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? year_start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? year_end { get; set; }

        public int? price { get; set; }

        [StringLength(100)]
        public string note { get; set; }

        [StringLength(150)]
        public string description { get; set; }

        public double? square { get; set; }

        public int? state_id { get; set; }

        public int? scope_id { get; set; }

        public int? implementer_id { get; set; }

        public int? building_type_id { get; set; }

        public virtual Building_Type Building_Type { get; set; }

        public virtual Church Church { get; set; }

        public virtual District District { get; set; }

        public virtual Implementer Implementer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project_Address> Project_Address { get; set; }

        public virtual Scope Scope { get; set; }

        public virtual State State { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projects_Project_Type> Projects_Project_Type { get; set; }
    }
}
