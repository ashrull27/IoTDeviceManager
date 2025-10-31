# IoT Device Manager

## Project Overview
A Windows desktop application built with C# and WPF for managing IoT devices. This application demonstrates MVVM architecture, real-time device communication simulation, and comprehensive device management capabilities.

### Practical Coding & System Flow

#### 1. Windows-Based IoT Tool
- **WPF Desktop Application** with modern UI design
- **Device Management Features:**
  - Display list of devices with mock/seed data
  - Add new device entries via dialog window
  - Edit existing device information
  - Delete devices with confirmation dialog
  - Toggle device connectivity status (Online/Offline)
  - Real-time status indicators (green for online, red for offline)
- **Action Logging System:**
  - Comprehensive logging of all operations (Add, Update, Delete, ToggleStatus)
  - Timestamped log entries with action details
  - Log levels (Info, Warning, Error, Success)
  - Maximum 100 log entries retained
- **MVVM Architecture:**
  - Clear separation of UI (Views) and business logic (ViewModels)
  - Models for Device and LogEntry
  - Services for data operations and device communication
  - RelayCommand helper for command binding

#### 2. System Architecture & Data Flow
- **Architecture Diagram** included showing:
  - Desktop application structure (MVVM layers)
  - Communication flow: IoT Devices ↔ Communication Service ↔ Application ↔ Database/Cloud
  - Data persistence layer
  - Error handling and retry mechanisms
- **Connectivity Failure Handling:**
  - Automatic error detection and logging
  - Connection timeout simulation
  - User notification system
  - Device status updates on communication failure
<img width="6164" height="3075" alt="System Architecture Diagram" src="https://github.com/user-attachments/assets/21ef90fd-fc9b-45b0-85cf-6fdbf0714b1e" />
<img width="3352" height="2394" alt="Data Flow - Normal Operation" src="https://github.com/user-attachments/assets/5f604ff4-024e-4b6f-bf40-be549efe4099" />

#### 3. Device Communication Handling (Simulation)
- **Real-time Communication Simulator:**
  - Timer-based event system (3-second intervals)
  - Random sensor data generation (Temperature, Humidity, Pressure, etc.)
  - Connection error simulation (10% failure rate)
  - Data display in real-time panel
  - Error handling for disconnected devices
- **Features:**
  - Start/Stop communication controls
  - Live data streaming display
  - Maximum 50 real-time data entries retained
  - Event-driven architecture using C# events
- **Real-world Implementation Notes:**
  - Current: Timer-based simulation with random values
  - Production: Would use MQTT, REST APIs, WebSockets, or serial communication
  - Protocols: MQTT (pub/sub), HTTP/REST, WebSocket, CoAP, or Modbus
  
<img width="1470" height="5332" alt="Real-time Communication Flow" src="https://github.com/user-attachments/assets/049d43c4-1c34-47c6-a2a9-5b7270297b89" />

#### 4. Flowchart & ERD Documentation
- **Flowcharts Created:**
  - Device CRUD operations workflow
  - Device communication flow
  - Error handling sequence
- **Entity Relationship:**
  - Device entity structure
  - LogEntry entity structure
  - Service relationships
  
<img width="4850" height="3659" alt="ERD" src="https://github.com/user-attachments/assets/5d3aaa40-88cb-420c-a3a0-1d50119aef6f" />

## Project Structure

```
IoTDeviceManager/
├── Data/                          # (Reserved for database/file storage)
├── Helpers/
│   └── RelayCommand.cs           # ICommand implementation for MVVM
├── Models/
│   ├── Device.cs                 # Device entity with INotifyPropertyChanged
│   └── LogEntry.cs               # Log entry model with timestamp and level
├── Services/
│   ├── DeviceService.cs          # Device CRUD operations and mock data
│   └── DeviceCommunicationService.cs  # Real-time communication simulator
├── ViewModel/
│   └── MainViewModel.cs          # Main application ViewModel (MVVM)
├── Views/
│   ├── App.xaml                  # Application configuration
│   ├── App.xaml.cs               # Application code-behind
│   ├── MainWindow.xaml           # Main UI layout
│   ├── MainWindow.xaml.cs        # Main window code-behind
│   ├── EditDeviceWindow.xaml     # Device edit dialog UI
│   └── EditDeviceWindow.xaml.cs  # Edit dialog code-behind
└── Properties/                    # Assembly information
```

## Setup & Build Instructions

### Prerequisites
- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET Framework 4.7.2 or higher** (or .NET 6/7/8 for modern .NET)
- **Windows 10/11** operating system

### Installation Steps

1. **Open Visual Studio 2022**
   - Launch Visual Studio 2022

2. **Create New Project**
   - File → New → Project
   - Select "WPF App (.NET Framework)" or "WPF Application" for .NET 6+
   - Name: `IoTDeviceManager`
   - Click "Create"

3. **Add Project Structure**
   - Right-click on project → Add → New Folder for each directory:
     - Data
     - Helpers
     - Models
     - Services
     - ViewModel
     - Views

4. **Add Source Files**
   - Copy each provided `.cs` and `.xaml` file to its respective folder
   - Ensure namespace matches: `IoTDeviceManager.[FolderName]`

5. **Update App.xaml**
   - Set `StartupUri="Views/MainWindow.xaml"`

6. **Build Solution**
   - Build → Build Solution (Ctrl+Shift+B)
   - Resolve any namespace or reference issues

