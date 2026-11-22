import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const gatewayUrl = process.env.VITE_GATEWAY_URL || 'http://127.0.0.1:8090';
const oauthClientId = process.env.OAUTH_CLIENT_ID || 'vehiculos-client';
const oauthClientSecret = process.env.OAUTH_CLIENT_SECRET || '';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5174,
    strictPort: true,
    proxy: {
      '/api': {
        target: gatewayUrl,
        changeOrigin: true,
      },
      '/oauth': {
        target: gatewayUrl,
        changeOrigin: true,
        configure: (proxy) => {
          proxy.on('proxyReq', (proxyReq) => {
            if (oauthClientSecret) {
              const basic = Buffer.from(`${oauthClientId}:${oauthClientSecret}`).toString('base64');
              proxyReq.setHeader('Authorization', `Basic ${basic}`);
            }
          });
        },
      },
    },
  },
  preview: {
    port: 4173,
    strictPort: true,
  },
});
