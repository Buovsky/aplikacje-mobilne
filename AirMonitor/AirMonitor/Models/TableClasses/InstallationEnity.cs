using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace AirMonitor.Models.TableClasses
{
    class InstallationEnity
    {
        public InstallationEnity()
        {
        }

        public string Id { get; set; }
        public string LocationString { get; set; }
        public string AddressString { get; set; }
        public double Elevation { get; set; }
        public bool IsAirlyInstallation { get; set; }
    }
}
