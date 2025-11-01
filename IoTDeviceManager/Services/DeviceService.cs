using System;
using System.Collections.Generic;
using System.Linq;
using IoTDeviceManager.Models;

namespace IoTDeviceManager.Services
{
    /// <summary>
    /// Service for managing IoT device data and operations
    /// </summary>
    public class DeviceService
    {
        private List<Device> _devices;

        public DeviceService()
        {
            _devices = new List<Device>();
            InitializeMockData();
        }

        /// <summary>
        /// Initialize mock/seed data for testing
        /// </summary>
        private void InitializeMockData()
        {
            _devices = new List<Device>
            {
                new Device
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Temperature",
                    Type = "Sensor",
                    IpAddress = "192.168.1.101",
                    IsOnline = true,
                    LastSeen = DateTime.Now,
                    FirmwareVersion = "v1.2.3",
                    Units = "°C",
                    Location = "Production Floor A"
                },
                new Device
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Humidity",
                    Type = "Sensor",
                    IpAddress = "192.168.1.102",
                    IsOnline = true,
                    LastSeen = DateTime.Now.AddMinutes(-5),
                    FirmwareVersion = "v1.2.1",
                    Units = "%",
                    Location = "Production Floor B"
                },
                new Device
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Sn Actuator",
                    Type = "Actuator",
                    IpAddress = "192.168.1.201",
                    IsOnline = false,
                    LastSeen = DateTime.Now.AddHours(-2),
                    FirmwareVersion = "v2.0.0",
                    Units = "N/A",
                    Location = "Assembly Line 1"
                },
                new Device
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Ga Gateway",
                    Type = "Gateway",
                    IpAddress = "192.168.1.1",
                    IsOnline = true,
                    LastSeen = DateTime.Now.AddSeconds(-30),
                    FirmwareVersion = "v3.1.0",
                    Units = "N/A",
                    Location = "Server Room"
                },
                new Device
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Pr Sensor",
                    Type = "Sensor",
                    IpAddress = "192.168.1.103",
                    IsOnline = false,
                    LastSeen = DateTime.Now.AddDays(-1),
                    FirmwareVersion = "v1.0.5",
                    Units = "kPa",
                    Location = "Quality Control Lab"
                }
            };
        }

        public List<Device> GetAllDevices()
        {
            return new List<Device>(_devices);
        }

        public Device GetDeviceById(string id)
        {
            return _devices.FirstOrDefault(d => d.Id == id);
        }

        public bool AddDevice(Device device)
        {
            if (device == null || string.IsNullOrEmpty(device.Name))
                return false;

            device.Id = Guid.NewGuid().ToString();
            device.LastSeen = DateTime.Now;
            _devices.Add(device);
            return true;
        }

        public bool UpdateDevice(Device updatedDevice)
        {
            var existingDevice = _devices.FirstOrDefault(d => d.Id == updatedDevice.Id);
            if (existingDevice == null)
                return false;

            existingDevice.Name = updatedDevice.Name;
            existingDevice.Type = updatedDevice.Type;
            existingDevice.IpAddress = updatedDevice.IpAddress;
            existingDevice.FirmwareVersion = updatedDevice.FirmwareVersion;
            existingDevice.Units = updatedDevice.Units;
            existingDevice.Location = updatedDevice.Location;
            existingDevice.IsOnline = updatedDevice.IsOnline;
            existingDevice.LastSeen = DateTime.Now;

            return true;
        }

        public bool DeleteDevice(string id)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return false;

            _devices.Remove(device);
            return true;
        }

        public bool ToggleDeviceStatus(string id)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return false;

            device.IsOnline = !device.IsOnline;
            device.LastSeen = DateTime.Now;
            return true;
        }
    }
}