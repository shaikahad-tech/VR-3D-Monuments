// Heritage Atlas — Enhanced UI, Artifact System, Narration Engine
// Load after monument-data.js and three.js

window.HeritageUI = (() => {

  // ── ENHANCED INFO CARD ──────────────────────────────────────────
  function showRichCard(key) {
    const d = window.MONUMENT_DATA_RICH?.[key];
    if (!d) return;

    const card = document.getElementById('infoCard');

    // Build tabs: Overview | Architecture | Artifacts | Quiz
    card.innerHTML = `
      <div style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:10px">
        <div>
          <div class="era">${d.era}</div>
          <h2 style="font-size:17px;color:#fff;margin:2px 0">${d.title}</h2>
          <div class="loc">${d.location}</div>
        </div>
        <button onclick="document.getElementById('infoCard').style.display='none'"
          style="background:none;border:1px solid #555;color:#aaa;border-radius:50%;width:24px;height:24px;cursor:pointer;font-size:14px;line-height:1">✕</button>
      </div>

      <div style="display:flex;gap:6px;margin-bottom:12px;flex-wrap:wrap">
        ${['Overview','Architecture','Artifacts','Quiz'].map((t,i)=>`
          <button onclick="HeritageUI.showTab('${key}',${i})" id="tab_${key}_${i}"
            style="background:${i===0?'rgba(220,181,116,.3)':'rgba(255,255,255,.06)'};border:1px solid ${i===0?'#dcb574':'#444'};
            color:${i===0?'#dcb574':'#888'};padding:4px 10px;border-radius:20px;font-size:10px;cursor:pointer;letter-spacing:1px;text-transform:uppercase">
            ${t}</button>`).join('')}
      </div>

      <div id="tabContent_${key}" class="body"></div>
    `;

    // Show overview by default
    showTab(key, 0);
    card.style.display = 'block';
  }

  function showTab(key, idx) {
    const d = window.MONUMENT_DATA_RICH?.[key];
    if (!d) return;
    const el = document.getElementById(`tabContent_${key}`);
    if (!el) return;

    // Update tab button styles
    for (let i = 0; i < 4; i++) {
      const btn = document.getElementById(`tab_${key}_${i}`);
      if (!btn) continue;
      btn.style.background = i === idx ? 'rgba(220,181,116,.3)' : 'rgba(255,255,255,.06)';
      btn.style.borderColor = i === idx ? '#dcb574' : '#444';
      btn.style.color = i === idx ? '#dcb574' : '#888';
    }

    if (idx === 0) { // Overview
      el.innerHTML = `
        <p style="color:#ccc;font-size:12px;line-height:1.6;margin-bottom:10px">${d.narration}</p>
        <div style="display:grid;grid-template-columns:1fr 1fr;gap:6px;font-size:11px">
          <div style="background:rgba(255,255,255,.04);padding:6px 8px;border-radius:8px">
            <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:3px">ARCHITECT</div>
            <div style="color:#ddd">${d.architect}</div>
          </div>
          <div style="background:rgba(255,255,255,.04);padding:6px 8px;border-radius:8px">
            <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:3px">DYNASTY</div>
            <div style="color:#ddd">${d.dynasty}</div>
          </div>
          <div style="background:rgba(255,255,255,.04);padding:6px 8px;border-radius:8px;grid-column:span 2">
            <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:3px">PURPOSE</div>
            <div style="color:#ddd">${d.purpose}</div>
          </div>
        </div>`;

    } else if (idx === 1) { // Architecture
      el.innerHTML = `
        <div style="margin-bottom:8px">
          <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:4px">ARCHITECTURAL STYLE</div>
          <div style="color:#ccc;font-size:12px">${d.style}</div>
        </div>
        <div style="margin-bottom:8px">
          <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:4px">STRUCTURAL COMPONENTS</div>
          <ul style="color:#bbb;font-size:11px;padding-left:14px;line-height:1.8">
            ${(d.components||[]).map(c=>`<li>${c}</li>`).join('')}
          </ul>
        </div>
        <div>
          <div style="color:#dcb574;font-size:9px;letter-spacing:1px;margin-bottom:4px">MATERIALS</div>
          <div style="color:#bbb;font-size:11px;line-height:1.5">${d.materials}</div>
        </div>`;

    } else if (idx === 2) { // Artifacts
      const arts = d.artifacts || [];
      el.innerHTML = arts.length ? arts.map(a => `
        <div style="background:rgba(255,255,255,.04);border-radius:8px;padding:8px 10px;margin-bottom:8px;border-left:2px solid #dcb574">
          <div style="color:#fff;font-size:12px;font-weight:600">${a.name}</div>
          <div style="display:flex;gap:6px;margin:4px 0;flex-wrap:wrap">
            <span style="background:rgba(220,181,116,.15);color:#dcb574;font-size:9px;padding:2px 6px;border-radius:10px">${a.type}</span>
            <span style="background:rgba(255,255,255,.08);color:#999;font-size:9px;padding:2px 6px;border-radius:10px">${a.material}</span>
          </div>
          <div style="color:#bbb;font-size:11px;line-height:1.4"><b style="color:#888">Usage:</b> ${a.usage}</div>
          <div style="color:#aaa;font-size:11px;line-height:1.4;margin-top:3px"><b style="color:#888">Symbolism:</b> ${a.symbolism}</div>
        </div>`).join('') : '<div style="color:#777;font-size:12px">No artifacts catalogued for this monument.</div>';

    } else if (idx === 3) { // Quiz
      const qs = d.quiz || [];
      el.innerHTML = qs.length ? `
        <div id="quizContainer_${key}"></div>` : '<div style="color:#777">No quiz available.</div>';
      if (qs.length) renderQuiz(key, 0, 0);
    }
  }

  let quizState = {};

  function renderQuiz(key, qIdx, score) {
    const d = window.MONUMENT_DATA_RICH?.[key];
    if (!d) return;
    const qs = d.quiz;
    const el = document.getElementById(`quizContainer_${key}`);
    if (!el) return;

    if (qIdx >= qs.length) {
      el.innerHTML = `
        <div style="text-align:center;padding:10px">
          <div style="font-size:28px;margin-bottom:8px">${score === qs.length ? '🏆' : score >= qs.length/2 ? '⭐' : '📚'}</div>
          <div style="color:#dcb574;font-size:16px;font-weight:600">${score}/${qs.length} Correct</div>
          <div style="color:#888;font-size:11px;margin-top:6px">${score === qs.length ? 'Perfect score!' : 'Keep exploring to learn more.'}</div>
          <button onclick="HeritageUI.renderQuiz('${key}',0,0)"
            style="margin-top:10px;background:rgba(220,181,116,.2);border:1px solid #dcb574;color:#dcb574;padding:6px 14px;border-radius:20px;font-size:11px;cursor:pointer">
            Retry Quiz</button>
        </div>`;
      return;
    }

    const q = qs[qIdx];
    el.innerHTML = `
      <div style="color:#888;font-size:10px;margin-bottom:6px">Question ${qIdx+1} of ${qs.length} · Score: ${score}</div>
      <div style="color:#fff;font-size:12px;line-height:1.5;margin-bottom:10px">${q.q}</div>
      <div style="display:flex;flex-direction:column;gap:6px">
        ${q.options.map((opt,i) => `
          <button onclick="HeritageUI.answerQuiz('${key}',${qIdx},${score},'${opt.replace(/'/g,"\\'")}','${q.a.replace(/'/g,"\\'")}')"
            style="background:rgba(255,255,255,.06);border:1px solid #444;color:#ccc;padding:7px 10px;border-radius:8px;
            font-size:11px;cursor:pointer;text-align:left;transition:all .15s"
            onmouseover="this.style.borderColor='#dcb574';this.style.color='#fff'"
            onmouseout="this.style.borderColor='#444';this.style.color='#ccc'">
            ${String.fromCharCode(65+i)}. ${opt}</button>`).join('')}
      </div>`;
  }

  function answerQuiz(key, qIdx, score, chosen, correct) {
    const d = window.MONUMENT_DATA_RICH?.[key];
    if (!d) return;
    const isRight = chosen === correct;
    const newScore = score + (isRight ? 1 : 0);
    const el = document.getElementById(`quizContainer_${key}`);
    if (!el) return;

    // Flash feedback
    const feedback = document.createElement('div');
    feedback.style.cssText = `position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);
      font-size:32px;pointer-events:none;transition:opacity .5s;z-index:999`;
    feedback.textContent = isRight ? '✓' : '✗';
    feedback.style.color = isRight ? '#4caf50' : '#f44336';
    el.style.position = 'relative';
    el.appendChild(feedback);
    setTimeout(() => { feedback.style.opacity = '0'; setTimeout(() => feedback.remove(), 500); }, 600);
    setTimeout(() => renderQuiz(key, qIdx + 1, newScore), 900);
  }

  // ── NARRATION ENGINE ────────────────────────────────────────────
  let narratorActive = false;
  let narratorTimer = null;

  function narrate(key) {
    const d = window.MONUMENT_DATA_RICH?.[key];
    if (!d || !d.narration) return;
    if (!('speechSynthesis' in window)) {
      window.showToast?.('Narration: Web Speech API not supported in this browser');
      return;
    }
    window.speechSynthesis.cancel();
    const utt = new SpeechSynthesisUtterance(d.narration);
    utt.rate = 0.88;
    utt.pitch = 1.0;
    utt.lang = 'en-GB';
    // Prefer a quality voice
    const voices = window.speechSynthesis.getVoices();
    const preferred = voices.find(v => v.name.includes('Daniel') || v.name.includes('Karen') || v.lang === 'en-GB');
    if (preferred) utt.voice = preferred;
    utt.onstart = () => { narratorActive = true; window.showToast?.('🎙 Narration started — press N to stop'); };
    utt.onend = () => { narratorActive = false; };
    window.speechSynthesis.speak(utt);
  }

  function stopNarration() {
    window.speechSynthesis?.cancel();
    narratorActive = false;
  }

  // ── TIMELINE MODE ───────────────────────────────────────────────
  function showTimeline() {
    const existing = document.getElementById('timelinePanel');
    if (existing) { existing.remove(); return; }

    const data = window.MONUMENT_DATA_RICH || {};
    const sorted = Object.entries(data)
      .filter(([,d]) => d.era)
      .sort((a,b) => {
        const yearA = parseInt(a[1].era.match(/-?\d+/)?.[0] || 0);
        const yearB = parseInt(b[1].era.match(/-?\d+/)?.[0] || 0);
        return yearA - yearB;
      });

    const panel = document.createElement('div');
    panel.id = 'timelinePanel';
    panel.style.cssText = `position:fixed;bottom:0;left:0;right:0;height:120px;
      background:rgba(8,10,14,.95);backdrop-filter:blur(20px);
      border-top:1px solid rgba(220,181,116,.3);padding:12px 20px;
      display:flex;align-items:center;gap:0;overflow-x:auto;z-index:200`;

    panel.innerHTML = `
      <div style="color:#dcb574;font-size:10px;letter-spacing:2px;text-transform:uppercase;
        white-space:nowrap;margin-right:16px;min-width:80px">TIMELINE</div>
      <div style="display:flex;align-items:center;gap:0;position:relative">
        <div style="position:absolute;top:50%;left:0;right:0;height:1px;background:rgba(220,181,116,.3)"></div>
        ${sorted.map(([key,d]) => `
          <div style="display:flex;flex-direction:column;align-items:center;margin:0 16px;cursor:pointer;min-width:80px"
            onclick="HeritageUI.showRichCard('${key}')">
            <div style="width:10px;height:10px;border-radius:50%;background:#dcb574;border:2px solid #1a1208;z-index:1;margin-bottom:6px"></div>
            <div style="color:#fff;font-size:10px;text-align:center;white-space:nowrap">${d.title}</div>
            <div style="color:#888;font-size:9px;text-align:center;white-space:nowrap">${d.era.split('(')[0].trim()}</div>
          </div>`).join('')}
      </div>
      <button onclick="document.getElementById('timelinePanel').remove()"
        style="position:fixed;right:12px;bottom:88px;background:rgba(220,181,116,.2);border:1px solid #dcb574;
        color:#dcb574;padding:4px 10px;border-radius:20px;font-size:10px;cursor:pointer;z-index:201">✕ Close</button>`;

    document.body.appendChild(panel);
  }

  // ── ARTIFACT VIEWER ─────────────────────────────────────────────
  function showArtifactLibrary() {
    const existing = document.getElementById('artifactPanel');
    if (existing) { existing.remove(); return; }

    const arts = window.ARTIFACT_LIBRARY || [];
    const panel = document.createElement('div');
    panel.id = 'artifactPanel';
    panel.style.cssText = `position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);
      width:min(600px,94vw);max-height:80vh;overflow-y:auto;
      background:rgba(8,10,14,.97);backdrop-filter:blur(20px);
      border:1px solid rgba(220,181,116,.3);border-radius:18px;padding:22px;z-index:300;
      box-shadow:0 30px 80px rgba(0,0,0,.7)`;

    panel.innerHTML = `
      <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
        <div>
          <div style="color:#dcb574;font-size:10px;letter-spacing:2px;text-transform:uppercase">ARTIFACT LIBRARY</div>
          <h2 style="color:#fff;font-size:18px;margin-top:2px">Cross-Civilization Collection</h2>
        </div>
        <button onclick="document.getElementById('artifactPanel').remove()"
          style="background:none;border:1px solid #555;color:#aaa;border-radius:50%;width:28px;height:28px;cursor:pointer;font-size:15px">✕</button>
      </div>
      <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(260px,1fr));gap:10px">
        ${arts.map(a => `
          <div style="background:rgba(255,255,255,.04);border-radius:10px;padding:12px;border-left:3px solid ${toCssColor(a.color)}">
            <div style="display:flex;justify-content:space-between;align-items:flex-start">
              <div style="color:#fff;font-size:12px;font-weight:600;flex:1">${a.name}</div>
              <span style="background:rgba(220,181,116,.15);color:#dcb574;font-size:9px;padding:2px 6px;border-radius:10px;white-space:nowrap;margin-left:6px">${a.type}</span>
            </div>
            <div style="color:#888;font-size:10px;margin:4px 0">${a.civilization}</div>
            <div style="color:#bbb;font-size:11px;line-height:1.4;margin-top:4px"><b style="color:#777">Material:</b> ${a.material}</div>
            <div style="color:#bbb;font-size:11px;line-height:1.4"><b style="color:#777">Usage:</b> ${a.usage}</div>
            <div style="color:#aaa;font-size:11px;line-height:1.4;margin-top:4px;border-top:1px solid rgba(255,255,255,.06);padding-top:4px">
              <i style="color:#888">${a.symbolism}</i></div>
          </div>`).join('')}
      </div>`;

    document.body.appendChild(panel);
  }

  function toCssColor(hex) {
    return '#' + hex.toString(16).padStart(6, '0');
  }

  // ── KEYBOARD HOOKS ──────────────────────────────────────────────
  document.addEventListener('keydown', e => {
    if (e.code === 'KeyN') {
      // N = narrate current monument
      const key = window._currentMonumentKey;
      if (key) narrate(key);
      else window.showToast?.('Look at a monument and press N to hear narration');
    }
    if (e.code === 'KeyT') showTimeline();
    if (e.code === 'KeyG') showArtifactLibrary();
  });

  // Public API
  return { showRichCard, showTab, renderQuiz, answerQuiz, narrate, stopNarration, showTimeline, showArtifactLibrary };
})();
