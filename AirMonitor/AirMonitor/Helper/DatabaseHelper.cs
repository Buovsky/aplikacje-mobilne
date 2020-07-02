using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;
using AirMonitor.Models;
using AirMonitor.Models.TableClasses;
using System.Linq;
using Newtonsoft.Json;

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

        public IEnumerable<Installation> GetInstallations()
        {
            return _connect?.Table<InstallationEntity>().Select(s => new Installation(s)).ToList();
        }

        private Installation GetInstallation(string id)
        {
            var entity = _connect?.Get<InstallationEntity>(id);
            return new Installation(entity);
        }

        public IEnumerable<Measurement> GetMeasurements()
        {
            return _connect?.Table<MeasurementEntity>().Select(s =>
            {
                var measurementItem = GetMeasurementItem(s.CurrentMeasurementItemId);
                var installation = GetInstallation(s.InstallationId);
                return new Measurement(measurementItem, installation);
            }).ToList();
        }

        private MeasurementItem GetMeasurementItem(int id)
        {
            var entity = _connect?.Get<MeasurementItemEntity>(id);
            var valueIds = JsonConvert.DeserializeObject<int[]>(entity.MeasurementValueIds);
            var indexIds = JsonConvert.DeserializeObject<int[]>(entity.AirQualityIndexIds);
            var standardIds = JsonConvert.DeserializeObject<int[]>(entity.AirQualityStandardIds);
            var values = _connect?.Table<MeasurementValue>().Where(s => valueIds.Contains(s.Id)).ToArray();
            var indexes = _connect?.Table<AirQualityIndex>().Where(s => indexIds.Contains(s.Id)).ToArray();
            var standards = _connect?.Table<AirQualityStandard>().Where(s => standardIds.Contains(s.Id)).ToArray();
            return new MeasurementItem(entity, values, indexes, standards);
        }
    }
}
