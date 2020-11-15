using System.Collections.Generic;
using System.Threading.Tasks;
using TestSlabon.Models.Entities;
using TestSlabon.Models.Request;
using TestSlabon.Models.Response;

namespace TestSlabon.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Auth(LoginRequest model);
        Task <List<UserResponse>> GetAll();
        Task<UserResponse> GetById(int id);
        Task<int> Update(UserRequest model);
        Task<Users> Add(UserRequest model);
        Task<bool> EmailExists(string email);
        Task<bool> UserExists(int id);
        Task<Users> Delete(UserResponse model);
        Task<bool> UserNameExists(UserRequest model);
    }
}
