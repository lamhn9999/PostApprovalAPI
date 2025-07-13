#!/bin/bash

URL="http://localhost:5000/posts"

for i in {1..10000}; do
    user_id=$(( ( RANDOM % 30) + 1 ))

    content=$(head /dev/urandom | tr -dc A-Za-z0-9 | head -c 1000)

    title="Post Title $i"
    image_url="https://example.com/image$i.jpg"

    curl -X POST "$URL" \
        -H "Content-Type: application/json" \
        -d "{
            \"title\": \"$title\",
            \"content\": \"$content\",
            \"imageUrl\": \"$image_url\",
            \"createdById\": $user_id
        }"

	echo ""
    echo "Posted $i (User $user_id)"
done
