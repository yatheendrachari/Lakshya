(function () {
    'use strict';

    /* ── Readiness ring animation ────────────────────────────────
       The partial renders an SVG circle with id="readinessCircle".
       We animate stroke-dashoffset on load and fix the track colour.
    ──────────────────────────────────────────────────────────────── */
    var circle = document.getElementById('readinessCircle');
    if (circle) {
        // fix track circle colour (it's hard-coded #e9ecef in the partial)
        var track = circle.previousElementSibling;
        if (track) track.setAttribute('stroke', 'rgba(255,255,255,.08)');

        // determine score from stroke colour class or data attribute
        // The partial sets stroke-dashoffset="251.2" (0%) on render —
        // we animate it to the correct offset based on the score text
        var scoreEl = circle.closest('.position-relative')
            .querySelector('.fw-bold.fs-5');
        var score = scoreEl ? parseInt(scoreEl.textContent) : 0;

        // circumference = 2π×40 ≈ 251.2
        var circ   = 251.2;
        var offset = circ - (circ * score / 100);

        // Fix stroke colour to match dark theme
        var green = '#34d399', amber = '#fbbf24', red = '#ff5e5e';
        circle.setAttribute('stroke', score >= 70 ? green : score >= 50 ? amber : red);

        // animate
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                circle.style.strokeDashoffset = offset;
            });
        });
    }

    /* ── Gap analysis submit button loading state ────────────────── */
    window.showLoading = function (btn) {
        setTimeout(function () {
            btn.disabled = true;
            btn.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2" role="status"></span>Analyzing&hellip;';
        }, 10);
    };

})();