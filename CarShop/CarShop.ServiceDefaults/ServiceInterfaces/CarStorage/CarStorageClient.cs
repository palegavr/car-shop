using CarShop.ServiceDefaults.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class CarStorageClient(HttpClient _httpClient)
    {

        public async Task<Car?> GetCarAsync(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                Car? car = await _httpClient.GetFromJsonAsync<Car>($"/api/cars/{id}", cancellationToken);
                return car;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                } 
                else
                {
                    throw;
                }
            }
        }

        public async Task<GetCarsResult> GetCarsAsync(GetCarsOptions? getCarsOptions = null, CancellationToken cancellationToken = default)
        {
            GetCarsResult result = (await _httpClient.GetFromJsonAsync<GetCarsResult>("/api/cars", cancellationToken))!;
            return result;
        }

        public static void ConfigureClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://localhost:5253");
        }
    }
}
