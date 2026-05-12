namespace NewDoor.Platform.DTO.Common
{
    public class FilterRequest
    {
        public string? FieldName { get; set; }
        public string? Operator { get; set; }
        public string? Value { get; set; }
    }

    public class FilterCriteria
    {
        public List<FilterRequest> Filters { get; set; } = new();
        public string? LogicalOperator { get; set; } = "AND";
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "ASC";
    }
}
