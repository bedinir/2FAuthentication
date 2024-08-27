using TwoStepAuthentication.Models;

namespace TwoStepAuthentication.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseData<LoginResponse>> Login(LoginRequest loginRequest);

    }
}
