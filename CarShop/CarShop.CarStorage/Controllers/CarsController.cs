using System.Net.Mime;
using CarShop.CarStorage.Repositories;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.CarStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    public class CarsController(
        CarsRepository _carsRepository, CarEditProcessesRepository _carEditProcessesRepository) : ControllerBase
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

        [HttpPost]
        [Route("update-or-create-car-edit-process")]
        public async Task<IActionResult> UpdateOrCreateCarEditProcessAsync(CarEditProcess carEditProcess)
        {
            CarEditProcess? carEditProcessInDb = await _carEditProcessesRepository
                .GetCarEditProcessByAdminIdAndCarId(carEditProcess.AdminId, carEditProcess.CarId);
            
            if (carEditProcessInDb is null)
            {
                await _carEditProcessesRepository.AddCarEditProcessAsync(carEditProcess);
            }
            else
            {
                carEditProcess.Id = carEditProcessInDb.Id;
                await _carEditProcessesRepository.UpdateCarEditProcessAsync(carEditProcess);
            }
            
            return Ok(carEditProcess);
        }
    }
}
