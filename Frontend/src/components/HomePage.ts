import './homePage.css';

export class HomePage {
  private container: HTMLElement;
  private allEvents: any[];
  private allRecentResults: any[];
  private allLiveMatches: any[];
  private allUpcomingMatches: any[];
  private allTopTeams: any[];
  private allTopAthletes: any[];

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;

    // Initialize with mock data
    this.allEvents = [
      { id: 1, name: "Чемпіонат міста з футболу", date: "2023-11-20", sport: "Футбол", teams: ["Динамо", "Скіфи"] },
      
    ];

    this.allRecentResults = [
      { team1: "Динамо", team2: "Вікторія", score: "2:1", date: "2023-11-15", sport: "Футбол" },
      { team1: "Олімпійці", team2: "Стрімкі", score: "89:76", date: "2023-11-14", sport: "Баскетбол" },
      { team1: "Стрибуни", team2: "Форхенди", score: "6:4, 6:3", date: "2023-11-13", sport: "Теніс" },
      { team1: "Вікторія", team2: "Скіфи", score: "1:1", date: "2023-11-12", sport: "Футбол" }
    ];
this.allLiveMatches = [
      { 
        team1: "Динамо", 
        team2: "Шахтар", 
        score: "1:0", 
        date: "2023-11-19T15:00", 
        status: "live", 
        sport: "Футбол",
        time: "62'", // Хвилина матчу
        events: ["⚽ Гол на 35' - Іваненко О. (Динамо)"] // Події матчу
      },
      { 
        team1: "Олімпійці", 
        team2: "Гіганти", 
        score: "45:42", 
        date: "2023-11-19T16:30", 
        status: "live", 
        sport: "Баскетбол",
        time: "3-тя чверть",
        events: ["🏀 3 очки на 25' - Петренко М. (Олімпійці)"]
      }
    ];
    this.allUpcomingMatches = [
      { team1: "Динамо", team2: "Скіфи", date: "2023-11-20T15:00", status: "upcoming", sport: "Футбол" },
      { team1: "Титани", team2: "Вікторія", date: "2023-11-21T17:00", status: "upcoming", sport: "Футбол" },
      { team1: "Олімпійці", team2: "Стрімкі", date: "2023-11-22T18:30", status: "upcoming", sport: "Баскетбол" },
      { team1: "Стрибуни", team2: "Форхенди", date: "2023-11-25T12:00", status: "upcoming", sport: "Теніс" }
    ];

    this.allTopTeams = [
      { name: "Динамо", sport: "Футбол", wins: 12, logo: "https://static-cse.canva.com/blob/847064/29.jpg" },
      { name: "Олімпійці", sport: "Баскетбол", wins: 8, logo: "https://static-cse.canva.com/blob/847064/29.jpg" },
      { name: "Стрибуни", sport: "Теніс", wins: 5, logo: "https://static-cse.canva.com/blob/847064/29.jpg" },
      { name: "Вікторія", sport: "Футбол", wins: 7, logo: "https://static-cse.canva.com/blob/847064/29.jpg" }
    ];

