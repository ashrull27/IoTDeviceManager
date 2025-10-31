graph TB
    subgraph "IoT Devices Layer"
        D1[Temperature Sensor]
        D2[Humidity Sensor]
        D3[Pressure Monitor]
        D4[Smart Actuator]
        D5[Gateway Device]
    end

    subgraph "Communication Layer"
        MQTT[MQTT Broker<br/>Mosquitto/HiveMQ]
        REST[REST API Gateway]
        WS[WebSocket Server]
        SERIAL[Serial/Modbus]
    end

    subgraph "Desktop Application - WPF"
        subgraph "Presentation Layer - XAML"
            UI1[MainWindow.xaml]
            UI2[EditDeviceWindow.xaml]
        end
        
        subgraph "ViewModel Layer - MVVM"
            VM[MainViewModel]
            CMD[RelayCommands]
        end
        
        subgraph "Business Logic Layer"
            DS[DeviceService<br/>CRUD Operations]
            DCS[DeviceCommunication<br/>Service]
            LOGGER[Logging Service]
        end
        
        subgraph "Data Layer"
            MODEL[Device Model<br/>LogEntry Model]
            CACHE[In-Memory Cache<br/>ObservableCollection]
        end
    end

    subgraph "Data Persistence Layer"
        DB[(Local Database<br/>SQLite/SQL Server)]
        FILES[Configuration Files<br/>JSON/XML]
    end

    subgraph "Cloud/Backend Layer"
        AZURE[Azure IoT Hub]
        AWS[AWS IoT Core]
        CLOUD_DB[(Cloud Database<br/>Cosmos DB/DynamoDB)]
        ANALYTICS[Analytics Service]
    end

    subgraph "Error Handling & Recovery"
        RETRY[Retry Mechanism]
        QUEUE[Message Queue]
        FALLBACK[Fallback Mode]
        ALERT[Alert System]
    end

    %% Device to Communication Layer
    D1 -->|Publish Data| MQTT
    D2 -->|HTTP POST| REST
    D3 -->|WebSocket| WS
    D4 -->|Serial Port| SERIAL
    D5 -->|Bridge Protocol| MQTT

    %% Communication to Application
    MQTT -->|Subscribe| DCS
    REST -->|Poll/Webhook| DCS
    WS -->|Real-time Stream| DCS
    SERIAL -->|COM Port Read| DCS

    %% Application Internal Flow
    DCS -->|Update Status| VM
    DS -->|CRUD Operations| VM
    VM -->|Data Binding| UI1
    VM -->|Commands| CMD
    CMD -->|Execute| DS
    UI2 -->|Input| VM
    VM -->|Update| MODEL
    MODEL -->|Store| CACHE

    %% Persistence
    DS -->|Save| DB
    DS -->|Export Config| FILES
    DB -->|Load| DS

    %% Cloud Integration
    DCS -->|Telemetry| AZURE
    DS -->|Sync| CLOUD_DB
    AZURE -->|Commands| DCS
    CLOUD_DB -->|Analytics| ANALYTICS

    %% Error Handling
    DCS -.->|Connection Failed| RETRY
    RETRY -.->|Max Retries| QUEUE
    QUEUE -.->|Offline Mode| FALLBACK
    FALLBACK -.->|Log Error| LOGGER
    LOGGER -.->|Critical Error| ALERT
    ALERT -.->|Notify User| UI1

    style D1 fill:#e74c3c,stroke:#c0392b,color:#fff
    style D2 fill:#e74c3c,stroke:#c0392b,color:#fff
    style D3 fill:#e74c3c,stroke:#c0392b,color:#fff
    style D4 fill:#e74c3c,stroke:#c0392b,color:#fff
    style D5 fill:#3498db,stroke:#2980b9,color:#fff
    style MQTT fill:#f39c12,stroke:#e67e22,color:#fff
    style REST fill:#f39c12,stroke:#e67e22,color:#fff
    style WS fill:#f39c12,stroke:#e67e22,color:#fff
    style VM fill:#9b59b6,stroke:#8e44ad,color:#fff
    style DS fill:#27ae60,stroke:#229954,color:#fff
    style DCS fill:#27ae60,stroke:#229954,color:#fff
    style DB fill:#34495e,stroke:#2c3e50,color:#fff
    style AZURE fill:#0078d4,stroke:#005a9e,color:#fff
    style AWS fill:#ff9900,stroke:#cc7a00,color:#fff