7. **Run Application**
   - Press F5 or click "Start" button
   - Application will launch with 5 mock devices pre-loaded

## Tools & Libraries Used

### Core Technologies
- **C# 10.0** - Primary programming language
- **WPF (Windows Presentation Foundation)** - UI framework
- **.NET Framework 4.7.2+** or **.NET 6/7/8** - Runtime environment
- **XAML** - UI markup language

### Design Patterns & Principles
- **MVVM (Model-View-ViewModel)** - Architectural pattern
- **INotifyPropertyChanged** - Property change notification
- **ICommand Pattern** - Command binding
- **Dependency Injection Ready** - Service-based architecture
- **Event-Driven Architecture** - Communication events
- **Observer Pattern** - Real-time data updates

### Built-in .NET Libraries
- `System.Collections.ObjectModel` - ObservableCollection for data binding
- `System.ComponentModel` - INotifyPropertyChanged interface
- `System.Threading` - Timer for simulation
- `System.Windows.Input` - ICommand interface
- `System.Linq` - Data querying

### UI Components
- **DataGrid** - Device list display
- **ListBox** - Log and real-time data display
- **Buttons with Commands** - Action triggers
- **Dialog Windows** - Edit/Add device forms
- **Status Indicators** - Visual device status (Ellipse elements)

## User Guide

### Main Window Overview
- **Header Section:** Displays online/offline device counts
- **Left Panel:** Device list with management actions
- **Right Panel:** Real-time communication and activity logs
- **Status Bar:** Current operation status and timestamp

### Managing Devices

#### Add New Device
1. Click "➕ Add Device" button
2. Fill in device information:
   - Device Name (required)
   - Device Type (Sensor/Actuator/Gateway/Controller/Monitor)
   - IP Address (required)
   - Firmware Version
   - Online Status (checkbox)
3. Click "💾 Save" to create device
4. Check Activity Logs for confirmation

#### Edit Existing Device
1. Select a device from the list
2. Click "✏️ Edit" button
3. Modify device information in dialog
4. Click "💾 Save" to update
5. Cancel to discard changes

#### Delete Device
1. Select a device from the list
2. Click "🗑️ Delete" button
3. Confirm deletion in popup dialog
4. Device will be removed from list

#### Toggle Device Status
1. Select a device from the list
2. Click "🔄 Toggle Status" button
3. Device status switches between Online/Offline
4. Status indicator updates immediately

### Real-time Communication

#### Start Communication Simulation
1. Navigate to "Real-time Device Communication" panel
2. Click "▶ Start" button
3. Watch live data streaming from simulated devices
4. Data updates every 3 seconds

#### Stop Communication
1. Click "⏸ Stop" button to halt communication
2. No new data will be received

#### View Communication Logs
- Real-time data appears in the communication panel
- Errors and connection issues logged separately
- Maximum 50 entries displayed
- Auto-scrolls to show latest data

### Activity Logs
- All actions are logged with timestamps
- Log levels: Info (blue), Warning (yellow), Error (red), Success (green)
- View up to 100 most recent log entries
- Click "Clear Logs" to reset log history

## Advanced Features

### Error Handling
- **Connection Failures:** Automatically detected and logged
- **Data Validation:** Input validation in edit dialogs
- **User Feedback:** Status messages and confirmation dialogs
- **Exception Handling:** Try-catch blocks in critical operations

### Performance Optimizations
- **ObservableCollection:** Efficient UI updates
- **Timer-based Updates:** Controlled update frequency
- **Log Limits:** Prevents memory overflow (100 logs, 50 data entries)
- **Event-driven:** Minimal resource usage when idle

### Code Quality
- **XML Documentation:** All classes and methods documented
- **Meaningful Names:** Self-explanatory variable and method names
- **Single Responsibility:** Each class has one clear purpose
- **Commented Code:** Complex logic explained inline
- **Error Messages:** User-friendly error descriptions

## Future Enhancements

### Planned Features
1. **Database Integration**
   - SQLite or SQL Server for data persistence
   - Entity Framework Core for ORM
   - Database migrations

2. **Real Device Communication**
   - MQTT broker integration (Mosquitto/HiveMQ)
   - REST API endpoints
   - WebSocket connections
   - Serial port communication

3. **Cloud Services**
   - Azure IoT Hub integration
   - AWS IoT Core support
   - Real-time data streaming to cloud

4. **Authentication & Security**
   - User login system
   - Role-based access control (RBAC)
   - Encrypted communication
   - Audit trail

5. **Advanced Features**
   - Device provisioning workflow
   - Firmware update management
   - Historical data charts
   - Alert/notification system
   - Export functionality (CSV/PDF)

6. **Scalability**
   - Support for 1000+ devices
   - Pagination and filtering
   - Background task processing
   - Performance monitoring

## Troubleshooting

### Build Errors
- **Namespace Issues:** Ensure all files use correct namespace format
- **Missing References:** Check project references in Solution Explorer
- **XAML Errors:** Verify all x:Class attributes match code-behind

### Runtime Issues
- **Window Not Appearing:** Check App.xaml StartupUri path
- **Data Not Binding:** Verify DataContext is set to MainViewModel
- **Commands Not Working:** Ensure RelayCommand is properly implemented

### Communication Issues
- **Too Many Errors:** Normal behavior (10% simulated failure rate)

---

**Developed By:** Mohammad `Ashrull B Zukaimi  
**Date:** 1/11/2025
**Version:** 1.0.0  
**Framework:** WPF .NET Framework 4.7.2+
