using CarShop.CarStorage.Repositories;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
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
            carForAdding.Id = 0;
            await _carsRepository.AddCarAsync(carForAdding);
            return Ok(carForAdding);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCarAsync(
            [FromRoute(Name = "id")] long id,
            [FromBody] UpdateCarRequest updateCarRequest)
        {
            await _carsRepository.UpdateCarAsync(id, updateCarRequest);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCarAsync([FromRoute(Name = "id")] long id)
        {
            await _carsRepository.DeleteCarAsync(id);
            return Ok(new {Id = id});
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCarAsync([FromRoute(Name = "id")] long id)
        {
            Car? car = await _carsRepository.GetCarByIdAsync(id, withAvaliableOptions: true);
            return car is null ? NotFound() : Ok(car);
        }

        [HttpGet]
        public async Task<IActionResult> GetCarsAsync([FromQuery] GetCarsOptions getCarsOptions)
        {
            GetCarsResult result = await _carsRepository.GetCarsAsync(getCarsOptions);
            return Ok(result);
        }

        [HttpPost]
        [Route("addcarconfiguration")]
        public async Task<IActionResult> AddConfiguration([FromBody] CarConfiguration carConfiguration)
        {
            carConfiguration.Id = Guid.Empty;
            await _carsRepository.AddCarConfiguration(carConfiguration);
            return Ok(carConfiguration);
        }
    }
}
