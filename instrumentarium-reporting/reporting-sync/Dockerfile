# Build stage
FROM node:22.18.0 AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install all dependencies (including dev dependencies)
RUN npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM node:22.18.0 AS production

WORKDIR /app

# Install PM2 globally
RUN npm install pm2 -g

# Copy package files
COPY package*.json ./

# Install only production dependencies
RUN npm ci --only=production && npm cache clean --force

# Copy built application from builder stage
COPY --from=builder /app/dist ./dist

# Copy migrations directory
COPY --from=builder /app/migrations ./migrations

# Copy ecosystem config
COPY --from=builder /app/ecosystem.config.js ./

# # Copy environment template
# COPY --from=builder /app/.env-example-docker ./config/.env

# Copy startup script
COPY --from=builder /app/start.sh ./start.sh
RUN chmod +x ./start.sh

# # MS SQL DB
# ENV MS_SQL_DB_CLIENT=mssql
# ENV MS_SQL_DB_HOST=172.16.3.126
# ENV MS_SQL_DB_PORT=1433
# ENV MS_SQL_DB_USERNAME=dss
# ENV MS_SQL_DB_PASSWORD=ba7Georg!En@iG0lemiQ
# ENV MS_SQL_DB_NAME=neispuo-dev


# # ClickHouse DB
# ENV CLICKHOUSE_DB_CLIENT=clickhouse
# ENV CLICKHOUSE_DB_HOST=http://localhost:30081
# ENV CLICKHOUSE_DB_USERNAME=default
# ENV CLICKHOUSE_DB_PASSWORD=clickhouse_password
# ENV CLICKHOUSE_DB_NAME=reporting


ENTRYPOINT ["./start.sh"]
