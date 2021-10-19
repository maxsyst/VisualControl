using System.Collections.Generic;
using VueExample.Models;
using VueExample.ResponseObjects;

namespace VueExample.Providers
{
    public interface IUserProvider
    {
        List<User> GetAll();
        User Authenticate(User user);
        Error IsExistUserDuplicate(User user);
        User RegistryUser(User user);
    }
}
