// EventPage.ts
import './eventPageLook.css';

interface EventData {
  id: number;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  sportType: string;
  participants: Participant[];
  matches: Match[];
  standings: Standing[];
  tournamentRounds: TournamentRound[];
}

interface Participant {
  id: number;
  name: string;
  type: 'team' | 'individual';
  logo?: string;
  photo?: string;
}

interface Match {
  id: number;
  round: number;
  participant1: Participant;
  participant2: Participant;
  date: string;
  status: 'upcoming' | 'live' | 'finished';
  score?: string;
  events?: MatchEvent[];
}

interface MatchEvent {
  time: string;
  description: string;
  type: 'goal' | 'penalty' | 'substitution' | 'card';
}

interface Standing {
  participant: Participant;
  points: number;
  wins: number;
  draws: number;
  losses: number;
  goalsFor: number;
  goalsAgainst: number;
}

interface TournamentRound {
  round: number;
  name: string;
  matches: Match[];
}

export class EventPageLook {
  private container: HTMLElement;
  private eventData: EventData;

  constructor(containerId: string, eventData?: EventData) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
    this.eventData = eventData || this.getDefaultEventData();
  }

  private getDefaultEventData(): EventData {
    return {
      id: 1,
      name: "Чемпіонат міста з футболу 2023",
      description: "Щорічний турнір серед аматорських команд міста",
      startDate: "2023-09-01",
      endDate: "2023-11-30",
      location: "Стадіон 'Юність', м. Київ",
      sportType: "Футбол",
      participants: [
        { id: 1, name: "Динамо", type: 'team', logo: "https://example.com/dynamo.png" },
        { id: 2, name: "Шахтар", type: 'team', logo: "https://example.com/shakhtar.png" },
        { id: 3, name: "Ворскла", type: 'team', logo: "https://example.com/vorskla.png" },
        { id: 4, name: "Зоря", type: 'team', logo: "https://example.com/zorya.png" }
      ],
      matches: [
        {
          id: 1,
          round: 1,
          participant1: { id: 1, name: "Динамо", type: 'team' },
          participant2: { id: 2, name: "Шахтар", type: 'team' },
          date: "2023-09-10T15:00",
          status: 'finished',
          score: "2:1",
          events: [
            { time: "35'", description: "Гол - Іваненко О. (Динамо)", type: 'goal' },
            { time: "78'", description: "Жовта картка - Петров В. (Шахтар)", type: 'card' }
          ]
        },
        {
          id: 2,
          round: 1,
          participant1: { id: 3, name: "Ворскла", type: 'team' },
          participant2: { id: 4, name: "Зоря", type: 'team' },
          date: "2023-09-11T17:00",
          status: 'finished',
          score: "1:1"
        },
        {
          id: 3,
          round: 2,
          participant1: { id: 1, name: "Динамо", type: 'team' },
          participant2: { id: 3, name: "Ворскла", type: 'team' },
          date: "2023-10-15T16:00",
          status: 'upcoming'
        }
      ],
      standings: [
        { 
          participant: { id: 1, name: "Динамо", type: 'team' },
          points: 4,
          wins: 1,
          draws: 0,
          losses: 0,
          goalsFor: 2,
          goalsAgainst: 1
        },
        { 
          participant: { id: 4, name: "Зоря", type: 'team' },
          points: 1,
          wins: 0,
          draws: 1,
          losses: 0,
          goalsFor: 1,
          goalsAgainst: 1
        },
        { 
          participant: { id: 3, name: "Ворскла", type: 'team' },
          points: 1,
          wins: 0,
          draws: 1,
          losses: 0,
          goalsFor: 1,
          goalsAgainst: 1
        },
        { 
          participant: { id: 2, name: "Шахтар", type: 'team' },
          points: 0,
          wins: 0,
          draws: 0,
          losses: 1,
          goalsFor: 1,
          goalsAgainst: 2
        }
      ],
      tournamentRounds: [
        {
          round: 1,
          name: "Груповий етап. Тур 1",
          matches: [
            {
              id: 1,
              round: 1,
              participant1: { id: 1, name: "Динамо", type: 'team' },
              participant2: { id: 2, name: "Шахтар", type: 'team' },
              date: "2023-09-10T15:00",
              status: 'finished',
              score: "2:1"
            },
            {
              id: 2,
              round: 1,
              participant1: { id: 3, name: "Ворскла", type: 'team' },
              participant2: { id: 4, name: "Зоря", type: 'team' },
              date: "2023-09-11T17:00",
              status: 'finished',
              score: "1:1"
            }
          ]
        },
        {
          round: 2,
          name: "Груповий етап. Тур 2",
          matches: [
            {
              id: 3,
              round: 2,
              participant1: { id: 1, name: "Динамо", type: 'team' },
              participant2: { id: 3, name: "Ворскла", type: 'team' },
              date: "2023-10-15T16:00",
              status: 'upcoming'
            },
            {
              id: 4,
              round: 2,
              participant1: { id: 2, name: "Шахтар", type: 'team' },
              participant2: { id: 4, name: "Зоря", type: 'team' },
              date: "2023-10-16T18:00",
              status: 'upcoming'
            }
          ]
        }
      ]
    };
  }

  async render() {
    this.container.innerHTML = `
      <main class="event-page">
        <!-- Event Header -->
        <section class="event-header">
          <div class="event-title">
            <h1>${this.eventData.name}</h1>
            <div class="event-sport">
              ${this.getSportIcon(this.eventData.sportType)} ${this.eventData.sportType}
            </div>
          </div>
          <div class="event-meta">
            <div class="event-dates">
              <span>${new Date(this.eventData.startDate).toLocaleDateString()}</span>
              <span> - </span>
              <span>${new Date(this.eventData.endDate).toLocaleDateString()}</span>
            </div>
            <div class="event-location">${this.eventData.location}</div>
          </div>
        </section>

        <!-- Event Description -->
        <section class="event-description">
          <h2>Про турнір</h2>
          <p>${this.eventData.description}</p>
        </section>

        <!-- Participants Section -->
        <section class="participants-section">
          <h2>Учасники (${this.eventData.participants.length})</h2>
          <div class="participants-grid">
            ${this.eventData.participants.map(participant => `
              <div class="participant-card">
                ${participant.logo ? `
                  <img src="${participant.logo}" alt="${participant.name}" class="participant-logo">
                ` : `
                  <div class="participant-placeholder">${participant.name.charAt(0)}</div>
                `}
                <h3>${participant.name}</h3>
              </div>
            `).join('')}
          </div>
        </section>

        <!-- Tournament Standings -->
        <section class="standings-section">
          <h2>Турнірна таблиця</h2>
          <div class="standings-table">
            <table>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Команда</th>
                  <th>І</th>
                  <th>В</th>
                  <th>Н</th>
                  <th>П</th>
                  <th>ГЗ</th>
                  <th>ГП</th>
                  <th>РМ</th>
                  <th>О</th>
                </tr>
              </thead>
              <tbody>
                ${this.eventData.standings
                  .sort((a, b) => b.points - a.points || (b.goalsFor - b.goalsAgainst) - (a.goalsFor - a.goalsAgainst))
                  .map((standing, index) => `
                    <tr>
                      <td>${index + 1}</td>
                      <td class="team-cell">
                        ${standing.participant.logo ? `
                          <img src="${standing.participant.logo}" alt="${standing.participant.name}" class="team-logo">
                        ` : ''}
                        ${standing.participant.name}
                      </td>
                      <td>${standing.wins + standing.draws + standing.losses}</td>
                      <td>${standing.wins}</td>
                      <td>${standing.draws}</td>
                      <td>${standing.losses}</td>
                      <td>${standing.goalsFor}</td>
                      <td>${standing.goalsAgainst}</td>
                      <td>${standing.goalsFor - standing.goalsAgainst}</td>
                      <td class="points-cell">${standing.points}</td>
                    </tr>
                  `).join('')}
              </tbody>
            </table>
          </div>
        </section>

        <!-- Tournament Rounds -->
        <section class="rounds-section">
          <h2>Тури турніру</h2>
          <div class="rounds-tabs">
            ${this.eventData.tournamentRounds.map(round => `
              <button class="round-tab ${round.round === 1 ? 'active' : ''}" data-round="${round.round}">
                ${round.name}
              </button>
            `).join('')}
          </div>
          <div class="rounds-content">
            ${this.eventData.tournamentRounds.map(round => `
              <div class="round-matches ${round.round === 1 ? 'active' : ''}" data-round="${round.round}">
                <h3>${round.name}</h3>
                <div class="matches-list">
                  ${round.matches.map(match => `
                    <div class="match-card ${match.status}">
                      <div class="match-teams">
                        <div class="team">
                          ${match.participant1.logo ? `
                            <img src="${match.participant1.logo}" alt="${match.participant1.name}" class="team-logo">
                          ` : ''}
                          <span class="team-name">${match.participant1.name}</span>
                        </div>
                        <div class="match-info">
                          <div class="match-score">${match.score || 'vs'}</div>
                          <div class="match-date">
                            ${new Date(match.date).toLocaleDateString()} • 
                            ${new Date(match.date).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
                          </div>
                        </div>
                        <div class="team">
                          ${match.participant2.logo ? `
                            <img src="${match.participant2.logo}" alt="${match.participant2.name}" class="team-logo">
                          ` : ''}
                          <span class="team-name">${match.participant2.name}</span>
                        </div>
                      </div>
                      ${match.events && match.events.length > 0 ? `
                        <div class="match-events">
                          <h4>Події матчу:</h4>
                          <ul>
                            ${match.events.map(event => `
                              <li class="event-${event.type}">${event.time} - ${event.description}</li>
                            `).join('')}
                          </ul>
                        </div>
                      ` : ''}
                      <button class="btn btn-outline match-details">Деталі матчу</button>
                    </div>
                  `).join('')}
                </div>
              </div>
            `).join('')}
          </div>
        </section>
      </main>
    `;

    this.setupEventListeners();
  }

  private setupEventListeners() {
    // Round tabs switching
    const roundTabs = document.querySelectorAll('.round-tab');
    roundTabs.forEach(tab => {
      tab.addEventListener('click', () => {
        const round = tab.getAttribute('data-round');
        
        // Update active tab
        roundTabs.forEach(t => t.classList.remove('active'));
        tab.classList.add('active');
        
        // Show corresponding content
        document.querySelectorAll('.round-matches').forEach(content => {
          content.classList.toggle('active', content.getAttribute('data-round') === round);
        });
      });
    });

    // Match details buttons
    document.querySelectorAll('.match-details').forEach(button => {
      button.addEventListener('click', (e) => {
        const matchCard = (e.target as HTMLElement).closest('.match-card');
        matchCard?.querySelector('.match-events')?.classList.toggle('expanded');
      });
    });
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