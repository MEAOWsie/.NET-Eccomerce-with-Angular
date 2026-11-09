# --- Étape 1 : compilation d'Angular ---
FROM node:22 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npx ng build --output-path dist \
    && mkdir /out \
    && if [ -d dist/browser ]; then cp -r dist/browser/. /out; else cp -r dist/. /out; fi

# --- Étape 2 : image finale légère avec nginx ---
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /out /usr/share/nginx/html
