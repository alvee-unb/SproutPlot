import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// The dev server proxies /api to the ASP.NET Core backend so the SPA can call
// the API with same-origin relative URLs (no CORS friction during development).
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5268',
        changeOrigin: true,
      },
    },
  },
})
