# ===== Base =====
FROM node:20-alpine AS base
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm ci
COPY . .

# ===== Dev target (Vite HMR) =====
FROM base AS dev
EXPOSE 5173
ENV NODE_ENV=development
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]

# ===== Build target =====
FROM base AS build
RUN npm run build

# ===== Prod target (static via nginx) =====
FROM nginx:alpine AS prod
COPY --from=build /usr/src/app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
