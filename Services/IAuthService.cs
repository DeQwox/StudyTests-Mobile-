using System.Threading.Tasks;

namespace OnlineTestingClient.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string login, string password);
}