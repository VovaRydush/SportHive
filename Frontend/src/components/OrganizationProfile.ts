import { allEvents, Event, Judge, judges, organizations, TeamIndivid, Teams, Trainer, trainers } from './db';
import './organizationProfile.css'
interface Organization {
    login: string;
    photo:string;
    NameOrganization: string;
    TypeOrganozation: string;
    Description: string;
    Country: string;
    Teams: TeamIndivid[];
    OrganizationJudge: Judge[];
    OrganizationTrainer: Trainer[];
    Events: Event[];
}
export class OrganizationProfile {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    const orgData = this.setDataOrganiz();
    

    this.container.innerHTML = `
      <section class="organization-profile">
        <!-- Шапка профілю -->
        <div class="org-header">
          <div class="org-logo-container">
            <img src="https://images.unsplash.com/photo-1543357480-c60d400e7ef6?auto=format&fit=crop&w=300&q=80" 
                 alt="Лого організації" class="org-logo">
            <div class="org-type-badge">${orgData.TypeOrganozation}</div>
          </div>
          
          <div class="org-main-info">
            <h1 class="org-title">${orgData.NameOrganization}</h1>
            <div class="org-meta">
              <span class="org-country">${this.getCountryFlag(orgData.Country)} ${orgData.Country}</span>
            </div>
            
            <div class="org-description-block">
              <h3>Про організацію</h3>
              <p class="org-description">${orgData.Description}</p>
            </div>
          </div>
        </div>

        <!-- Команди -->
        <div class="org-section">
          <h2 class="section-title">Наші команди</h2>
          <div class="teams-grid">
            ${orgData.Teams.map(team => `
              <div class="team-card">
                <div class="team-header">
                  <span class="team-sport-icon">${this.getSportIcon(team.sport)}</span>
                  <h3 class="team-name">${team.name}</h3>
                </div>
                <div class="team-details">
                  <p><strong>Вид спорту:</strong> ${team.sport}</p>
                </div>
              </div>
            `).join('')}
          </div>
        </div>

        <!-- Персонал -->
        <div class="org-staff-section">
          <div class="staff-column">
            <h2 class="section-title">Судді</h2>
            <div class="staff-list">
              ${orgData.OrganizationJudge.map(judge => `
                <div class="staff-card">
                  <div class="staff-info">
                    <h3 class="staff-name">${judge.FirsName+" "+judge.LastName}</h3>
                    <p class="staff-category">${judge.Category} категорія</p>
                  </div>
                </div>
              `).join('')}
            </div>
          </div>
          
          <div class="staff-column">
            <h2 class="section-title">Тренери</h2>
            <div class="staff-list">
              ${orgData.OrganizationTrainer.map(trainer => `
                <div class="staff-card">
                  <div class="staff-info">
                    <h3 class="staff-name">${trainer.FirsName+" "+trainer.LastName}</h3>
                    <p class="staff-sport">${this.getSportIcon(trainer.SportType)} ${trainer.SportType}</p>
                  </div>
                </div>
              `).join('')}
            </div>
          </div>
        </div>
        <!-- Події -->
        <div class="org-section">
          <h2 class="section-title">Останні події</h2>
          <div class="events-timeline">
            ${orgData.Events.map(event => `
              <div class="event-item">
                <div class="event-date">${new Date(event.date).toLocaleDateString('uk-UA', { day: 'numeric', month: 'long' })}</div>
                <div class="event-content">
                  <h3 class="event-title">${event.NameEvent}</h3>
                  <p class="event-description">${event.description}</p>
                  <div class="event-meta">
                    <span class="event-location">🏟️ ${event.location}</span>
                  </div>
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
      'Гімнастика': '🤸',
      'Теніс': '🎾',
      'Волейбол': '🏐',
      'Бокс': '🥊',
      'Легка атлетика': '🏃',
      'Важка атлетика': '🏋️'
    };
    return icons[sportType] || '🏅';
  }

  private getCountryFlag(country: string): string {
    const flags: Record<string, string> = {
      'Україна': '🇺🇦',
      'США': '🇺🇸',
      'Німеччина': '🇩🇪'
    };
    return flags[country] || '🌍';
  }
  private setDataOrganiz() : Organization
  {
    var organization = organizations.find(o=>o.login === localStorage.getItem('login'));
    var organizationJudge = judges.filter(j => organization?.OrganizationJudge.includes(j.login));
    var organizationTrainer = trainers.filter(t => organization?.OrganizationTrainer.includes(t.login));
    var events = allEvents.filter(e => organization?.Events.includes(e.NameEvent));
      return {
        login: localStorage.getItem('login') ?? "",
        photo: organization?.photo ?? "",
        NameOrganization: organization?.NameOrganization ?? "",
        TypeOrganozation: organization?.TypeOrganozation ?? "",
        Description: organization?.Description ?? "",
        Country: organization?.Description ?? "",
        Teams: organization?.Teams ?? [],
        OrganizationJudge: organizationJudge,
        OrganizationTrainer: organizationTrainer,
        Events: events ?? []
    };
  }
}
