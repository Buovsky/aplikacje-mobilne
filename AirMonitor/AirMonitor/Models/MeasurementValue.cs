using SQLite;
namespace AirMonitor.Models
{
    public class MeasurementValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}