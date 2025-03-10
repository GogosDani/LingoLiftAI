import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  base: "/",
  plugins: [react()],
  preview: {
    port: 4000,
    strictPort: true,
  },
  server: {
    port: 4000,
    strictPort: true,
    host: "0.0.0.0",
  },
});