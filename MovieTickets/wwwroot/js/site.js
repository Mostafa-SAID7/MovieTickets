// progressive DOM-safe handlers
(function () {
    // theme utilities (persist to localStorage)
    const root = document.documentElement;
    const themeIcon = document.getElementById('theme-icon');
    function applyTheme(t) {
        if (t === 'dark') {
            root.classList.add('dark');
            root.classList.remove('light');
            if (themeIcon) themeIcon.className = 'bi bi-sun-fill';
        } else {
            root.classList.remove('dark');
            root.classList.add('light');
            if (themeIcon) themeIcon.className = 'bi bi-moon-fill';
        }
    }
    // initial theme from localStorage or matchMedia
    const saved = localStorage.getItem('mt_theme');
    if (saved) applyTheme(saved);
    else applyTheme(window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');

    const themeToggle = document.getElementById('theme-toggle');
    const mobileThemeToggle = document.getElementById('mobile-theme-toggle');
    function toggleTheme() {
        const isDark = document.documentElement.classList.contains('dark');
        const next = isDark ? 'light' : 'dark';
        applyTheme(next);
        localStorage.setItem('mt_theme', next);
    }
    themeToggle?.addEventListener('click', toggleTheme);
    mobileThemeToggle?.addEventListener('click', function () { toggleTheme(); document.getElementById('mobile-menu')?.classList.toggle('hidden'); });

    // mobile menu toggle (accessible)
    const mobileToggle = document.getElementById('mobile-toggle');
    const mobileMenu = document.getElementById('mobile-menu');
    mobileToggle?.addEventListener('click', function () {
        const expanded = this.getAttribute('aria-expanded') === 'true';
        this.setAttribute('aria-expanded', (!expanded).toString());
        if (mobileMenu) {
            if (mobileMenu.hasAttribute('hidden')) {
                mobileMenu.removeAttribute('hidden');
                mobileMenu.classList.add('mobile-menu-enter');
            } else {
                mobileMenu.setAttribute('hidden', '');
            }
        }
    });

    // newsletter submit (demo)
    const newsletterForm = document.getElementById('newsletterForm');
    newsletterForm?.addEventListener('submit', function (e) {
        e.preventDefault();
        const email = document.getElementById('newsletterEmail')?.value;
        const msg = document.getElementById('newsletterMsg');
        if (!email || !/^\S+@\S+\.\S+$/.test(email)) {
            if (msg) { msg.textContent = 'Please enter a valid email.'; msg.className = 'text-danger'; }
            return;
        }
        // demo: fake success
        if (msg) { msg.textContent = 'Subscribed — check your inbox.'; msg.className = 'text-success'; }
        newsletterForm.reset();
    });
})();