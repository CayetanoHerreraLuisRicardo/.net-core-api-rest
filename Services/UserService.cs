using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSlabon.Data;
using TestSlabon.Models.Entities;
using TestSlabon.Models.Request;
using TestSlabon.Models.Response;

namespace TestSlabon.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly SlabonContext _context;
        public UserService(IConfiguration configuration, SlabonContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<LoginResponse> Auth(LoginRequest model)
        {
            LoginResponse oRes = new LoginResponse();
                string pass = Utils.Helper.GetSHA256(model.Password);
                Users oUser = await _context.Users.Where(x => x.UserName == model.UserName && x.Password == pass).FirstOrDefaultAsync();
                if (oUser == null) return null;
                oRes.PkuserId = oUser.PkuserId;
                oRes.Email = oUser.Email;
                oRes.Token = Utils.Helper.GenarateToken(oUser, _configuration);
            return oRes;
        }
        public async Task<List<UserResponse>> GetAll()
        {
            return await _context.Users.Select(a => new UserResponse
            {
                PkuserId = a.PkuserId,
                Email = a.Email,
                UserName = a.UserName,
                Gender = a.Gender,
                Status = a.Status
            }).OrderByDescending(b => b.PkuserId).ToListAsync();
        }

        public async Task<UserResponse> GetById(int id)
        {
            return await _context.Users.Select(a => new UserResponse
            {
                PkuserId = a.PkuserId,
                Email = a.Email,
                UserName = a.UserName,
                Status = a.Status,
                Gender = a.Gender,
            }).Where(b => b.PkuserId == id).FirstOrDefaultAsync();
        }

        public async Task<int> Update(UserRequest model) 
        {
            try
            {
                Users oUser = await _context.Users.Where(x => x.PkuserId == model.PkuserId && x.Status == true).FirstOrDefaultAsync();
                if (oUser == null)
                {
                    return Utils.Constants.NOT_EXISTS;
                }
                if (await RepeatedEmail(model))
                {
                    return Utils.Constants.REPEATED_EMAIL;
                }
                if (await UserNameExists(model))
                {
                    return Utils.Constants.REPEATED_USERNAME;
                }

                oUser.Status = true;
                oUser.Email = model.Email;
                oUser.UserName = model.UserName;
                oUser.Password = Utils.Helper.GetSHA256(model.Password);
                oUser.Gender = model.Gender;
                _context.Entry(oUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Utils.Constants.SUCCESS_OPERATION;
            }
            catch (DbUpdateConcurrencyException)
            {
                return Utils.Constants.ERROR;
            }
        }

        public async Task<Users> Add(UserRequest model)
        {
            if (await EmailExists(model.Email)) return null;
            Users oUser = new Users();
            oUser.Email = model.Email;
            oUser.UserName = model.UserName;
            oUser.Password = model.Password;
            oUser.Status = true;
            oUser.Gender = model.Gender;
            DateTime date = DateTime.Now;
            oUser.CreatedAt = date;
            _context.Users.Add(oUser);
            await _context.SaveChangesAsync();
            return oUser;
        }
        public async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.PkuserId == id);
        }
        public async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(e => e.Email == email);
        }
        public async Task<bool> UserNameExists(UserRequest model)
        {
            return await _context.Users.AnyAsync(e => e.UserName == model.UserName && e.PkuserId != model.PkuserId);
        }
        private async Task<bool> RepeatedEmail(UserRequest model)
        {
            return await _context.Users.AnyAsync(e => e.Email == model.Email && e.PkuserId != model.PkuserId);
        }

        public async Task<Users> Delete(UserResponse model)
        {
            Users oUser = _context.Users.Find(model.PkuserId);
            oUser.Status = false;
            _context.Entry(oUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return oUser;
        }
    }
}
