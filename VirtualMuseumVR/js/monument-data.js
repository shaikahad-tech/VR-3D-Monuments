// Monument Data — rich architectural, historical, artifact metadata
window.MONUMENT_DATA_RICH = {
  TajMahal: {
    title:"Taj Mahal", era:"1632–1653 CE (Mughal)", location:"Agra, India", color:0xf3f0eb,
    architect:"Ustad Ahmad Lahori",
    style:"Mughal — Indo-Islamic composite",
    components:["Bulbous onion dome (gumbad)","Four minarets (manāra)","Iwan arched portal","Pietra dura inlay","Charbagh garden","Hauz-i-Kausar reflecting pool"],
    materials:"White Makrana marble; red sandstone; 28 semi-precious stones incl. lapis lazuli, turquoise, jade",
    purpose:"Mausoleum built by Shah Jahan for Mumtaz Mahal who died in childbirth (1631)",
    dynasty:"Mughal Empire",
    narration:"You stand before the Taj Mahal — the pinnacle of Mughal architecture. The central dome rises 73 meters, its white marble shifting from pink at dawn to gold in moonlight. Notice the four minarets angled outward — an ingenious earthquake safeguard, designed to fall away from the tomb.",
    artifacts:[
      {name:"Pietra Dura Inlay Panel",type:"decorative",material:"Semi-precious stone",origin:"Agra workshops",usage:"Wall decoration",symbolism:"Paradise garden motifs — flowers symbolize divine beauty"},
      {name:"Calligraphy Cartouche",type:"inscription",material:"Black marble inlay",origin:"Quranic verses",usage:"Religious inscription on iwan",symbolism:"Verses from Surah Al-Fajr — 'O soul at peace, return to thy Lord'"},
      {name:"Cenotaph of Mumtaz Mahal",type:"sculpture",material:"White marble with inlay",origin:"Central chamber",usage:"Memorial marker above real tomb",symbolism:"Eternal love; feminine form represented by floral designs"}
    ],
    quiz:[
      {q:"Who commissioned the Taj Mahal?",a:"Emperor Shah Jahan",options:["Akbar","Shah Jahan","Aurangzeb","Humayun"]},
      {q:"Why do the minarets tilt outward?",a:"Earthquake safety",options:["Aesthetic choice","Earthquake safety","Poor construction","Wind resistance"]},
      {q:"What is pietra dura?",a:"Stone inlay decoration",options:["A type of dome","Stone inlay decoration","Persian calligraphy","A garden style"]}
    ]
  },
  RedFort: {
    title:"Red Fort (Lal Qila)", era:"1639–1648 CE (Mughal)", location:"Delhi, India", color:0xb24131,
    architect:"Ustad Ahmad Lahori",
    style:"Mughal Architecture",
    components:["Lahori Gate","Delhi Gate","Diwan-i-Aam (Public Audience Hall)","Diwan-i-Khas (Private Audience Hall)","Moti Masjid (Pearl Mosque)","Chhatta Chowk (covered bazaar)"],
    materials:"Red sandstone defensive walls; white marble interior palaces; formerly inlaid with precious stones",
    purpose:"Main residence of the Mughal Emperors, constructed when the capital moved from Agra to Delhi",
    dynasty:"Mughal Empire",
    narration:"The Red Fort represents the zenith of Mughal creativity under Shah Jahan. Enclosed by massive red sandstone walls, the fort contained palaces of pristine white marble. The famous Peacock Throne once resided here before it was looted by Nadir Shah in 1739.",
    artifacts:[
      {name:"The Peacock Throne (replica)",type:"furniture",material:"Gold and precious gems",origin:"Diwan-i-Khas",usage:"Emperor's throne",symbolism:"Unparalleled wealth and power of the Mughal Empire"}
    ],
    quiz:[
      {q:"Which emperor commissioned the Red Fort?",a:"Shah Jahan",options:["Akbar","Jahangir","Shah Jahan","Aurangzeb"]},
      {q:"What was the primary material used for the fort's massive outer walls?",a:"Red sandstone",options:["White marble","Red sandstone","Granite","Limestone"]}
    ]
  },
  QutubMinar: {
    title:"Qutub Minar", era:"1193–1368 CE (Delhi Sultanate)", location:"Delhi, India", color:0xc66c44,
    architect:"Qutb ud-Din Aibak (begun); completed by Iltutmish",
    style:"Indo-Islamic — Ghurid",
    components:["Five tapering storeys (tiers)","Fluted shaft","Projecting balconies (muqarnas brackets)","Epigraphic bands","Sandstone and marble alternating bands"],
    materials:"Red Rajputana sandstone; white marble (upper storeys)",
    purpose:"Victory tower; minaret for call to prayer; assertion of Islamic power",
    dynasty:"Delhi Sultanate — Mamluk Dynasty",
    narration:"The Qutub Minar, rising 72.5 meters, is the world's tallest brick minaret. Built as a victory tower following the defeat of the last Hindu kingdom in Delhi, each storey is separated by a projecting balcony with muqarnas — honeycomb corbelling of Persian origin.",
    artifacts:[
      {name:"Iron Pillar of Delhi",type:"metallurgy",material:"98% pure iron",origin:"4th–5th century CE, Gupta period",usage:"Originally a Vishnu standard",symbolism:"Remarkable rust-resistance for 1600+ years; ancient metallurgical mastery"},
      {name:"Quwwat-ul-Islam Screen",type:"architecture fragment",material:"Sandstone with arabesque carving",origin:"First mosque in India",usage:"Ornamental screen",symbolism:"Synthesis of Hindu craftsmanship and Islamic geometric design"}
    ],
    quiz:[
      {q:"What is the height of Qutub Minar?",a:"72.5 meters",options:["45 meters","72.5 meters","100 meters","58 meters"]},
      {q:"What architectural element are muqarnas?",a:"Honeycomb corbelling",options:["A type of arch","Honeycomb corbelling","Pointed towers","Floor tiles"]}
    ]
  },
  GreatPyramidGiza: {
    title:"Great Pyramid of Giza", era:"c. 2580–2560 BCE (Old Kingdom)", location:"Giza, Egypt", color:0xd8c08c,
    architect:"Hemiunu (vizier to Khufu)",
    style:"Ancient Egyptian — True Pyramid",
    components:["King's Chamber (granite)","Queen's Chamber","Grand Gallery (corbelled vault)","Descending passage","Original Tura limestone casing (stripped)","Mortuary temple","Causeway"],
    materials:"2.3 million limestone blocks (avg. 2.5 tons); Aswan granite (interior chambers); white Tura limestone casing",
    purpose:"Royal funerary monument for Pharaoh Khufu; ensure ka (life force) survived death",
    dynasty:"Fourth Dynasty, Old Kingdom",
    narration:"For 3,800 years the Great Pyramid stood as the tallest structure on Earth. Its base is aligned to the cardinal directions to within one-fifteenth of a degree. Inside, the Grand Gallery rises 8.5 meters — a corbelled vault of breathtaking precision, each course of stone offset 6 centimeters inward.",
    artifacts:[
      {name:"Cartouche of Khufu",type:"inscription",material:"Red ochre paint on limestone",origin:"Relieving chambers",usage:"Royal name stamp",symbolism:"Identifies builder; only Khufu's name found inside the pyramid"},
      {name:"Canopic Jar",type:"funerary vessel",material:"Alabaster",origin:"Old Kingdom burial tradition",usage:"Stored mummified organs",symbolism:"Four sons of Horus guard liver, lungs, stomach, intestines"},
      {name:"Shabti Figure",type:"sculpture",material:"Faience (glazed ceramic)",origin:"Egyptian funerary tradition",usage:"Servant figurine placed in tomb",symbolism:"Would labor on behalf of the deceased in the afterlife"}
    ],
    quiz:[
      {q:"How many limestone blocks are in the Great Pyramid?",a:"~2.3 million",options:["500,000","~2.3 million","5 million","1 million"]},
      {q:"Who was the architect of the Great Pyramid?",a:"Hemiunu",options:["Imhotep","Hemiunu","Khufu","Sneferu"]}
    ]
  },
  Colosseum: {
    title:"Roman Colosseum", era:"72–80 CE (Roman Imperial)", location:"Rome, Italy", color:0xc6a681,
    architect:"Vespasian (commissioned); Titus (completed)",
    style:"Roman Imperial — Flavian Amphitheatre",
    components:["Four-storey facade","Doric/Ionic/Corinthian/Composite orders per level","80 arched entrances (vomitoria)","Velarium (awning system)","Hypogeum (underground tunnels)","Arena floor over trapdoors","Cavea (seating tiers)"],
    materials:"Travertine limestone (exterior piers); tufa concrete (radial walls); brick-faced concrete; iron clamps (now looted)",
    purpose:"Public spectacle venue — gladiatorial combat, venationes (animal hunts), executions, mock naval battles",
    dynasty:"Flavian Dynasty — Roman Empire",
    narration:"The Colosseum could fill in 15 minutes through its 80 vomitoria — the numbered entrances each tier of Roman society used. Beneath the arena, the hypogeum housed 36 trap doors, 32 animal pens, and an elevator system of counterweighted platforms that could raise beasts directly into the arena.",
    artifacts:[
      {name:"Gladiator Helmet (Galea)",type:"weapon/armor",material:"Bronze",origin:"Roman gladiatorial workshops",usage:"Head protection for secutor-class gladiator",symbolism:"Visored design prevented eye contact — dehumanizing the fighter"},
      {name:"Gladius",type:"weapon",material:"Iron with bone grip",origin:"Roman military",usage:"Short thrusting sword of gladiators",symbolism:"Symbol of Roman martial prowess"},
      {name:"Spectator Token (Tessera)",type:"coin/token",material:"Bone or ivory",origin:"Arena administration",usage:"Numbered entry ticket indicating gate, tier, row, seat",symbolism:"First ticketing system in history"}
    ],
    quiz:[
      {q:"What is a vomitorium?",a:"Numbered arena entrance/exit",options:["A dining room","Numbered arena entrance/exit","Underground tunnel","Animal cage"]},
      {q:"What are the four architectural orders on the facade?",a:"Doric, Ionic, Corinthian, Composite",options:["Doric, Ionic, Corinthian, Composite","Greek, Roman, Gothic, Byzantine","Tuscan, Doric, Ionic, Corinthian","None — it has one order"]}
    ]
  },
  Parthenon: {
    title:"Parthenon", era:"447–432 BCE (Classical Greece)", location:"Athens, Greece", color:0xeae0c8,
    architect:"Ictinus & Callicrates; supervised by Phidias",
    style:"Classical Greek — Doric order with Ionic frieze",
    components:["Peristyle (46 outer Doric columns)","Opisthodomos (rear porch)","Naos (inner cella)","Entasis (column curvature)","Metopes (carved panels)","Continuous Ionic frieze","Pediment sculptures","Stylobate (curved platform)"],
    materials:"13,400 blocks of Pentelic marble; iron and lead clamps; wooden roof (lost)",
    purpose:"Temple to Athena Parthenos; treasury; symbol of Athenian democracy and power",
    dynasty:"Classical Athens — Age of Pericles",
    narration:"Nothing in the Parthenon is perfectly straight. The stylobate curves upward 6 centimeters across its width. The columns lean inward slightly. Corner columns are thicker. These optical refinements — called eurhythmia — counteract the illusion of sagging that geometrically perfect lines would create at this scale.",
    artifacts:[
      {name:"Phidias's Athena Parthenos (replica)",type:"sculpture",material:"Gold and ivory (chryselephantine) — original lost",origin:"448 BCE Athens",usage:"Cult statue inside cella, 12 meters tall",symbolism:"Athena as warrior-goddess; shield depicted the Battle of Marathon"},
      {name:"Elgin Marble Fragment",type:"frieze",material:"Pentelic marble",origin:"Parthenon frieze",usage:"Depicts Panathenaic procession",symbolism:"Athenian civic identity — the procession of all citizens to honor Athena"}
    ],
    quiz:[
      {q:"What is entasis?",a:"Subtle convex swelling of columns",options:["A type of frieze","Subtle convex swelling of columns","The curved stylobate","A Greek sacrifice"]},
      {q:"Who supervised the Parthenon's construction?",a:"Phidias",options:["Socrates","Pericles","Phidias","Plato"]}
    ]
  },
  AngkorWat: {
    title:"Angkor Wat", era:"1113–1150 CE (Khmer Empire)", location:"Siem Reap, Cambodia", color:0x8c8066,
    architect:"Suryavarman II (patron); Khmer master builders",
    style:"Khmer — Temple-mountain (quincunx plan)",
    components:["Central prasat (tower) — Mount Meru","Four corner towers","Concentric galleries","Bas-relief galleries (800m of carvings)","Moat (190m wide)","Cruciform terrace","Naga balustrades"],
    materials:"Sandstone blocks (10 million tons); laterite fill; timber (lost)",
    purpose:"State temple dedicated to Vishnu; later Buddhist; royal mausoleum",
    dynasty:"Khmer Empire",
    narration:"Angkor Wat's layout encodes the Hindu cosmos. The central tower is Mount Meru, home of the gods. The moat represents the cosmic ocean. The three gallery levels represent the realms of the underworld, earth, and heaven. At sunrise on the spring equinox, the sun rises directly over the central tower when viewed from the main entrance.",
    artifacts:[
      {name:"Apsara Relief Carving",type:"bas-relief",material:"Sandstone",origin:"Angkor Wat gallery walls",usage:"Decorative / religious",symbolism:"Celestial dancers (apsaras) — there are 1,796 unique apsara figures, each with a different expression"},
      {name:"Churning of the Ocean of Milk Panel",type:"bas-relief",material:"Sandstone",origin:"East gallery",usage:"Narrative religious scene",symbolism:"Creation myth — gods and demons churn cosmic ocean to produce amrita (immortality nectar)"}
    ],
    quiz:[
      {q:"What does the central tower of Angkor Wat represent?",a:"Mount Meru",options:["The king's palace","Mount Meru","A watchtower","The Buddhist stupa"]},
      {q:"How long is Angkor Wat's bas-relief gallery?",a:"~800 meters",options:["200 meters","~800 meters","2 kilometers","400 meters"]}
    ]
  },
  GreatWallSection: {
    title:"Great Wall of China", era:"7th C. BCE – 17th C. CE (multiple dynasties)", location:"Northern China", color:0x998c7a,
    architect:"Multiple — General Meng Tian (Qin); expanded under Ming",
    style:"Chinese military architecture",
    components:["Battlements (crenellations)","Watchtowers (every 300-500m)","Signal fire platforms (beacon towers)","Garrison stations","Ramparts (road on top)","Foundation rammed earth core","Brick/stone facing (Ming sections)"],
    materials:"Rammed earth (Qin); fired grey brick bonded with lime/sticky-rice mortar (Ming); granite foundations",
    purpose:"Defense against northern nomadic tribes (Xiongnu, Mongols); border control; trade regulation",
    dynasty:"Qin through Ming Dynasty",
    narration:"The 'sticky rice mortar' used in Ming Dynasty sections is a remarkable material — organic amylopectin from glutinous rice mixed with lime creates a mortar harder than the bricks it binds, which is why Ming sections survive today while earlier earthen walls have returned to dust.",
    artifacts:[
      {name:"Bronze Crossbow Trigger Mechanism",type:"weapon",material:"Bronze",origin:"Qin Dynasty armory",usage:"Advanced repeating crossbow mechanism",symbolism:"Standardized Qin military technology — triggers found across the empire are interchangeable"},
      {name:"Signal Fire Beacon",type:"military",material:"Stone platform",origin:"Ming Dynasty watchtowers",usage:"1 fire = enemy approaching; 2 = 500 troops; 3 = 1000; 4 = 5000+",symbolism:"First military communication network in East Asia"}
    ],
    quiz:[
      {q:"What unusual material was used in Ming Dynasty mortar?",a:"Sticky rice (glutinous rice)",options:["Animal blood","Sticky rice (glutinous rice)","Egg whites","Clay and straw"]},
      {q:"What dynasty built the most iconic stone sections?",a:"Ming Dynasty",options:["Qin Dynasty","Han Dynasty","Ming Dynasty","Tang Dynasty"]}
    ]
  },
  PetraTreasury: {
    title:"Al-Khazneh, Petra", era:"1st C. CE (Nabataean Kingdom)", location:"Petra, Jordan", color:0xd1805a,
    architect:"Nabataean craftsmen (Hellenistic influence)",
    style:"Nabataean — Hellenistic facade cut into sandstone",
    components:["Tholos (circular temple top)","Broken pediment","Urns (finials)","Corinthian columns (six lower)","Eagle reliefs","Dark interior triclinium","The Siq approach canyon","Rose-red sandstone cliff face"],
    materials:"Rose-red Nubian sandstone (carved directly — not built); no mortar",
    purpose:"Royal tomb (likely of Aretas IV); later used for treasury by Bedouin legend",
    dynasty:"Nabataean Kingdom",
    narration:"Al-Khazneh is not built — it is carved. Nabataean craftsmen worked downward from the top of the cliff face, removing stone to reveal the facade within. The engineering challenge was enormous: scaffolding had to support workers at the top while the lower sections were still uncarved cliff.",
    artifacts:[
      {name:"Nabataean Incense Burner",type:"ritual vessel",material:"Ceramic",origin:"Petra domestic context",usage:"Burning frankincense and myrrh for trade/ritual",symbolism:"Petra controlled the incense trade route — the source of Nabataean wealth"},
      {name:"Nabataean Coin (Bronze)",type:"coin",material:"Bronze",origin:"Petra mint, 1st C. BCE",usage:"Trade currency",symbolism:"Depicts Aretas IV — confirms Hellenistic cultural influence on Nabataean identity"}
    ],
    quiz:[
      {q:"How was Al-Khazneh constructed?",a:"Carved directly from the cliff face",options:["Built with sandstone blocks","Carved directly from the cliff face","Constructed of Roman concrete","Built with mud brick"]},
      {q:"What was the primary source of Nabataean wealth?",a:"Control of the incense trade",options:["Gold mining","Control of the incense trade","Silk weaving","Agriculture"]}
    ]
  },
  ChichenItza: {
    title:"El Castillo, Chichén Itzá", era:"9th–12th C. CE (Maya-Toltec)", location:"Yucatán, Mexico", color:0x9e9482,
    architect:"Maya master builders",
    style:"Mesoamerican — stepped pyramid (talud-tablero)",
    components:["Nine stepped tiers","Four stairways (91 steps each + 1 top platform = 365)","Temple of Kukulcan atop","Serpent balustrades","Inner pyramid (inside the outer shell)","Acoustic chirped echo (El Castillo handclap)"],
    materials:"Limestone blocks; lime stucco facing (originally painted red); rubble fill core",
    purpose:"Temple to Kukulcán (feathered serpent god); astronomical calendar in stone",
    dynasty:"Classic/Terminal Classic Maya; Toltec influence",
    narration:"El Castillo is a calendar in stone. Four stairways of 91 steps each plus the top platform equal exactly 365 — the solar year. At the spring and autumn equinoxes, the setting sun casts a serpentine shadow of seven triangles down the northern balustrade, creating the illusion of the feathered serpent Kukulcán descending to Earth.",
    artifacts:[
      {name:"Chac Mool Sculpture",type:"sculpture",material:"Limestone",origin:"Upper temple",usage:"Ritual offering receptacle — the bowl held sacrificial offerings",symbolism:"Reclining figure with head turned 90° — the messenger between humans and gods"},
      {name:"Jade Death Mask",type:"funerary",material:"Jade mosaic",origin:"Burial within inner pyramid",usage:"Funeral mask of buried ruler",symbolism:"Jade represented immortality and life force (ch'ulel) in Maya belief"}
    ],
    quiz:[
      {q:"What does the stairway count of El Castillo represent?",a:"365 days of the solar year",options:["The Maya Long Count","365 days of the solar year","The number of Maya gods","260-day ritual calendar"]},
      {q:"What phenomenon occurs at the equinox?",a:"A serpent shadow descends the staircase",options:["The temple glows at dawn","A serpent shadow descends the staircase","The shadow disappears entirely","A nearby cenote lights up"]}
    ]
  },
  Stonehenge: {
    title:"Stonehenge", era:"c. 3000–1500 BCE (Neolithic/Bronze Age)", location:"Wiltshire, England", color:0x8c8c8c,
    architect:"Unknown — built in multiple phases over 1,500 years",
    style:"Megalithic — post-and-lintel stone circle",
    components:["Sarsen trilithons (upright + lintel)","Bluestone inner circle","Altar Stone","Heel Stone","Aubrey Holes (cremation pits)","Avenue approach","Slaughter Stone"],
    materials:"Sarsen sandstone (30+ tons each, from Marlborough Downs 25km away); bluestones (4 tons, from Preseli Hills Wales 250km away)",
    purpose:"Disputed — astronomical calendar; cremation burial site; healing sanctuary; ancestor veneration",
    dynasty:"Neolithic / Bronze Age Britain",
    narration:"The bluestones of Stonehenge were transported 250 kilometers from the Preseli Hills in Wales — an extraordinary feat without wheeled vehicles or metal tools. Current evidence suggests they were dragged on sledges and floated on rafts. The sarsen trilithons were shaped using stone hammers — each one weighing over 30 tons.",
    artifacts:[
      {name:"Amesbury Archer's Copper Knife",type:"weapon",material:"Copper",origin:"Burial near Stonehenge, c. 2300 BCE",usage:"Personal weapon of wealthy Bronze Age chieftain",symbolism:"Oldest Bell Beaker culture burial in Britain — may represent the first metallurgists to arrive from Europe"},
      {name:"Chalk Plaques",type:"ritual object",material:"Chalk",origin:"Aubrey Holes",usage:"Unknown ritual purpose",symbolism:"Geometric markings — possibly lunar calendar notation"}
    ],
    quiz:[
      {q:"Where do Stonehenge's bluestones come from?",a:"Preseli Hills, Wales — 250km away",options:["Local quarry 5km away","Preseli Hills, Wales — 250km away","Scotland","France"]},
      {q:"What alignment does Stonehenge demonstrate?",a:"Sunrise at summer solstice",options:["Moonrise at winter solstice","Sunrise at summer solstice","Polaris (North Star)","Spring equinox sunset"]}
    ]
  },
  EiffelTower: {
    title:"Eiffel Tower", era:"1887–1889 CE (Belle Époque)", location:"Paris, France", color:0x4d4540,
    architect:"Gustave Eiffel (engineer); Stephen Sauvestre (architect of detailing)",
    style:"Structural Expressionism — wrought-iron lattice",
    components:["Four arched legs (piliers)","First platform (57m)","Second platform (115m)","Top platform (276m)","Lattice girder trusses","Hydraulic elevators","Flagpole and lightning rod"],
    materials:"Puddled wrought iron (7,300 tons); 2.5 million rivets; 60 tons of paint",
    purpose:"Entrance arch for 1889 World's Fair; radio transmission tower",
    dynasty:"French Third Republic",
    narration:"The Eiffel Tower was designed using the mathematics of wind resistance — its curved profile was calculated to resist horizontal wind forces with minimal material. Eiffel computed that wind pressure at the top would be 7kg per square meter. The hyperbolic profile that results happens to be beautiful — form following structural logic.",
    artifacts:[
      {name:"Gustave Eiffel's Office Model",type:"tools/equipment",material:"Wood and metal scale model",origin:"Eiffel's personal office (top floor)",usage:"Engineering reference model",symbolism:"Eiffel maintained a small apartment and office at the top — he used it for meteorological experiments"},
      {name:"Construction Rivet",type:"industrial artifact",material:"Wrought iron",origin:"Tower construction 1887–89",usage:"Structural fastener — each heated red-hot and driven by a team of 4",symbolism:"2.5 million rivets; none failed in 130+ years of service"}
    ],
    quiz:[
      {q:"What problem did the curved profile of the Eiffel Tower solve?",a:"Wind resistance",options:["Earthquake resistance","Wind resistance","Weight distribution","Foundation stability"]},
      {q:"How many rivets are in the Eiffel Tower?",a:"2.5 million",options:["500,000","2.5 million","10 million","1.2 million"]}
    ]
  },
  HampiChariot: {
    title:"Stone Chariot, Hampi", era:"15th–16th C. CE (Vijayanagara)", location:"Hampi, Karnataka, India", color:0xa68c66,
    architect:"Vijayanagara Empire master sculptors under Krishna Deva Raya",
    style:"Dravidian — Vijayanagara period temple art",
    components:["Monolithic stone chariot (Garuda Ratha)","Four ornamental wheels (functional rotation)","Carved elephant balustrades","Mandapa (pillared hall) atop chariot","Shikhara-style vimana roof","Detachable stone wheels"],
    materials:"Grey-green granite; carved from a single geological outcrop; lime mortar jointing",
    purpose:"Shrine to Garuda (Vishnu's vahana) within the Vittala Temple complex; processional chariot form symbolizes divine vehicle",
    dynasty:"Vijayanagara Empire — Tuluva Dynasty",
    narration:"This stone chariot is actually a shrine to Garuda, the eagle-mount of Lord Vishnu. Built in the 15th century within the Vittala Temple complex, its four wheels once turned on their axles — a remarkable feat of monolithic stone engineering. The entire chariot was carved from local granite, and the level of detail in the spokes, hub caps, and floral motifs rivals anything produced in marble.",
    artifacts:[
      {name:"Musical Pillar",type:"architecture",material:"Granite",origin:"Vittala Temple mandapa",usage:"Pillars that produce musical notes when struck",symbolism:"Demonstrates mastery of acoustics in stone — each pillar tuned to a different note of the Indian raga system"},
      {name:"Garuda Idol Fragment",type:"sculpture",material:"Black granite",origin:"Interior of the stone chariot",usage:"Devotional idol",symbolism:"Garuda symbolizes speed, power, and martial prowess — protector against serpents (nagas)"}
    ],
    quiz:[
      {q:"Which deity's vehicle (vahana) is the stone chariot dedicated to?",a:"Garuda (Vishnu's eagle mount)",options:["Nandi (Shiva's bull)","Garuda (Vishnu's eagle mount)","Hamsa (Brahma's swan)","Airavata (Indra's elephant)"]},
      {q:"What is remarkable about the chariot's wheels?",a:"They could actually rotate on their stone axles",options:["They are painted gold","They could actually rotate on their stone axles","They contain hidden chambers","They are made of metal"]}
    ]
  },
  KonarkSunTemple: {
    title:"Konark Sun Temple", era:"1250 CE (Eastern Ganga Dynasty)", location:"Konark, Odisha, India", color:0x8c6648,
    architect:"King Narasimhadeva I; designed by architect Bisu Maharana",
    style:"Kalinga Architecture — Nagara (North Indian temple style)",
    components:["24 elaborately carved wheels (sundial function)","7 horses pulling the chariot eastward","Jagamohana (assembly hall — surviving)","Deul (main sanctuary — collapsed)","Natya Mandapa (dance hall)","Chlorite stone erotic sculptures","Three-tier Pidha Deula roof"],
    materials:"Khondalite stone (iron-rich, weathers to rust color); chlorite; laterite; iron beams (internal framework)",
    purpose:"Temple dedicated to Surya (Sun God); designed as a colossal stone chariot carrying the sun across the sky; astronomical observatory",
    dynasty:"Eastern Ganga Dynasty",
    narration:"The Konark Sun Temple is a colossal chariot designed to carry the sun god Surya across the sky. Its 24 wheels are not merely decorative — each functions as a sundial, with the shadow cast by the spokes accurately telling the time of day to within a minute. Seven horses strain eastward, pulling the chariot toward the dawn. The now-collapsed main tower once rose over 60 meters — deliberately designed so that the first rays of sunrise would illuminate the sanctum.",
    artifacts:[
      {name:"Sundial Wheel",type:"architecture",material:"Khondalite stone",origin:"Temple platform",usage:"Each wheel is a functioning sundial — the shadow of the axle hub on the spokes tells the time",symbolism:"The cycle of time (kalachakra); each wheel represents one hour of the day"},
      {name:"Surya Idol",type:"sculpture",material:"Chlorite (greenish stone)",origin:"Temple sanctum",usage:"Primary deity — Sun God depicted with boots (possibly Persian influence)",symbolism:"Surya drives his chariot across the sky each day; depicted with 7 horses representing 7 days of the week"},
      {name:"Naga-Nagi Relief",type:"sculpture",material:"Khondalite",origin:"Temple exterior walls",usage:"Decorative/mythological panel",symbolism:"Serpent deities guarding the temple — fertility and protection symbolism"}
    ],
    quiz:[
      {q:"How many wheels does the Konark chariot have?",a:"24 (representing 24 hours)",options:["12","24 (representing 24 hours)","7","365"]},
      {q:"What practical function do the carved wheels serve?",a:"They function as sundials",options:["Water collection","They function as sundials","Structural support","Musical instruments"]}
    ]
  },
  AjantaCaves: {
    title:"Ajanta Caves", era:"2nd C. BCE – 5th C. CE", location:"Aurangabad, Maharashtra, India", color:0x806b59,
    architect:"Buddhist monk-artists across 700 years (Satavahana & Vakataka periods)",
    style:"Rock-cut Buddhist architecture — Chaitya halls and Viharas",
    components:["30 rock-cut caves in horseshoe cliff","Chaitya grihas (prayer halls with stupa)","Viharas (monasteries with cells)","Horseshoe-arch facades (gavaksha)","Elaborate mural paintings","Sculpted Buddha and Bodhisattva figures","Pillared porticos"],
    materials:"Basalt cliff face (Deccan Traps volcanic rock); natural mineral pigments for murals (lapis lazuli blue, red ochre, lamp black); lime plaster ground",
    purpose:"Buddhist monastery and prayer complex; retreat for monks during monsoon season (Chaturmas); artistic expression of Jataka tales",
    dynasty:"Satavahana Dynasty (early caves); Vakataka Dynasty (later caves, including the famous murals)",
    narration:"The Ajanta Caves were carved over seven centuries into a horseshoe-shaped cliff above the Waghora River. Lost to the jungle for over a thousand years, they were rediscovered by a British hunting party in 1819. The murals inside Cave 1 and Cave 2 are among the finest surviving examples of ancient Indian painting. The artists used natural pigments — lapis lazuli for blue, red ochre, lamp black — applied to lime plaster in a technique similar to European fresco.",
    artifacts:[
      {name:"Padmapani Bodhisattva Mural",type:"painting",material:"Natural pigments on lime plaster",origin:"Cave 1, Ajanta",usage:"Devotional wall painting",symbolism:"Avalokiteshvara holding a lotus — the embodiment of compassion; considered the finest single figure in all of Indian art"},
      {name:"Carved Stupa",type:"architecture",material:"Basalt (carved in-situ)",origin:"Cave 19 Chaitya hall",usage:"Object of circumambulation (pradakshina) for meditation",symbolism:"Represents the Buddha's presence; the dome symbolizes the cosmos, the harmika above it the heavenly realm"}
    ],
    quiz:[
      {q:"How were the Ajanta Caves rediscovered?",a:"By a British hunting party in 1819",options:["By local farmers","By a British hunting party in 1819","By archaeologists in 1905","They were never lost"]},
      {q:"What type of rock are the caves carved from?",a:"Basalt from the Deccan Traps",options:["Limestone","Sandstone","Basalt from the Deccan Traps","Granite"]}
    ]
  },
  ChristTheRedeemer: {
    title:"Christ the Redeemer", era:"1922–1931 CE", location:"Rio de Janeiro, Brazil", color:0xdcd6cc,
    architect:"Heitor da Silva Costa (design); Paul Landowski (sculpture); Gheorghe Leonida (face)",
    style:"Art Deco — monumental figurative sculpture",
    components:["Reinforced concrete statue","Soapstone mosaic exterior tiles","Internal steel framework","Pedestal chapel (Nossa Senhora Aparecida)","Observation deck at base","Lightning rod system (struck 3-6 times per year)"],
    materials:"Reinforced concrete skeleton; outer layer of over 6 million hand-cut soapstone triangle tiles; steel internal frame",
    purpose:"Symbol of Christianity and cultural icon of Brazil; votive offering from the Catholic community",
    dynasty:"Brazilian First Republic / Vargas Era",
    narration:"Christ the Redeemer stands 30 meters tall atop Corcovado mountain, 700 meters above Rio's harbor. The statue is covered in over 6 million tiny triangular soapstone tiles, each cut and placed by hand. Its arms span 28 meters — not outstretched in benediction as commonly believed, but in the Brazilian gesture of embrace. The statue is struck by lightning 3 to 6 times every year; in 2014, a bolt chipped the tip of the right thumb.",
    artifacts:[
      {name:"Soapstone Tile Sample",type:"building material",material:"Soapstone (pedra-sabão)",origin:"Quarries of Minas Gerais, Brazil",usage:"6 million tiles covering the statue exterior",symbolism:"Chosen for its durability against tropical weather and its soft, warm color that glows at sunset"},
      {name:"Original Scale Model",type:"model",material:"Plaster",origin:"Paul Landowski's Paris studio, 1924",usage:"1:10 scale reference for the construction team",symbolism:"The face was sculpted by Romanian artist Gheorghe Leonida — its serene expression took months to finalize"}
    ],
    quiz:[
      {q:"What material covers the exterior of Christ the Redeemer?",a:"Soapstone tiles",options:["White marble","Soapstone tiles","Painted concrete","Ceramic tiles"]},
      {q:"How wide is the arm span of the statue?",a:"28 meters",options:["15 meters","28 meters","40 meters","20 meters"]}
    ]
  },
  StatueOfLiberty: {
    title:"Statue of Liberty", era:"1875–1886 CE", location:"Liberty Island, New York, USA", color:0x66c6a6,
    architect:"Frédéric Auguste Bartholdi (sculptor); Alexandre Gustave Eiffel (structural engineer)",
    style:"Neoclassical — colossal figurative sculpture",
    components:["Copper outer skin (2.4mm thick)","Wrought-iron armature (Eiffel design)","Torch with gold-leaf flame","Tabula ansata (tablet with date July 4, 1776)","Seven-ray crown (7 continents/oceans)","Pedestal (Richard Morris Hunt, architect)","Fort Wood (11-point star base)"],
    materials:"Copper sheets (300 total, 2.4mm thick — now oxidized to green patina); wrought-iron skeleton; concrete and granite pedestal",
    purpose:"Gift from France celebrating Franco-American friendship and republican ideals; beacon for immigrants arriving by sea",
    dynasty:"French Third Republic / United States Gilded Age",
    narration:"The Statue of Liberty's green color is not paint — it's patina. The 2.4mm-thick copper skin has oxidized over 130 years to form a protective layer of copper carbonate. Beneath the skin, Gustave Eiffel — yes, the same engineer — designed an internal iron skeleton using a revolutionary system of flexible iron bars that allows the copper skin to move independently of the frame, preventing cracking in high winds.",
    artifacts:[
      {name:"Broken Chain at Feet",type:"sculpture element",material:"Copper",origin:"Base of the statue",usage:"Largely invisible from ground level — a broken chain lies at Liberty's feet",symbolism:"Abolition of slavery — Bartholdi originally wanted Liberty to hold the chains, but changed to a subtle placement"},
      {name:"Original Torch",type:"architectural element",material:"Copper with amber glass panels",origin:"Replaced in 1986",usage:"Now displayed in the museum lobby",symbolism:"The original flame was modified with windows in 1916; the replacement is clad in 24-karat gold leaf"}
    ],
    quiz:[
      {q:"Who designed the internal iron framework of the Statue of Liberty?",a:"Gustave Eiffel",options:["Frédéric Bartholdi","Gustave Eiffel","Richard Morris Hunt","Auguste Rodin"]},
      {q:"What do the seven rays of the crown represent?",a:"The seven continents and oceans",options:["The seven deadly sins","The seven days of the week","The seven continents and oceans","The seven French republics"]}
    ]
  },
  EasterIslandMoai: {
    title:"Moai of Rapa Nui", era:"1250–1500 CE", location:"Easter Island (Rapa Nui), Chile", color:0x736659,
    architect:"Rapa Nui master carvers — each village sponsored its own moai",
    style:"Polynesian monumental sculpture — ancestor veneration",
    components:["Elongated head (represents ancestral mana)","Deep-set eye sockets (coral and obsidian eyes added post-transport)","Heavy brow ridge","Long nose and thin lips","Truncated torso with hands on belly","Pukao (red scoria topknot)","Ahu (stone platform base)"],
    materials:"Tuff (compressed volcanic ash from Rano Raraku quarry); pukao carved from red scoria of Puna Pau quarry; coral and obsidian for eyes",
    purpose:"Ancestor worship — each moai embodies the mana (spiritual power) of a deceased chief, protecting the village from the ahu platform",
    dynasty:"Rapa Nui civilization (Polynesian)",
    narration:"The moai were not dragged horizontally — they walked. Recent experiments have demonstrated that by rocking a standing moai back and forth with ropes attached to its head, a team of 18 people can 'walk' a 5-ton statue several hundred meters per day. The statues face inland, watching over their village. Only the seven moai of Ahu Akivi face the sea — they are said to represent the seven original explorers sent by King Hotu Matua.",
    artifacts:[
      {name:"Coral Eye with Obsidian Pupil",type:"ritual object",material:"White coral disk with red scoria iris and obsidian pupil",origin:"Excavated from fallen moai at Anakena",usage:"Inserted into the moai's eye socket to 'activate' its mana (spiritual power)",symbolism:"The moai was considered spiritually inert until its eyes were placed — the ceremony was called 'opening the eyes'"},
      {name:"Rongorongo Tablet",type:"inscription",material:"Wooden tablet with shark-tooth stylus incisions",origin:"Rapa Nui, undeciphered script",usage:"Unknown — possibly genealogical or astronomical records",symbolism:"One of the few examples of writing independently invented in Polynesia — still undeciphered"}
    ],
    quiz:[
      {q:"How were the moai transported from the quarry?",a:"'Walked' upright using ropes to rock them forward",options:["Dragged on wooden sledges","Rolled on logs","'Walked' upright using ropes to rock them forward","Carried by hundreds of workers"]},
      {q:"Which direction do most moai face?",a:"Inland, watching over their village",options:["Out to sea","Inland, watching over their village","South toward Antarctica","Toward the quarry"]}
    ]
  },
  LeaningTowerPisa: {
    title:"Leaning Tower of Pisa", era:"1173–1372 CE (Romanesque)", location:"Pisa, Italy", color:0xf0ebdc,
    architect:"Bonanno Pisano (attributed, disputed); completed by Tommaso di Andrea Pisano",
    style:"Italian Romanesque — free-standing campanile",
    components:["Blind arcade base storey","Six open loggia stories (30 columns each)","Belfry (bell chamber, 7 bells)","198 columns total","Internal spiral staircase (294 steps)","Marble exterior cladding"],
    materials:"White Carrara marble (exterior); grey Verrucano limestone (structure); soft clay subsoil (cause of lean)",
    purpose:"Bell tower (campanile) for Pisa Cathedral; civic prestige symbol of the Pisan Republic's maritime wealth",
    dynasty:"Republic of Pisa",
    narration:"Construction of the tower began in 1173 and the lean appeared almost immediately — by the time the third storey was complete, the foundation on the south side had sunk into the soft clay. Construction stopped for almost a century, which may have saved the tower — the pause allowed the soil to settle and strengthen. When building resumed, engineers deliberately made the upper stories slightly taller on the south side to compensate, giving the tower a subtle banana curve.",
    artifacts:[
      {name:"Bell 'Assunta' (largest bell)",type:"instrument",material:"Bronze",origin:"Cast in 1655",usage:"Largest of the seven bells — each tuned to a musical note of the major scale",symbolism:"The bells were silenced for decades over fears that their vibration would worsen the lean"},
      {name:"Galileo's Pendulum Experiment (commemorative)",type:"scientific",material:"Iron weights and rope",origin:"Legendary experiment, c. 1589",usage:"Galileo reportedly dropped two spheres of different mass from the tower to demonstrate they fall at equal speed",symbolism:"Foundational experiment in modern physics — though historians debate whether it actually occurred here"}
    ],
    quiz:[
      {q:"What caused the tower to lean?",a:"Soft clay subsoil on the south side",options:["An earthquake","Soft clay subsoil on the south side","Poor quality marble","Intentional design"]},
      {q:"What is the tower's current lean angle?",a:"About 3.99 degrees",options:["About 3.99 degrees","About 10 degrees","About 1 degree","About 7 degrees"]}
    ]
  },
  SydneyOperaHouse: {
    title:"Sydney Opera House", era:"1959–1973 CE", location:"Sydney, New South Wales, Australia", color:0xf5f5f3,
    architect:"Jørn Utzon (original design); completed by Peter Hall",
    style:"Expressionist / Organic Modern — shell structure",
    components:["Interlocking roof shells (derived from segments of a single sphere)","Chevron-pattern glass curtain walls","Monumental podium (Bennelong Point)","Concert Hall (2,679 seats)","Joan Sutherland Theatre (1,507 seats)","Forecourt and Monumental Steps","Over 1 million Swedish roof tiles"],
    materials:"Precast concrete ribs clad in 1,056,006 Swedish-made ceramic tiles (cream and matte white chevron pattern); pink granite podium; laminated timber and birch plywood interiors",
    purpose:"Multi-venue performing arts centre; national icon and UNESCO World Heritage Site",
    dynasty:"Modern Australia",
    narration:"Utzon's original design was deemed 'unbuildable' — no engineer could calculate how to construct the free-form shells. The breakthrough came when Utzon realized that every shell could be cut from the surface of a single sphere of uniform radius (75.2 meters). This meant that all 2,194 precast concrete ribs could be manufactured using the same mould — a manufacturing revolution. Utzon resigned before completion due to political disputes, and never saw the finished interior.",
    artifacts:[
      {name:"Utzon's Spherical Geometry Drawing",type:"architectural drawing",material:"Ink on tracing paper",origin:"Utzon's Hellebæk studio, 1961",usage:"The famous diagram showing how every roof shell derives from a sphere of 75.2m radius",symbolism:"The geometric insight that made the 'impossible' design buildable — one of architecture's eureka moments"},
      {name:"Swedish Roof Tile Sample",type:"building material",material:"Glazed ceramic (Höganäs, Sweden)",origin:"Höganäs factory, Sweden",usage:"1,056,006 tiles in a cream/matte white chevron pattern",symbolism:"Self-cleaning due to glazed surface — they look as pristine today as in 1973"}
    ],
    quiz:[
      {q:"What geometric insight made the Opera House shells buildable?",a:"All shells are segments of a single sphere",options:["Each shell has a unique curvature","All shells are segments of a single sphere","They are parabolic curves","They used flexible formwork"]},
      {q:"How many roof tiles cover the Sydney Opera House?",a:"Over 1 million",options:["100,000","Over 1 million","500,000","5 million"]}
    ]
  }
};

