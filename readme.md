# ConfigHub
ConfigHub is a centralized solution for application configuration management. At the same time it is a case study project to demonstrate a common application of the Singleton design pattern (or should I say antipattern?).

## Technical Specification:

### Core Singleton Manager
- Must ensure only one instance can exist
- Thread-safe and concurrency-ready

### Configuration Handling
- Support JSON/YAML format
- Persistent storage (local files with option to extend functionality for cloud databases)

### REST API
- CRUD operations on configurations
- Basic security with API keys or JWT tokens

#### Web Dashboard (Optional)
- View configurations, update settings
- Audit log viewer showing historical changes

#### Deliverables:
- Complete source code
- Unit and integration test suite
- Comprehensive documentation
- Deployment guide