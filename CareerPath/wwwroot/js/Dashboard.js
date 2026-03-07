(function () {
    'use strict';

    /* ── helpers ─────────────────────────────────────────────────────── */
    function readJson(id) {
        try {
            var el = document.getElementById(id);
            return el && el.textContent.trim() ? JSON.parse(el.textContent) : null;
        } catch (e) {
            console.warn('[dashboard] JSON parse failed for #' + id, e);
            return null;
        }
    }
    function safeNum(v, max) {
        var n = parseFloat(v);
        return isNaN(n) ? 0 : Math.min(Math.max(n, 0), max || 10);
    }

    /* ── Chart.js shared defaults ────────────────────────────────────── */
    var GRID_COLOR   = 'rgba(255,255,255,.06)';
    var TICK_COLOR   = '#9090c0';
    var FONT_FAMILY  = "'Instrument Sans', sans-serif";
    var TOOLTIP_BG   = '#111120';

    function tooltipDefaults() {
        return {
            backgroundColor: TOOLTIP_BG,
            borderColor: 'rgba(255,255,255,.1)',
            borderWidth: 1,
            titleColor: '#eaeaf4',
            bodyColor: '#9090c0',
            padding: 10
        };
    }

    /* ══════════════════════════════════════════════════════════════════
       1. RADAR CHART — 15 skill dimensions
    ══════════════════════════════════════════════════════════════════ */
    var rd = readJson('radarData');
    if (rd && document.getElementById('radarChart')) {
        new Chart(document.getElementById('radarChart'), {
            type: 'radar',
            data: {
                labels: [
                    'Coding', 'Communication', 'Problem Solving', 'Teamwork',
                    'Analytical', 'Presentation', 'Networking', 'Research',
                    'Internships', 'Projects', 'Leadership', 'Field Courses',
                    'Extracurricular', 'GPA', 'Confidence'
                ],
                datasets: [{
                    label: 'Your Profile',
                    data: [
                        safeNum(rd.codingSkills), safeNum(rd.communicationSkills),
                        safeNum(rd.problemSolvingSkills), safeNum(rd.teamworkSkills),
                        safeNum(rd.analyticalSkills), safeNum(rd.presentationSkills),
                        safeNum(rd.networkingSkills), safeNum(rd.researchExperience),
                        safeNum(rd.internships), safeNum(rd.projects),
                        safeNum(rd.leadershipPositions), safeNum(rd.fieldSpecificCourses),
                        safeNum(rd.extracurricularActivities), safeNum(rd.gpa),
                        safeNum(rd.overallConfidence)
                    ],
                    backgroundColor: 'rgba(91,141,238,.15)',
                    borderColor: 'rgba(91,141,238,.8)',
                    borderWidth: 2,
                    pointBackgroundColor: 'rgba(147,180,247,1)',
                    pointRadius: 3,
                    pointHoverRadius: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                scales: {
                    r: {
                        min: 0, max: 10,
                        ticks: { display: false, stepSize: 2 },
                        grid:        { color: GRID_COLOR },
                        angleLines:  { color: GRID_COLOR },
                        pointLabels: { color: TICK_COLOR, font: { size: 10, family: FONT_FAMILY } }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: Object.assign(tooltipDefaults(), {
                        callbacks: {
                            label: function (ctx) {
                                return ' ' + ctx.label + ': ' + ctx.parsed.r.toFixed(1) + ' / 10';
                            }
                        }
                    })
                },
                animation: { duration: 900, easing: 'easeOutQuart' }
            }
        });
    }

    /* ══════════════════════════════════════════════════════════════════
       2. BAR CHART — student vs ideal for top career
    ══════════════════════════════════════════════════════════════════ */
    var bd = readJson('barData');
    if (bd && bd.length && document.getElementById('barChart')) {
        new Chart(document.getElementById('barChart'), {
            type: 'bar',
            data: {
                labels: bd.map(function (x) { return x.skill || x.Skill; }),
                datasets: [
                    {
                        label: 'Your Level',
                        data: bd.map(function (x) { return safeNum(x.studentValue || x.StudentValue, 100); }),
                        backgroundColor: 'rgba(91,141,238,.7)',
                        borderRadius: 4,
                        borderSkipped: false
                    },
                    {
                        label: 'Ideal Level',
                        data: bd.map(function (x) { return safeNum(x.idealValue || x.IdealValue, 100); }),
                        backgroundColor: 'rgba(52,211,153,.35)',
                        borderRadius: 4,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                scales: {
                    x: {
                        grid: { color: GRID_COLOR },
                        ticks: { color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 } },
                        beginAtZero: true
                    },
                    y: {
                        grid: { display: false },
                        ticks: { color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 } }
                    }
                },
                plugins: {
                    legend: {
                        labels: { color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 }, boxWidth: 12 }
                    },
                    tooltip: tooltipDefaults()
                },
                animation: { duration: 800, easing: 'easeOutQuart' }
            }
        });
    }

    /* ══════════════════════════════════════════════════════════════════
       3. CONFIDENCE CHART — per skill confidence score (%)
    ══════════════════════════════════════════════════════════════════ */
    var cd = readJson('confidenceData');
    if (cd && cd.labels && cd.labels.length && document.getElementById('confidenceChart')) {
        new Chart(document.getElementById('confidenceChart'), {
            type: 'bar',
            data: {
                labels: cd.labels,
                datasets: [{
                    label: 'Data Confidence %',
                    data: cd.values,
                    backgroundColor: cd.values.map(function (v) {
                        return v >= 80 ? 'rgba(52,211,153,.7)'
                            : v >= 50 ? 'rgba(251,191,36,.7)'
                                :            'rgba(248,113,113,.7)';
                    }),
                    borderRadius: 4,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                scales: {
                    x: {
                        min: 0, max: 100,
                        grid: { color: GRID_COLOR },
                        ticks: {
                            color: TICK_COLOR,
                            font: { family: FONT_FAMILY, size: 11 },
                            callback: function (v) { return v + '%'; }
                        }
                    },
                    y: {
                        grid: { display: false },
                        ticks: { color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 } }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: Object.assign(tooltipDefaults(), {
                        callbacks: {
                            label: function (ctx) { return ' Confidence: ' + ctx.parsed.x.toFixed(1) + '%'; }
                        }
                    })
                },
                animation: { duration: 800, easing: 'easeOutQuart' }
            }
        });
    }

    /* ══════════════════════════════════════════════════════════════════
       4. READINESS TIMELINE — line chart over multiple analyses
    ══════════════════════════════════════════════════════════════════ */
    var td = readJson('timelineData');
    if (td && td.length > 1 && document.getElementById('timelineChart')) {
        new Chart(document.getElementById('timelineChart'), {
            type: 'line',
            data: {
                labels: td.map(function (p) { return p.date || p.Date; }),
                datasets: [{
                    label: 'Readiness %',
                    data: td.map(function (p) { return safeNum(p.readiness || p.Readiness, 100); }),
                    borderColor: 'rgba(52,211,153,.9)',
                    backgroundColor: 'rgba(52,211,153,.08)',
                    borderWidth: 2.5,
                    pointBackgroundColor: '#34d399',
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                scales: {
                    y: {
                        min: 0, max: 100,
                        grid: { color: GRID_COLOR },
                        ticks: {
                            color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 },
                            callback: function (v) { return v + '%'; }
                        }
                    },
                    x: {
                        grid: { display: false },
                        ticks: { color: TICK_COLOR, font: { family: FONT_FAMILY, size: 11 } }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: Object.assign(tooltipDefaults(), {
                        callbacks: {
                            label: function (ctx) { return ' Readiness: ' + ctx.parsed.y.toFixed(1) + '%'; }
                        }
                    })
                },
                animation: { duration: 900, easing: 'easeOutQuart' }
            }
        });
    }

    /* ══════════════════════════════════════════════════════════════════
       5. ANIMATED BARS / RINGS on page load
    ══════════════════════════════════════════════════════════════════ */
    function animateWidth(id, dataAttr) {
        var el = document.getElementById(id);
        if (!el) return;
        var w = parseFloat(el.dataset[dataAttr] || el.getAttribute('data-width') || 0);
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                el.style.width = Math.min(w, 100) + '%';
            });
        });
    }
    animateWidth('readinessBar',  'width');
    animateWidth('lpProgressFill','width');

    var ring = document.getElementById('gapRingFill');
    if (ring) {
        var offset = parseFloat(ring.dataset.offset || ring.getAttribute('data-offset') || 220);
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                ring.style.strokeDashoffset = offset;
            });
        });
    }

    /* ══════════════════════════════════════════════════════════════════
       6. MODULE TOGGLE — AJAX POST to Dashboard/ToggleModule
    ══════════════════════════════════════════════════════════════════ */
    var toggleUrlEl = document.getElementById('toggleUrl');
    var toggleUrl   = toggleUrlEl ? JSON.parse(toggleUrlEl.textContent) : null;

    // Read antiforgery token from meta tag
    function getAntiforgeryToken() {
        var meta = document.querySelector('meta[name="request-verification-token"]');
        return meta ? meta.content : '';
    }

    var totalModules     = 0;
    var completedModules = 0;

    // Count initial state from DOM
    document.querySelectorAll('.module-row').forEach(function (row) {
        totalModules++;
        if (row.classList.contains('mod-done')) completedModules++;
    });

    window.toggleModule = function (moduleId, btn) {
        if (!toggleUrl) return;

        btn.disabled = true;

        fetch(toggleUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getAntiforgeryToken()
            },
            body: '__RequestVerificationToken=' + encodeURIComponent(getAntiforgeryToken())
                + '&moduleId=' + encodeURIComponent(moduleId)
        })
            .then(function (r) { return r.json(); })
            .then(function (data) {
                if (!data.success) throw new Error(data.error || 'Toggle failed');

                var row = document.getElementById('modrow-' + moduleId);
                if (!row) return;

                if (data.isCompleted) {
                    row.classList.add('mod-done');
                    btn.classList.add('checked');
                    btn.querySelector('.mod-check-icon').textContent = '✓';
                    completedModules++;
                } else {
                    row.classList.remove('mod-done');
                    btn.classList.remove('checked');
                    btn.querySelector('.mod-check-icon').textContent = '';
                    completedModules = Math.max(0, completedModules - 1);
                }

                updateProgress();
            })
            .catch(function (err) {
                console.error('[dashboard] Toggle error:', err);
            })
            .finally(function () {
                btn.disabled = false;
            });
    };

    function updateProgress() {
        if (totalModules === 0) return;
        var pct = completedModules / totalModules * 100;

        // update number display
        var numEl = document.querySelector('.lp-prog-num');
        if (numEl) numEl.textContent = completedModules;

        // update % display
        var pctEl = document.querySelector('.lp-prog-pct');
        if (pctEl) pctEl.textContent = Math.round(pct) + '%';

        // update bar
        var fill = document.getElementById('lpProgressFill');
        if (fill) fill.style.width = Math.min(pct, 100) + '%';
    }

    /* ══════════════════════════════════════════════════════════════════
       7. UPDATE PROFILE MODAL
    ══════════════════════════════════════════════════════════════════ */
    var modal = document.getElementById('updateModal');

    window.openUpdateModal = function () {
        if (modal) {
            modal.classList.add('open');
            document.body.style.overflow = 'hidden';
        }
    };

    window.closeUpdateModal = function () {
        if (modal) {
            modal.classList.remove('open');
            document.body.style.overflow = '';
        }
    };

    // close on Escape
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeUpdateModal();
    });

})();