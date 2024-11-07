using System.ComponentModel.DataAnnotations.Schema;

namespace CarShop.CarStorage.Database.Entities.CarEditProcess;

[Table("CarEditProcesses")]
public class CarEditProcess
{
    public long Id { get; set; }
    public long AdminId { get; set; }
    public long CarId { get; set; }
    public CarEditProcessData Process { get; set; }
}