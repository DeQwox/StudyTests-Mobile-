using OnlineTestingClient.Models;
using System.Threading.Tasks;

namespace OnlineTestingClient.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string login, string password);
    Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterApiDto dto);
}