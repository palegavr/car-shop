using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.Web.ModelBuilders;

namespace CarShop.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllersWithViews(options =>
        {
			options.ModelBinderProviders.Insert(0, new DoubleModelBinderProvider());
		});

        builder.Services.AddHttpClient<CarStorageClient>(CarStorageClient.ConfigureClient);

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        /*if (!app.Environment.IsDevelopment() || true)
        {
            app.UseExceptionHandler("/Error/NotFound404");
			app.UseStatusCodePagesWithReExecute("/Error/{0}");

			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
        }*/

        //app.UseHttpsRedirection();
		app.UseStatusCodePagesWithReExecute("/Error/{0}");
		app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}");


		app.Run();
    }
}
