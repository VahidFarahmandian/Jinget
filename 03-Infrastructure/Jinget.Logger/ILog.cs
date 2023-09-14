using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Jinget.Logger
{
    public interface ILog
    {
        Task Log(HttpContext context);
    }
}