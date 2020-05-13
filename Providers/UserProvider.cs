using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VueExample.Contexts;
using VueExample.Helpers;
using VueExample.Models;
using VueExample.ResponseObjects;

namespace VueExample.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly AppSettings _appSettings;
        private readonly List<User> _usersList;

        public UserProvider(IOptions<AppSettings> appSettings, Srv6Context srv6Context)
        {
            _appSettings = appSettings.Value;
            _usersList = GetUsers();
        }
        
        public User Authenticate(User user)
        {
            var currentUser = _usersList.SingleOrDefault(x => x.Username == user.Username && x.Password == user.Password);

          
            if (currentUser == null)
                return null;

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, currentUser.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            currentUser.Token = tokenHandler.WriteToken(token);
            currentUser.Password = null;

            return currentUser;
        }

        public List<User> GetAll()
        {
            return _usersList;
        }

        public Error IsExistUserDuplicate(User user)
        {
            using (var visualControlContext = new VisualControlContext())
            {
                if (visualControlContext.Users.Any(x => x.Username == user.Username))
                {
                    return new Error("Пользователь с таким логином уже существует", "RE001");
                }

                if (visualControlContext.Users.Any(x => x.Email == user.Email))
                {
                    return new Error("Пользователь с таким почтовым ящиком уже существует", "RE002");
                }
            }

            return null;
        }

       
        public User RegistryUser(User user)
        {
            using (var visualControlContext = new VisualControlContext())
            {
               visualControlContext.Users.Add(user);
               visualControlContext.SaveChanges();
            }

            return user;
        }

        private List<User> GetUsers()
        {
            using (var visualControlContext = new VisualControlContext())
            {
                return visualControlContext.Users.ToList();
            }
        }
    }
}
