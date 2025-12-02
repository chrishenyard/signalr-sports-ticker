# syntax=docker/dockerfile:1
################################################################################
# Build stage
FROM node:25-alpine@sha256:26ded7f450a0ad37241d2ae97ea521a59cb551a1785c8a950f74b0a291ad3aea AS build

WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .

ARG VITE_SIGNALR_HUB_URL=https://localhost:9001
ENV VITE_SIGNALR_HUB_URL=$VITE_SIGNALR_HUB_URL

#RUN npm run build
RUN npx react-router build --mode production

################################################################################
# Production stage with nginx
FROM nginx@sha256:4c175d0d849aae0e0eedc64d718ef6323bed2bc68ee673e2d0a1bd5d501d0e5f

COPY --from=build /app/build/client /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]
