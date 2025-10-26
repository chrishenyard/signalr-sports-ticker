# syntax=docker/dockerfile:1
ARG NODE_VERSION=24.10.0

################################################################################
# Build stage
FROM node:${NODE_VERSION}-alpine as build

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
FROM nginx:alpine

COPY --from=build /app/build/client /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]
