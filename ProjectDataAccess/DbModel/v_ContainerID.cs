using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataAccess.DbModel
{
    public class v_ContainerID
    {
        [Required]
        public int ContainerId { get; set; }
        [Required]
        public string Source { get; set; }
        [Required]
        public string ContainerReference { get; set; }
    }
}
