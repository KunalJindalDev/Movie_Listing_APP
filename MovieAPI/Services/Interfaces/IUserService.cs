using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IUserService
    {
        bool Signup(SignupRequest request);
        LoginResponse Login(LoginRequest request);
    }
}
