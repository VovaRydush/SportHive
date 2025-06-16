import './teamPage.css';

// Статичні дані для демонстрації
const staticTeamData: Team = {
  TeamName: "Динамо Київ",
   Stats: {
    totalGames: 24,
    wins: 15,
    losses: 5,
    draws: 4,
    goalsScored: 42,
    goalsConceded: 18
  },
  TeamPhoto: "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/FC_Dynamo_Kyiv_logo.svg/1200px-FC_Dynamo_Kyiv_logo.svg.png",
  LoginTrainer: "dynamo_coach",
  Trainer: {
    FullName: "Олександр Шовковський",
    Photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Oleksandr_Shovkovskiy_2016.jpg/800px-Oleksandr_Shovkovskiy_2016.jpg",
    Experience: "Тренує з 2018 року. Колишній воротар збірної України."
  },
  TeamAthletes: [
    {
      Athlete: {
        FullName: "Віктор Циганков",
        Photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7e/Viktor_Tsyhankov_2021.jpg/800px-Viktor_Tsyhankov_2021.jpg",
        Position: "Півзахисник",
        Stats: "12 голів, 8 асистів у сезоні"
      }
    },
    {
      Athlete: {
        FullName: "Микита Бураченко",
        Photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1e/Mykola_Shaparenko_2021.jpg/800px-Mykola_Shaparenko_2021.jpg",
        Position: "Півзахисник",
        Stats: "5 голів, 3 асисти у сезоні"
      }
    },
    {
      Athlete: {
        FullName: "Ілля Забарний",
        Photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8e/Illia_Zabarnyi_2021.jpg/800px-Illia_Zabarnyi_2021.jpg",
        Position: "Захисник",
        Stats: "27 матчів у сезоні"
      }
    },
    {
      Athlete: {
        FullName: "Георгій Бушчан",
        Photo: "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3b/Heorhiy_Bushchan_2021.jpg/800px-Heorhiy_Bushchan_2021.jpg",
        Position: "Воротар",
        Stats: "14 'сухих' матчів"
      }
    }
  ],
  TypeSport: "Футбол",
  OrganizationTeam: [
    {
      Organization: {
        Name: "ФФУ",
        Logo: "https://upload.wikimedia.org/wikipedia/uk/thumb/6/6f/Ukrainian_Association_of_Football_logo.svg/1200px-Ukrainian_Association_of_Football_logo.svg.png",
        Description: "Федерація футболу України"
      }
    },
    {
      Organization: {
        Name: "УПЛ",
        Logo: "https://upload.wikimedia.org/wikipedia/uk/thumb/9/9e/Ukrainian_Premier_League_logo.svg/1200px-Ukrainian_Premier_League_logo.svg.png",
        Description: "Українська Прем'єр-ліга"
      }
    }
  ],
  eMatchesTeams: [
    {
      Match: {
        Date: "2023-11-25T15:00",
        Team1: "Динамо Київ",
        Team2: "Шахтар Донецьк",
        Status: "upcoming"
      }
    },
    {
      Match: {
        Date: "2023-12-02T17:00",
        Team1: "Динамо Київ",
        Team2: "Зоря Луганськ",
        Status: "upcoming"
      }
    },
    {
      Match: {
        Date: "2023-11-18T19:30",
        Team1: "Динамо Київ",
        Team2: "Ворскла Полтава",
        Score: "3:1",
        Status: "finished"
      }
    },
    {
      Match: {
        Date: "2023-11-05T14:00",
        Team1: "Динамо Київ",
        Team2: "Дніпро-1",
        Score: "2:2",
        Status: "finished"
      }
    }
  ]
};

// Інтерфейси залишаються незмінними
interface Team {
  TeamName: string;
  TeamPhoto: string;
  LoginTrainer: string;
  Trainer: Trainer;
  TeamAthletes: TeamAthlete[];
  TypeSport: string;
  Stats?: TeamStats;
  OrganizationTeam?: OrganizationTeam[];
  eMatchesTeams?: EMatchesTeam[];
}
interface TeamStats {
  totalGames: number;
  wins: number;
  losses: number;
  draws: number;
  goalsScored: number;
  goalsConceded: number;
}

interface Trainer {
  FullName: string;
  Photo: string;
  Experience: string;
}

interface TeamAthlete {
  Athlete: Athlete;
}

interface Athlete {
  FullName: string;
  Photo: string;
  Position: string;
  Stats: string;
}

interface OrganizationTeam {
  Organization: Organization;
}

interface Organization {
  Name: string;
  Logo: string;
  Description: string;
}

interface EMatchesTeam {
  Match: Match;
}

interface Match {
  Date: string;
  Team1: string;
  Team2: string;
  Score?: string;
  Status: 'upcoming' | 'live' | 'finished';
}

export class TeamPageLook {
  private container: HTMLElement;
  private teamData: Team;

