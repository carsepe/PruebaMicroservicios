namespace Inventario.API.Common.Errors
{
    public class JsonApiError
    {
        public string Status { get; set; } = "500";
        public string Title { get; set; } = "Error";
        public string Detail { get; set; } = string.Empty;
        public string? Source { get; set; }
    }
}
