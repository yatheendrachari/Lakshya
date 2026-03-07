// // careerpath.js — wwwroot/js/careerpath.js
//
// /* ── Career data ───────────────────────────────────────────────────
//    In MVC the Razor view injects window.careerData BEFORE this script
//    loads (see @section Scripts in Index.cshtml).
//    The block below is a DEMO FALLBACK for standalone HTML use only —
//    it is skipped automatically when window.careerData is already set.
// ──────────────────────────────────────────────────────────────────── */
// if (!window.careerData) {
//     window.careerData = [
//         {
//             rank:1, name:"Software Engineer", slug:"software-engineer",
//             match:94, field:"Technology", subField:"Full-Stack Development",
//             strengths:["Analytical Thinking","Problem Solving","Logical Reasoning"],
//             gaps:[
//                 { name:"System Design",   advice:"Study distributed systems and practice with design docs." },
//                 { name:"Cloud Platforms", advice:"Earn an AWS or GCP associate-level certification." }
//             ],
//             salary:"$120K – $160K", avgSalary:"$140K",
//             demand:"High", growth:"+25% by 2030",
//             degree:"Bachelor's in CS or equivalent",
//             experience:"Entry to Mid-level", workMode:"Hybrid / Remote",
//             readiness:70, timeToReady:"6 months"
//         },
//         {
//             rank:2, name:"Data Scientist", slug:"data-scientist",
//             match:89, field:"Analytics", subField:"Predictive Modelling",
//             strengths:["Statistical Reasoning","Data Visualisation","Mathematical Aptitude"],
//             gaps:[
//                 { name:"Machine Learning",         advice:"Complete Andrew Ng's ML Specialisation on Coursera." },
//                 { name:"Deep Learning Frameworks", advice:"Build 3 projects using PyTorch or TensorFlow." }
//             ],
//             salary:"$110K – $145K", avgSalary:"$128K",
//             demand:"High", growth:"+35% by 2030",
//             degree:"Master's in Statistics / Data Science",
//             experience:"Mid-level", workMode:"Hybrid",
//             readiness:60, timeToReady:"10 months"
//         },
//         {
//             rank:3, name:"UX Product Designer", slug:"ux-product-designer",
//             match:85, field:"Design", subField:"Human-Computer Interaction",
//             strengths:["Creative Thinking","User Empathy","Visual Communication"],
//             gaps:[
//                 { name:"Prototyping Tools",    advice:"Master Figma advanced features — auto-layout and variables." },
//                 { name:"User Research Methods",advice:"Practice moderated usability testing and synthesise findings." }
//             ],
//             salary:"$105K – $135K", avgSalary:"$120K",
//             demand:"Medium", growth:"+15% by 2030",
//             degree:"Bachelor's in Design / HCI",
//             experience:"Entry to Mid-level", workMode:"On-site / Hybrid",
//             readiness:55, timeToReady:"12 months"
//         },
//         {
//             rank:4, name:"DevOps Engineer", slug:"devops-engineer",
//             match:82, field:"Infrastructure", subField:"CI/CD & Cloud Ops",
//             strengths:["Systems Thinking","Automation Mindset","Attention to Detail"],
//             gaps:[
//                 { name:"Cloud Architecture",advice:"Get AWS Solutions Architect Associate certification." },
//                 { name:"Kubernetes",        advice:"Deploy a production-grade cluster on a personal project." }
//             ],
//             salary:"$115K – $150K", avgSalary:"$133K",
//             demand:"High", growth:"+28% by 2030",
//             degree:"Bachelor's in CS / Engineering",
//             experience:"Mid-level", workMode:"Remote-first",
//             readiness:75, timeToReady:"4 months"
//         }
//     ];
// }
//
// /* ── Reveal handler ─────────────────────────────────────────────── */
// function handleReveal() {
//     const btn     = document.getElementById('generateBtn');
//     const overlay = document.getElementById('revealOverlay');
//     const below   = document.getElementById('belowReveal');
//     const cards   = document.querySelectorAll('.c-card');
//
//     /* Loading state on button */
//     btn.classList.add('loading');
//     btn.disabled = true;
//
//     /* Simulate API call — replace with real form POST in MVC */
//     setTimeout(() => {
//
//         /* 1. Remove overlay (frosted glass fades out) */
//         overlay.classList.add('gone');
//
//         /* 2. Reveal each card with staggered pop-in */
//         cards.forEach(card => {
//             /* Remove blur inline style — CSS handles the rest */
//             card.classList.add('revealed');
//         });
//
//         /* 3. Show comparison + next steps below */
//         setTimeout(() => {
//             below.classList.add('visible');
//
//             /* Animate readiness bars once visible */
//             document.querySelectorAll('.rbar-fill').forEach(bar => {
//                 const w = bar.style.width;
//                 bar.style.width = '0';
//                 requestAnimationFrame(() => {
//                     requestAnimationFrame(() => { bar.style.width = w; });
//                 });
//             });
//
//             /* Scroll so comparison is in view */
//             below.scrollIntoView({ behavior:'smooth', block:'start' });
//         }, 450);
//
//     }, 1400); /* ← 1.4s simulated API delay */
// }
//
// /* ── Split Detail Overlay ────────────────────────────────────────── */
// function openPanel(rank) {
//     const d = (window.careerData || []).find(c => c.rank === rank);
//     if (!d) return;
//
//     document.getElementById('detailLeft').innerHTML  = buildMiniCard(d);
//     document.getElementById('detailHeadTitle').textContent = d.name;
//     document.getElementById('detailBody').innerHTML  = buildDetailContent(d);
//
//     const overlay = document.getElementById('detailOverlay');
//     overlay.classList.add('open');
//     document.body.style.overflow = 'hidden';
//
//     /* animate readiness bar in mini card */
//     requestAnimationFrame(() => {
//         requestAnimationFrame(() => {
//             const fill = overlay.querySelector('.mini-rbar-fill');
//             if (fill) fill.style.width = fill.dataset.width;
//         });
//     });
//
//     overlay.querySelector('.detail-right').scrollTop = 0;
//     overlay.querySelector('.detail-right').focus();
// }
//
// function closePanel() {
//     document.getElementById('detailOverlay').classList.remove('open');
//     document.body.style.overflow = '';
// }
//
// document.addEventListener('keydown', e => {
//     if (e.key === 'Escape') closePanel();
// });
//
// /* ── Mini card (left column) ─────────────────────────────────────── */
// function buildMiniCard(d) {
//     const dc = d.demand === 'High' ? 'd-high' : d.demand === 'Medium' ? 'd-medium' : 'd-low';
//     return `
//     <div class="mini-card" data-rank="${d.rank}">
//       <div class="mini-rank">#${d.rank} Match</div>
//
//       <div class="mini-score">${d.match}<sup>%</sup></div>
//       <div class="mini-score-label">Match Score</div>
//
//       <div class="mini-name">${esc(d.name)}</div>
//       <div class="mini-field">${esc(d.field)} &middot; ${esc(d.subField)}</div>
//
//       <hr class="mini-divider"/>
//
//       <div class="mini-stat">
//         <span class="mini-stat-l">Salary</span>
//         <span class="mini-stat-v">${esc(d.salary)}</span>
//       </div>
//       <div class="mini-stat">
//         <span class="mini-stat-l">Job Growth</span>
//         <span class="mini-stat-v" style="color:var(--emerald);">${esc(d.growth)}</span>
//       </div>
//       <div class="mini-stat">
//         <span class="mini-stat-l">Demand</span>
//         <span class="demand-chip ${dc}" style="font-size:.62rem;">${esc(d.demand)}</span>
//       </div>
//       <div class="mini-stat">
//         <span class="mini-stat-l">Work Mode</span>
//         <span class="mini-stat-v">${esc(d.workMode)}</span>
//       </div>
//
//       <div class="mini-readiness-wrap">
//         <div class="mini-readiness-label">
//           <span>Your Readiness</span><span>${d.readiness}%</span>
//         </div>
//         <div class="mini-rbar">
//           <div class="mini-rbar-fill" data-width="${d.readiness}%" style="width:0%"></div>
//         </div>
//       </div>
//     </div>`;
// }
//
// /* ── Full career detail (right column) ───────────────────────────── */
// function buildDetailContent(d) {
//     const dc = d.demand === 'High' ? 'd-high' : d.demand === 'Medium' ? 'd-medium' : 'd-low';
//
//     /* strengths */
//     const strsHTML = (d.strengths || []).map(s => `
//     <div class="dr-str-row"><span>✓</span><span>${esc(s)}</span></div>`).join('');
//
//     /* gaps */
//     const gapsHTML = (d.gaps || []).map(g => `
//     <div class="dr-gap-row">
//       <span>⚠</span>
//       <div>
//         <div style="font-weight:600;">${esc(g.name)}</div>
//         <div class="dr-gap-adv">${esc(g.advice)}</div>
//       </div>
//     </div>`).join('');
//
//     /* responsibilities */
//     const respHTML = (d.responsibilities || []).length
//         ? `<div class="dr-sec">
//         <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>What You Will Do</div>
//         <ul class="resp-list">
//           ${(d.responsibilities).map(r => `<li>${esc(r)}</li>`).join('')}
//         </ul>
//       </div>`
//         : '';
//
//     /* required skills */
//     const skillsHTML = (d.requiredSkills || []).length
//         ? `<div class="dr-sec">
//         <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Skills Required</div>
//         <div class="skills-grid">
//           ${(d.requiredSkills).map(s => `
//             <div class="skill-chip">
//               <div class="skill-chip-name">
//                 ${esc(s.name)}
//                 <span class="skill-chip-level">${s.level}/4</span>
//               </div>
//               <div class="skill-bar">
//                 <div class="skill-bar-fill" style="width:${s.level * 25}%"></div>
//               </div>
//               <div class="skill-label">${esc(s.levelLabel)}</div>
//             </div>`).join('')}
//         </div>
//       </div>`
//         : '';
//
//     /* career progression */
//     const progHTML = (d.careerProgression || []).length
//         ? `<div class="dr-sec">
//         <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Typical Career Progression</div>
//         <div class="prog-steps">
//           ${(d.careerProgression).map((step, i, arr) => `
//             <div class="prog-step">
//               <div class="prog-step-years">${esc(step.yearsRange)}</div>
//               <div class="prog-step-title">${esc(step.title)}</div>
//             </div>
//             ${i < arr.length - 1 ? '<span class="prog-arrow">→</span>' : ''}`).join('')}
//         </div>
//       </div>`
//         : '';
//
//     /* top companies */
//     const companiesHTML = (d.topCompanies || []).length
//         ? `<div class="dr-sec">
//         <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Top Companies Hiring</div>
//         <div class="company-chips">
//           ${(d.topCompanies).map(c => `<span class="company-chip">${esc(c)}</span>`).join('')}
//         </div>
//       </div>`
//         : '';
//
//     /* description */
//     const descHTML = d.description
//         ? `<div class="dr-sec">
//         <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>About This Career</div>
//         <p style="font-size:.9rem;color:var(--slate);line-height:1.7;">${esc(d.description)}</p>
//       </div>`
//         : '';
//
//     return `
//     <!-- quick stats -->
//     <div class="dr-stats">
//       <div class="dr-stat-card">
//         <div class="dr-stat-val blue">${esc(d.avgSalary || d.salary)}</div>
//         <div class="dr-stat-label">Avg Salary</div>
//       </div>
//       <div class="dr-stat-card">
//         <div class="dr-stat-val green">${esc(d.growth)}</div>
//         <div class="dr-stat-label">Job Growth</div>
//       </div>
//       <div class="dr-stat-card">
//         <div class="dr-stat-val amber">${esc(d.timeToReady)}</div>
//         <div class="dr-stat-label">Time to Ready</div>
//       </div>
//       <div class="dr-stat-card">
//         <div class="dr-stat-val">${esc(d.experience)}</div>
//         <div class="dr-stat-label">Experience</div>
//       </div>
//     </div>
//
//     <!-- description -->
//     ${descHTML}
//
//     <!-- responsibilities -->
//     ${respHTML}
//
//     <!-- strengths for this career -->
//     ${strsHTML ? `<div class="dr-sec">
//       <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Your Strengths for This Role</div>
//       ${strsHTML}
//     </div>` : ''}
//
//     <!-- gaps to close -->
//     ${gapsHTML ? `<div class="dr-sec">
//       <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Skills to Develop</div>
//       ${gapsHTML}
//     </div>` : ''}
//
//     <!-- required skills -->
//     ${skillsHTML}
//
//     <!-- progression -->
//     ${progHTML}
//
//     <!-- companies -->
//     ${companiesHTML}
//
//     <!-- CTA -->
//     <div class="dr-cta">
//       <div class="dr-cta-icon">🎯</div>
//       <h5>Am I Ready for This Career?</h5>
//       <p>Get a detailed gap analysis — your readiness score, critical skills to close, and a personalised learning roadmap.</p>
//       <a href="/Student/Careers/Details/${esc(d.slug)}" class="btn-gen" style="text-decoration:none;display:inline-flex;">
//         <span class="b-icon">→</span>
//         <span class="b-label">Run Full Gap Analysis</span>
//       </a>
//     </div>
//   `;
// }
//
// function esc(s) {
//     const d = document.createElement('div');
//     d.appendChild(document.createTextNode(String(s)));
//     return d.innerHTML;
// }



