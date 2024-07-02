using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataAccess.DbModel
{
    public class DataContainerID
    {
        [Column("DataContainerID")]
        public int dataContainerID { get; set; }

        [Required]
        public int ContainerID { get; set; }
        public string? DataContainerReference { get; set; }
    }
}