// Artifact categories for the expanded artifact hall
window.ARTIFACT_LIBRARY = [
  { id:"A001", name:"Lothal Seal (Indus Valley)", civilization:"Indus Valley c.2600 BCE", type:"seal", material:"Steatite", usage:"Trade/identity seal", symbolism:"Bull motif — possibly representing authority or divine power", color:0x8a7a6a },
  { id:"A002", name:"Rosetta Stone", civilization:"Ptolemaic Egypt 196 BCE", type:"inscription", material:"Granodiorite", usage:"Decree in three scripts (hieroglyphic, Demotic, Greek)", symbolism:"Key to deciphering Egyptian hieroglyphics", color:0x4a4a4a },
  { id:"A003", name:"Antikythera Mechanism", civilization:"Ancient Greece c.100 BCE", type:"scientific instrument", material:"Bronze", usage:"Astronomical calculator — predicted eclipses, Olympic games", symbolism:"First known analog computer", color:0x8a7050 },
  { id:"A004", name:"Tang Dynasty Tang Sancai Horse", civilization:"Tang China c.700 CE", type:"sculpture", material:"Lead-glazed earthenware", usage:"Burial figurine (mingqi)", symbolism:"Horses symbolized military strength and the Silk Road", color:0xd4883a },
  { id:"A005", name:"Illuminated Quran Page", civilization:"Islamic Golden Age c.1200 CE", type:"manuscript", material:"Vellum, gold leaf, lapis lazuli ink", usage:"Sacred text; calligraphic art form", symbolism:"Geometric arabesque border represents infinite nature of God", color:0xc8a840 },
  { id:"A006", name:"Roman Legionary Eagle (Aquila)", civilization:"Roman Empire c.100 CE", type:"military standard", material:"Gold-plated bronze", usage:"Sacred standard of a Roman legion", symbolism:"Loss of the Eagle was the greatest dishonor — wars fought to recover them", color:0xdaa520 },
  { id:"A007", name:"Viking Runestone Fragment", civilization:"Norse c.900 CE", type:"inscription", material:"Granite", usage:"Memorial inscription in Elder Futhark runes", symbolism:"Commemorates the dead; asserts land ownership", color:0x6a6a6a },
  { id:"A008", name:"Samurai Katana", civilization:"Feudal Japan c.1400 CE", type:"weapon", material:"Tamahagane steel (folded 1000x)", usage:"Primary weapon of the samurai class", symbolism:"Soul of the samurai — the blade is a living object in Shinto belief", color:0x3a3a3a },
  { id:"A009", name:"Mayan Jade Pectoral", civilization:"Classic Maya c.600 CE", type:"jewelry", material:"Jadeite", usage:"Worn by royalty as a status marker", symbolism:"Jade = maize = life; green color connects to agricultural fertility", color:0x4a8a4a },
  { id:"A010", name:"Mauryan Ashoka Pillar Capital", civilization:"Mauryan India c.250 BCE", type:"architecture", material:"Chunar sandstone (highly polished)", usage:"Imperial proclamation column; Buddhist dhamma symbol", symbolism:"Four lions back-to-back = power in all directions; adopted as India's national emblem", color:0xc8b870 }
];

console.log('[HeritageAtlas] Monument data loaded:', Object.keys(window.MONUMENT_DATA_RICH).length, 'monuments,', window.ARTIFACT_LIBRARY.length, 'artifacts');
