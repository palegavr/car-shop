using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace CarShop.Web.ModelBuilders
{
	public class DoubleBinder : IModelBinder
	{
		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (valueProviderResult != ValueProviderResult.None)
			{
				string value = valueProviderResult.FirstValue!;

				value = value.Replace(',', '.');

				if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
				{
					bindingContext.Result = ModelBindingResult.Success(result);
					return;
				}
			}
			
			return;
		}
	}

	public class DoubleModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder? GetBinder(ModelBinderProviderContext context)
		{
			if (context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?))
			{
				return new BinderTypeModelBinder(typeof(DoubleBinder));
			}
			return null;
		}
	}


}
