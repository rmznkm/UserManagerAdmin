using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;
using UserAPI.Services;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices userServices;

        public UserController(IUserServices userServices)
        {
            this.userServices = userServices;
        }

        [HttpPost]
        public async Task<ActionResult<UserRegisterResponse>> RegisterAsync(UserRegisterRequest userRegisterRequest)
        {
            var result = await userServices.RegisterAsync(userRegisterRequest);
            return result;
        }
    }
}
