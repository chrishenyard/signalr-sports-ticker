docker build -t ticker-app:latest .
docker run -it --rm -p 5173:5173 --name ticker-app  ticker-app:latest

