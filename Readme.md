🪓 Haunted Mound

Temná 2D akční hra vytvořená v Unity, inspirovaná estetikou Haunted Mound. Hráč čelí mocnému bossovi v atmosférickém prostředí s důrazem na vizuální styl a plynulý souboj.

Odkaz na video prezentaci: (Zde doplň svůj odkaz na YouTube, pokud máš)
🎮 Gameplay
Cíl hry

    Porazit hlavního bosse v náročném souboji.

    Správně využívat mechaniky pohybu a útoků.

    Přežít s omezeným počtem životů, které se dynamicky zobrazují v UI.

Ovládání

    WASD - Pohyb postavy.

    Mezerník - skok

    Levé tlačítko myši / Pravé tlačítko myší - Útok mačetou a revolverem

    Q - Raven

    ESC - Pauza (Plánováno).

Herní mechaniky

    ✅ Boss AI Systém - Boss má vlastní logiku chování, útočí a reaguje na hráče.

    ✅ Dynamic Health Bar - Graficky stylizovaný (Gothic Frame) ukazatel zdraví s plynulým ubýváním (Fill Amount).

    ✅ Intro Sequence - Dynamický zoom kamery na bosse před začátkem souboje.

    ✅ Victory Screen - Speciální obrazovka po poražení bosse s vítěznou fanfárou.

    ✅ Soundtrack System - Atmosférická hudba v menu i během samotného boje.

📊 Statistiky & UI
Prvek	Popis	Styl
Boss Health Bar	Dynamické UI s "odřezáváním" HP	Gothic / Scary vibe
Intro Zoom	Zpoždění UI prvků pro filmový efekt	Smooth Camera Lerp
Victory UI	Obrazovka po smrti bosse	Triumfální hudba
🚀 Plánované features (Roadmap)
Priorita 1 (Základní rozšíření)

    Více fází bosse - Při 50 % HP boss změní útoky a vzhled.

    Dash Ability - Úhybný manévr pro hráče.

    Particle efekty - Krev, jiskry při nárazu meče a prach při pohybu.

Priorita 2 (Polishing)

    Vlastní Soundtrack - Kompletní nahrazení placeholderů unikátní hudbou (HM styl).

    Settings Menu - Nastavení hlasitosti hudby a citlivosti ovládání.

    Screen Shake - Otřes obrazovky při silných útocích bosse.

🛠️ Technické info
Struktura projektu
Plaintext

project-root/
├── Assets/
│   ├── Sprites/         # Hráč, Boss, Gothic Frame
│   ├── Scripts/         # Boss.cs, HealthBar.cs, IntroManager.cs
│   ├── Fonts/           # Fonty
│   ├── Audio/           # Hudba a SFX
│   └── Prefabs/         # WinningScreen, BossBarUI
├── ProjectSettings/
└── UserSettings/

Engine & Technologie

    Unity 2022.3+ (nebo tvá verze)

    C# Scripting - Event-based UI update, Coroutines pro delaye.

    Universal Render Pipeline (URP) - Pro lepší post-processing a světla (pokud používáš).

    Sprite Animation - Frame-by-frame animace pro bosse a hráče.

📝 Changelog
v1.2 (Current)

    ✅ Přidán Gothic Frame pro Health Bar s průhledným vnitřkem.

    ✅ Implementováno zpoždění (delay) pro UI po ukončení intra.

    ✅ Opraveno vrstvení UI (Z-order) – Bar je nyní správně nad rámem.

    ✅ Přidána vítězná hudba na Winning Screen.

v1.1

    ✅ Základní pohyb hráče a detekce kolizí.

    ✅ Intro zoom sekvence pomocí kamery.

    ✅ Systém ubírání životů bosse.

👨‍💻 Autor

Vytvořeno jako semestrální projekt / vlastní hra.
Ondřej Lampa/Grok AI
