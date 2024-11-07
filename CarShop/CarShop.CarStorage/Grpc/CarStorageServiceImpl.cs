using System.ComponentModel.DataAnnotations;
using CarShop.CarStorage.Extensions;
using CarShop.CarStorage.Repositories;
using CarShop.CarStorage.Repositories.CarsRepository;
using CarShop.CarStorageService.Grpc;
using Grpc.Core;
using Car = CarShop.CarStorage.Database.Entities.Car.Car;
using CarConfiguration = CarShop.CarStorage.Database.Entities.CarConfiguration;
using GetCarEditProcessRequest = CarShop.CarStorageService.Grpc.GetCarEditProcessRequest;
using UpdateCarRequest = CarShop.CarStorageService.Grpc.UpdateCarRequest;

namespace CarShop.CarStorage.Grpc;

public class CarStorageServiceImpl(
    CarsRepository _carsRepository, 
    CarEditProcessesRepository _carEditProcessesRepository,
    CarConfigurationsRepository _carConfigurationsRepository) : CarStorageService.Grpc.CarStorageService.CarStorageServiceBase
{
    public override async Task<AddCarReply> AddCar(AddCarRequest request, ServerCallContext context)
    {
        Car carForAdding = request.Car.FromGrpcMessage();
        carForAdding.Id = 0;
        try
        {
            await _carsRepository.AddCarAsync(carForAdding);
        }
        catch (ValidationException e)
        {
            return new AddCarReply
            {
                Result = AddCarReply.Types.AddCarResult.BadRequest
            };
        }
        
        return new AddCarReply
        {
            Car = carForAdding.ToGrpcMessage(),
            Result = AddCarReply.Types.AddCarResult.Success
        };
    }

    public override async Task<UpdateCarReply> UpdateCar(UpdateCarRequest request, ServerCallContext context)
    {
        try
        {
            await _carsRepository.UpdateCarAsync(request.CarId, new()
            {
                Brand = request.HasBrand ? request.Brand : null,
                Model = request.HasModel ? request.Model : null,
                Color = request.HasColor ? request.Color : null,
                Count = request.HasCount ? request.Count : null,
                CorpusType = request.HasCorpusType ? request.CorpusType.FromGrpcMessage() : null,
                FuelType = request.HasFuelType ? request.FuelType.FromGrpcMessage() : null,
                EngineCapacity = request.HasEngineCapacity ? request.EngineCapacity : null,
                ImageUrl = request.HasImageUrl ? request.ImageUrl : null,
                PriceForStandartConfiguration = request.HasPriceForStandartConfiguration
                    ? request.PriceForStandartConfiguration
                    : null,
                BigImageURLs = request.UpdateBigImageUrls ? request.BigImageUrls.ToArray() : null,
                AdditionalCarOptions = request.UpdateAdditionalCarOptions
                    ? request.AdditionalCarOptions.FromGrpcMessage().ToArray()
                    : null,
            });
        }
        catch (ValidationException)
        {
            return new UpdateCarReply
            {
                Result = UpdateCarReply.Types.UpdateCarResult.BadRequest
            };
        }

        return new UpdateCarReply
        {
            Result = UpdateCarReply.Types.UpdateCarResult.Success
        };
    }

    public override async Task<DeleteCarReply> DeleteCar(DeleteCarRequest request, ServerCallContext context)
    {
        if (await _carsRepository.GetCarByIdAsync(request.CarId) is null)
        {
            return new DeleteCarReply
            {
                Result = DeleteCarReply.Types.DeleteCarResult.CarNotFound
            };
        }
        
        await _carsRepository.DeleteCarAsync(request.CarId);
        return new DeleteCarReply
        {
            Result = DeleteCarReply.Types.DeleteCarResult.Success
        };
    }

    public override async Task<GetCarReply> GetCar(GetCarRequest request, ServerCallContext context)
    {
        Car? car = await _carsRepository.GetCarByIdAsync(request.CarId);
        if (car is null)
        {
            return new GetCarReply
            {
                Result = GetCarReply.Types.GetCarResult.CarNotFound
            };
        }

        return new GetCarReply
        {
            Car = car.ToGrpcMessage(),
            Result = GetCarReply.Types.GetCarResult.Success
        };
    }

    public override async Task<GetCarsReply> GetCars(GetCarsRequest request, ServerCallContext context)
    {
        var result = await _carsRepository.GetCarsAsync(new GetCarsOptions
        {
            Brand = request.HasBrand ? request.Brand : null,
            CorpusType = request.HasCorpusType ? request.CorpusType.FromGrpcMessage() : null,
            FuelType = request.HasFuelType ? request.FuelType.FromGrpcMessage() : null,
            MaximumPrice = request.HasMaximumPrice ? request.MaximumPrice : null,
            MinimumPrice = request.HasMinimumPrice ? request.MinimumPrice : null,
            MinimumEngineCapacity = request.HasMinimumEngineCapacity ? request.MinimumEngineCapacity : null,
            MaximumEngineCapacity = request.HasMaximumEngineCapacity ? request.MaximumEngineCapacity : null,
            EndIndex = request.HasEndIndex ? request.EndIndex : null,
            StartIndex = request.HasStartIndex ? request.StartIndex : null,
            SortBy = request.HasSortBy ? request.SortBy.FromGrpcMessage() : null,
            SortType = request.HasSortType ? request.SortType.FromGrpcMessage() : null,
        });

        return new GetCarsReply
        {
            Cars = { result.Cars.Select(car => car.ToGrpcMessage()) },
            TotalResultsCount = result.TotalResultsCount,
        };
    }

    public override async Task<AddCarConfigurationReply> AddCarConfiguration(AddCarConfigurationRequest request, ServerCallContext context)
    {
        CarConfiguration carConfiguration = request.CarConfiguration.FromGrpcMessage();
        carConfiguration.Id = Guid.Empty;

        Car? car = await _carsRepository.GetCarByIdAsync(carConfiguration.CarId);
        if (car is null)
        {
            return new AddCarConfigurationReply
            {
                Result = AddCarConfigurationReply.Types.AddCarConfigurationResult.CarNotFound
            };
        }

        if (!carConfiguration.IsCorrectBy(car.AdditionalCarOptions))
        {
            return new AddCarConfigurationReply
            {
                Result = AddCarConfigurationReply.Types.AddCarConfigurationResult.CarConfigurationHaveUnavailableOptions
            };
        }
        
        await _carConfigurationsRepository.AddAsync(carConfiguration);
        return new AddCarConfigurationReply
        {
            CarConfiguration = carConfiguration.ToGrpcMessage(),
            Result = AddCarConfigurationReply.Types.AddCarConfigurationResult.Success
        };
    }

    public override async Task<UpdateCarConfigurationReply> UpdateCarConfiguration(UpdateCarConfigurationRequest request, ServerCallContext context)
    {
        CarConfiguration carConfiguration = request.CarConfiguration.FromGrpcMessage();
        if (await _carConfigurationsRepository.GetByIdAsync(carConfiguration.Id) is null)
        {
            return new UpdateCarConfigurationReply
            {
                Result = UpdateCarConfigurationReply.Types.UpdateCarConfigurationResult.CarConfigurationNotFound
            };
        }
        
        await _carConfigurationsRepository.UpdateAsync(carConfiguration);
        return new UpdateCarConfigurationReply
        {
            CarConfiguration = carConfiguration.ToGrpcMessage(),
            Result = UpdateCarConfigurationReply.Types.UpdateCarConfigurationResult.Success
        };
    }

    public override async Task<GetCarConfigurationReply> GetCarConfiguration(GetCarConfigurationRequest request, ServerCallContext context)
    {
        CarConfiguration? carConfiguration = await _carConfigurationsRepository
            .GetByIdAsync(Guid.Parse(request.CarConfigurationId));

        if (carConfiguration is null)
        {
            return new GetCarConfigurationReply
            {
                Result = GetCarConfigurationReply.Types.GetCarConfigurationResult.CarConfigurationNotFound
            };
        }

        return new GetCarConfigurationReply
        {
            CarConfiguration = carConfiguration.ToGrpcMessage(),
            Result = GetCarConfigurationReply.Types.GetCarConfigurationResult.Success
        };
    }

    public override async Task<GetCarConfigurationsReply> GetCarConfigurations(GetCarConfigurationsRequest request, ServerCallContext context)
    {
        var carConfigurations =
            await _carConfigurationsRepository.GetForCarAsync(request.CarId);

        return new GetCarConfigurationsReply
        {
            CarConfiguration = { carConfigurations.Select(c => c.ToGrpcMessage()) },
        };
    }

    public override async Task<GetCarEditProcessReply> GetCarEditProcess(GetCarEditProcessRequest request, ServerCallContext context)
    {
        var carEditProcess = await _carEditProcessesRepository
            .GetByAdminIdAndCarIdAsync(request.AdminId, request.CarId);

        if (carEditProcess is null)
        {
            return new GetCarEditProcessReply
            {
                Result = GetCarEditProcessReply.Types.GetCarEditProcessResult.NotFound
            };
        }

        return new GetCarEditProcessReply
        {
            CarEditProcess = carEditProcess.ToGrpcMessage(),
            Result = GetCarEditProcessReply.Types.GetCarEditProcessResult.Success
        };
    }

    public override async Task<DeleteCarEditProcessReply> DeleteCarEditProcess(DeleteCarEditProcessRequest request, ServerCallContext context)
    {
        var carEditProcess = await _carEditProcessesRepository
            .GetByAdminIdAndCarIdAsync(request.AdminId, request.CarId);

        if (carEditProcess is null)
        {
            return new DeleteCarEditProcessReply
            {
                Result = DeleteCarEditProcessReply.Types.DeleteCarEditProcessResult.NotFound
            };
        }

        await _carEditProcessesRepository.DeleteAsync(carEditProcess.Id);
        return new DeleteCarEditProcessReply
        {
            Result = DeleteCarEditProcessReply.Types.DeleteCarEditProcessResult.Success,
        };
    }

    public override async Task<UpdateOrCreateCarEditProcessReply> UpdateOrCreateCarEditProcess(UpdateOrCreateCarEditProcessRequest request, ServerCallContext context)
    {
        var carEditProcessInDb = await _carEditProcessesRepository
            .GetByAdminIdAndCarIdAsync(request.CarEditProcess.AdminId, request.CarEditProcess.CarId);
        
        var carEditProcess = request.CarEditProcess.FromGrpcMessage();
        try
        {
            if (carEditProcessInDb is null)
            {
                if (await _carsRepository.GetCarByIdAsync(carEditProcess.CarId) is null)
                {
                    return new UpdateOrCreateCarEditProcessReply
                    {
                        Result = UpdateOrCreateCarEditProcessReply.Types.UpdateOrCreateCarEditProcessResult.CarNotFound
                    };
                }

                await _carEditProcessesRepository.AddAsync(carEditProcess);
            }
            else
            {
                carEditProcess.Id = carEditProcessInDb.Id;
                await _carEditProcessesRepository.UpdateAsync(carEditProcess);
            }
        }
        catch (ValidationException)
        {
            return new UpdateOrCreateCarEditProcessReply
            {
                Result = UpdateOrCreateCarEditProcessReply.Types.UpdateOrCreateCarEditProcessResult.BadRequest
            };
        }
        

        return new UpdateOrCreateCarEditProcessReply
        {
            CarEditProcess = carEditProcess.ToGrpcMessage(),
            Result = UpdateOrCreateCarEditProcessReply.Types.UpdateOrCreateCarEditProcessResult.Success
        };
    }
}