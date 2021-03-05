using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog.Events;

namespace TemplateService.Models
{
    public class LogLevelRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LogEventLevel LogLevel { get; set; }
    }
}