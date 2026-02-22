using System.Text.Json;

namespace Mini_Dating_App_BE.Middleware
{
    public class ErrorException
    {
        public int StatusCode { get; set; }

        public string? Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
