FROM node@sha256:26ded7f450a0ad37241d2ae97ea521a59cb551a1785c8a950f74b0a291ad3aea AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npx react-router build --mode production

ARG VITE_SIGNALR_HUB_URL=https://localhost:9001
ENV VITE_SIGNALR_HUB_URL=$VITE_SIGNALR_HUB_URL

FROM nginx@sha256:b3c656d55d7ad751196f21b7fd2e8d4da9cb430e32f646adcf92441b72f82b14
COPY --from=build /app/build/client /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 5173
CMD ["nginx", "-g", "daemon off;"]
