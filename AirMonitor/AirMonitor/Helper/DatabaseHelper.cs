using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;
using AirMonitor.Models;
using AirMonitor.Models.TableClasses;

namespace AirMonitor.Helper
{
    public partial class DatabaseHelper
    {
        private SQLiteConnection _connect;
        public void Initialize()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "dataBase.db");

            _connect = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);


            _connect.CreateTable<InstallationEntity>();
            _connect.CreateTable<MeasurementEntity>();
            _connect.CreateTable<MeasurementItemEntity>();
            _connect.CreateTable<MeasurementValue>();
            _connect.CreateTable<AirQualityIndex>();
            _connect.CreateTable<AirQualityStandard>();
        }

        public void SaveMeasurements(IEnumerable<Measurement> measurements)
        {
            _connect?.RunInTransaction(() =>
            {
                _connect?.DeleteAll<MeasurementValue>();
                _connect?.DeleteAll<AirQualityIndex>();
                _connect?.DeleteAll<AirQualityStandard>();
                _connect?.DeleteAll<MeasurementItemEntity>();
                _connect?.DeleteAll<MeasurementEntity>();


                foreach (var measurement in measurements)
                {
                    _connect?.InsertAll(measurement.Current.Values, false);
                    _connect?.InsertAll(measurement.Current.Indexes, false);
                    _connect?.InsertAll(measurement.Current.Standards, false);

                    var measurementItemEntity = new MeasurementItemEntity(measurement.Current);
                    _connect?.Insert(measurementItemEntity);

                    var measurementEntity = new MeasurementEntity(measurementItemEntity.Id, measurement.Installation.Id);
                    _connect?.Insert(measurementEntity);
                }
            });
        }
    }
}
