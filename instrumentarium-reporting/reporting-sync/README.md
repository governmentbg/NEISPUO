# Reporting Sync

A TypeScript-based data synchronization service that transfers data from Microsoft SQL Server to ClickHouse for reporting purposes. This service provides both CLI and cron-based execution modes for managing data synchronization jobs.

## 🚀 Features

- **Dual Execution Modes**: CLI interface for manual operations and cron-based automated synchronization
- **Multi-Database Support**: Syncs data from MS SQL Server to ClickHouse
- **Comprehensive Job Management**: 17+ predefined synchronization jobs for various data entities
- **Migration System**: Automated database schema migrations for both MS SQL and ClickHouse
- **Error Handling & Logging**: Robust error handling with detailed logging
- **Graceful Shutdown**: Proper cleanup of database connections
- **Data Transformation**: Support for custom data transformations during sync
- **Job Scheduling**: Configurable cron schedules for automated synchronization

## 📋 Prerequisites

- **Node.js**: >= 18.0.0
- **Microsoft SQL Server**: Source database
- **ClickHouse**: Target database
- **TypeScript**: For development

## 🛠️ Installation

1. **Install dependencies**:

   ```bash
   npm install
   ```

2. **Configure environment**:
   ```bash
   cp config/.env-example config/.env
   # Edit config/.env with your database credentials
   ```

## ⚙️ Configuration

### Environment Variables

Create a `.env` file in the `config/` directory with the following variables:

```env
# MS SQL DB Configuration
MS_SQL_DB_CLIENT=mssql
MS_SQL_DB_HOST=your-mssql-host
MS_SQL_DB_PORT=1433
MS_SQL_DB_USERNAME=your-username
MS_SQL_DB_PASSWORD=your-password
MS_SQL_DB_NAME=your-database-name

# ClickHouse DB Configuration
CLICKHOUSE_DB_CLIENT=clickhouse
CLICKHOUSE_DB_HOST=http://your-clickhouse-host:port
CLICKHOUSE_DB_USERNAME=default
CLICKHOUSE_DB_PASSWORD=your-password
CLICKHOUSE_DB_NAME=reporting
```

## 🏃‍♂️ Usage

### CLI Mode

Run the interactive CLI for manual operations:

```bash
# Development
npm run start:cli-dev

# Production
npm run build

npm run start:cli
```

**Available CLI Commands:**

- `run-migrations` - Execute database migrations
- `show-database-jobs` - Display configured cron jobs
- `show-sync-status` - Show synchronization status
- `show-migration-status` - Display migration status
- `update-job-configuration` - Update job configurations
- Various sync commands for individual data entities

### Migration Mode

Run database migrations directly (used by the start script):

```bash
# Development
npm run start:migration-dev

# Production
npm run build

npm run start:migration
```

### Cron Mode

Run the automated cron service:

```bash
# Development
npm run start:cron-dev

# Production
npm run build

npm run start:cron
```

## 📊 Synchronization Jobs

The service includes 17 predefined synchronization jobs:

### Core Data Entities

- **Institutions** (`sync-institutions`) - Educational institution data
- **Students** (`sync-students`) - Student information
- **Students Details** (`sync-students-details`) - Detailed student records
- **Graduated Students** (`sync-graduated-students`) - Graduation records

### Supporting Data

- **Phones** (`sync-phones`) - Contact information
- **Classes** (`sync-classes`) - Class/course data
- **Foreign Languages** (`sync-foreign-languages`) - Language offerings
- **Helpdesk** (`sync-helpdesk`) - Support ticket data
- **Hostels** (`sync-hostels`) - Accommodation information
- **Personal N** (`sync-personal-n`) - Personal identification data
- **RZI Students** (`sync-rzi-students`) - Regional student data

### Specialized Data

- **Immigrants DOB Hours** (`sync-immigrants-dob-hours`) - Immigration-related data
- **Occupations by Qualifications** (`sync-occupations-by-qualifications`) - Career data
- **Refugee Data**:
  - `sync-refugee-withdrawn-requests`
  - `sync-refugees-received-rejected-admission-by-region`
  - `sync-refugees-searching-or-received-admission`

## 🗄️ Database Schema

### Source Database (MS SQL Server)

The service connects to MS SQL Server views organized in schemas:

- `inst_basic.*` - Basic institution data
- `reporting.*` - Reporting-specific views
- `refugee.*` - Refugee-related data

### Target Database (ClickHouse)

ClickHouse tables are created with optimized schemas for analytical queries:

- Optimized data types for performance
- Partitioning strategies for large datasets
- Indexes for common query patterns

## 🔧 Development

### Project Structure

```
reporting-sync/
├── config/                 # Configuration files
│   └── .env-example       # Environment variables template
├── migrations/            # Database migrations
│   ├── clickhouse/        # ClickHouse schema migrations
│   └── mssql/            # MS SQL migrations
├── src/
│   ├── constants/         # Configuration constants
│   ├── databases/         # Database clients and repositories
│   ├── entrypoints/       # Application entry points (CLI, Cron, Migration)
│   ├── enums/            # TypeScript enums
│   ├── interfaces/        # TypeScript interfaces
│   ├── jobs/             # Synchronization job implementations
│   ├── scripts/          # CLI command implementations
│   ├── services/         # Core business logic
│   └── utils/            # Utility functions
├── package.json
├── tsconfig.json
└── tsconfig.build.json
```

### Key Components

#### Services

- **CLI Service** (`cli.service.ts`) - Interactive command-line interface
- **Cron Service** (`cron.service.ts`) - Automated job scheduling
- **Sync Service** (`sync.service.ts`) - Core synchronization logic
- **ClickHouse Migration Service** (`clickhouse-migration.service.ts`) - ClickHouse migration logic

#### Database Clients

- **MS SQL Client** (`mssql-client.ts`) - Connection management for MS SQL Server
- **ClickHouse Client** (`clickhouse-client.ts`) - Connection management for ClickHouse

#### Job Management

- **Cron Job Manager** (`cron-job-manager.service.ts`) - Job lifecycle management
- **Job Registry** (`cron-job-registry.ts`) - Centralized job definitions

## 📝 Logging

The service provides comprehensive logging:

- **Info logs** - General operational information
- **Error logs** - Error conditions and stack traces
- **Sync result logs** - Detailed synchronization outcomes

## 🔄 Data Transformation

The service supports custom data transformations during synchronization:

```typescript
// Example: Date transformation
{
  columnName: 'BirthDate',
  transformFunction: transformDate,
}
```
