#!/bin/sh
set -e

echo "Creating DynamoDB table EventDetails with UserId (HASH) + Timestamp (RANGE)..."

aws dynamodb create-table \
  --table-name EventDetails \
  --attribute-definitions AttributeName=UserId,AttributeType=S AttributeName=Timestamp,AttributeType=S \
  --key-schema AttributeName=UserId,KeyType=HASH AttributeName=Timestamp,KeyType=RANGE \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:8000 \
  --region us-west-2 || echo "Table might already exist"

echo "DynamoDB init complete."

# Keep container alive
tail -f /dev/null