using Refit;
using System.Threading.Tasks;

namespace TemplateService.Interfaces
{
    public interface IExternalIp
    {
        [Get("")]
        Task<string> GetMyExternalIp();
    }
}