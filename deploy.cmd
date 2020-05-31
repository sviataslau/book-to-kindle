gcloud builds submit --tag gcr.io/book-to-kindle/book-to-kindle-bot
gcloud run deploy --image gcr.io/book-to-kindle/book-to-kindle-bot --platform managed