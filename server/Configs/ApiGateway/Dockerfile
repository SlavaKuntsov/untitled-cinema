FROM node:22-alpine AS builder

WORKDIR /app/client

COPY client/package*.json ./

RUN npm install

COPY client .

RUN npm run build --configuration=production

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY server/Configs/ApiGateway/nginx.conf /etc/nginx/nginx.conf

COPY --from=builder /app/client/dist /usr/share/nginx/html

EXPOSE 80