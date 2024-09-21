using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.CarStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class CarsController : ControllerBase
    {


        [HttpPost]
        public IActionResult AddCar(
            [FromBody] Car carForAdding)
        {
            return Ok();
        }
    }
}
