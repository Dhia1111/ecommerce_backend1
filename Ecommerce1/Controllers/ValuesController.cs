using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce1.Controllers
{
    [Route("api/Ecommerce")]
    [ApiController]
    public class EcommerceController : ControllerBase
    {
     
        [HttpPost("SingUp", Name ="SignUp")]
        public async Task<ActionResult<bool>> SignUp()
        {
            await Task.Delay(3000);  
           
            return  Ok(true);

        }

    }
}
