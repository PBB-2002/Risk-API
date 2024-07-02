using System.ComponentModel.DataAnnotations;

namespace ProjectDataAccess.DbModel
{
    public class Data_Version
    {
        [Required]
        public int DataContainerId { get; set; }
        [Required]
        public int id_version { get; set; }
        [Required]
        public string created_by { get; set; }
        [Required]
        public DateTime created_on { get; set; }   
        public string? updated_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? submit_by { get; set; }
        public DateTime? submit_on { get; set; }
        public string? description { get; set; }
    }
}
