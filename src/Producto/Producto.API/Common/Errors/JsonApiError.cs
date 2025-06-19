namespace Producto.API.Common.Errors
{
    public class JsonApiError
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string? Source { get; set; } 
    }
}
