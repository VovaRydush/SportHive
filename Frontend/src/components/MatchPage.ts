import { AthconsteProf, judges, match, TeamIndivid, Teams, users } from './db';
import './matchPage.css'

export class MatchPage {
  private container: HTMLElement;
  private MatchId: string;
  constructor(containerId: string,matchId:string) {
   this.MatchId = matchId;
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    const matchData = this.buildTeamData(this.MatchId);

    this.container.innerHTML = `
      <section class="match-page">
        <!-- Заголовок матчу -->
        <div class="match-header">
          <div class="event-info">
            <h1>${matchData.NameEvent}</h1>
            <div class="event-meta">
              <span class="sport-type">${this.getSportIcon(matchData.TypeSport)} ${matchData.TypeSport}</span>
              <span class="tour-info">Тур ${matchData.Tour} • Група ${matchData.Group}</span>
              <span class="system-info">Система: ${this.getSystemName(matchData.systems)}</span>
            </div>
          </div>
          <div class="match-status ${matchData.StatusMatch.toLowerCase()}">
            ${this.getStatusText(matchData.StatusMatch)}
          </div>
        </div>

        <!-- Основна інформація про матч -->
        <div class="match-main">
          <!-- Команди -->
          <div class="teams-container">
            <div class="team">
              <img src="${matchData.FirstTeam.TeamPhoto}" alt="${matchData.NameFirstTeam}" 
                   onerror="this.src='https://via.placeholder.com/150'">
              <h2>${matchData.NameFirstTeam}</h2>
            </div>
            
            <div class="match-vs">
              <span class="match-time">
                ${new Date(matchData.DataMatch).toLocaleDateString()} • 
                ${new Date(matchData.DataMatch).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
              </span>
              <span class="vs">VS</span>
              <span class="match-location">📍 ${matchData.LocationName}</span>
            </div>
            
            <div class="team">
              <img src="${matchData.SecondTeam.TeamPhoto}" alt="${matchData.NameSecondTeam}" 
                   onerror="this.src='https://via.placeholder.com/150'">
              <h2>${matchData.NameSecondTeam}</h2>
            </div>
          </div>

          <!-- Додаткова інформація -->
          <div class="match-additional-info">
            <h3>Деталі матчу</h3>
            <p>${matchData.description}</p>
            <p><strong>Суддя:</strong> ${matchData.Judge.FirsName} ${matchData.Judge.LastName} (${matchData.Judge.Category})</p>
            <p><strong>Додаткова інформація:</strong> ${matchData.AddInformation}</p>
          </div>
        </div>

        <!-- Склади команд -->
        <div class="team-squads">
          <div class="squad">
            <h3>Склад ${matchData.NameFirstTeam}</h3>
            <div class="players-list">
              ${matchData.FirstTeam.TeamAthletes.map(player => `
                <div class="player-card">
                  <span class="player-name">${player.FullName}</span>
                  <span class="player-status">${player.AthleteStatus}</span>
                </div>
              `).join('')}
            </div>
          </div>
          
          <div class="squad">
            <h3>Склад ${matchData.NameSecondTeam}</h3>
            <div class="players-list">
              ${matchData.SecondTeam.TeamAthletes.map(player => `
                <div class="player-card">
                  <span class="player-name">${player.FullName}</span>
                  <span class="player-status">${player.AthleteStatus}</span>
                </div>
              `).join('')}
            </div>
          </div>
        </div>
                <!-- Події матчу -->
        <div class="match-events">
          <h3>Хід матчу</h3>
          <div class="events-timeline">
            ${matchData.events.map(event => `
              <div class="event ${event.type}">
                <div class="event-time">${event.time}</div>
                <div class="event-icon">${this.getEventIcon(event.type)}</div>
                <div class="event-details">
                  ${event.player ? `
                    <div class="event-player">
                      <span class="team-badge ${event.team === 'Динамо' ? 'team1' : 'team2'}">${event.team}</span>
                      ${event.player}
                    </div>
                  ` : ''}
                  <div class="event-description">${event.description}</div>
                </div>
              </div>
            `).join('')}
          </div>
        </div>
      </section>
    `;
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      'Футбол': '⚽',
      'Баскетбол': '🏀',
      'Волейбол': '🏐',
      'Теніс': '🎾',
      'Хокей': '🏒'
    };
    return icons[sportType] || '🏅';
  }

  private getSystemName(system: string): string {
    const systems: Record<string, string> = {
      'RoundRobin': 'Круговий',
      'PlayOff': 'Плейоф',
      'MixedSystem': 'Змішана система',
      'SwissSystem': 'Швейцарська система',
      'QualificationByStandards': 'Кваліфікація за нормативами',
      'GroupStage': 'Груповий етап',
      'Final': 'Фінал',
      'OlympicSystem': 'Олімпійська система',
      'KnockoutSystem': 'Система з вибуванням',
      'DoubleElimination': 'Подвійне вибування'
    };
    return systems[system] || system;
  }

  private getStatusText(status: string): string {
    const statuses: Record<string, string> = {
      'Live': '🟢 LIVE',
      'Finished': '🔴 Завершено',
      'Upcoming': '🟡 Очікується'
    };
    return statuses[status] || status;
  }
  private getEventIcon(eventType: string): string {
    const icons: Record<string, string> = {
      'start': '▶️',
      'goal': '⚽',
      'yellow_card': '🟨',
      'red_card': '🟥',
      'substitution': '🔄',
      'end_half': '⏸️',
      'end': '⏹️',
      'penalty': '🎯',
      'foul': '⚠️',
      'injury': '💉',
      'corner': '↗️',
      'offside': '🚩',
      'free_kick': '🎯',
      'penalty_missed': '❌'
    };
    return icons[eventType] || '🔵';
  }
  private getAthletes(athletesTeam:AthconsteProf){
    
  }
  private buildTeamData(MatchId: string) {
    const Match = match.find(t => t.idMatch === MatchId);
    const JudgeInfo = judges.find(j => j.login === Match?.loginJudge);
    const FirstTeam = Teams.find(t => t.name === Match?.team1);
    const SecondTeam = Teams.find(t => t.name === Match?.team2);
    const athletesFirstTeam = this.getAthletesByLogins(FirstTeam?.AthleteLogins || []);
    const athletesSecondTeam = this.getAthletesByLogins(SecondTeam?.AthleteLogins || []);
    if (!Match) throw new Error("Команду не знайдено");
    return {
      NameEvent: Match.NameEvent,
      systems: Match.systems,
      DataStart: Match.DataStart,
      DataEnd: Match.DataEnd,
      TypeSport: Match.sport,
      description: Match.description,
      StatusMatch: Match.status,
      NameFirstTeam: Match.team1,
      FirstTeam: {
        TeamName: FirstTeam?.name,
        TeamPhoto: FirstTeam?.logo,
        TypeSport: FirstTeam?.sport,
        TeamAthletes: this.AthletesGet(athletesFirstTeam)
      },
      NameSecondTeam: Match.team2,
      SecondTeam: {
        TeamName: SecondTeam?.name,
        TeamPhoto: SecondTeam?.logo,
        TypeSport: SecondTeam?.sport,
        TeamAthletes:this.AthletesGet(athletesSecondTeam)
      },
      loginJudge: JudgeInfo?.login,
      events: Match.dataFotball,

      Judge: {
        FirsName: JudgeInfo?.FirsName,
        LastName: JudgeInfo?.LastName,
        Category: JudgeInfo?.Category
      },
      DataMatch: Match.date,
      LocationName: Match.LocationName,
      Tour: Match.Tour,
      Group: Match.Group,
      AddInformation: Match.AddInformation
    }
  }
  private getAthletesByLogins(logins: string[]): any[] {
    return users.filter(user => logins.includes(user.login));
  }
  private AthletesGet(logins: string[]): { FullName: string; AthleteStatus: string }[] {
  return users
    .filter(user => logins.includes(user.login))
    .map(user => ({
      FullName: user.name,
      AthleteStatus: "Основний склад"
    }));
}


}
