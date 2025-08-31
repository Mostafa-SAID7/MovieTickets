// movies.js - vanilla JS
(() => {
    const debounce = (fn, delay = 300) => {
        let t;
        return (...args) => {
            clearTimeout(t);
            t = setTimeout(() => fn(...args), delay);
        };
    };

    const container = document.getElementById('movies-container');
    const searchInput = document.getElementById('live-search');
    const categorySelect = document.getElementById('filter-category');
    const cinemaSelect = document.getElementById('filter-cinema');
    const clearBtn = document.getElementById('clear-filters');

    const initial = window.__MOVIES_INITIAL || {};
    if (searchInput && initial.query) searchInput.value = initial.query;
    if (categorySelect && initial.categoryId) categorySelect.value = initial.categoryId;
    if (cinemaSelect && initial.cinemaId) cinemaSelect.value = initial.cinemaId;

    const getState = () => {
        return {
            q: searchInput ? searchInput.value.trim() : '',
            categoryId: categorySelect ? categorySelect.value : '',
            cinemaId: cinemaSelect ? cinemaSelect.value : '',
            page: 1
        };
    };

    const buildQuery = (state) => {
        const params = new URLSearchParams();
        if (state.q) params.set('searchString', state.q);
        if (state.categoryId) params.set('categoryId', state.categoryId);
        if (state.cinemaId) params.set('cinemaId', state.cinemaId);
        if (state.page) params.set('pageNumber', state.page);
        return params.toString();
    };

    const ajaxLoad = async (state, push = true) => {
        try {
            const qs = buildQuery(state);
            const url = `/Movies/Index?${qs}`;
            const resp = await fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            if (!resp.ok) throw new Error('Network error');

            const html = await resp.text();
            container.innerHTML = html;

            if (push) {
                const newUrl = `/Movies?${qs}`;
                window.history.pushState({ state }, '', newUrl);
            }

            attachPaginationHandlers();
        } catch (err) {
            console.error(err);
        }
    };

    const attachPaginationHandlers = () => {
        document.querySelectorAll('.js-page').forEach(a => {
            a.addEventListener('click', (e) => {
                e.preventDefault();
                const page = parseInt(a.getAttribute('data-page'), 10);
                if (!isFinite(page)) return;
                const st = getState(); st.page = page;
                ajaxLoad(st, true);
                window.scrollTo({ top: 0, behavior: 'smooth' });
            });
        });
    };

    const onFilterChange = debounce(() => {
        const st = getState();
        st.page = 1;
        ajaxLoad(st, true);
    }, 350);

    if (searchInput) searchInput.addEventListener('input', onFilterChange);
    if (categorySelect) categorySelect.addEventListener('change', onFilterChange);
    if (cinemaSelect) cinemaSelect.addEventListener('change', onFilterChange);
    if (clearBtn) clearBtn.addEventListener('click', (e) => {
        e.preventDefault();
        if (searchInput) searchInput.value = '';
        if (categorySelect) categorySelect.value = '';
        if (cinemaSelect) cinemaSelect.value = '';
        onFilterChange();
    });

    // back/forward
    window.addEventListener('popstate', (e) => {
        const params = new URLSearchParams(location.search);
        const s = {
            q: params.get('searchString') || '',
            categoryId: params.get('categoryId') || '',
            cinemaId: params.get('cinemaId') || '',
            page: parseInt(params.get('pageNumber') || '1', 10) || 1
        };

        if (searchInput) searchInput.value = s.q;
        if (categorySelect) categorySelect.value = s.categoryId;
        if (cinemaSelect) cinemaSelect.value = s.cinemaId;
        ajaxLoad(s, false);
    });

    // initial attach for server-rendered content
    attachPaginationHandlers();
})();
