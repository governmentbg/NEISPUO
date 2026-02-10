#!/bin/sh

echo "🚀 Starting reporting-sync service..."

# Run migrations first
echo "📦 Running database migrations..."
node dist/entrypoints/migration.entrypoints.js

# Check if migrations were successful
if [ $? -eq 0 ]; then
    echo "✅ Migrations completed successfully"
    echo "🚀 Starting PM2 application..."
    exec pm2-runtime start ecosystem.config.js
else
    echo "❌ Migrations failed. Exiting..."
    exit 1
fi
