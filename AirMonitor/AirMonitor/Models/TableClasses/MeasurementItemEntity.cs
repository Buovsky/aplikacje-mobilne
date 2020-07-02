using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace AirMonitor.Models.TableClasses
{
    class MeasurementItemEntity
    {
        public MeasurementItemEntity(MeasurementItem measurementItem)
        {
            if (measurementItem == null) return;

            FromDateTime = measurementItem.FromDateTime;
            TillDateTime = measurementItem.TillDateTime;
        }

        public int Id { get; set; }
        public string MeasurementValueIds { get; set; }
        public string AirQualityIndexIds { get; set; }
        public string AirQualityStandardIds { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
    }
}
