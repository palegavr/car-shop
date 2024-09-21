using CarShop.CarStorage.Repositories;
using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.CarStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class CarsController(CarsRepository _carsRepository) : ControllerBase
    {


        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddCarAsync(
            [FromBody] Car carForAdding)
        {
            await _carsRepository.AddCarAsync(carForAdding);
            return Ok();
        }
    }
}
