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
            carForAdding.Id = 0;
            await _carsRepository.AddCarAsync(carForAdding);
            Car carAfterAdd = (await _carsRepository.GetCarByIdAsync(carForAdding.Id))!;
            return Ok(carAfterAdd);
        }

        [HttpPost]
        [Route("{id}/update")]
        public async Task<IActionResult> UpdateCarAsync(Car carWithNewData, [FromRoute(Name = "id")] long id)
        {
            carWithNewData.Id = id;
            await _carsRepository.UpdateCarAsync(carWithNewData);
            Car carAfterUpdate = (await _carsRepository.GetCarByIdAsync(carWithNewData.Id))!;
            return Ok(carAfterUpdate);
        }

        [HttpPost]
        [Route("{id}/delete")]
        public async Task<IActionResult> DeleteCarAsync([FromRoute(Name = "id")] long id)
        {
            await _carsRepository.DeleteCarAsync(id);
            return Ok(new {Id = id});
        }
    }
}
