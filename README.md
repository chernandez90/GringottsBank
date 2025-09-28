# 🏛️ Gringotts Wizarding Bank ATM

A magical banking application built with Vue.js frontend and .NET Core API backend, themed after the famous goblin-run bank from the Harry Potter universe. Experience authentic parchment-styled UI and complete banking functionality in Docker containers.

## ✨ Features

### 🏦 **Banking Operations**

- **Withdraw Gold** 💰 - Remove funds from your vault
- **Deposit Treasure** 💎 - Add funds to your vault
- **Transfer Between Vaults** 🔄 - Move funds between Checking and Savings
- **Transaction History** 📜 - View all your magical banking activities

### 🎨 **Magical UI**

- **Authentic Parchment Theme** - Realistic aged paper texture with stains and wear
- **Gringotts Branding** - Official logo and Wizarding Bank styling
- **Medieval Typography** - Cinzel and Uncial Antiqua fonts
- **Responsive Design** - Works on all devices
- **Navigation Bar** - Easy navigation between screens

### 🛡️ **Technical Features**

- **Dockerized Architecture** - Full containerization with Docker Compose
- **Real-time API Integration** - Live backend database updates
- **CORS Enabled** - Proper cross-origin resource sharing
- **UTC to Local Time** - Automatic timezone conversion for transactions
- **Error Handling** - Robust error management and user feedback

## 🏗️ Architecture

```
┌─────────────────────────┐    ┌─────────────────────────┐
│     Frontend (Vue.js)   │    │   Backend (.NET Core)   │
│   Port: 3000           │◄───┤   Port: 8080/8081       │
│   - Gringotts ATM UI   │    │   - RESTful API         │
│   - Parchment Theme    │    │   - In-Memory Database  │
│   - Transaction Logic  │    │   - Entity Framework    │
└─────────────────────────┘    └─────────────────────────┘
```

## 🚀 Quick Start

### Prerequisites

- **Docker** and **Docker Compose** installed on your system
- **Git** for cloning the repository

### Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/chernandez90/GringottsBank.git
   cd GringottsBank
   ```

2. **Start the Application**

   ```bash
   docker-compose up --build -d
   ```

3. **Access the ATM**

   - Frontend: http://localhost:3000
   - Backend API: http://localhost:8080
   - Swagger Documentation: http://localhost:8080/swagger

4. **Stop the Application**
   ```bash
   docker-compose down
   ```

### Development Setup

For development with hot reload:

```bash
# Start in development mode
docker-compose up --build

# View logs
docker-compose logs -f

# Restart specific service
docker-compose restart gringottsbankui
```

## 📊 Default Account Data

The application starts with pre-seeded accounts:

| Account Type | Initial Balance | Account ID |
| ------------ | --------------- | ---------- |
| 💎 Savings   | 1,000.00 G      | 1          |
| 🏦 Checking  | 2,500.50 G      | 2          |

## 🔌 API Endpoints

### Account Information

- `GET /api/AccountInfo` - Get all accounts
- `GET /api/AccountInfo/{id}` - Get specific account

### Transactions

- `GET /api/AccountTransactions` - Get all transactions
- `GET /api/AccountTransactions/account/{accountId}` - Get transactions by account
- `POST /api/AccountTransactions` - Create new transaction
- `POST /api/AccountTransactions/transfer` - Transfer between accounts

### Example API Usage

```bash
# Get all accounts
curl http://localhost:8080/api/AccountInfo

# Create a deposit
curl -X POST http://localhost:8080/api/AccountTransactions \
  -H "Content-Type: application/json" \
  -d '{"accountId": 1, "transactionType": "Deposit", "amount": 100.0}'

# Transfer funds
curl -X POST http://localhost:8080/api/AccountTransactions/transfer \
  -H "Content-Type: application/json" \
  -d '{"fromAccountId": 1, "toAccountId": 2, "amount": 50.0}'