// careerpath.js — wwwroot/js/careerpath.js
// window.careerData is injected by the Razor view BEFORE this script loads.
// See @section Scripts in Index.cshtml.

/* ── Reveal handler (for the generate button on GET page) ──────── */
function handleReveal() {
    const btn     = document.getElementById('generateBtn');
    const overlay = document.getElementById('revealOverlay');
    const below   = document.getElementById('belowReveal');
    const cards   = document.querySelectorAll('.c-card');

    if (!btn || !overlay) return;

    btn.classList.add('loading');
    btn.disabled = true;

    setTimeout(() => {
        overlay.classList.add('gone');

        cards.forEach(card => card.classList.add('revealed'));

        setTimeout(() => {
            if (below) {
                below.classList.add('visible');

                document.querySelectorAll('.rbar-fill').forEach(bar => {
                    const w = bar.style.width;
                    bar.style.width = '0';
                    requestAnimationFrame(() => {
                        requestAnimationFrame(() => { bar.style.width = w; });
                    });
                });

                below.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }, 450);
    }, 1400);
}

/* ── Split detail overlay ──────────────────────────────────────── */
function openPanel(rank) {
    const data = window.careerData || [];
    const d    = data.find(c => c.rank === rank);
    if (!d) {
        console.warn('openPanel: no career found for rank', rank, '— careerData:', data);
        return;
    }

    const overlay   = document.getElementById('detailOverlay');
    const leftEl    = document.getElementById('detailLeft');
    const titleEl   = document.getElementById('detailHeadTitle');
    const bodyEl    = document.getElementById('detailBody');
    const rightEl   = overlay ? overlay.querySelector('.detail-right') : null;

    if (!overlay || !leftEl || !titleEl || !bodyEl) {
        console.error('openPanel: overlay elements not found in DOM');
        return;
    }

    titleEl.textContent  = d.name;
    leftEl.innerHTML     = buildMiniCard(d);
    bodyEl.innerHTML     = buildDetailContent(d);

    overlay.classList.add('open');
    document.body.style.overflow = 'hidden';

    /* animate readiness bar in mini card */
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            const fill = leftEl.querySelector('.mini-rbar-fill');
            if (fill) fill.style.width = fill.dataset.width;
        });
    });

    if (rightEl) rightEl.scrollTop = 0;
}

