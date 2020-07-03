using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace AirMonitor.Models.TableClasses
{
    public class MeasurementEntity
    {
        public MeasurementEntity()
        {
        }

        public MeasurementEntity(int currentMeasurementItemId, string installationId)
        {
            CurrentMeasurementItemId = currentMeasurementItemId;
            InstallationId = installationId;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        //public int CurrentMeasurementId { get; set; }
        public int CurrentMeasurementItemId { get; }
        public string InstallationId { get; set; }

    }
}
