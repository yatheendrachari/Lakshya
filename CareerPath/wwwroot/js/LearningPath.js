(function () {
    'use strict';

    /* ══════════════════════════════════════════════════════════════════
       DATA — read the server-injected JSON blob.
       The Razor view writes:
         <script id="modulesData" type="application/json">[...]</script>
       Using type="application/json" means the browser never executes it,
       so it's XSS-safe.  We just JSON.parse the textContent.
    ══════════════════════════════════════════════════════════════════ */
    var modules = [];

    try {
        var dataEl = document.getElementById('modulesData');
        if (dataEl && dataEl.textContent.trim()) {
            modules = JSON.parse(dataEl.textContent) || [];
        }
    } catch (e) {
        console.warn('[learningpath] Failed to parse modulesData:', e);
        modules = [];
    }

    /* ══════════════════════════════════════════════════════════════════
       PUBLIC — called by onclick="selectModule(idx)" in the Razor view
    ══════════════════════════════════════════════════════════════════ */
    window.selectModule = function (idx) {
        if (typeof idx !== 'number' || idx < 0 || idx >= modules.length) {
            console.warn('[learningpath] selectModule: invalid index', idx);
            return;
        }

        /* ── highlight active module ─────────────────────────────────── */
        document.querySelectorAll('.rm-module').forEach(function (el) {
            el.classList.remove('active');
        });
        var target = document.getElementById('module-' + idx);
        if (target) target.classList.add('active');

        /* ── update label ────────────────────────────────────────────── */
        var labelEl = document.getElementById('resLabel');
        if (labelEl) {
            var name = (modules[idx] && modules[idx].concept) ? modules[idx].concept : '—';
            labelEl.innerHTML = 'Module: <span>' + esc(name) + '</span>';
        }

        /* ── render resources ────────────────────────────────────────── */
        renderResources(modules[idx] ? modules[idx].resource : null);

        /* ── smooth scroll to resources ──────────────────────────────── */
        var resSection = document.getElementById('resourcesSection');
        if (resSection) {
            resSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    };

    /* ══════════════════════════════════════════════════════════════════
       RENDER — writes HTML into #resColumns
    ══════════════════════════════════════════════════════════════════ */
    function renderResources(res) {
        var col = document.getElementById('resColumns');
        if (!col) return;

        /* no resources at all */
        if (!res) {
            col.innerHTML =
                '<div style="grid-column:1/-1;text-align:center;' +
                'color:var(--muted);padding:3rem 1rem;font-size:.85rem">' +
                'No resources available for this module.</div>';
            return;
        }

        var videos   = safeArr(res.videos);
        var courses  = safeArr(res.courses);
        var articles = safeArr(res.articles);
        var papers   = safeArr(res.papers);

        col.innerHTML =
            renderCol('🎬', 'Videos',   videos,   renderVideoCard)  +
            renderCol('📚', 'Courses',  courses,  renderOtherCard)  +
            renderCol('📰', 'Articles', articles, renderOtherCard)  +
            renderCol('📄', 'Papers',   papers,   renderOtherCard);
    }

    /* ─── column wrapper ─────────────────────────────────────────── */
    function renderCol(icon, label, items, cardFn) {
        var inner = items.length
            ? items.map(function (item) {
                try { return cardFn(item); }
                catch (e) { console.warn('[learningpath] card render error:', e); return ''; }
            }).join('')
            : '<div class="res-empty-col">None available</div>';

        return '<div>' +
            '<div class="res-col-head">' +
            icon + ' ' + label +
            ' <span class="col-count">' + items.length + '</span>' +
            '</div>' +
            inner +
            '</div>';
    }

    /* ─── video card ─────────────────────────────────────────────── */
    function renderVideoCard(v) {
        var thumb = safeStr(v.thumbnail)
            ? '<img class="vid-thumb" src="' + esc(v.thumbnail) + '" alt="" loading="lazy"/>'
            : '';

        var freshBadge = v.isFresh
            ? '<span class="fresh-badge">✦ Fresh</span>'
            : '';

        var viewsStr = v.views ? formatViews(v.views) + ' views' : '';

        var metaParts = [
            safeStr(v.source)  ? '<span class="res-card-source">' + esc(v.source)  + '</span>' : '',
            safeStr(v.channel) ? '<span>'                         + esc(v.channel) + '</span>' : '',
            viewsStr           ? '<span>'                         + esc(viewsStr)  + '</span>' : '',
            freshBadge
        ].filter(Boolean).join('');

        var href = safeStr(v.url) ? esc(v.url) : '#';

        return '<div class="res-card">' +
            '<a href="' + href + '" target="_blank" rel="noopener noreferrer">' +
            thumb +
            '<div class="res-card-title">' + esc(safeStr(v.title) || 'Untitled') + '</div>' +
            (metaParts ? '<div class="res-card-meta">' + metaParts + '</div>' : '') +
            (safeStr(v.description)
                ? '<div class="res-card-desc">' + esc(v.description) + '</div>'
                : '') +
            '</a>' +
            '</div>';
    }

    /* ─── course / article / paper card ─────────────────────────── */
    function renderOtherCard(r) {
        var href = safeStr(r.url) ? esc(r.url) : '#';

        var meta = safeStr(r.source)
            ? '<div class="res-card-meta"><span class="res-card-source">' + esc(r.source) + '</span></div>'
            : '';

        return '<div class="res-card">' +
            '<a href="' + href + '" target="_blank" rel="noopener noreferrer">' +
            '<div class="res-card-title">' + esc(safeStr(r.title) || 'Untitled') + '</div>' +
            meta +
            (safeStr(r.description)
                ? '<div class="res-card-desc">' + esc(r.description) + '</div>'
                : '') +
            '</a>' +
            '</div>';
    }

    /* ══════════════════════════════════════════════════════════════════
       HELPERS
    ══════════════════════════════════════════════════════════════════ */

    /** Always return an array, even if input is null/undefined/not-array */
    function safeArr(val) {
        return Array.isArray(val) ? val : [];
    }

    /** Return trimmed string or empty string for null/undefined */
    function safeStr(val) {
        if (val === null || val === undefined) return '';
        return String(val).trim();
    }

    /** Escape HTML special chars to prevent XSS */
    function esc(s) {
        return safeStr(s)
            .replace(/&/g,  '&amp;')
            .replace(/</g,  '&lt;')
            .replace(/>/g,  '&gt;')
            .replace(/"/g,  '&quot;')
            .replace(/'/g,  '&#39;');
    }

    /** Format large view counts: 1200000 → "1.2M", 45000 → "45K" */
    function formatViews(n) {
        var num = parseInt(n, 10);
        if (isNaN(num) || num <= 0) return '';
        if (num >= 1000000) return (num / 1000000).toFixed(1).replace(/\.0$/, '') + 'M';
        if (num >= 1000)    return (num / 1000)   .toFixed(1).replace(/\.0$/, '') + 'K';
        return String(num);
    }

})();