// =============================================================================
// Audio Atmosphere System — Procedural ambient sound via Web Audio API
// Zero external audio files — everything is generated from oscillators & noise
// =============================================================================

window.AudioAtmosphere = (() => {
  let ctx = null;
  let masterGain = null;
  let droneGain = null;
  let footstepGain = null;
  let currentTheme = null;
  let droneOscillators = [];
  let isInitialized = false;
  let footstepPhase = 0;
  let lastFootstepTime = 0;

  // Theme → frequency/parameter map
  const THEME_AUDIO = {
    Ancient:    { baseFreq: 55,  modFreq: 0.3, filterFreq: 300,  volume: 0.12, character: 'warm' },
    Modern:    { baseFreq: 82,  modFreq: 0.1, filterFreq: 800,  volume: 0.08, character: 'clean' },
    Mystical:  { baseFreq: 65,  modFreq: 0.5, filterFreq: 400,  volume: 0.14, character: 'ethereal' },
    Industrial:{ baseFreq: 41,  modFreq: 0.2, filterFreq: 200,  volume: 0.10, character: 'rumble' },
    Reflective:{ baseFreq: 73,  modFreq: 0.15,filterFreq: 600,  volume: 0.09, character: 'shimmer' },
    Default:   { baseFreq: 60,  modFreq: 0.2, filterFreq: 400,  volume: 0.06, character: 'warm' }
  };

  function init() {
    if (isInitialized) return;
    try {
      ctx = new (window.AudioContext || window.webkitAudioContext)();
      masterGain = ctx.createGain();
      masterGain.gain.value = 0.5;
      masterGain.connect(ctx.destination);

      droneGain = ctx.createGain();
      droneGain.gain.value = 0;
      droneGain.connect(masterGain);

      footstepGain = ctx.createGain();
      footstepGain.gain.value = 0;
      footstepGain.connect(masterGain);

      isInitialized = true;
    } catch(e) {
      console.warn('[AudioAtmosphere] Web Audio not available:', e);
    }
  }

  function stopDrone() {
    droneOscillators.forEach(o => {
      try { o.stop(); } catch(e) {}
    });
    droneOscillators = [];
  }

  function setTheme(themeName) {
    if (!isInitialized) return;
    if (themeName === currentTheme) return;
    currentTheme = themeName;

    const params = THEME_AUDIO[themeName] || THEME_AUDIO.Default;

    // Fade out old drone
    droneGain.gain.linearRampToValueAtTime(0, ctx.currentTime + 1.5);

    setTimeout(() => {
      stopDrone();

      // Create new layered drone
      const filter = ctx.createBiquadFilter();
      filter.type = 'lowpass';
      filter.frequency.value = params.filterFreq;
      filter.Q.value = 1.5;
      filter.connect(droneGain);

      // Base tone
      const osc1 = ctx.createOscillator();
      osc1.type = 'sine';
      osc1.frequency.value = params.baseFreq;
      osc1.connect(filter);
      osc1.start();
      droneOscillators.push(osc1);

      // Harmonic fifth
      const osc2 = ctx.createOscillator();
      osc2.type = 'sine';
      osc2.frequency.value = params.baseFreq * 1.5;
      const g2 = ctx.createGain();
      g2.gain.value = 0.3;
      osc2.connect(g2);
      g2.connect(filter);
      osc2.start();
      droneOscillators.push(osc2);

      // Sub-bass for depth
      const osc3 = ctx.createOscillator();
      osc3.type = 'sine';
      osc3.frequency.value = params.baseFreq * 0.5;
      const g3 = ctx.createGain();
      g3.gain.value = 0.15;
      osc3.connect(g3);
      g3.connect(filter);
      osc3.start();
      droneOscillators.push(osc3);

      // LFO modulation for life
      const lfo = ctx.createOscillator();
      lfo.type = 'sine';
      lfo.frequency.value = params.modFreq;
      const lfoGain = ctx.createGain();
      lfoGain.gain.value = params.baseFreq * 0.02;
      lfo.connect(lfoGain);
      lfoGain.connect(osc1.frequency);
      lfo.start();
      droneOscillators.push(lfo);

      // Character-specific additions
      if (params.character === 'ethereal') {
        const shimmer = ctx.createOscillator();
        shimmer.type = 'triangle';
        shimmer.frequency.value = params.baseFreq * 4;
        const sg = ctx.createGain();
        sg.gain.value = 0.04;
        shimmer.connect(sg);
        sg.connect(filter);
        shimmer.start();
        droneOscillators.push(shimmer);
      }

      if (params.character === 'shimmer') {
        const chime = ctx.createOscillator();
        chime.type = 'sine';
        chime.frequency.value = params.baseFreq * 6;
        const cg = ctx.createGain();
        cg.gain.value = 0.02;
        chime.connect(cg);
        cg.connect(filter);
        chime.start();
        droneOscillators.push(chime);
      }

      // Fade in new drone
      droneGain.gain.linearRampToValueAtTime(params.volume, ctx.currentTime + 2.0);
    }, 1600);
  }

  function playFootstep() {
    if (!isInitialized || !ctx) return;
    const now = ctx.currentTime;
    if (now - lastFootstepTime < 0.35) return;
    lastFootstepTime = now;

    // Short noise burst for footstep
    const bufferSize = ctx.sampleRate * 0.04;
    const buffer = ctx.createBuffer(1, bufferSize, ctx.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < bufferSize; i++) {
      data[i] = (Math.random() * 2 - 1) * Math.exp(-i / (bufferSize * 0.15));
    }
    const source = ctx.createBufferSource();
    source.buffer = buffer;

    const filter = ctx.createBiquadFilter();
    filter.type = 'bandpass';
    filter.frequency.value = 800 + (footstepPhase % 2) * 200;
    filter.Q.value = 2;

    const env = ctx.createGain();
    env.gain.setValueAtTime(0.15, now);
    env.gain.exponentialRampToValueAtTime(0.001, now + 0.08);

    source.connect(filter);
    filter.connect(env);
    env.connect(masterGain);
    source.start(now);
    source.stop(now + 0.1);

    footstepPhase++;
  }

  function playMonumentProximityPing() {
    if (!isInitialized || !ctx) return;
    const now = ctx.currentTime;
    const osc = ctx.createOscillator();
    osc.type = 'sine';
    osc.frequency.setValueAtTime(880, now);
    osc.frequency.exponentialRampToValueAtTime(1760, now + 0.1);

    const env = ctx.createGain();
    env.gain.setValueAtTime(0.06, now);
    env.gain.exponentialRampToValueAtTime(0.001, now + 0.3);

    osc.connect(env);
    env.connect(masterGain);
    osc.start(now);
    osc.stop(now + 0.35);
  }

  function resume() {
    if (ctx && ctx.state === 'suspended') ctx.resume();
  }

  return { init, setTheme, playFootstep, playMonumentProximityPing, resume };
})();
