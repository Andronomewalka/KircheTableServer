namespace RawData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Project_Address
    {
        public int id { get; set; }

        public int? project_id { get; set; }

        public int? district_id { get; set; }

        public int? address_id { get; set; }

        public virtual Address Address { get; set; }

        public virtual Projects Projects { get; set; }
    }
}
