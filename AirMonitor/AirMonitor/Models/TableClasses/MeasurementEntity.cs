using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace AirMonitor.Models.TableClasses
{
    class MeasurementEntity
    {
        public MeasurementEntity()
        {
        }

        public int Id { get; set; }
        public int CurrentMeasurementId { get; set; }
        public string InstallationId { get; set; }

    }
}
