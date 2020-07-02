using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace AirMonitor.Models.TableClasses
{
    public class InstallationEntity
    {
        public InstallationEntity()
        {
        }

        public string Id { get; set; }
        public string LocationString { get; set; }
        public string AddressString { get; set; }
        public double Elevation { get; set; }
        public bool IsAirlyInstallation { get; set; }
    }
}
