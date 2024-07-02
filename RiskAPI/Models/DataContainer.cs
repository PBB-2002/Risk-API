using ProjectDataAccess.DbModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RiskAPI.Models
{
    public class DataContainerList
    {        
        public int DataContainerID { get; set; }       
        public int id_version { get; set; }
        public string Title { get; set; }        
        public string Description { get; set; }  
        public bool approved { get; set; }
        public string ContainerReference { get; set; }
        public v_DataContainer_VER? data_Version { get; set; }
    }

    public class DataContainerRequest
    {
        [Required]
        public int DataContainerID { get; set; }
        [JsonIgnore]
        public int id_version { get; set; }
        [Required]
        public bool approved { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? DataContainerReference { get; set; }
        [Required]
        public int operationType { get; set; }
        [Required]
        public string ContainerReference { get; set; }
        public v_DataContainer_VER? data_Version { get; set; }
    }

    public class DataVersionRequest
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

    public class DataRiskListbyDCID {
        public int DataContainerID { get; set; }
        public int id_version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool approved { get; set; }
        public v_DataContainer_VER? data_Version { get; set; }
        public List<v_DataRisk_CFG> dataRiskList { get; set; }
    }

    public class DataRiskRequest
    {
        [Required]
        public int DataContainerID { get; set; }
        [Required]
        public bool approved { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? DataContainerReference { get; set; }
        [Required]
        public int operationType { get; set; }
        [Required]
        public string ContainerReference { get; set; }
        public v_DataContainer_VER? data_Version { get; set; }
        public List<DataRiskDet> dataRisk { get; set; }
    }
    public class DataRiskDet {
        [JsonIgnore]
        public int DataContainerID { get; set; }
        [JsonIgnore]
        public int id_version { get; set; }
        [JsonIgnore]
        public int RiskID { get; set; }
        public int? CQARiskID { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Probability { get; set; }
        public string? ImpactDescription { get; set; }
        public double? GrossRiskValue { get; set; }
        public string? MitigationType { get; set; }
        public string? MitigationActionDescription { get; set; }
        public string? MitigatedProbability { get; set; }
        public double? MitigationCost { get; set; }
        public double? MitigatedImpactValue { get; set; }
        public double? NetRiskCost { get; set; }
        public string? Currency_Code { get; set; }
        public string? Comment { get; set; }
        public bool? isChecked { get; set; }
        public bool? isManual { get; set; }
        public string? RiskOwner { get; set; }
    }
}
