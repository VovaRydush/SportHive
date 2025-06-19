// EventPage.ts
import { allEvents, Match, match, Standing, Teams, users } from './db';
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
  standings: Standing[];
}

interface Participant {
  id: number;
  name: string;
  type: 'team' | 'individual';
  logo?: string;
  photo?: string;
}

export class EventPageLook {
  private container: HTMLElement;
  private eventName: string;
  private eventData: EventData;

  constructor(containerId: string, EventName: string) {
    const element = document.getElementById(containerId);
   this.eventName = EventName;
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.eventData = this.getDefaultEventData();
    console.log(this.eventData);
    this.container = element;
  }

  private getDefaultEventData(): EventData {
    return this.setInfo(this.eventName);
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
          <h2>Учасники (${this.eventData.participants})</h2>
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
                </tr>
              </thead>
              <tbody>
                ${this.eventData.standings
                  .sort((a, b) => b.points - a.points)
                  .map((standing, index) => `
                    <tr>
                      <td>${index + 1}</td>
                      <td class="team-cell">
                        ${standing.participant}
                      </td>
                      <td>${standing.wins + standing.draws + standing.losses}</td>
                      <td>${standing.wins}</td>
                      <td>${standing.draws}</td>
                      <td>${standing.losses}</td>
                      <td class="points-cell">${standing.points}</td>
                    </tr>
                  `).join('')}
              </tbody>
            </table>
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
  private setInfo(NameEvent: string) : any
  {
    const Event = allEvents.find(t => t.NameEvent === NameEvent);
   
    var participant; 
    if(Event?.TypeMatch === "TeamMatch")
    {
      const athletesFirstTeam = this.getsTeamsByLogins(Event?.teams || [])
      participant = this.AthletesGet(athletesFirstTeam);
    }
    if(Event?.TypeMatch === "IndividualMatch")
    {
      const athletesFirstTeam = this.getAthletesByLogins(Event?.athletes || []);
      participant = this.AthletesGet(athletesFirstTeam);
    }
    
    return {
      name: Event?.NameEvent,
      description: Event?.description ?? "",
      startDate: Event?.date ?? "",
      endDate: Event?.endDate ?? "",
      location: Event?.location ?? "",
      sportType: Event?.sport ?? [],
      participants: participant ?? [],
      standings: Event?.standings ?? [],
    };
  }
  private getAthletesByLogins(logins: string[]): any[] {
      return users.filter(user => logins.includes(user.login));
    }
    private AthletesGet(logins: string[]): { name: string; type: string; logo: string }[] {
    return users
      .filter(user => logins.includes(user.login))
      .map(user => ({
        name: user.name,
        type: "individual",
        logo: user.photo
      }));}

  private getsTeamsByLogins(names: string[]): any[] {
      return Teams.filter(user => names.includes(user.name));
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