```

## 🐳 Docker Configuration

### Services

- **gringottsbankapi** - .NET Core API (Ports: 8080, 8081)
- **gringottsbankui** - Vue.js Frontend (Port: 3000)

### Environment Variables

| Variable                 | Value       | Description          |
| ------------------------ | ----------- | -------------------- |
| `ASPNETCORE_ENVIRONMENT` | Development | API environment      |
| `NODE_ENV`               | development | Frontend environment |

### Volumes

- Frontend source code mounted for development hot reload
- Node modules cached in anonymous volume

## 🎭 UI Theme Details

### Parchment Paper Effect

- **Layered backgrounds** with age variations and ink stains
- **Paper fiber simulation** using repeating linear gradients
- **Authentic colors** (#f5e6d3, #e8d5b7, #dfc49a, #d4b896, #c9a876)
- **Aging effects** with burn marks, wrinkles, and wear patterns
- **Depth shadows** for realistic paper curl effect

### Typography

- **Cinzel** - Primary medieval serif font for UI elements
- **Uncial Antiqua** - Ancient manuscript font for headings
- **Text shadows** and **letter spacing** for authentic feel

## 🔧 Technology Stack

### Frontend

- **Vue.js 3** - Progressive JavaScript framework
- **Vite** - Fast build tool and dev server
- **CSS3** - Advanced styling with gradients and effects
- **Docker** - Containerization

### Backend

- **.NET 9.0** - Modern C# web framework
- **Entity Framework Core** - ORM with In-Memory database
- **Swagger/OpenAPI** - API documentation
- **MediatR** - CQRS pattern implementation
- **Docker** - Containerization

## 📁 Project Structure

```
GringottsBank/
├── 📄 docker-compose.yml           # Container orchestration
├── 📁 GringottsBankAPI/            # Backend API
│   ├── 📁 Controllers/             # API controllers
│   ├── 📁 Models/                  # Data models
│   ├── 📁 DTOs/                    # Data transfer objects
│   ├── 📁 Services/                # Business logic
│   └── 📄 Program.cs              # API entry point
├── 📁 GringottsBankUI/             # Frontend application
│   ├── 📁 src/
│   │   ├── 📁 components/         # Vue components
│   │   ├── 📁 services/           # API services
│   │   └── 📄 App.vue            # Main application
│   ├── 📄 package.json           # Dependencies
│   └── 📄 vite.config.js         # Build configuration
└── 📄 README.md                   # This file
```

## 🎯 User Guide

### Making Transactions

1. **Select Operation** - Choose from the main menu options
2. **Choose Account** - Select between Checking or Savings vault
3. **Enter Amount** - Use quick amounts or enter custom value
4. **Confirm** - Review and execute your transaction
5. **View Result** - See updated balances and confirmation

### Viewing History

1. **Click "Transaction History"** from main menu
2. **Select Account** - Choose specific vault or "All Vaults"
3. **Review Transactions** - See chronological list with:
   - Transaction type and amount
   - Account affected
   - Local timestamp
   - Current balance after transaction

### Transferring Funds

1. **Select "Transfer Between Vaults"**
2. **Choose Source** - Select account to transfer from
3. **Choose Destination** - Select account to transfer to
4. **Enter Amount** - Cannot exceed available balance
5. **Execute Transfer** - Creates two linked transactions

## 🐛 Troubleshooting

### Common Issues

**Container won't start:**

```bash
# Check container status
docker-compose ps

# View logs
docker-compose logs gringottsbankapi
docker-compose logs gringottsbankui
```

**API connection issues:**

```bash
# Test API directly
curl http://localhost:8080/api/AccountInfo

# Restart services
docker-compose restart
```

**Frontend not loading:**

```bash
# Rebuild with no cache
docker-compose build --no-cache
docker-compose up -d
```

### Port Conflicts

If ports 3000, 8080, or 8081 are in use:

1. Stop conflicting services
2. Or modify ports in `docker-compose.yml`
3. Restart containers

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- **J.K. Rowling** - For creating the magical world of Harry Potter
- **Warner Bros** - For the Gringotts Bank inspiration
- **Google Fonts** - For Cinzel and Uncial Antiqua typography
- **Docker Community** - For containerization platform

---

**May your vaults be ever full of gold!** 🏛️✨

_"Fortius Quo Fidelius"_ - Strength through Loyalty (Gringotts Motto)
