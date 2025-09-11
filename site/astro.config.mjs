import { defineConfig } from 'astro/config';
import tailwind from '@astrojs/tailwind';

export default defineConfig({
  integrations: [tailwind()],
  site: 'https://barrientos.io',
  base: '/VictoryRoad',
  output: 'static',
  build: {
    assets: 'assets'
  }
});