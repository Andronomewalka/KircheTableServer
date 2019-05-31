namespace RawData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Projects_Project_Type
    {
        public int id { get; set; }

        public int project_id { get; set; }

        public int district_id { get; set; }

        public int project_type_id { get; set; }

        public virtual Project_Type Project_Type { get; set; }

        public virtual Projects Projects { get; set; }
    }
}
