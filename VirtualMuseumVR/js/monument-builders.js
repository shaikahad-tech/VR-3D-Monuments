import * as THREE from 'three';

// Utility functions for building procedural geometry
function addBox(parent, mat, [w,h,d], pos, rot){
  const m = new THREE.Mesh(new THREE.BoxGeometry(w,h,d), mat);
  m.position.set(pos[0], pos[1], pos[2]);
  if(rot) m.rotation.set(rot[0]||0, rot[1]||0, rot[2]||0);
  parent.add(m); return m;
}
function addSphere(parent, mat, r, pos, scale, rot){
  const m = new THREE.Mesh(new THREE.SphereGeometry(r, 32, 16), mat);
  m.position.set(pos[0], pos[1], pos[2]);
  if(scale) m.scale.set(scale[0], scale[1], scale[2]);
  if(rot) m.rotation.set(rot[0]||0, rot[1]||0, rot[2]||0);
  parent.add(m); return m;
}
function addCylinder(parent, mat, rT, rB, h, pos, rot, radial=16){
  const m = new THREE.Mesh(new THREE.CylinderGeometry(rT, rB, h, radial), mat);
  m.position.set(pos[0], pos[1], pos[2]);
  if(rot) m.rotation.set(rot[0]||0, rot[1]||0, rot[2]||0);
  parent.add(m); return m;
}
function addCone(parent, mat, r, h, pos, rot, radial=16){
  const m = new THREE.Mesh(new THREE.ConeGeometry(r, h, radial), mat);
  m.position.set(pos[0], pos[1], pos[2]);
  if(rot) m.rotation.set(rot[0]||0, rot[1]||0, rot[2]||0);
  parent.add(m); return m;
}

