FROM node:22-alpine AS builder

WORKDIR /app/client

COPY client/package*.json ./

RUN npm install

COPY client .

RUN npm run build --configuration=production

FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY server/nginx/nginx.conf /etc/nginx/nginx.conf

COPY --from=builder /app/client/dist/untitled-cinema/browser /usr/share/nginx/html

EXPOSE 80