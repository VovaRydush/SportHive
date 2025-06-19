import { AthconsteProf, match, Organization, organizations, Stats, Teams, Trainer, trainers, users } from './db';
import { NotificationKarina } from './Notification';
import './teamPage.css';


// Інтерфейси залишаються незмінними
interface Team {
  TeamName: string;
  TeamPhoto: string;
  LoginTrainer: string;
  Trainer: Trainer;
  TeamAthletes: AthconsteProf[];
  TypeSport: string;
  Stats: Stats;
  OrganizationTeam: Organization[];
  eMatchesTeams?: EMatchesTeam[];

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

  constructor(containerId: string, nameTeam: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
    this.teamData = this.getInfoTeam(nameTeam);
    console.log(this.teamData);
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
                    <img src="${org.photo}" alt="${org.NameOrganization}" 
                         title="${org.NameOrganization}" onerror="this.src='default-org-logo.png'">
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
            <img src="${this.teamData.Trainer.Photo  ?? ""}" alt="${this.teamData.Trainer.FirsName ?? "" + " " + this.teamData.Trainer.LastName ?? "" }" 
                 class="trainer-photo" onerror="this.src='default-trainer-photo.png'">
            <div class="trainer-info">
              <h3>${this.teamData.Trainer.FirsName ?? ""}</h3>
              <p>${this.teamData.Trainer.LastName  ?? ""}</p>
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
        <!-- Кнопки управління -->
        <div class="athlete-actions">
          <button class="btn-icon btn-edit" data-id="${teamAthlete.name}">
            <svg viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z"/>
            </svg>
          </button>
          <button class="btn-icon btn-delete" data-id="${teamAthlete.name}">
            <svg viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M19,4H15.5L14.5,3H9.5L8.5,4H5V6H19M6,19A2,2 0 0,0 8,21H16A2,2 0 0,0 18,19V7H6V19Z"/>
            </svg>
          </button>
        </div>
        
        <!-- Основна інформація про гравця -->
        <img src="${teamAthlete.photo}" alt="${teamAthlete.name}" 
             class="athlete-photo" onerror="this.src='default-athlete-photo.png'">
        <div class="athlete-info">
          <h3>${teamAthlete.name}</h3>
          <p>Позиція: ${teamAthlete.Position}</p>
        </div>
      </div>
    `).join('')}
  </div>
</section>
<section class="stats-section">
        <h2>Статистика команди</h2>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-value">${this.teamData.Stats?.TotalMatches || 0}</div>
            <div class="stat-label">Зіграно матчів</div>
          </div>
          <div class="stat-card win">
            <div class="stat-value">${this.teamData.Stats?.Wins || 0}</div>
            <div class="stat-label">Перемоги</div>
          </div>
          <div class="stat-card loss">
            <div class="stat-value">${this.teamData.Stats?.Losses || 0}</div>
            <div class="stat-label">Поразки</div>
          </div>
          <div class="stat-card draw">
            <div class="stat-value">${this.teamData.Stats?.Draws || 0}</div>
            <div class="stat-label">Нічиї</div>
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
                    ${new Date(match.Match.Date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
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
  private getInfoTeam(NameTeam: string): Team {
    const teamInfo = Teams.find(t => t.name === NameTeam);
    if (!teamInfo) {
      var nit = new NotificationKarina();
      nit.show(`Team ${NameTeam} not found`,'error');
      throw new Error();
    }

    const trainer = trainers.find(t => t.login === teamInfo.LoginTrainer);

    const teamAthletes = users.filter(a => teamInfo.AthleteLogins.includes(a.login));

    const organizationTeam = organizations.filter(org =>
      org.Teams.some(team => team.name === NameTeam)
    );

    const eMatchesTeams = match
        .filter(m => m.team1 === NameTeam || m.team2 === NameTeam)
        .map(m => ({
            Match: {
                Date: m.date,
                Team1: m.team1,
                Team2: m.team2,
                Score: m.score,
                Status: m.status as 'upcoming' | 'live' | 'finished'
            }
        }));

    const completeTeam: Team = {
      TeamName: teamInfo.name ?? "",
      TeamPhoto: teamInfo.logo ?? "",
      LoginTrainer: teamInfo.LoginTrainer ?? "",
      Trainer: trainer || {} as Trainer,
      TeamAthletes: teamAthletes ?? [],
      TypeSport: teamInfo.sport ?? "",
      Stats: teamInfo.stats ?? undefined,
      OrganizationTeam: organizationTeam ?? [],
      eMatchesTeams: eMatchesTeams ?? []
    };

    return completeTeam;
  }
}