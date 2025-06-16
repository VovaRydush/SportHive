import './organizationProfile.css'
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
    // Розширені мок дані
    const orgData = {
      login: "org_sportlife",
      NameOrganization: "Спортивне Життя",
      TypeOrganozation: "Спортивна федерація",
      Description: "Провідний організатор спортивних змагань у регіоні. Заснована 2010 року з метою популяризації здорового способу життя та розвитку спортивної інфраструктури. Організація має 12 власних спортивних майданчиків та тісно співпрацює з місцевими школами.",
      Country: "Україна, Київ",
      DateFoundation: "2010-05-15T00:00:00",
      Teams: [
        { Name: "Динамо", SportType: "Футбол", Founded: 2012, Members: 25 },
        { Name: "Олімпійці", SportType: "Баскетбол", Founded: 2015, Members: 15 },
        { Name: "Стрімкі", SportType: "Легка атлетика", Founded: 2018, Members: 32 },
        { Name: "Титани", SportType: "Важка атлетика", Founded: 2013, Members: 18 }
      ],
      OrganizationJudge: [
        { Name: "Іван Петренко", Category: "Міжнародна", Experience: "12 років" },
        { Name: "Олена Сидорова", Category: "Національна", Experience: "8 років" },
        { Name: "Михайло Ковальчук", Category: "Міжнародна", Experience: "15 років" }
      ],
      OrganizationTrainer: [
        { Name: "Михайло Коваль", SportType: "Футбол", Qualification: "Тренер UEFA Pro" },
        { Name: "Анна Мельник", SportType: "Гімнастика", Qualification: "Майстер спорту" },
        { Name: "Олексій Шевченко", SportType: "Бокс", Qualification: "Заслужений тренер" }
      ],
      Events: [
        { 
          Name: "Чемпіонат міста з футболу", 
          Date: "2023-10-15", 
          Participants: 120,
          Location: "Стадіон 'Динамо'",
          Description: "Щорічний турнір серед аматорських команд міста"
        },
        { 
          Name: "Кубок весни з баскетболу", 
          Date: "2023-04-05", 
          Participants: 80,
          Location: "Палац спорту",
          Description: "Весняний турнір для молодіжних команд"
        },
        { 
          Name: "Зимові ігри", 
          Date: "2022-12-20", 
          Participants: 200,
          Location: "Спорткомплекс 'Олімпійський'",
          Description: "Мультиспортивні змагання у зимових видах спорту"
        }
      ]
    };

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
              <span class="org-founded">Заснована: ${new Date(orgData.DateFoundation).toLocaleDateString()}</span>
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
                  <span class="team-sport-icon">${this.getSportIcon(team.SportType)}</span>
                  <h3 class="team-name">${team.Name}</h3>
                </div>
                <div class="team-details">
                  <p><strong>Вид спорту:</strong> ${team.SportType}</p>
                  <p><strong>Заснована:</strong> ${team.Founded}</p>
                  <p><strong>Учасники:</strong> ${team.Members} осіб</p>
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
                    <h3 class="staff-name">${judge.Name}</h3>
                    <p class="staff-category">${judge.Category} категорія</p>
                    <p class="staff-experience">Досвід: ${judge.Experience}</p>
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
                    <h3 class="staff-name">${trainer.Name}</h3>
                    <p class="staff-sport">${this.getSportIcon(trainer.SportType)} ${trainer.SportType}</p>
                    <p class="staff-qualification">${trainer.Qualification}</p>
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
                <div class="event-date">${new Date(event.Date).toLocaleDateString('uk-UA', { day: 'numeric', month: 'long' })}</div>
                <div class="event-content">
                  <h3 class="event-title">${event.Name}</h3>
                  <p class="event-description">${event.Description}</p>
                  <div class="event-meta">
                    <span class="event-location">🏟️ ${event.Location}</span>
                    <span class="event-participants">👥 ${event.Participants} учасників</span>
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
}