export const MONUMENT_BUILDERS = {
  RedFort: () => {
    const g = new THREE.Group();
    const redStone = new THREE.MeshLambertMaterial({ color:0xb24131 });
    const whiteMarble = new THREE.MeshLambertMaterial({ color:0xf0ebdc });
    const dark = new THREE.MeshLambertMaterial({ color:0x1a1a1a });
    
    // Massive defensive wall base
    addBox(g, redStone, [30, 6, 12], [0, 3, 0]);
    
    // Lahori Gate (central entrance)
    addBox(g, redStone, [8, 8, 14], [0, 4, 1]);
    
    // Main archway
    addBox(g, dark, [3, 5, 1], [0, 2.5, 8]);
    addCylinder(g, dark, 1.5, 1.5, 1, [0, 5, 8], [Math.PI/2, 0, 0]);
    
    // Octagonal Chatris (domed pavilions)
    const addChatri = (x, y, z, mat) => {
      addCylinder(g, mat, 1.2, 1.2, 0.4, [x, y, z], null, 8); // Base
      for(let i=0; i<8; i++){ // Pillars
        const a = i * Math.PI / 4;
        addCylinder(g, mat, 0.15, 0.15, 1.5, [x + Math.sin(a)*0.9, y+0.75, z + Math.cos(a)*0.9]);
      }
      addSphere(g, mat, 1.1, [x, y+1.6, z], [1, 0.8, 1]); // Dome
      addCylinder(g, mat, 0.1, 0.1, 0.6, [x, y+2.4, z]); // Finial
    };
    
    addChatri(-3, 8, 6, whiteMarble);
    addChatri(3, 8, 6, whiteMarble);
    
    // Smaller flanking chhatris
    addChatri(-7, 6, 4, redStone);
    addChatri(7, 6, 4, redStone);
    
    // Intricate battlements (Merlons)
    for(let i=-14.5; i<=14.5; i+=1.2){
      if(Math.abs(i) > 4) {
        addBox(g, redStone, [0.8, 1.2, 0.5], [i, 6.6, 5.75]);
      }
    }
    return g;
  },

  QutubMinar: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0xc66c44 });
    const white = new THREE.MeshLambertMaterial({ color:0xe0d6cc });
    let h = 0, r = 4.5;
    
    // 5 distinct storeys
    for(let i=0;i<5;i++){
      const mat = (i===3 || i===4) ? white : stone;
      const tierH = 8 - i; // Tiers get shorter
      const nextR = r * 0.75;
      
      // Main shaft taper
      addCylinder(g, mat, nextR, r, tierH, [0, h + tierH/2, 0], null, 32);
      
      // Detailed fluting
      const flutes = 24;
      for(let f=0;f<flutes;f++){
        const a = f*Math.PI*2/flutes;
        // 1st tier: alternating wedge and circle. 2nd: circle. 3rd: wedge.
        const isAngular = (i===0 && f%2===0) || (i===2);
        if(isAngular){
           addBox(g, mat, [0.8*r/4.5, tierH, 0.8*r/4.5], [Math.sin(a)*r*0.95, h + tierH/2, Math.cos(a)*r*0.95], [0, a+Math.PI/4, 0]);
        } else {
           addCylinder(g, mat, 0.4*r/4.5, 0.4*r/4.5, tierH, [Math.sin(a)*r*0.95, h + tierH/2, Math.cos(a)*r*0.95]);
        }
      }
      
      h += tierH;
      
      // Balcony with intricate Muqarnas corbels
      if(i<4) {
        addCylinder(g, stone, r*1.4, r*1.1, 0.4, [0, h, 0], null, 32);
        for(let b=0;b<32;b++){
          const a = b*Math.PI*2/32;
          addBox(g, stone, [0.3, 1.2, 1.8], [Math.sin(a)*r*1.1, h-0.6, Math.cos(a)*r*1.1], [0,a,0]);
        }
      }
      r = nextR;
    }
    // Cupola (ruined/removed in reality, but often depicted)
    addCylinder(g, stone, r*0.8, r, 1.5, [0, h+0.75, 0]);
    return g;
  },

  HampiChariot: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0xa68c66 });
    const dark = new THREE.MeshLambertMaterial({ color:0x5a4a36 });
    
    // Base chassis with detailed carvings
    addBox(g, stone, [6.5, 1.8, 8.5], [0, 1.8, 0]);
    addBox(g, stone, [5.8, 0.8, 7.8], [0, 3.1, 0]);
    
    // 4 Detailed Wheels with spokes
    for(let w of [[-3.4,2.5],[3.4,2.5],[-3.4,-2.5],[3.4,-2.5]]) {
      addCylinder(g, stone, 1.4, 1.4, 0.4, [w[0], 1.4, w[1]], [0, 0, Math.PI/2], 32); // Rim
      addCylinder(g, dark, 1.2, 1.2, 0.45, [w[0], 1.4, w[1]], [0, 0, Math.PI/2], 32); // Inner recess
      addSphere(g, stone, 0.35, [w[0]*1.1, 1.4, w[1]]); // Hub
      // Spokes
      for(let s=0;s<8;s++){
        const a = s*Math.PI/4;
        addBox(g, stone, [0.3, 2.4, 0.1], [w[0], 1.4, w[1]], [Math.PI/2, a, Math.PI/2]);
      }
    }
    
    // Mandapa (Shrine body)
    addBox(g, stone, [4.2, 4.5, 4.2], [0, 5.75, 0]);
    addBox(g, dark, [2, 3, 4.3], [0, 5.5, 0]); // Hollow interior
    
    // Intricate Pilasters
    for(let x of [-1.9, 1.9]) {
      for(let z of [-1.9, 1.9]) {
        addCylinder(g, stone, 0.3, 0.3, 4.5, [x, 5.75, z]);
        addBox(g, stone, [0.8, 0.4, 0.8], [x, 3.7, z]); // Base
        addBox(g, stone, [0.9, 0.4, 0.9], [x, 7.8, z]); // Capital
      }
    }
    
    // Vimana (Tower tiers)
    addBox(g, stone, [4.8, 0.6, 4.8], [0, 8.3, 0]);
    addBox(g, stone, [3.8, 1.5, 3.8], [0, 9.35, 0]);
    
    // Ribbed dome (Amalaka/Shikhara style)
    const dome = new THREE.Mesh(new THREE.SphereGeometry(2.2, 32, 16), stone);
    dome.position.set(0, 10.5, 0);
    dome.scale.set(1, 0.8, 1);
    g.add(dome);
    
    // Kalasha (Finial)
    addCylinder(g, stone, 0.3, 0.1, 1.5, [0, 12.5, 0]);
    addSphere(g, stone, 0.4, [0, 12.5, 0]);
    
    return g;
  },

  KonarkSunTemple: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x8c6648 });
    const dark = new THREE.MeshLambertMaterial({ color:0x5c422c });
    
    // Massive Platform with terraces
    addBox(g, stone, [26, 4, 36], [0, 2, 0]);
    addBox(g, stone, [24, 2, 34], [0, 5, 0]);
    
    // 24 Detailed Wheels
    for(let i=0; i<12; i++){
      const z = -15 + i*2.7;
      for(let side of [-1, 1]) {
        const x = side * 13.2;
        addCylinder(g, stone, 1.8, 1.8, 0.5, [x, 1.8, z], [0, 0, Math.PI/2], 32);
        addCylinder(g, dark, 1.5, 1.5, 0.55, [x, 1.8, z], [0, 0, Math.PI/2], 32);
        addSphere(g, stone, 0.4, [x - side*0.1, 1.8, z]);
        // 8 thick, 8 thin spokes
        for(let s=0;s<8;s++){
          const a = s*Math.PI/4;
          addBox(g, stone, [0.5, 3, 0.2], [x, 1.8, z], [Math.PI/2, a, Math.PI/2]);
          addBox(g, stone, [0.15, 3, 0.1], [x, 1.8, z], [Math.PI/2, a+Math.PI/8, Math.PI/2]);
        }
      }
    }
    
    // Jagamohana (Assembly Hall)
    addBox(g, stone, [18, 10, 18], [0, 11, 6]);
    
    // Elaborate Pidha Deula (Pyramidal roof)
    let h = 16, s = 19;
    for(let tier=0; tier<3; tier++){ // 3 major tiers
      const steps = tier === 2 ? 5 : 6;
      for(let step=0; step<steps; step++){
        addBox(g, dark, [s, 0.6, s], [0, h, 6]);
        s -= 0.6; h += 0.8;
      }
      // Recess between tiers
      addBox(g, stone, [s-1, 1.5, s-1], [0, h+0.75, 6]);
      s -= 1.5; h += 1.5; 
    }
    // Amalaka and Kalasha
    addCylinder(g, stone, 3.5, 3.5, 1.5, [0, h+0.75, 6], null, 24); 
    addSphere(g, stone, 1.5, [0, h+2.5, 6]); 
    
    // Ruined main sanctuary (Deul) base behind Jagamohana
    addBox(g, stone, [20, 12, 20], [0, 8, -12]);
    addBox(g, dark, [18, 4, 18], [0, 16, -12]);
    
    return g;
  },

  AjantaCaves: () => {
    const g = new THREE.Group();
    const rock = new THREE.MeshLambertMaterial({ color:0x806b59 });
    const dark = new THREE.MeshLambertMaterial({ color:0x1a1510 }); // Deep shadow
    
    // Curved cliff face
    const cliff = new THREE.Mesh(new THREE.CylinderGeometry(30, 30, 20, 32, 1, false, Math.PI*1.2, Math.PI*0.6), rock);
    cliff.position.set(0, 10, 15);
    g.add(cliff);
    
    // Main Chaitya Hall facade (Cave 19 style)
    const facade = new THREE.Group();
    facade.position.set(0, 0, -10);
    g.add(facade);
    
    addBox(facade, rock, [18, 20, 5], [0, 10, 0]); // Face block
    addBox(facade, dark, [14, 16, 6], [0, 8, 0]); // Cutout
    
    // Intricate Horseshoe arch window
    addCylinder(facade, rock, 5.5, 5.5, 2, [0, 12, 1], [Math.PI/2, 0, 0], 32);
    addCylinder(facade, dark, 4.5, 4.5, 2.5, [0, 12, 1], [Math.PI/2, 0, 0], 32);
    
    // Lower portico columns
    for(let x of [-4.5, -1.5, 1.5, 4.5]) {
      addCylinder(facade, rock, 0.6, 0.6, 6, [x, 3, 1]);
      addBox(facade, rock, [1.5, 0.8, 1.5], [x, 6.4, 1]); // Cushion capital
    }
    
    // Inner Stupa with Buddha
    addCylinder(facade, rock, 2.5, 2.5, 3, [0, 1.5, -4]); // Stupa base
    addSphere(facade, rock, 2.5, [0, 3, -4], [1, 0.8, 1]); // Stupa dome
    // Buddha carved in front of stupa
    addBox(facade, rock, [2, 3, 1], [0, 1.5, -2]); 
    addSphere(facade, rock, 0.7, [0, 3.5, -1.8]); 
    
    return g;
  },

  GreatPyramidGiza: () => {
    const g = new THREE.Group();
    const sand = new THREE.MeshLambertMaterial({ color:0xd8c08c });
    const darkSand = new THREE.MeshLambertMaterial({ color:0xb8a06c });
    
    // Stepped pyramid (approximation of limestone blocks)
    // To avoid excessive boxes, we'll use a custom geometry for steps
    const tiers = 40;
    const size = 30;
    const h = 20;
    for(let i=0; i<tiers; i++){
      const tierSize = size * (1 - i/tiers);
      const tierH = h / tiers;
      const y = i * tierH + tierH/2;
      addBox(g, sand, [tierSize, tierH, tierSize], [0, y, 0]);
    }
    
    // The Sphinx - Highly detailed
    const sb = new THREE.Vector3(0,0,-25);
    // Paws
    addBox(g, darkSand, [3.5, 1.5, 4], [sb.x-2, sb.y+0.75, sb.z+6]); 
    addBox(g, darkSand, [3.5, 1.5, 4], [sb.x+2, sb.y+0.75, sb.z+6]); 
    // Body
    addBox(g, darkSand, [7, 4, 12], [sb.x, sb.y+2, sb.z-1]); 
    // Chest
    addBox(g, darkSand, [6, 6, 4], [sb.x, sb.y+3, sb.z+3.5]); 
    // Head
    addSphere(g, sand, 2.2, [sb.x, sb.y+7.5, sb.z+4]);
    // Nemes Headdress
    addBox(g, darkSand, [6, 5, 3], [sb.x, sb.y+6.5, sb.z+2.5]); 
    addBox(g, darkSand, [7, 2, 3], [sb.x, sb.y+8.5, sb.z+3]); 
    
    return g;
  },

  Colosseum: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0xc6a681 });
    const inner = new THREE.MeshLambertMaterial({ color:0xa68661 });
    const cx = 22, cz = 18; // Elliptical dimensions
    
    const arcCount = 48; // Highly detailed arches
    
    for(let tier=0; tier<4; tier++){
      const y = tier*3.5 + 1.75;
      for(let a=0; a<arcCount; a++){
        const ang = a*Math.PI*2/arcCount;
        
        // Realistic ruined section (South side missing outer walls)
        const isRuined = (ang > Math.PI*0.1 && ang < Math.PI*0.9);
        if (tier >= 2 && isRuined) continue; 
        
        const px = Math.sin(ang)*cx, pz = Math.cos(ang)*cz;
        
        // Pillars/Pilasters
        addBox(g, stone, [1.2, 3.5, 1.5], [px, y, pz], [0, ang, 0]);
        
        if (tier < 3) {
          // Arches
          const nAng = (a+0.5)*Math.PI*2/arcCount;
          addBox(g, stone, [2.2, 1.0, 1.5], [Math.sin(nAng)*cx, y+1.25, Math.cos(nAng)*cz], [0, nAng, 0]);
          // Columns attached to pillars (Doric, Ionic, Corinthian)
          addCylinder(g, stone, 0.4, 0.4, 3.5, [px + Math.sin(ang)*0.8, y, pz + Math.cos(ang)*0.8]);
        } else {
          // Attic story (solid wall with small windows)
          const nAng = (a+0.5)*Math.PI*2/arcCount;
          addBox(g, stone, [2.5, 3.5, 1.2], [Math.sin(nAng)*cx, y, Math.cos(nAng)*cz], [0, nAng, 0]);
          // Pilasters on attic
          addBox(g, stone, [0.8, 3.5, 0.4], [px + Math.sin(ang)*0.6, y, pz + Math.cos(ang)*0.6], [0, ang, 0]);
        }
      }
    }
    
    // Hypogeum (Underground tunnels exposed)
    const arenaMat = new THREE.MeshLambertMaterial({ color:0x5d4d3d });
    addCylinder(g, arenaMat, 11, 11, 2, [0, 1, 0], null, 32); // Elliptical base
    for(let x=-6; x<=6; x+=2){
      addBox(g, stone, [0.5, 2, 14], [x, 2, 0]);
    }
    
    return g;
  },

  Stonehenge: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x8c8c8c });
    const grass = new THREE.MeshLambertMaterial({ color:0x405e2e });
    
    // Sarsen Circle (Outer)
    const sars = 30; // Real count is 30
    const r = 8;
    for(let i=0;i<sars;i++){
      const a = i*Math.PI*2/sars;
      // Some stones are missing or fallen
      if(i%7===0 || i%11===0) {
        if (i%7===0) addBox(g, stone, [1.5, 4, 1], [Math.sin(a)*r, 0.5, Math.cos(a)*r], [Math.PI/2, a, 0]); // Fallen
        continue; 
      }
      
      const tiltX = (Math.random()-0.5)*0.1, tiltZ = (Math.random()-0.5)*0.1;
      addBox(g, stone, [1.6, 4.5, 1.1], [Math.sin(a)*r, 2.25, Math.cos(a)*r], [tiltX,a,tiltZ]);
      
      // Lintels
      if(i%2===0 && Math.random() > 0.1){
        const ma = a + Math.PI/sars;
        addBox(g, stone, [2.2, 0.8, 1.2], [Math.sin(ma)*r, 4.9, Math.cos(ma)*r], [0,ma + Math.PI/2,0]);
      }
    }
    
    // Trilithon Horseshoe (Inner massive stones)
    const trilis = [[-3, -2], [-4.5, 2], [0, 4.5], [4.5, 2], [3, -2]];
    trilis.forEach((pos, i) => {
      const a = Math.atan2(pos[0], pos[1]);
      const h = 6 + (i===2 ? 1 : 0); // Center is tallest
      addBox(g, stone, [1.8, h, 1.2], [pos[0]-0.9*Math.cos(a), h/2, pos[1]+0.9*Math.sin(a)], [0, a, 0]);
      addBox(g, stone, [1.8, h, 1.2], [pos[0]+0.9*Math.cos(a), h/2, pos[1]-0.9*Math.sin(a)], [0, a, 0]);
      addBox(g, stone, [3.8, 1.0, 1.4], [pos[0], h+0.5, pos[1]], [0, a + Math.PI/2, 0]);
    });
    
    // Bluestones (Small inner circle)
    for(let i=0; i<40; i++){
      if(Math.random() > 0.4) continue;
      const a = i*Math.PI*2/40;
      addBox(g, stone, [0.8, 2, 0.8], [Math.sin(a)*5.5, 1, Math.cos(a)*5.5], [0,a,0]);
    }
    
    // Altar stone & Heel stone
    addBox(g, stone, [3, 0.5, 1.5], [0, 0.25, 2]);
    addBox(g, stone, [1.5, 4.8, 1.5], [0, 2.4, 22], [0.1, Math.PI/6, 0.1]); // Heel stone far out
    
    addCylinder(g, grass, 28, 28, 0.2, [0, -0.1, 0], null, 64);
    return g;
  },

  Parthenon: () => {
    const g = new THREE.Group();
    const marble = new THREE.MeshLambertMaterial({ color:0xeae0c8 });
    const darkMarble = new THREE.MeshLambertMaterial({ color:0xd0c6ae });
    
    // Stylobate (Crepidoma) 3 steps
    for(let i=0;i<3;i++) addBox(g, marble, [22-i*0.6, 0.5, 12-i*0.6], [0, 0.25+i*0.5, 0]);
    
    // Peristyle Columns (8 x 17 Doric)
    const cx=17, cz=8, sx=1.2, sz=1.35;
    for(let x=0;x<cx;x++) {
      for(let z=0;z<cz;z++){
        if(!(x===0||x===cx-1||z===0||z===cz-1)) continue;
        const px = (x-(cx-1)/2)*sx;
        const pz = (z-(cz-1)/2)*sz;
        addCylinder(g, marble, 0.35, 0.42, 5, [px, 4.0, pz]); // Fluted taper
        addBox(g, marble, [0.9, 0.2, 0.9], [px, 6.6, pz]); // Doric capital
      }
    }
    
    // Entablature (Architrave + Frieze)
    addBox(g, marble, [20.4, 0.8, 10.4], [0, 7.1, 0]);
    addBox(g, darkMarble, [20.4, 0.8, 10.4], [0, 7.9, 0]); // Frieze (metopes/triglyphs represented by color difference)
    
    // Pediments
    const pedGeo = new THREE.CylinderGeometry(5.2, 5.2, 20.4, 3);
    pedGeo.rotateZ(Math.PI/2); pedGeo.rotateX(Math.PI/2);
    const ped = new THREE.Mesh(pedGeo, marble);
    ped.position.set(0, 9.6, 0);
    ped.scale.set(1, 0.35, 1);
    g.add(ped);
    
    // Cella (Inner sanctuary)
    addBox(g, darkMarble, [14, 5, 6], [0, 4.0, 0]);
    
    // Ruined roof
    const roofMat = new THREE.MeshLambertMaterial({ color:0x8a3a2a });
    addBox(g, roofMat, [5, 0.2, 5], [-5, 10.5, 0], [0,0,Math.PI/12]); // Collapsed roof tiles
    
    return g;
  },

  EiffelTower: () => {
    const g = new THREE.Group();
    const iron = new THREE.MeshLambertMaterial({ color:0x4d4540, wireframe:true });
    const solidIron = new THREE.MeshLambertMaterial({ color:0x3a332f });
    
    // Legs & Base
    const addLeg = (x, z, rot) => {
      // Intricate lattice pillars
      addBox(g, iron, [2.5, 8, 2.5], [x, 4, z], [0, rot, 0.25]);
      addBox(g, iron, [1.8, 8, 1.8], [x*0.65, 11, z*0.65], [0, rot, 0.15]);
      // Decorative arches at base
      addCylinder(g, solidIron, 4, 4, 0.5, [x*0.5, 2, 0], [Math.PI/2, 0, 0], 32); 
    };
    addLeg(-6, -6, Math.PI/4);
    addLeg(6, -6, -Math.PI/4);
    addLeg(-6, 6, Math.PI*3/4);
    addLeg(6, 6, -Math.PI*3/4);

    // Platforms with railings
    addBox(g, solidIron, [13, 0.6, 13], [0, 8, 0]);
    addBox(g, solidIron, [8, 0.6, 8], [0, 15, 0]);
    addBox(g, solidIron, [3, 0.6, 3], [0, 27, 0]);

    // Main tower body
    const towerGeo = new THREE.CylinderGeometry(0.3, 4, 12, 4);
    towerGeo.rotateY(Math.PI/4);
    const tower = new THREE.Mesh(towerGeo, iron);
    tower.position.set(0, 21, 0);
    g.add(tower);

    // Spire and antenna
    addCylinder(g, solidIron, 0.8, 0.8, 2, [0, 28, 0]); // Cupola
    addCylinder(g, solidIron, 0.1, 0.1, 4, [0, 31, 0]);
    return g;
  },

  ChristTheRedeemer: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0xdcd6cc });
    const baseMat = new THREE.MeshLambertMaterial({ color:0x555555 });
    
    // Corcovado peak & Pedestal
    addCylinder(g, baseMat, 3, 4, 3, [0, 1.5, 0], null, 16);
    addCylinder(g, stone, 1.5, 1.5, 2, [0, 4, 0], null, 8); // Art deco pedestal
    
    // Robe/Body (Tapered with folds representation)
    addCylinder(g, stone, 1.2, 1.8, 8, [0, 9, 0], null, 16); 
    addCylinder(g, stone, 1.4, 1.4, 4, [0, 9, 0.5], null, 8); // Front fold
    
    // Outstretched Arms (Art deco style)
    addBox(g, stone, [15, 1.5, 1.5], [0, 13, 0]); 
    // Hands
    addBox(g, stone, [1, 0.8, 1], [-8, 13, 0]); 
    addBox(g, stone, [1, 0.8, 1], [8, 13, 0]); 
    
    // Head and Face
    addSphere(g, stone, 1.2, [0, 14.8, 0]); 
    addBox(g, stone, [1.4, 1.0, 1.4], [0, 15.2, 0]); // Crown/Hair
    
    return g;
  },

  StatueOfLiberty: () => {
    const g = new THREE.Group();
    const copper = new THREE.MeshLambertMaterial({ color:0x66c6a6 });
    const pedMat = new THREE.MeshLambertMaterial({ color:0xcbbd9d });
    
    // Fort Wood (11-point star)
    for(let i=0;i<11;i++) {
      addBox(g, pedMat, [9, 2, 3], [0, 1, 0], [0, i*Math.PI*2/11, 0]);
    }
    // Pedestal (Neoclassical)
    addBox(g, pedMat, [6, 2, 6], [0, 3, 0]);
    addBox(g, pedMat, [4.5, 8, 4.5], [0, 8, 0]);
    addBox(g, pedMat, [5, 1, 5], [0, 12.5, 0]); 
    
    // Body (Copper robes)
    addCylinder(g, copper, 1.5, 2.5, 9, [0, 17.5, 0], null, 16);
    // Robe folds
    for(let i=0;i<6;i++){
      addCylinder(g, copper, 0.3, 0.6, 9, [Math.sin(i)*1.5, 17.5, Math.cos(i)*1.5]);
    }
    
    // Head & Crown
    addSphere(g, copper, 1.2, [0, 23, 0]);
    addCylinder(g, copper, 1.4, 1.4, 0.4, [0, 23.5, 0]); // Crown base
    for(let i=0;i<7;i++) { // 7 Rays
      addCone(g, copper, 0.1, 1.5, [0, 24, 0], [0, 0, -Math.PI/3 + i*Math.PI/9]);
    }
    
    // Torch Arm
    addCylinder(g, copper, 0.4, 0.5, 5, [1.8, 21, 0], [0, 0, Math.PI/8]);
    addCylinder(g, copper, 0.8, 0.2, 1, [2.8, 23.5, 0]); // Chalice
    addSphere(g, new THREE.MeshLambertMaterial({ color:0xffd700 }), 0.7, [2.8, 24.2, 0]); // Gold Flame
    
    // Tablet Arm
    addBox(g, copper, [1.5, 2, 0.2], [-1.2, 19, 1.5], [-Math.PI/6, -Math.PI/4, 0]);
    
    return g;
  },

  EasterIslandMoai: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x736659 });
    const pukaoMat = new THREE.MeshLambertMaterial({ color:0xa65341 }); 
    const ahuMat = new THREE.MeshLambertMaterial({ color:0x50453d }); 
    
    // Ahu (Stone platform)
    addBox(g, ahuMat, [8, 1.5, 6], [0, 0.75, 0]);
    
    // Tapered Body
    addCylinder(g, stone, 1.8, 2.5, 6, [0, 4.5, 0], null, 8);
    // Hands on belly
    addBox(g, stone, [3, 0.5, 2], [0, 3.5, 1.5]); 
    
    // Massive Head
    addBox(g, stone, [3.2, 5.5, 3.2], [0, 10, 0.5]);
    
    // Facial Features
    addBox(g, stone, [3.6, 1.0, 1.2], [0, 12, 2.2]); // Heavy Brow
    addBox(g, stone, [0.8, 3.5, 1.5], [0, 10, 2.5]); // Long Nose
    addBox(g, stone, [3.6, 1.8, 1.5], [0, 7.5, 2]); // Strong Jaw/Chin
    addBox(g, stone, [0.5, 3, 0.5], [-1.8, 10, 0.5]); // Elongated Ears
    addBox(g, stone, [0.5, 3, 0.5], [ 1.8, 10, 0.5]); 
    
    // Pukao (Red scoria topknot)
    addCylinder(g, pukaoMat, 2.2, 2.2, 1.8, [0, 13.6, 0.5]);
    addCylinder(g, pukaoMat, 1.0, 1.0, 0.5, [0, 14.7, 0.5]); // Top knot nub
    
    return g;
  },

  AngkorWat: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x8c8066 });
    const dark = new THREE.MeshLambertMaterial({ color:0x6c6046 });
    
    // 3 stepped rectangular enclosures with galleries
    addBox(g, stone, [32, 2, 24], [0, 1, 0]);
    addBox(g, stone, [22, 2, 16], [0, 3, 0]);
    addBox(g, stone, [12, 3, 10], [0, 5.5, 0]);
    
    // Gallery roofs (cruciform)
    addBox(g, dark, [30, 0.5, 1], [0, 2.2, 0]);
    addBox(g, dark, [1, 0.5, 22], [0, 2.2, 0]);
    
    // Lotus bud towers (Quincunx)
    const addTower = (x, y, z, scale) => {
      addBox(g, stone, [2.5*scale, 3*scale, 2.5*scale], [x, y+1.5*scale, z]);
      // Tiers of the lotus
      for(let i=0;i<6;i++){
        const r = (1.8-i*0.25)*scale;
        addCylinder(g, stone, r, r*1.1, 0.8*scale, [x, y+(3.4+i*0.8)*scale, z], null, 8);
      }
      addSphere(g, stone, 0.5*scale, [x, y+8.2*scale, z]); // Finial
    };
    
    // 4 Corner towers
    addTower(-5, 7, -4, 0.7); 
    addTower( 5, 7, -4, 0.7);
    addTower(-5, 7,  4, 0.7);  
    addTower( 5, 7,  4, 0.7);
    // Central massive tower
    addTower(0, 7, 0, 1.4);
    
    return g;
  },

  GreatWallSection: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x998c7a });
    const brick = new THREE.MeshLambertMaterial({ color:0x807361 });
    
    // Winding path sections using multiple boxes
    for(let i=-2; i<=2; i++) {
      const x = Math.sin(i)*3;
      const z = i*6;
      const rotY = Math.cos(i)*0.2;
      addBox(g, stone, [8, 6, 6.5], [x, 3, z], [0, rotY, 0]);
      
      // Crenellations along this section
      for(let dz=-2.5; dz<=2.5; dz+=1.5){
        addBox(g, brick, [0.8, 1.5, 1], [x - 3.6*Math.cos(rotY), 6.75, z + dz], [0, rotY, 0]);
        addBox(g, brick, [0.8, 1.5, 1], [x + 3.6*Math.cos(rotY), 6.75, z + dz], [0, rotY, 0]);
      }
    }
    
    // Watchtower
    addBox(g, brick, [11, 10, 11], [0, 5, 0]);
    addBox(g, stone, [12, 1, 12], [0, 10.5, 0]);
    // Arched doorways
    addBox(g, new THREE.MeshLambertMaterial({ color:0x222 }), [3, 4, 11.5], [0, 7, 0]);
    
    // Tower crenellations
    for(let x=-5;x<=5;x+=2.5) {
      addBox(g, stone, [1, 1.5, 0.5], [x, 11.75, -5.75]);
      addBox(g, stone, [1, 1.5, 0.5], [x, 11.75,  5.75]);
    }
    for(let z=-3.5;z<=3.5;z+=2.5) {
      addBox(g, stone, [0.5, 1.5, 1], [-5.75, 11.75, z]);
      addBox(g, stone, [0.5, 1.5, 1], [ 5.75, 11.75, z]);
    }
    
    return g;
  },

  PetraTreasury: () => {
    const g = new THREE.Group();
    const rock = new THREE.MeshLambertMaterial({ color:0xd1805a }); // Rose-red sandstone
    const darkRock = new THREE.MeshLambertMaterial({ color:0x8a4b31 });
    const voidMat = new THREE.MeshLambertMaterial({ color:0x1a0f0a });
    
    // Cliff back wall & overhang
    addBox(g, rock, [26, 30, 8], [0, 15, -4]);
    
    // Lower level
    addBox(g, rock, [20, 2, 4], [0, 1, 2]); // Base
    
    // 6 Corinthian columns
    for(let x of [-8, -4.5, -1.5, 1.5, 4.5, 8]) {
      addCylinder(g, rock, 0.7, 0.8, 10, [x, 7, 3]); 
      addBox(g, rock, [1.6, 1.0, 1.6], [x, 12.5, 3]); // Capital
    }
    // Main Entrance void
    addBox(g, voidMat, [4, 8, 3], [0, 6, 1]); 
    
    // Lower Triangular Pediment
    const pedGeo = new THREE.CylinderGeometry(10, 10, 4, 3);
    pedGeo.rotateZ(Math.PI/2); pedGeo.rotateX(Math.PI/2);
    const ped = new THREE.Mesh(pedGeo, rock);
    ped.position.set(0, 15, 3);
    g.add(ped);

    // Upper level
    addBox(g, darkRock, [20, 1, 3], [0, 17.5, 2]); // Cornice
    
    // Broken pediment wings
    addBox(g, rock, [5, 8, 2], [-6.5, 22, 2]); // Left wing
    addBox(g, rock, [5, 8, 2], [ 6.5, 22, 2]); // Right wing
    
    // Wing columns
    addCylinder(g, rock, 0.6, 0.6, 8, [-8, 22, 3]);
    addCylinder(g, rock, 0.6, 0.6, 8, [-5, 22, 3]);
    addCylinder(g, rock, 0.6, 0.6, 8, [ 5, 22, 3]);
    addCylinder(g, rock, 0.6, 0.6, 8, [ 8, 22, 3]);
    
    // Central Tholos (round temple)
    addCylinder(g, rock, 3.5, 3.5, 8, [0, 22, 2]);
    // Tholos columns
    for(let a=0; a<Math.PI; a+=Math.PI/3){
      addCylinder(g, rock, 0.5, 0.5, 8, [Math.cos(a)*3.8, 22, 2+Math.sin(a)*3.8]);
    }
    addCone(g, rock, 4, 3, [0, 27.5, 2]); // Conical roof
    addSphere(g, rock, 0.8, [0, 29.5, 2]); // Urn
    
    return g;
  },

  ChichenItza: () => {
    const g = new THREE.Group();
    const stone = new THREE.MeshLambertMaterial({ color:0x9e9482 });
    const darkStone = new THREE.MeshLambertMaterial({ color:0x7a7161 });
    
    const tiers = 9;
    const baseSize = 24;
    const tierH = 1.4;
    
    // 9 Terraces
    for(let i=0;i<tiers;i++){
      const s = baseSize - i*2.2;
      addBox(g, stone, [s, tierH, s], [0, i*tierH + tierH/2, 0]);
      addBox(g, darkStone, [s+0.2, 0.2, s+0.2], [0, i*tierH + tierH, 0]); // Cornice
    }
    
    // 4 Stairways (365 steps total conceptually)
    const stairMat = new THREE.MeshLambertMaterial({ color:0x8e8472 });
    for(let s=0;s<4;s++){
      const a = s*Math.PI/2;
      const px = Math.sin(a)*(baseSize/2);
      const pz = Math.cos(a)*(baseSize/2);
      
      const rampGeo = new THREE.CylinderGeometry(0.5, 6, tiers*tierH, 4);
      rampGeo.rotateY(Math.PI/4); rampGeo.rotateX(Math.PI/2);
      const stair = new THREE.Mesh(rampGeo, stairMat);
      stair.position.set(px, tiers*tierH*0.5, pz);
      stair.rotation.set(0, a, 0);
      stair.scale.set(1, 1, 0.5);
      g.add(stair);
      
      // Feathered Serpent Heads at the base of North stairway (s===0 is North in our local space)
      if (s === 0) {
        addBox(g, stone, [1.5, 1.5, 2], [px + 1.5, 0.75, pz + 4]); 
        addBox(g, stone, [1.5, 1.5, 2], [px - 1.5, 0.75, pz + 4]); 
      }
    }
    
    // Upper Temple
    const h = tiers*tierH;
    addBox(g, stone, [6, 4, 6], [0, h+2, 0]);
    addBox(g, darkStone, [6.5, 0.8, 6.5], [0, h+4.4, 0]);
    addBox(g, new THREE.MeshLambertMaterial({ color:0x222 }), [2, 3, 6.2], [0, h+1.5, 0]); // Doors
    
    return g;
  },

  LeaningTowerPisa: () => {
    const g = new THREE.Group();
    const marble = new THREE.MeshLambertMaterial({ color:0xf0ebdc });
    const shadow = new THREE.MeshLambertMaterial({ color:0x1a1a1a });
    
    const pivot = new THREE.Group();
    pivot.rotation.z = Math.PI/180 * 3.99; // True ~4 degree lean
    g.add(pivot);
    
    // Blind arcade base
    addCylinder(pivot, marble, 4.5, 4.5, 2.5, [0, 1.25, 0], null, 32);
    // 15 columns attached to base
    for(let c=0;c<15;c++){
      const a = c*Math.PI*2/15;
      addCylinder(pivot, marble, 0.2, 0.2, 2.5, [Math.sin(a)*4.6, 1.25, Math.cos(a)*4.6]);
    }
    
    // 6 Arcaded loggia stories
    for(let i=0;i<6;i++){
      const y = i*2.2 + 3.6;
      addCylinder(pivot, shadow, 3.2, 3.2, 2.2, [0, y, 0], null, 32); // Inner core
      addCylinder(pivot, marble, 4.2, 4.2, 0.3, [0, y-0.95, 0], null, 32); // Balcony floor
      // 30 delicate columns per tier
      for(let c=0;c<30;c++){
        const a = c*Math.PI*2/30;
        addCylinder(pivot, marble, 0.12, 0.12, 1.9, [Math.sin(a)*3.9, y, Math.cos(a)*3.9]);
      }
    }
    
    // Belfry top (narrower, 16 columns)
    const belfryY = 17.5;
    addCylinder(pivot, shadow, 2.8, 2.8, 2.5, [0, belfryY, 0], null, 32);
    addCylinder(pivot, marble, 3.4, 3.4, 0.3, [0, belfryY-1.1, 0], null, 32);
    for(let c=0;c<16;c++){
      const a = c*Math.PI*2/16;
      addCylinder(pivot, marble, 0.15, 0.15, 2.2, [Math.sin(a)*3.1, belfryY, Math.cos(a)*3.1]);
    }
    
    return g;
  },

  SydneyOperaHouse: () => {
    const g = new THREE.Group();
    const sailMat = new THREE.MeshStandardMaterial({ color:0xf5f5f3, roughness:0.2, metalness:0.1 });
    const glassMat = new THREE.MeshStandardMaterial({ color:0x1a2530, roughness:0.1, metalness:0.8 });
    const baseMat = new THREE.MeshLambertMaterial({ color:0xa68259 });
    const waterMat = new THREE.MeshLambertMaterial({ color:0x2d6086 });
    
    // Monumental podium
    addBox(g, baseMat, [20, 1.5, 12], [0, 0.75, 0]);
    addBox(g, baseMat, [24, 0.5, 14], [0, 0.25, 0]); // Broad steps
    
    // Function to create spherical shell segments
    const addShell = (x, z, radius, rotY, scale) => {
      // We simulate the spherical triangles using a sphere slice
      const shellGeo = new THREE.SphereGeometry(radius, 32, 16, 0, Math.PI/4, 0, Math.PI/2);
      const shell1 = new THREE.Mesh(shellGeo, sailMat);
      shell1.position.set(x, 1.5, z);
      shell1.rotation.set(-Math.PI/2, rotY, 0);
      shell1.scale.setScalar(scale);
      g.add(shell1);
      
      const shell2 = new THREE.Mesh(shellGeo, sailMat);
      shell2.position.set(x, 1.5, z);
      shell2.rotation.set(-Math.PI/2, rotY + Math.PI/2, 0);
      shell2.scale.set(-scale, scale, scale); // Mirror
      g.add(shell2);
      
      // Chevron Glass Wall
      addBox(g, glassMat, [radius*scale*0.8, radius*scale*0.6, 0.2], [x + Math.sin(rotY)*radius*0.4, 1.5 + radius*scale*0.3, z + Math.cos(rotY)*radius*0.4], [0, rotY+Math.PI/4, 0]);
    };
    
    // Concert Hall (Main group)
    addShell(-4, 0, 5, Math.PI/4, 1.2);
    addShell(-2, 0, 4, Math.PI/4, 1.0);
    addShell(0,  0, 3, Math.PI/4, 0.8);
    
    // Opera Theater (Smaller group)
    addShell(4, -2, 4, Math.PI/4, 1.0);
    addShell(5.5, -2, 3, Math.PI/4, 0.8);
    addShell(7,  -2, 2, Math.PI/4, 0.6);
    
    // Bennelong Restaurant (Smallest)
    addShell(6, 3, 2.5, Math.PI/4, 0.7);
    
    // Harbor Water
    addBox(g, waterMat, [36, 0.1, 24], [0, 0.05, 0]);
    
    return g;
  }
};
