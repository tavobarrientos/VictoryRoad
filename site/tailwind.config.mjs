/** @type {import('tailwindcss').Config} */
export default {
  content: ['./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}'],
  theme: {
    extend: {
      colors: {
        primary: '#3B82F6',
        secondary: '#8B5CF6',
        accent: '#EC4899',
        success: '#10B981',
        danger: '#EF4444',
        warning: '#F59E0B',
        pokemon: '#4F46E5',
        trainer: '#059669',
        energy: '#DC2626',
        gray: {
          600: '#4B5563',
          700: '#374151',
          800: '#1F2937',
        }
      },
      backgroundImage: {
        'header-gradient': 'linear-gradient(135deg, #667EEA 0%, #764BA2 100%)',
        'pokemon-gradient': 'linear-gradient(135deg, #6366F1 0%, #8B5CF6 100%)',
        'trainer-gradient': 'linear-gradient(135deg, #10B981 0%, #059669 100%)',
        'energy-gradient': 'linear-gradient(135deg, #F59E0B 0%, #DC2626 100%)',
      },
      animation: {
        'float': 'float 6s ease-in-out infinite',
        'float-delayed': 'float 6s ease-in-out 3s infinite',
        'fade-in': 'fadeIn 0.5s ease-in',
        'slide-up': 'slideUp 0.5s ease-out',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-20px)' },
        },
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        slideUp: {
          '0%': { transform: 'translateY(20px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
      },
    },
  },
  plugins: [],
}