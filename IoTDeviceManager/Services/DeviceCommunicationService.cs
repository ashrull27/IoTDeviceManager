using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IoTDeviceManager.Models;

namespace IoTDeviceManager.Services
{
    /// <summary>
    /// Simulates real-time communication with IoT devices
    /// In production: would use MQTT, REST API, WebSockets, or serial communication
    /// </summary>
    public class DeviceCommunicationService
    {
        private Timer _simulationTimer;
        private Random _random;
        private bool _isRunning;
        private DeviceService _deviceService;

        public event EventHandler<DeviceDataReceivedEventArgs> DataReceived;
        public event EventHandler<DeviceErrorEventArgs> ConnectionError;

        public DeviceCommunicationService()
        {
            _random = new Random();
            _isRunning = false;
        }

        /// <summary>
        /// Set the device service to check device status
        /// </summary>
        public void SetDeviceService(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Start simulating device communication
        /// </summary>
        public void StartCommunication()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            // Simulate receiving data every 3 seconds
            _simulationTimer = new Timer(SimulateDataReceived, null, 1000, 3000);
        }

        /// <summary>
        /// Stop device communication simulation
        /// </summary>
        public void StopCommunication()
        {
            _isRunning = false;
            _simulationTimer?.Dispose();
        }

        /// <summary>
        /// Simulate receiving data from a device
        /// </summary>
        private void SimulateDataReceived(object state)
        {
            try
            {
                // Get list of online devices only
                if (_deviceService != null)
                {
                    var onlineDevices = _deviceService.GetAllDevices()
                        .Where(d => d.IsOnline)
                        .ToList();

                    // If no devices are online, don't generate data
                    if (onlineDevices.Count == 0)
                    {
                        return;
                    }

                    // Randomly select an online device
                    var selectedDevice = onlineDevices[_random.Next(onlineDevices.Count)];

                    // Simulate random connection failures (10% chance)
                    if (_random.Next(100) < 10)
                    {
                        OnConnectionError(new DeviceErrorEventArgs
                        {
                            DeviceId = selectedDevice.Id,
                            ErrorMessage = "Connection timeout - device not responding",
                            Timestamp = DateTime.Now
                        });
                        return;
                    }

                    // Simulate receiving sensor data from the selected online device
                    var dataType = GetDataTypeForDevice(selectedDevice);
                    var sensorData = new DeviceDataReceivedEventArgs
                    {
                        DeviceId = selectedDevice.Id,
                        DeviceName = selectedDevice.Name,
                        DataType = dataType,
                        Value = GenerateValueForDataType(dataType),
                        Timestamp = DateTime.Now,
                        Unit = GetUnitForDataType(dataType)  // Use appropriate unit for data type
                    };

                    OnDataReceived(sensorData);
                }
                else
                {
                    // Fallback to old simulation if service not set
                    if (_random.Next(100) < 10)
                    {
                        OnConnectionError(new DeviceErrorEventArgs
                        {
                            DeviceId = "SIMULATED_DEVICE",
                            ErrorMessage = "Connection timeout - device not responding",
                            Timestamp = DateTime.Now
                        });
                        return;
                    }

                    var sensorData = new DeviceDataReceivedEventArgs
                    {
                        DeviceId = $"SENSOR_{_random.Next(1, 6):D2}",
                        DataType = GetRandomDataType(),
                        Value = GenerateRandomValue(),
                        Timestamp = DateTime.Now,
                        Unit = GetUnit()
                    };

                    OnDataReceived(sensorData);
                }
            }
            catch (Exception ex)
            {
                OnConnectionError(new DeviceErrorEventArgs
                {
                    DeviceId = "UNKNOWN",
                    ErrorMessage = $"Communication error: {ex.Message}",
                    Timestamp = DateTime.Now
                });
            }
        }

        private string GetRandomDataType()
        {
            string[] types = { "Temperature", "Humidity", "Pressure", "Vibration", "Status" };
            return types[_random.Next(types.Length)];
        }

