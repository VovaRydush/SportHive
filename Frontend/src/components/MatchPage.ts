import './matchPage.css'
export class MatchPage {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    // Мок дані для матчу
    const matchData = {
      IdEvent: 123,
      NameEvent: "Чемпіонат міста з футболу",
      EventPhoto: "data:image/png;base64,...",
      systems: "GroupStage",
      DataStart: "2023-11-15T00:00:00",
      DataEnd: "2023-12-20T00:00:00",
      TypeSport: "Футбол",
      description: "Щорічний турнір серед аматорських команд міста",
      StatusMatch: "Upcoming",
      NameFirstTeam: "Динамо",
      FirstTeam: {
        TeamName: "Динамо",
        TeamPhoto: "data:image/png;base64,...",
        TypeSport: "Футбол",
        TeamAthletes: [
          { loginAthlets: "player1", AthleteStatus: "Основний склад" },
          { loginAthlets: "player2", AthleteStatus: "Основний склад" },
          // ... інші гравці
        ]
      },
      NameSecondTeam: "Скіфи",
      SecondTeam: {
        TeamName: "Скіфи",
        TeamPhoto: "data:image/png;base64,...",
        TypeSport: "Футбол",
        TeamAthletes: [
          { loginAthlets: "player3", AthleteStatus: "Основний склад" },
          { loginAthlets: "player4", AthleteStatus: "Запас" },
          // ... інші гравці
        ]
      },
      loginJudge: "judge1",
      Judge: {
        FirsName: "Іван",
        LastName: "Петренко",
        Category: "Міжнародна"
      },
      DataMatch: "2023-11-20T15:00:00",
      LocationName: "Стадіон 'Динамо'",
      Tour: 3,
      Group: 1,
      AddInformation: "Матч відбудеться за будь-яких погодних умов"
    };

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
                ${new Date(matchData.DataMatch).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
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
                  <span class="player-name">${player.loginAthlets}</span>
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
                  <span class="player-name">${player.loginAthlets}</span>
                  <span class="player-status">${player.AthleteStatus}</span>
                </div>
              `).join('')}
            </div>
          </div>
        </div>

        <!-- Турнірна інформація -->
        <div class="tournament-info">
          <h3>Турнірна інформація</h3>
          <div class="info-grid">
            <div class="info-card">
              <span class="info-label">Дата початку</span>
              <span class="info-value">${new Date(matchData.DataStart).toLocaleDateString()}</span>
            </div>
            <div class="info-card">
              <span class="info-label">Дата завершення</span>
              <span class="info-value">${matchData.DataEnd ? new Date(matchData.DataEnd).toLocaleDateString() : 'Не вказано'}</span>
            </div>
            <div class="info-card">
              <span class="info-label">Система проведення</span>
              <span class="info-value">${this.getSystemName(matchData.systems)}</span>
            </div>
            <div class="info-card">
              <span class="info-label">Статус</span>
              <span class="info-value">${this.getStatusText(matchData.StatusMatch)}</span>
            </div>
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
}
