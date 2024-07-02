namespace RiskAPI.Helpers
{
    public class Messages
    {
        public class DataContainerRiskControllerMessgaes
        {
            public const string contRefInputMessage = "Please provide containerReference";
            public const string datacontidInputMessage = "Please provide Data Container ID";
            public const string datacontainervalMessage = "Data Container with given Title is already present in the system";

            public const string getDataContainerRiskMessage = "Record(s) retrieved successfully.";
            public const string addDataContainerRiskMessage = "Record inserted successfully.";
            public const string updateDataContainerRiskMessage = "Record updated successfully.";
            public const string deleteDataContainerRiskMessage = "Record deleted successfully.";

            public const string NoDataContainerRiskMessage = "Record(s) Not Found.";
            public const string BadRequestMessage = "Bad Request.";
            public const string getErrorMessage = "Error while retrieving record(s).";
            public const string addErrorMessage = "Error while inserting record.";
            public const string updateErrorMessage = "Error while updating record.";
            public const string deleteErrorMessage = "Error while deleting record.";
            public const string versionmismatchMessage = "Version mismatch";
        }
    }
}