    this.allTopAthletes = [
      { name: "Олександр Іваненко", sport: "Футбол", stats: "24 голи", photo: "https://static-cse.canva.com/blob/847064/29.jpg" },
      { name: "Марія Петренко", sport: "Баскетбол", stats: "18.5 очків/гра", photo: "https://static-cse.canva.com/blob/847064/29.jpg" },
      { name: "Ігор Семенов", sport: "Теніс", stats: "85% виграних подач", photo: "https://static-cse.canva.com/blob/847064/29.jpg" }
    ];
  }

  async render() {
    this.container.innerHTML = `
      <main class="home-page">
        <!-- Search and Filter Section -->
        <section class="search-filter-section">
          <div class="search-box">
            <input type="text" id="searchInput" placeholder="Пошук матчів, команд, гравців...">
            <button class="btn btn-primary" id="searchButton">Пошук</button>
          </div>
          <div class="filter-controls">
            <select id="sportFilter" class="filter-select">
              <option value="all">Всі види спорту</option>
              <option value="Футбол">Футбол</option>
              <option value="Баскетбол">Баскетбол</option>
              <option value="Теніс">Теніс</option>
            </select>
            <button class="btn btn-outline" id="resetFilters">Скинути фільтри</button>
          </div>
        </section>

        <!-- Hero Section -->
        <section class="hero-section">
          <div class="hero-slider">
            ${this.allEvents.map(event => `
              <div class="hero-slide" data-sport="${event.sport}">
                <h2>${event.name}</h2>
                <p>${new Date(event.date).toLocaleDateString()} • ${event.sport}</p>
                <div class="teams-preview">
                  <span>${event.teams[0]} vs ${event.teams[1]}</span>
                </div>
              </div>
            `).join('')}
          </div>
          <div class="hero-actions">
            <button class="btn btn-primary">Перегялнути</button>
          </div>
        </section>
 <!-- Live Matches Section -->
        <section class="live-matches-section">
          <h2 class="section-title">Матчі LIVE</h2>
          <div class="live-matches-list" id="liveMatchesList"></div>
        </section>
        <!-- Main Grid -->
        <section class="main-grid">
          <!-- Recent Results with sport tabs -->
          <div class="card recent-results">
            <div class="section-header">
              <h2 class="section-title">Останні результати</h2>
              <div class="sport-tabs" id="resultsTabs"></div>
            </div>
            <div class="results-list" id="resultsList"></div>
          </div>

          <!-- Upcoming Matches with sport tabs -->
          <div class="card upcoming-matches">
            <div class="section-header">
              <h2 class="section-title">Найближчі матчі</h2>
              <div class="sport-tabs" id="matchesTabs"></div>
            </div>
            <div class="matches-list" id="matchesList"></div>
          </div>

          <!-- Top Teams -->
          <div class="card top-teams">
            <h2 class="section-title">Топ команди</h2>
            <div class="teams-grid" id="teamsGrid"></div>
          </div>

          <!-- Top Athletes -->
          <div class="card top-athletes">
            <h2 class="section-title">Кращі атлети</h2>
            <div class="athletes-grid" id="athletesGrid"></div>
          </div>
        </section>

        <!-- Calendar Section -->
        <section class="calendar-section">
          <h2 class="section-title">Календар подій</h2>
          <div class="mini-calendar">
            <p>Листопад 2023</p>
            <div class="calendar-days">
              <!-- Дні місяця з позначками подій -->
            </div>
          </div>
        </section>
      </main>
    `;

    // Render initial content
    this.renderFilteredContent();
    this.setupEventListeners();
    
  }

  private renderFilteredContent(filterSport: string = 'all', searchQuery: string = '') {
    // Filter data based on sport type and search query
    const filteredRecentResults = this.filterData(this.allRecentResults, filterSport, searchQuery);
    const filteredUpcomingMatches = this.filterData(this.allUpcomingMatches, filterSport, searchQuery);
    const filteredLiveMatches = this.filterData(this.allLiveMatches, filterSport, searchQuery);
    const filteredTopTeams = this.filterData(this.allTopTeams, filterSport, searchQuery);
    const filteredTopAthletes = this.filterData(this.allTopAthletes, filterSport, searchQuery);

    // Get unique sports for tabs
    const recentResultsSports = [...new Set(this.allRecentResults.map(r => r.sport))];
    const upcomingMatchesSports = [...new Set(this.allUpcomingMatches.map(m => m.sport))];
    const liveMatchesSports = [...new Set(this.allLiveMatches.map(m => m.sport))];

    // Render LIVE matches
    const liveMatchesList = document.getElementById('liveMatchesList');
    if (liveMatchesList) {
      if (filteredLiveMatches.length > 0) {
        liveMatchesList.innerHTML = filteredLiveMatches.map(match => `
          <div class="live-match-card">
            <div class="live-match-header">
              <div class="live-match-sport">
                ${this.getSportIcon(match.sport)} ${match.sport}
              </div>
              <div class="live-match-time">
                ${match.time}
              </div>
              <div class="live-match-status">
                ${this.getStatusText(match.status)}
              </div>
            </div>
            <div class="live-match-teams">
              <div class="team">
                <span class="team-name">${match.team1}</span>
                <span class="team-score">${match.score.split(':')[0]}</span>
              </div>
              <div class="vs">vs</div>
              <div class="team">
                <span class="team-name">${match.team2}</span>
                <span class="team-score">${match.score.split(':')[1]}</span>
              </div>
            </div>
            ${match.events && match.events.length > 0 ? `
              <div class="live-match-events">
                <h4>Останні події:</h4>
                <ul>
                 ${match.events.map((event: string) => `<li>${event}</li>`).join('')}
                </ul>
              </div>
            ` : ''}
            <button class="btn btn-outline watch-live">Переглянути</button>
          </div>
        `).join('');
      } else {
        liveMatchesList.innerHTML = `
          <div class="no-live-matches">
            <p>Наразі немає матчів у прямому ефірі</p>
          </div>
        `;
      }
    }

    // Render sport tabs for results
    const resultsTabs = document.getElementById('resultsTabs');
    if (resultsTabs) {
      resultsTabs.innerHTML = `
        <button class="sport-tab ${filterSport === 'all' ? 'active' : ''}" data-sport="all">Всі</button>
        ${recentResultsSports.map(sport => `
          <button class="sport-tab ${filterSport === sport ? 'active' : ''}" data-sport="${sport}">
            ${this.getSportIcon(sport)} ${sport}
          </button>
        `).join('')}
      `;
    }

    // Render results list
    const resultsList = document.getElementById('resultsList');
    if (resultsList) {
      resultsList.innerHTML = filteredRecentResults.map(match => `
        <div class="result-card">
          <div class="teams">
            <span class="team">${match.team1}</span>
            <span class="vs">vs</span>
            <span class="team">${match.team2}</span>
          </div>
          <div class="score">${match.score}</div>
          <div class="match-meta">
            <span>${new Date(match.date).toLocaleDateString()}</span>
            <span>${this.getSportIcon(match.sport)} ${match.sport}</span>
          </div>
        </div>
      `).join('');
    }

    // Render sport tabs for matches
    const matchesTabs = document.getElementById('matchesTabs');
    if (matchesTabs) {
      matchesTabs.innerHTML = `
        <button class="sport-tab ${filterSport === 'all' ? 'active' : ''}" data-sport="all">Всі</button>
        ${upcomingMatchesSports.map(sport => `
          <button class="sport-tab ${filterSport === sport ? 'active' : ''}" data-sport="${sport}">
            ${this.getSportIcon(sport)} ${sport}
          </button>
        `).join('')}
      `;
    }

    // Render matches list
    const matchesList = document.getElementById('matchesList');
    if (matchesList) {
      matchesList.innerHTML = filteredUpcomingMatches.map(match => `
        <div class="match-card ${match.status}">
          <div class="teams">
            <span class="team">${match.team1}</span>
            <span class="vs">vs</span>
            <span class="team">${match.team2}</span>
          </div>
          <div class="match-time">
            ${new Date(match.date).toLocaleDateString()} • 
            ${new Date(match.date).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
          </div>
          <div class="match-status">
            ${this.getStatusText(match.status)}
          </div>
          <div class="match-sport">
            ${this.getSportIcon(match.sport)} ${match.sport}
          </div>
        </div>
      `).join('');
    }

    // Render top teams
    const teamsGrid = document.getElementById('teamsGrid');
    if (teamsGrid) {
      teamsGrid.innerHTML = filteredTopTeams.map(team => `
        <div class="team-card">
          <img src="${team.logo}" alt="${team.name}" onerror="this.src=''">
          <h3>${team.name}</h3>
          <p>${this.getSportIcon(team.sport)} ${team.sport}</p>
          <div class="team-stats">
            <span>${team.wins} перемог</span>
          </div>
        </div>
      `).join('');
    }

    // Render top athletes
    const athletesGrid = document.getElementById('athletesGrid');
    if (athletesGrid) {
      athletesGrid.innerHTML = filteredTopAthletes.map(athlete => `
        <div class="athlete-card">
          <img src="${athlete.photo}" alt="${athlete.name}" class="athlete-photo" 
               onerror="this.src='https://images.unsplash.com/photo-1508214751196-bcfd4ca60f91?auto=format&fit=crop&w=100&q=80'">
          <div class="athlete-info">
            <h3>${athlete.name}</h3>
            <p>${this.getSportIcon(athlete.sport)} ${athlete.sport}</p>
            <div class="athlete-stats">
              ${athlete.stats}
            </div>
          </div>
        </div>
      `).join('');
    }
  }

  private filterData(data: any[], sportFilter: string, searchQuery: string): any[] {
    return data.filter(item => {
      const matchesSport = sportFilter === 'all' || item.sport === sportFilter;
      const matchesSearch = searchQuery === '' || 
        JSON.stringify(item).toLowerCase().includes(searchQuery.toLowerCase());
      return matchesSport && matchesSearch;
    });
  }

  private setupEventListeners() {
    // Sport filter dropdown
    const sportFilter = document.getElementById('sportFilter');
    if (sportFilter) {
      sportFilter.addEventListener('change', (e) => {
        const filterValue = (e.target as HTMLSelectElement).value;
        this.renderFilteredContent(filterValue);
      });
    }

    // Search functionality
    const searchButton = document.getElementById('searchButton');
    const searchInput = document.getElementById('searchInput') as HTMLInputElement;
    if (searchButton && searchInput) {
      searchButton.addEventListener('click', () => {
        const searchQuery = searchInput.value;
        const sportFilter = (document.getElementById('sportFilter') as HTMLSelectElement).value;
        this.renderFilteredContent(sportFilter, searchQuery);
      });

      searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
          const searchQuery = searchInput.value;
          const sportFilter = (document.getElementById('sportFilter') as HTMLSelectElement).value;
          this.renderFilteredContent(sportFilter, searchQuery);
        }
      });
    }

    // Reset filters button
    const resetFilters = document.getElementById('resetFilters');
    if (resetFilters) {
      resetFilters.addEventListener('click', () => {
        (document.getElementById('sportFilter') as HTMLSelectElement).value = 'all';
        if (searchInput) searchInput.value = '';
        this.renderFilteredContent();
      });
    }

    // Sport tabs for results
    document.addEventListener('click', (e) => {
      const target = e.target as HTMLElement;
      if (target.classList.contains('sport-tab')) {
        const sport = target.dataset.sport || 'all';
        const searchQuery = searchInput?.value || '';
        this.renderFilteredContent(sport, searchQuery);
      }
    });
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      'Футбол': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Теніс': '🎾'
    };
    return icons[sportType] || '🏅';
  }

  private getStatusText(status: string): string {
    const statuses: Record<string, string> = {
      'live': '🟢 LIVE',
      'finished': '🔴 Завершено',
      'upcoming': '🟡 Очікується'
    };
    return statuses[status] || status;
  }
}