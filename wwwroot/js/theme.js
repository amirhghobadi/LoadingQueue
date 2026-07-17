/* ====================================
   THEME MANAGEMENT
   ==================================== */

// Initialize theme from localStorage
(function initTheme() {
    const savedTheme = localStorage.getItem('app-theme') || 'light';
    applyTheme(savedTheme);
})();

function applyTheme(theme) {
    if (theme === 'dark') {
        document.documentElement.setAttribute('data-theme', 'dark');
    } else {
        document.documentElement.removeAttribute('data-theme');
    }
    localStorage.setItem('app-theme', theme);
    updateThemeToggleButton(theme);
}

function toggleTheme() {
    const currentTheme = localStorage.getItem('app-theme') || 'light';
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    applyTheme(newTheme);
}

function updateThemeToggleButton(theme) {
    const btn = document.getElementById('themeToggleBtn');
    if (btn) {
        if (theme === 'dark') {
            btn.innerHTML = '☀️ Light';
            btn.classList.add('active');
        } else {
            btn.innerHTML = '🌙 Dark';
            btn.classList.remove('active');
        }
    }
}

/* ====================================
   PAGE LOADING ANIMATION
   ==================================== */

function showPageLoader() {
    const loader = document.createElement('div');
    loader.className = 'page-loader animate-fade-in';
    loader.id = 'pageLoader';
    loader.innerHTML = `
        <div>
            <div class="loader-spinner"></div>
            <div class="loader-text">جاری است...</div>
        </div>
    `;
    document.body.appendChild(loader);
}

function hidePageLoader() {
    const loader = document.getElementById('pageLoader');
    if (loader) {
        loader.style.animation = 'fadeIn 0.3s ease-out reverse';
        setTimeout(() => loader.remove(), 300);
    }
}

// Show loader on page load
window.addEventListener('load', () => {
    setTimeout(hidePageLoader, 800);
});
