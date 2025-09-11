import { defineConfig } from 'astro/config';
import tailwind from '@astrojs/tailwind';

export default defineConfig({
  integrations: [tailwind()],
  site: 'https://barrientos.io',
  base: '/victory-road',
  output: 'static',
  build: {
    assets: 'assets'
  }
});