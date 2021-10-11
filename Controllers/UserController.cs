using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VueExample.Models;
using VueExample.Providers;
using VueExample.ResponseObjects;
using VueExample.ViewModels;

namespace VueExample.Controllers
{
    [Authorize]
    [Route("auth/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public UserController(IUserProvider userProvider, IMapper mapper)
        {
            _userProvider = userProvider;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserViewModel userViewModel)
        {
            var currentUser = _userProvider.Authenticate(_mapper.Map<User>(userViewModel));

            if (currentUser == null)
                return BadRequest(new StandardResponseObject { Message = "Пароль или имя пользователя не совпадают" });

            return Ok(new StandardResponseObject<User> { Body = currentUser, Message = "Успешная авторизация" });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registry([FromBody] RegistryViewModel registryViewModel)
        {
            var user = _mapper.Map<User>(registryViewModel);

            var duplicateError = _userProvider.IsExistUserDuplicate(user);
            if (duplicateError == null)
            {
                var currentUser = _userProvider.RegistryUser(user);
                if (currentUser.Id == 0)
                    return BadRequest(new StandardResponseObject { Message = "Ошибка при регистрации пользователя" });

                return Ok(new StandardResponseObject<User> { Body = currentUser, Message = "Успешная регистрация" });
            }
            else
            {
                return BadRequest(new StandardResponseObject { Message = duplicateError.Message });
            }
        }
    }
}

