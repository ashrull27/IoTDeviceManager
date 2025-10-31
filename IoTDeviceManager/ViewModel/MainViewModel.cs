using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IoTDeviceManager.Helpers;
using IoTDeviceManager.Models;
using IoTDeviceManager.Services;
using IoTDeviceManager.Views;

namespace IoTDeviceManager.ViewModel
{
    /// <summary>
    /// Main ViewModel implementing MVVM pattern
    /// Manages device list, logs, and communication
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceCommunicationService _communicationService;

        private Device _selectedDevice;
        private string _statusMessage;
        private int _onlineDevicesCount;
        private int _offlineDevicesCount;

        public ObservableCollection<Device> Devices { get; set; }
        public ObservableCollection<LogEntry> Logs { get; set; }
        public ObservableCollection<string> RealtimeData { get; set; }

        // Commands
        public ICommand AddDeviceCommand { get; }
        public ICommand EditDeviceCommand { get; }
        public ICommand DeleteDeviceCommand { get; }
        public ICommand ToggleStatusCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearLogsCommand { get; }
        public ICommand StartCommunicationCommand { get; }
        public ICommand StopCommunicationCommand { get; }

        public Device SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public int OnlineDevicesCount
        {
            get => _onlineDevicesCount;
            set
            {
                _onlineDevicesCount = value;
                OnPropertyChanged(nameof(OnlineDevicesCount));
            }
        }

        public int OfflineDevicesCount
        {
            get => _offlineDevicesCount;
            set
            {
                _offlineDevicesCount = value;
                OnPropertyChanged(nameof(OfflineDevicesCount));
            }
        }

        public MainViewModel()
        {
            // Initialize services
            _deviceService = new DeviceService();
            _communicationService = new DeviceCommunicationService();

            // Initialize collections
            Devices = new ObservableCollection<Device>();
            Logs = new ObservableCollection<LogEntry>();
            RealtimeData = new ObservableCollection<string>();

            // Initialize commands
            AddDeviceCommand = new RelayCommand(AddDevice);
            EditDeviceCommand = new RelayCommand(EditDevice, CanEditOrDelete);
            DeleteDeviceCommand = new RelayCommand(DeleteDevice, CanEditOrDelete);
            ToggleStatusCommand = new RelayCommand(ToggleStatus, CanEditOrDelete);
            RefreshCommand = new RelayCommand(RefreshDevices);
            ClearLogsCommand = new RelayCommand(ClearLogs);
            StartCommunicationCommand = new RelayCommand(StartCommunication);
            StopCommunicationCommand = new RelayCommand(StopCommunication);

            // Subscribe to communication events
            _communicationService.DataReceived += OnDataReceived;
            _communicationService.ConnectionError += OnConnectionError;

            // Load initial data
            LoadDevices();
            UpdateDeviceStats();
            AddLog("Application Started", "System", "IoT Device Manager initialized", LogLevel.Success);
        }

        private void LoadDevices()
        {
            Devices.Clear();
            var devices = _deviceService.GetAllDevices();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
        }

        private void AddDevice(object parameter)
        {
            var editWindow = new IoTDeviceManager.Views.EditDeviceWindow();
            if (editWindow.ShowDialog() == true)
            {
                var newDevice = editWindow.Device;
                if (_deviceService.AddDevice(newDevice))
                {
                    Devices.Add(newDevice);
                    AddLog("Device Added", newDevice.Name, $"New device created with ID: {newDevice.Id}", LogLevel.Success);
                    UpdateDeviceStats();
                    StatusMessage = $"Device '{newDevice.Name}' added successfully";
                }
            }
        }

        private void EditDevice(object parameter)
        {
            if (SelectedDevice == null) return;

            var editWindow = new IoTDeviceManager.Views.EditDeviceWindow(SelectedDevice);
            if (editWindow.ShowDialog() == true)
            {
                if (_deviceService.UpdateDevice(SelectedDevice))
                {
                    AddLog("Device Updated", SelectedDevice.Name, "Device information modified", LogLevel.Info);
                    UpdateDeviceStats();
                    StatusMessage = $"Device '{SelectedDevice.Name}' updated successfully";
                }
            }
        }

        private void DeleteDevice(object parameter)
        {
            if (SelectedDevice == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedDevice.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string deviceName = SelectedDevice.Name;
                string deviceId = SelectedDevice.Id;

                if (_deviceService.DeleteDevice(deviceId))
                {
                    Devices.Remove(SelectedDevice);
                    AddLog("Device Deleted", deviceName, $"Device removed from system", LogLevel.Warning);
                    UpdateDeviceStats();
                    StatusMessage = $"Device '{deviceName}' deleted";
                    SelectedDevice = null;
                }
            }
        }

        private void ToggleStatus(object parameter)
        {
            if (SelectedDevice == null) return;

            if (_deviceService.ToggleDeviceStatus(SelectedDevice.Id))
            {
                bool newStatus = SelectedDevice.IsOnline;
                string status = newStatus ? "Online" : "Offline";
                AddLog("Status Changed", SelectedDevice.Name, $"Device status changed to {status}", LogLevel.Info);
                UpdateDeviceStats();
                StatusMessage = $"{SelectedDevice.Name} is now {status}";
            }
        }

        private bool CanEditOrDelete(object parameter)
        {
            return SelectedDevice != null;
        }

        private void RefreshDevices(object parameter)
        {
            LoadDevices();
            UpdateDeviceStats();
            AddLog("Devices Refreshed", "System", $"Device list updated - {Devices.Count} devices loaded", LogLevel.Info);
            StatusMessage = "Device list refreshed";
        }

        private void ClearLogs(object parameter)
        {
            Logs.Clear();
            AddLog("Logs Cleared", "System", "Log history cleared by user", LogLevel.Info);
        }

        private void StartCommunication(object parameter)
        {
            _communicationService.StartCommunication();
            AddLog("Communication Started", "System", "Real-time device communication enabled", LogLevel.Success);
            StatusMessage = "Real-time communication started";
        }

        private void StopCommunication(object parameter)
        {
            _communicationService.StopCommunication();
            AddLog("Communication Stopped", "System", "Real-time device communication disabled", LogLevel.Warning);
            StatusMessage = "Real-time communication stopped";
        }

        private void OnDataReceived(object sender, DeviceDataReceivedEventArgs e)
        {
            // Update on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                string dataMessage = $"[{e.Timestamp:HH:mm:ss}] {e.DeviceId}: {e.DataType} = {e.Value}{e.Unit}";
                RealtimeData.Insert(0, dataMessage);

                // Keep only last 50 entries
                while (RealtimeData.Count > 50)
                {
                    RealtimeData.RemoveAt(RealtimeData.Count - 1);
                }

                AddLog("Data Received", e.DeviceId, $"{e.DataType}: {e.Value}{e.Unit}", LogLevel.Info);
            });
        }

        private void OnConnectionError(object sender, DeviceErrorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddLog("Connection Error", e.DeviceId, e.ErrorMessage, LogLevel.Error);
                StatusMessage = $"Error: {e.ErrorMessage}";
            });
        }

        private void UpdateDeviceStats()
        {
            OnlineDevicesCount = Devices.Count(d => d.IsOnline);
            OfflineDevicesCount = Devices.Count(d => !d.IsOnline);
        }

        private void AddLog(string action, string deviceName, string details, LogLevel level = LogLevel.Info)
        {
            var log = new LogEntry(action, deviceName, details, level);
            Logs.Insert(0, log);

            // Keep only last 100 logs
            while (Logs.Count > 100)
            {
                Logs.RemoveAt(Logs.Count - 1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}