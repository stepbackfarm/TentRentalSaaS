module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        'text-base'  : 'var(--color-text-base)',
        'text-muted' : 'var(--color-text-muted)',
        'bg-base'    : 'var(--color-bg-base)',
        'bg-muted'   : 'var(--color-bg-muted)',
        'primary'    : 'var(--color-primary)',
        'secondary'  : 'var(--color-secondary)',
        'accent'     : 'var(--color-accent)',
        'danger'     : 'var(--color-danger)',
      }
    },
  },
  plugins: [],
}