  constructor(containerId: string, teamData?: Team) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
    this.teamData = staticTeamData; // Використовуємо статичні дані, якщо не передано інші
  }

  // Решта класу залишається незмінною
  async render() {
    this.container.innerHTML = `
      <main class="team-page">
        <!-- Team Header Section -->
        <section class="team-header">
          <div class="team-photo">
            <img src="${this.teamData.TeamPhoto}" alt="${this.teamData.TeamName}" onerror="this.src='default-team-photo.png'">
          </div>
          <div class="team-info">
            <h1>${this.teamData.TeamName}</h1>
            <div class="sport-type">
              ${this.getSportIcon(this.teamData.TypeSport)} ${this.teamData.TypeSport}
            </div>
            ${this.teamData.OrganizationTeam && this.teamData.OrganizationTeam.length > 0 ? `
              <div class="organizations">
                <h3>Організації:</h3>
                <div class="organization-logos">
                  ${this.teamData.OrganizationTeam.map(org => `
                    <img src="${org.Organization.Logo}" alt="${org.Organization.Name}" 
                         title="${org.Organization.Name}" onerror="this.src='default-org-logo.png'">
                  `).join('')}
                </div>
              </div>
            ` : ''}
          </div>
        </section>

        <!-- Trainer Section -->
        <section class="trainer-section">
          <h2>Тренер</h2>
          <div class="trainer-card">
            <img src="${this.teamData.Trainer.Photo}" alt="${this.teamData.Trainer.FullName}" 
                 class="trainer-photo" onerror="this.src='default-trainer-photo.png'">
            <div class="trainer-info">
              <h3>${this.teamData.Trainer.FullName}</h3>
              <p>${this.teamData.Trainer.Experience}</p>
              <p>Логін: ${this.teamData.LoginTrainer}</p>
            </div>
          </div>
        </section>

        <!-- Athletes Section -->
        <section class="athletes-section">
          <h2>Гравці</h2>
          <div class="athletes-grid">
            ${this.teamData.TeamAthletes.map(teamAthlete => `
              <div class="athlete-card">
                <img src="${teamAthlete.Athlete.Photo}" alt="${teamAthlete.Athlete.FullName}" 
                     class="athlete-photo" onerror="this.src='default-athlete-photo.png'">
                <div class="athlete-info">
                  <h3>${teamAthlete.Athlete.FullName}</h3>
                  <p>Позиція: ${teamAthlete.Athlete.Position}</p>
                  <p>Статистика: ${teamAthlete.Athlete.Stats}</p>
                </div>
              </div>
            `).join('')}
          </div>
        </section>
<section class="stats-section">
        <h2>Статистика команди</h2>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-value">${this.teamData.Stats?.totalGames || 0}</div>
            <div class="stat-label">Зіграно матчів</div>
          </div>
          <div class="stat-card win">
            <div class="stat-value">${this.teamData.Stats?.wins || 0}</div>
            <div class="stat-label">Перемоги</div>
          </div>
          <div class="stat-card loss">
            <div class="stat-value">${this.teamData.Stats?.losses || 0}</div>
            <div class="stat-label">Поразки</div>
          </div>
          <div class="stat-card draw">
            <div class="stat-value">${this.teamData.Stats?.draws || 0}</div>
            <div class="stat-label">Нічиї</div>
          </div>
          <div class="stat-card">
            <div class="stat-value">${this.teamData.Stats?.goalsScored || 0}</div>
            <div class="stat-label">Забито голів</div>
          </div>
          <div class="stat-card">
            <div class="stat-value">${this.teamData.Stats?.goalsConceded || 0}</div>
            <div class="stat-label">Пропущено голів</div>
          </div>
        </div>
      </section>
        <!-- Matches Section -->
        <section class="matches-section">
          <h2>Матчі</h2>
          <div class="matches-tabs">
            <button class="tab-button active" data-tab="upcoming">Майбутні</button>
            <button class="tab-button" data-tab="finished">Завершені</button>
          </div>
          <div class="matches-list" id="upcomingMatches">
            ${this.teamData.eMatchesTeams 
              ?.filter(m => m.Match.Status === 'upcoming')
              .map(match => `
                <div class="match-card upcoming">
                  <div class="match-teams">
                    <span class="team">${match.Match.Team1}</span>
                    <span class="vs">vs</span>
                    <span class="team">${match.Match.Team2}</span>
                  </div>
                  <div class="match-date">
                    ${new Date(match.Match.Date).toLocaleDateString()} • 
                    ${new Date(match.Match.Date).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
                  </div>
                </div>
              `).join('') || '<p>Немає майбутніх матчів</p>'}
          </div>
          <div class="matches-list hidden" id="finishedMatches">
            ${this.teamData.eMatchesTeams 
              ?.filter(m => m.Match.Status === 'finished')
              .map(match => `
                <div class="match-card finished">
                  <div class="match-teams">
                    <span class="team">${match.Match.Team1}</span>
                    <span class="vs">vs</span>
                    <span class="team">${match.Match.Team2}</span>
                  </div>
                  <div class="match-score">${match.Match.Score}</div>
                  <div class="match-date">
                    ${new Date(match.Match.Date).toLocaleDateString()}
                  </div>
                </div>
              `).join('') || '<p>Немає завершених матчів</p>'}
          </div>
        </section>
      </main>
    `;
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      'Футбол': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Теніс': '🎾',
      'Хокей': '🏒',
      'Бокс': '🥊'
    };
    return icons[sportType] || '🏅';
  }
}