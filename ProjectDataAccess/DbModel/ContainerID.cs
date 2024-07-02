using System.ComponentModel.DataAnnotations;

namespace ProjectDataAccess.DbModel
{
    public class ContainerID
    {
        [Required]
        public int ContainerId { get; set; }
        [Required]
        public string Source { get; set; }
        [Required]
        public string ContainerReference { get; set; }
    }
}
