using System.ComponentModel.DataAnnotations;

namespace ProjectDataAccess.DbModel
{
    public class Data_Control
    {
        [Required]
        public int DataContainerId { get; set; }
        [Required]
        public int id_version { get; set; }
        [Required]
        public bool approved { get; set; }
    }
}
