using System.ComponentModel.DataAnnotations.Schema;
using CarShop.ServiceDefaults.ServiceInterfaces.ApiGateway;
using CarShop.ServiceDefaults.ServiceInterfaces.Web;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

[Table("CarEditProcesses")]
public class CarEditProcess
{
    public long Id { get; set; }
    public long AdminId { get; set; }
    public long CarId { get; set; }
    public CarEditProcessData Process { get; set; }
}