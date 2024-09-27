using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class CarStorageClient(HttpClient _httpClient)
    {
        public const string BASE_ADDRESS = "http://localhost:5253";

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
			UriBuilder uriBuilder = new UriBuilder(new Uri(new Uri(BASE_ADDRESS), "api/cars"));
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (getCarsOptions is not null)
            {
				if (getCarsOptions.StartIndex is not null)
					query[nameof(getCarsOptions.StartIndex)] = getCarsOptions.StartIndex.ToString();

				if (getCarsOptions.EndIndex is not null)
					query[nameof(getCarsOptions.EndIndex)] = getCarsOptions.EndIndex.ToString();

				if (!string.IsNullOrEmpty(getCarsOptions.Brand))
					query[nameof(getCarsOptions.Brand)] = getCarsOptions.Brand;

				if (getCarsOptions.MinimumEngineCapacity is not null)
					query[nameof(getCarsOptions.MinimumEngineCapacity)] = getCarsOptions.MinimumEngineCapacity?.ToString(CultureInfo.InvariantCulture);

				if (getCarsOptions.MaximumEngineCapacity is not null)
					query[nameof(getCarsOptions.MaximumEngineCapacity)] = getCarsOptions.MaximumEngineCapacity?.ToString(CultureInfo.InvariantCulture);

				if (getCarsOptions.FuelType is not null)
					query[nameof(getCarsOptions.FuelType)] = getCarsOptions.FuelType.ToString();

				if (getCarsOptions.CorpusType is not null)
					query[nameof(getCarsOptions.CorpusType)] = getCarsOptions.CorpusType.ToString();

				if (getCarsOptions.MinimumPrice is not null)
					query[nameof(getCarsOptions.MinimumPrice)] = getCarsOptions.MinimumPrice?.ToString(CultureInfo.InvariantCulture);

				if (getCarsOptions.MaximumPrice is not null)
					query[nameof(getCarsOptions.MaximumPrice)] = getCarsOptions.MaximumPrice?.ToString(CultureInfo.InvariantCulture);

				if (getCarsOptions.SortType is not null)
					query[nameof(getCarsOptions.SortType)] = getCarsOptions.SortType.ToString();

				if (getCarsOptions.SortBy is not null)
					query[nameof(getCarsOptions.SortBy)] = getCarsOptions.SortBy.ToString();
			}

			uriBuilder.Query = query.ToString();
			GetCarsResult result = (await _httpClient.GetFromJsonAsync<GetCarsResult>(uriBuilder.ToString(), cancellationToken))!;
            return result;
        }

        public static void ConfigureClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(BASE_ADDRESS);
        }
    }
}