function closePanel() {
    const overlay = document.getElementById('detailOverlay');
    if (overlay) overlay.classList.remove('open');
    document.body.style.overflow = '';
}

document.addEventListener('keydown', e => {
    if (e.key === 'Escape') closePanel();
});

/* ── Mini card (left column) ───────────────────────────────────── */
function buildMiniCard(d) {
    const dc = d.demandLevel === 'High'   ? 'd-high'
        : d.demandLevel === 'Medium' ? 'd-medium'
            :                              'd-low';

    return `
        <div class="mini-card" data-rank="${d.rank}">
            <div class="mini-rank">#${d.rank} Match</div>

            <div class="mini-score">${d.match}<sup>%</sup></div>
            <div class="mini-score-label">Match Score</div>

            <div class="mini-name">${esc(d.name)}</div>
            <div class="mini-field">${esc(d.field)} &middot; ${esc(d.subField)}</div>

            <hr class="mini-divider"/>

            <div class="mini-stat">
                <span class="mini-stat-l">Salary</span>
                <span class="mini-stat-v">${esc(d.salary || d.averageSalary)}</span>
            </div>
            <div class="mini-stat">
                <span class="mini-stat-l">Job Growth</span>
                <span class="mini-stat-v" style="color:var(--emerald);">${esc(d.growth)}</span>
            </div>
            <div class="mini-stat">
                <span class="mini-stat-l">Demand</span>
                <span class="demand-chip ${dc}" style="font-size:.62rem;">${esc(d.demandLevel)}</span>
            </div>
            <div class="mini-stat">
                <span class="mini-stat-l">Work Mode</span>
                <span class="mini-stat-v">${esc(d.workMode)}</span>
            </div>

            <div class="mini-readiness-wrap">
                <div class="mini-readiness-label">
                    <span>Your Readiness</span><span>${d.readiness}%</span>
                </div>
                <div class="mini-rbar">
                    <div class="mini-rbar-fill" data-width="${d.readiness}%" style="width:0%"></div>
                </div>
            </div>
        </div>`;
}

