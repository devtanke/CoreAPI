using CoreAPI.Helpers;
using CoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DB = CoreAPI.Utils.Database.MySql;

namespace CoreAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        private List<User> _users = new List<User>();
        private readonly AppSetting _appSettings;

        public UserService(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            username = username.Normalize();
            password = password.Normalize();
            List<Dictionary<string, string>> records = DB.Query($"SELECT * FROM user WHERE name = '{username}'");

            if (records.Count == 0)
                return null;

            string hash = records[0]["hash"] ?? "";
            string salt = records[0]["salt"] ?? "";
            if (string.IsNullOrEmpty(salt))
                return null;

            string saltedPassword = DB.SaltedPassword(password, salt);
            if (string.IsNullOrEmpty(hash) || saltedPassword != hash)
                return null;

            User user = new User();
            int i = 0;
            int.TryParse(records[0]["id"], out i);
            user.Id = i;
            user.Username = records[0]["name"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id + "")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            foreach (User u in _users)
                if (u.Username == user.Username)
                    _users.Remove(u);

            _users.Add(user);

            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            return _users.WithoutPasswords();
        }
    }
}
