import { UserProfile } from './AthleteProfile';
import { EventPageLook } from './EventPageLook';
import './homePage.css';
import { MatchPage } from './MatchPage';
import { TeamPageLook } from './TeamPage';
import { allEvents, allLiveMatches, allRecentResults, allTopAthletes, allTopTeams, allUpcomingMatches, match } from './db';

export class HomePage {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;

    // Initialize with mock data


    match.forEach(element => {
      if (element.status === "live") {
        allLiveMatches.push(element);
      }
      if (element.status === "upcoming") {
        allUpcomingMatches.push(element);
      }
      if (element.status === "finished") {
        allRecentResults.push(element);
      }
    });


  }

  async render() {
    this.container.innerHTML = `
      <main class="home-page">
    
        <!-- Hero Section -->
        <section class="hero-section">
          <div class="hero-slider">
            ${allEvents.map(event => `
              <div class="hero-slide" data-sport="${event.sport}">
                <h2>${event.NameEvent}</h2>
                <p>${new Date(event.date).toLocaleDateString()} • ${event.sport}</p>
                <div class="teams-preview">
                  <span>${event.teams[0]} vs ${event.teams[1]}</span>
                </div>
              </div>
            
          </div>
          <div class="hero-actions">
            <button class="btn btn-primary" id="look-event" data-id="${event.NameEvent}">Переглянути</button>
          </div>`).join('')}
          
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
      </main>
    `;

    this.renderFilteredContent();
    this.setupEventListeners();
  }

  private renderFilteredContent(filterSport: string = 'all', searchQuery: string = '') {
    // Filter data based on sport type and search query
    const filteredRecentResults = this.filterData(allRecentResults, filterSport, searchQuery);
    const filteredUpcomingMatches = this.filterData(allUpcomingMatches, filterSport, searchQuery);
    const filteredLiveMatches = this.filterData(allLiveMatches, filterSport, searchQuery);
    const filteredTopTeams = this.filterData(allTopTeams, filterSport, searchQuery);
    const filteredTopAthletes = this.filterData(allTopAthletes, filterSport, searchQuery);

    // Get unique sports for tabs
    const recentResultsSports = [...new Set(allRecentResults.map(r => r.sport))];
    const upcomingMatchesSports = [...new Set(allUpcomingMatches.map(m => m.sport))];
    const liveMatchesSports = [...new Set(allLiveMatches.map(m => m.sport))];

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
            <button class="btn btn-outline watch-live look-matchik" data-id="${match.idMatch}">Переглянути</button>
          </div>
        `).join('');
        document.querySelectorAll('.look-matchik').forEach(btn => {
          btn.addEventListener('click', () => {
            const id = (btn as HTMLElement).getAttribute('data-id');
            const loginModal = new MatchPage('app', id || "");
            loginModal.render();
          });
        });
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
        <div class="result-card match-trigger">
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
        <div class="match-card ${match.status} match-trigger" data-id="${match.idMatch}">
          <div class="teams">
            <span class="team">${match.team1}</span>
            <span class="vs">vs</span>
            <span class="team">${match.team2}</span>
          </div>
          <div class="match-time">
            ${new Date(match.date).toLocaleDateString()} • 
            ${new Date(match.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
          </div>
          <div class="match-status">
            ${this.getStatusText(match.status)}
          </div>
          <div class="match-sport">
            ${this.getSportIcon(match.sport)} ${match.sport}
          </div>
        </div>
      `).join('');
      document.querySelectorAll('.match-trigger').forEach(card => {
        card.addEventListener('click', () => {
          const id = (card as HTMLElement).getAttribute('data-id');
          const loginModal = new MatchPage('app', id || "");
          loginModal.render();
        });
      });
    }

    // Render top teams
    const teamsGrid = document.getElementById('teamsGrid');
    if (teamsGrid) {
      teamsGrid.innerHTML = filteredTopTeams.map(team => `
        <div class="team-card team-trigger" data-id="${team.name}">
          <img src="${team.logo}" alt="${team.name}" onerror="this.src=''">
          <h3>${team.name}</h3>
          <p>${this.getSportIcon(team.sport)} ${team.sport}</p>
          <div class="team-stats">
            <span>${team.wins} перемог</span>
          </div>
        </div>
      `).join('');
    }

    const btn = document.getElementById('look-event');
    btn?.addEventListener('click', () => {
      const nameEvent = btn.getAttribute('data-id');
      const loginModal = new EventPageLook('app', nameEvent || "");
      loginModal.render();
    });


    // Render top athletes
    const athletesGrid = document.getElementById('athletesGrid');
    if (athletesGrid) {
      athletesGrid.innerHTML = filteredTopAthletes.map(athlete => `
        <div class="athlete-card trigger-athelete" data-id="${athlete.login}">
          <img src="${athlete.photo}" alt="${athlete.name}" class="athlete-photo" 
               onerror="this.src='https://template.canva.com/EAGZeVbaBh4/1/0/1600w-FQWnYg_IWXU.jpg'">
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

    document.querySelectorAll('.team-trigger').forEach(btn => {
      btn.addEventListener('click', async () => {
        const id = (btn as HTMLElement).getAttribute('data-id');
        const profile = new TeamPageLook("app",id || "");
        await profile.render();
      });
    });

    document.querySelectorAll('.trigger-athelete').forEach(btn => {
      btn.addEventListener('click', async () => {
        const id = (btn as HTMLElement).getAttribute('data-id');
        const profile = new UserProfile("app",id || "");
        await profile.render();
      });
    });
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