using System.ComponentModel.DataAnnotations;

namespace ProjectDataAccess.DbModel
{
    public class DataRisk
    {
        [Required]
        public int DataContainerID { get; set; }
        [Required]
        public int id_version { get; set; }
        [Required]
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
