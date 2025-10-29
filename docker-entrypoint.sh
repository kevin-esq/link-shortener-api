#!/bin/bash
set -e

echo "🔐 Checking JWT RSA keys..."

# Keys directory
KEYS_DIR="/app/Keys"

# Check if keys already exist
if [ ! -f "$KEYS_DIR/private.pem" ] || [ ! -f "$KEYS_DIR/public.pem" ]; then
    echo "🔑 Generating RSA key pair for JWT..."
    
    # Generate RSA private key (2048 bits)
    openssl genpkey -algorithm RSA -out "$KEYS_DIR/private.pem" -pkeyopt rsa_keygen_bits:2048
    
    # Generate public key from private key
    openssl rsa -pubout -in "$KEYS_DIR/private.pem" -out "$KEYS_DIR/public.pem"
    
    echo "✅ RSA keys generated successfully!"
else
    echo "✅ RSA keys already exist. Skipping generation."
fi

# Set permissions
chmod 600 "$KEYS_DIR/private.pem"
chmod 644 "$KEYS_DIR/public.pem"

echo "🚀 Starting application..."

# Execute the main command
exec "$@"
