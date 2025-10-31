using System;
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

        public event EventHandler<DeviceDataReceivedEventArgs> DataReceived;
        public event EventHandler<DeviceErrorEventArgs> ConnectionError;

        public DeviceCommunicationService()
        {
            _random = new Random();
            _isRunning = false;
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
                // Simulate random connection failures (10% chance)
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

                // Simulate receiving sensor data
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

        private double GenerateRandomValue()
        {
            return Math.Round(_random.NextDouble() * 100, 2);
        }

        private string GetUnit()
        {
            string[] units = { "°C", "%", "kPa", "Hz", "" };
            return units[_random.Next(units.Length)];
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
        public string DataType { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{DeviceId} - {DataType}: {Value}{Unit} at {Timestamp:HH:mm:ss}";
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