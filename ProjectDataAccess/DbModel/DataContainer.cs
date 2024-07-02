using System.ComponentModel.DataAnnotations;

namespace ProjectDataAccess.DbModel
{
    public class DataContainer
    {
        [Required]
        public int DataContainerID { get; set; }
        [Required]
        public int id_version { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
