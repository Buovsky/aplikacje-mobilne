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

        public MeasurementEntity(int currentMeasurementItemId, string installationId)
        {
            CurrentMeasurementItemId = currentMeasurementItemId;
            InstallationId = installationId;
        }

        public int Id { get; set; }
        public int CurrentMeasurementId { get; set; }
        public int CurrentMeasurementItemId { get; }
        public string InstallationId { get; set; }

    }
}