        /// <summary>
        /// Get appropriate data type based on device type and name
        /// </summary>
        private string GetDataTypeForDevice(Device device)
        {
            string deviceName = device.Name.ToLower();

            switch (device.Type?.ToLower())
            {
                case "sensor":
                    // Match exact device name for sensors
                    if (deviceName == "temperature" || deviceName.Contains("temp"))
                        return "Temperature";
                    else if (deviceName == "humidity" || deviceName.Contains("humid"))
                        return "Humidity";
                    else if (deviceName.Contains("pr") || deviceName.Contains("press"))
                        return "Pressure";
                    else
                        // Generic sensor - random measurement
                        return new[] { "Temperature", "Humidity", "Pressure", "Vibration" }[_random.Next(4)];

                case "actuator":
                    // Actuators report status or action performed
                    return new[] { "Status", "Position", "Speed" }[_random.Next(3)];

                case "gateway":
                    // Gateways report connection and throughput stats (no Connected Devices)
                    return new[] { "Status", "Throughput" }[_random.Next(2)];

                case "controller":
                    // Controllers report control parameters
                    return new[] { "Status", "Output", "Setpoint" }[_random.Next(3)];

                case "monitor":
                    // Monitors can track various metrics
                    return new[] { "Status", "Level", "Count" }[_random.Next(3)];

                default:
                    return GetRandomDataType();
            }
        }

        private double GenerateRandomValue()
        {
            return Math.Round(_random.NextDouble() * 100, 2);
        }

        /// <summary>
        /// Generate realistic value based on data type
        /// </summary>
        private double GenerateValueForDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "temperature":
                    return Math.Round(_random.NextDouble() * 50 + 15, 2); // 15-65°C

                case "humidity":
                    return Math.Round(_random.NextDouble() * 60 + 30, 2); // 30-90%

                case "pressure":
                    return Math.Round(_random.NextDouble() * 30 + 95, 2); // 95-125 kPa

                case "vibration":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100 Hz

                case "status":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100%

                case "position":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100%

                case "speed":
                    return Math.Round(_random.NextDouble() * 3000, 0); // 0-3000 RPM

                case "throughput":
                    return Math.Round(_random.NextDouble() * 1000, 2); // 0-1000 Mbps

                case "output":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100%

                case "setpoint":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100

                case "level":
                    return Math.Round(_random.NextDouble() * 100, 2); // 0-100%

                case "count":
                    return _random.Next(0, 1000); // 0-1000

                default:
                    return Math.Round(_random.NextDouble() * 100, 2);
            }
        }

        private string GetUnit()
        {
            string[] units = { "°C", "%", "kPa", "Hz", "" };
            return units[_random.Next(units.Length)];
        }

        /// <summary>
        /// Get appropriate unit for data type
        /// </summary>
        private string GetUnitForDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "temperature":
                    return "°C";

                case "humidity":
                case "status":
                case "position":
                case "output":
                case "level":
                    return "%";

                case "pressure":
                    return "kPa";

                case "vibration":
                    return "Hz";

                case "speed":
                    return "RPM";

                case "throughput":
                    return "Mbps";

                case "connected devices":
                case "count":
                    return "";

                case "setpoint":
                    return "";

                default:
                    return "";
            }
        }

        /// <summary>
        /// Simulate sending command to device
        /// </summary>
        public async Task<bool> SendCommandAsync(string deviceId, string command)
        {
            await Task.Delay(500); // Simulate network delay

            // Simulate 15% command failure rate
            if (_random.Next(100) < 15)
            {
                OnConnectionError(new DeviceErrorEventArgs
                {
                    DeviceId = deviceId,
                    ErrorMessage = $"Failed to send command '{command}' - device unreachable",
                    Timestamp = DateTime.Now
                });
                return false;
            }

            return true;
        }

        protected virtual void OnDataReceived(DeviceDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        protected virtual void OnConnectionError(DeviceErrorEventArgs e)
        {
            ConnectionError?.Invoke(this, e);
        }
    }

    // Event argument classes
    public class DeviceDataReceivedEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DataType { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{DeviceName ?? DeviceId} - {DataType}: {Value}{Unit} at {Timestamp:HH:mm:ss}";
        }
    }

    public class DeviceErrorEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"[ERROR] {DeviceId} - {ErrorMessage} at {Timestamp:HH:mm:ss}";
        }
    }
}