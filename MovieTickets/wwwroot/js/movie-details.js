// movie-details.js - thumbnails, lightbox, trailer embed, lazy images
(() => {
    // helpers
    const qs = sel => document.querySelector(sel);
    const qsa = sel => Array.from(document.querySelectorAll(sel));

    // Lazy load images (data-src)
    const lazyObserver = new IntersectionObserver((entries) => {
        entries.forEach(en => {
            if (en.isIntersecting) {
                const img = en.target;
                const data = img.getAttribute('data-src');
                if (data) { img.src = data; img.removeAttribute('data-src'); }
                lazyObserver.unobserve(img);
            }
        });
    }, { rootMargin: '150px' });

    qsa('img.lazy').forEach(img => lazyObserver.observe(img));

    // Thumbnails -> carousel sync
    const thumbs = qsa('.thumb');
    const carouselEl = qs('#galleryCarousel');
    const carousel = bootstrap.Carousel && carouselEl ? new bootstrap.Carousel(carouselEl, { interval: false, ride: false }) : null;

    function setActiveThumb(index) {
        thumbs.forEach(t => t.classList.remove('active'));
        const target = thumbs.find(t => parseInt(t.getAttribute('data-slide-index'), 10) === index);
        if (target) target.classList.add('active');
    }

    // attach click
    thumbs.forEach(btn => {
        btn.addEventListener('click', (e) => {
            const idx = parseInt(btn.getAttribute('data-slide-index'), 10);
            if (!isNaN(idx) && carousel) {
                carousel.to(idx);
            }
        });
        // lazy thumb image
        const img = btn.querySelector('img.lazy');
        if (img) lazyObserver.observe(img);
    });

    // update active thumb on slide
    if (carouselEl) {
        carouselEl.addEventListener('slid.bs.carousel', function (ev) {
            setActiveThumb(ev.to ?? ev.relatedTarget?.getAttribute('data-bs-slide-to') ?? 0);
            // update mainPoster src to match slide image for visual continuity
            const activeImg = carouselEl.querySelector('.carousel-item.active img');
            if (activeImg) {
                qs('#mainPoster').src = activeImg.getAttribute('data-src') || activeImg.src;
            }
        });
    }
    // initialize active thumb
    setActiveThumb(0);

    // Lightbox open on thumbnail or gallery click
    const lightboxModal = qs('#lightboxModal');
    const lightboxImg = qs('#lightboxImg');
    const bsLightbox = bootstrap.Modal && lightboxModal ? new bootstrap.Modal(lightboxModal, {}) : null;

    qsa('.gallery-img, .thumb-img').forEach(el => {
        el.addEventListener('click', function (ev) {
            const src = this.getAttribute('data-src') || this.src;
            if (src) {
                lightboxImg.src = src;
                bsLightbox?.show();
            }
        });
    });

    // Trailer handling (convert youtube/vimeo)
    const playBtn = qs('#playTrailerBtn');
    const trailerModalEl = qs('#trailerModal');
    const trailerIframe = qs('#trailerIframe');
    const bsTrailer = bootstrap.Modal && trailerModalEl ? new bootstrap.Modal(trailerModalEl, {}) : null;

    function toEmbedUrl(rawUrl) {
        if (!rawUrl) return '';
        try {
            const u = new URL(rawUrl);
            if (u.hostname.includes('youtube.com')) {
                const v = u.searchParams.get('v');
                if (v) return `https://www.youtube.com/embed/${v}?autoplay=1&rel=0`;
            }
            if (u.hostname.includes('youtu.be')) {
                const id = u.pathname.slice(1);
                if (id) return `https://www.youtube.com/embed/${id}?autoplay=1&rel=0`;
            }
            if (u.hostname.includes('vimeo.com')) {
                const id = u.pathname.split('/').filter(Boolean).pop();
                if (id) return `https://player.vimeo.com/video/${id}?autoplay=1`;
            }
            return rawUrl;
        } catch (e) {
            return '';
        }
    }

    if (playBtn) {
        playBtn.addEventListener('click', () => {
            const raw = playBtn.getAttribute('data-trailer');
            const embed = toEmbedUrl(raw);
            if (embed && trailerIframe) {
                trailerIframe.src = embed;
                bsTrailer?.show();
            } else {
                // optional: show toast / alert
                alert('Trailer not available or invalid URL.');
            }
        });
    }

    // clear trailer on close
    if (trailerModalEl) {
        trailerModalEl.addEventListener('hidden.bs.modal', () => {
            if (trailerIframe) trailerIframe.src = '';
        });
    }

    // wishlist toggle (local UI demo)
    const wishBtn = qs('#wishBtn');
    if (wishBtn) {
        wishBtn.addEventListener('click', () => {
            const pressed = wishBtn.getAttribute('aria-pressed') === 'true';
            wishBtn.setAttribute('aria-pressed', (!pressed).toString());
            wishBtn.classList.toggle('btn-outline-secondary');
            wishBtn.classList.toggle('btn-danger');
            wishBtn.querySelector('i')?.classList.toggle('bi-heart');
            wishBtn.querySelector('i')?.classList.toggle('bi-heart-fill');
            // TODO: call backend to save wishlist
        });
    }

    // bootstrap form validation (booking)
    (function () {
        'use strict';
        var forms = document.querySelectorAll('.needs-validation');
        Array.prototype.slice.call(forms).forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    })();

})();