/* ── Full career detail (right column) ────────────────────────── */
function buildDetailContent(d) {

    /* strengths */
    const strsHTML = (d.strengths || []).map(s => `
        <div class="dr-str-row"><span>✓</span><span>${esc(s)}</span></div>`
    ).join('');

    /* gaps */
    const gapsHTML = (d.gaps || []).map(g => `
        <div class="dr-gap-row">
            <span>⚠</span>
            <div>
                <div style="font-weight:600;">${esc(g.name)}</div>
                <div class="dr-gap-adv">${esc(g.advice)}</div>
            </div>
        </div>`
    ).join('');

    /* description */
    const descHTML = d.description ? `
        <div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>About This Career</div>
            <p style="font-size:.9rem;color:var(--slate);line-height:1.7;">${esc(d.description)}</p>
        </div>` : '';

    /* responsibilities */
    const respHTML = (d.responsibilities || []).length ? `
        <div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>What You Will Do</div>
            <ul class="resp-list">
                ${(d.responsibilities).map(r => `<li>${esc(r)}</li>`).join('')}
            </ul>
        </div>` : '';

    /* required skills */
    const skillsHTML = (d.requiredSkills || []).length ? `
        <div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Skills Required</div>
            <div class="skills-grid">
                ${(d.requiredSkills).map(s => `
                    <div class="skill-chip">
                        <div class="skill-chip-name">
                            ${esc(s.name)}
                            <span class="skill-chip-level">${s.level}/4</span>
                        </div>
                        <div class="skill-bar">
                            <div class="skill-bar-fill" style="width:${s.level * 25}%"></div>
                        </div>
                        <div class="skill-label">${esc(s.levelLabel)}</div>
                    </div>`).join('')}
            </div>
        </div>` : '';

    /* career progression */
    const progHTML = (d.careerProgression || []).length ? `
        <div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Typical Career Progression</div>
            <div class="prog-steps">
                ${(d.careerProgression).map((step, i, arr) => `
                    <div class="prog-step">
                        <div class="prog-step-years">${esc(step.yearsRange)}</div>
                        <div class="prog-step-title">${esc(step.title)}</div>
                    </div>
                    ${i < arr.length - 1 ? '<span class="prog-arrow">→</span>' : ''}`
    ).join('')}
            </div>
        </div>` : '';

    /* top companies */
    const companiesHTML = (d.topCompanies || []).length ? `
        <div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Top Companies Hiring</div>
            <div class="company-chips">
                ${(d.topCompanies).map(c => `<span class="company-chip">${esc(c)}</span>`).join('')}
            </div>
        </div>` : '';

    return `
        <!-- Quick stats -->
        <div class="dr-stats">
            <div class="dr-stat-card">
                <div class="dr-stat-val blue">${esc(d.averageSalary || d.salary)}</div>
                <div class="dr-stat-label">Avg Salary</div>
            </div>
            <div class="dr-stat-card">
                <div class="dr-stat-val green">${esc(d.growth)}</div>
                <div class="dr-stat-label">Job Growth</div>
            </div>
            <div class="dr-stat-card">
                <div class="dr-stat-val amber">${esc(d.timeToReady || d.experience)}</div>
                <div class="dr-stat-label">${d.timeToReady ? 'Time to Ready' : 'Experience'}</div>
            </div>
            <div class="dr-stat-card">
                <div class="dr-stat-val">${esc(d.workMode)}</div>
                <div class="dr-stat-label">Work Mode</div>
            </div>
        </div>

        ${descHTML}
        ${respHTML}

        ${strsHTML ? `<div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Your Strengths for This Role</div>
            ${strsHTML}
        </div>` : ''}

        ${gapsHTML ? `<div class="dr-sec">
            <div class="dr-sec-title"><span class="dr-sec-title-dot"></span>Skills to Develop</div>
            ${gapsHTML}
        </div>` : ''}

        ${skillsHTML}
        ${progHTML}
        ${companiesHTML}

        <!-- CTA -->
        <div class="dr-cta">
            <div class="dr-cta-icon">🎯</div>
            <h5>Am I Ready for This Career?</h5>
            <p>Get a full gap analysis — your readiness score, critical skills to close, and a personalised learning roadmap.</p>
            <a href="/Student/Careers/Details/${esc(d.slug)}"
               class="btn-gen"
               style="text-decoration:none;display:inline-flex;width:100%;justify-content:center;">
                <span class="b-icon">→</span>
                <span class="b-label">View Full Career Details</span>
            </a>
        </div>`;
}

/* ── HTML escape helper ─────────────────────────────────────────── */
function esc(s) {
    const el = document.createElement('div');
    el.appendChild(document.createTextNode(String(s ?? '')));
    return el.innerHTML;
}