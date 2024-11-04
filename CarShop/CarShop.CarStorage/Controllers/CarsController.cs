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
        CarsRepository _carsRepository, CarEditProcessesRepository _carEditProcessesRepository,
        AdditionalCarOptionsRepository _additionalCarOptionsRepository,
        CarConfigurationsRepository _carConfigurationsRepository) : ControllerBase
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
            List<AdditionalCarOption> additionalCarOptions = updateCarRequest.AdditionalCarOptions;
            if (additionalCarOptions.Count > 0 && additionalCarOptions[0].Id < 0)
            {
                return BadRequest();
            }
            
            var additionalCarOptionsInDb = await _additionalCarOptionsRepository.GetAdditionalCarOptionsForCar(id);
            
            foreach (AdditionalCarOption additionalCarOption in additionalCarOptions)
            {
                AdditionalCarOption? optionInDb = additionalCarOptionsInDb
                    .SingleOrDefault(o => o.Type == additionalCarOption.Type);

                if (optionInDb is not null)
                {
                    optionInDb.Type = additionalCarOption.Type;
                    optionInDb.Price = additionalCarOption.Price;
                    optionInDb.IsRequired = additionalCarOption.IsRequired;
                    await _additionalCarOptionsRepository.UpdateAdditionalCarOptionAsync(optionInDb);
                }
                else
                {
                    additionalCarOption.CarId = id;
                    await _additionalCarOptionsRepository.AddAdditionalCarOptionAsync(additionalCarOption);
                }
            }
            
            foreach (var optionInDb in additionalCarOptionsInDb)
            {
                AdditionalCarOption? option = additionalCarOptions
                    .SingleOrDefault(o => o.Type == optionInDb.Type);

                if (option is null)
                {
                    await _additionalCarOptionsRepository.DeleteAdditionalCarOptionAsync(optionInDb.Id);
                }
            }
            
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCarAsync([FromRoute(Name = "id")] long id)
        {
            await _carsRepository.DeleteCarAsync(id);
            return Ok();
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
            await _carConfigurationsRepository.AddCarConfiguration(carConfiguration);
            return Ok(carConfiguration);
        }
        
        [HttpPut]
        [Route("update-car-configuration")]
        public async Task<IActionResult> UpdateCarConfiguration([FromBody] CarConfiguration carConfiguration)
        {
            await _carConfigurationsRepository.UpdateCarConfigurationAsync(carConfiguration);
            return Ok(carConfiguration);
        }

        [HttpGet]
        [Route("{id}/car-configurations")]
        public async Task<IActionResult> GetCarConfigurationsOfCar([FromRoute(Name = "id")] long id)
        {
            return Ok(await _carConfigurationsRepository.GetCarConfigurationsOfCar(id));
        }

        [HttpPost]
        [Route("update-or-create-car-edit-process")]
        public async Task<IActionResult> UpdateOrCreateCarEditProcessAsync([FromBody] CarEditProcess carEditProcess)
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

        [HttpPost]
        [Route("get-car-edit-process")]
        public async Task<IActionResult> GetCarEditProcessAsync([FromBody] GetCarEditProcessRequest getCarEditProcessRequest)
        {
            CarEditProcess? carEditProcess = await _carEditProcessesRepository
                .GetCarEditProcessByAdminIdAndCarId(getCarEditProcessRequest.AdminId, getCarEditProcessRequest.CarId);

            if (carEditProcess is null)
            {
                return NotFound();
            }
            
            return Ok(carEditProcess);
        }

        [HttpPost]
        [Route("delete-car-edit-process")]
        public async Task<IActionResult> DeleteCarEditProcessAsync([FromBody] DeleteEditProcessRequest deleteEditProcessRequest)
        {
            CarEditProcess? carEditProcess = await _carEditProcessesRepository
                .GetCarEditProcessByAdminIdAndCarId(deleteEditProcessRequest.AdminId, deleteEditProcessRequest.CarId);
            
            if (carEditProcess is null)
            {
                return NotFound();
            }

            await _carEditProcessesRepository.DeleteCarEditProcessAsync(carEditProcess.Id);
            return Ok();
        }
    }
}
