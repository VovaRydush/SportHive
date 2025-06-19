import { organizations, Teams, Trainer, trainers } from "./db";

export class TrainerProfile {
  private container: HTMLElement;
  private loginTrainer: string;
  constructor(containerId: string, loginTrainers: string) {
    this.loginTrainer = loginTrainers;
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    const trainerData = {
      login: "trainer_koval",
      FirsName: "Михайло",
      LastName: "Коваль",
      DataBirth: "1980-05-15T00:00:00",
      Teams: [
        { Name: "Динамо", SportType: "Футбол" },
        { Name: "Юніорська збірна", SportType: "Футбол" }
      ],
      Organizations: [
        { Name: "Спортивне Життя", Position: "Головний тренер" }
      ],
      Photo: "data:image/png;base64,...",
      LatestMatches: [
        {
          Date: "2023-06-10",
          Team1: "Динамо",
          Team2: "Скіфи",
          Score: "2:1",
          Result: "win",
          Competition: "Чемпіонат міста"
        },
        {
          Date: "2023-05-28",
          Team1: "Динамо",
          Team2: "Вікторія",
          Score: "1:1",
          Result: "draw",
          Competition: "Кубок ліги"
        },
        {
          Date: "2023-05-15",
          Team1: "Динамо",
          Team2: "Олімпік",
          Score: "0:2",
          Result: "loss",
          Competition: "Чемпіонат міста"
        }
      ],
      Stats: {
        TotalMatches: 42,
        Wins: 30,
        Draws: 7,
        Losses: 5
      }
    };

    this.container.innerHTML = `
      <section class="user-profile">
        <!-- Шапка профілю -->
        <div class="profile-header">
          <div class="profile-photo">
            <img src="${trainerData.Photo}" alt="Фото тренера" 
                 onerror="this.src='https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=300&q=80'">
            <div class="sport-badge">🏆 Тренер</div>
          </div>
          
          <div class="profile-main">
            <h1>${trainerData.FirsName} ${trainerData.LastName}</h1>
            <div class="profile-details">
              <div class="detail-block">
                <h3>Особисті дані</h3>
                <p><strong>Дата народження:</strong> ${new Date(trainerData.DataBirth).toLocaleDateString()}</p>
                <p><strong>Вік:</strong> ${this.calculateAge(trainerData.DataBirth)} років</p>
                <p><strong>Логін:</strong> ${trainerData.login}</p>
              </div>
              
              <div class="detail-block">
                <h3>Команди</h3>
                ${trainerData.Teams.map(team => `
                  <p><strong>${team.Name}:</strong> ${team.SportType}</p>
                `).join('')}
              </div>
              
              <div class="detail-block">
                <h3>Організації</h3>
                ${trainerData.Organizations.map(org => `
                  <p><strong>${org.Name}:</strong> ${org.Position}</p>
                `).join('')}
              </div>
            </div>
          </div>
        </div>

        <!-- Статистика -->
        <div class="stats-section">
          <h2>Статистика</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${trainerData.Stats.TotalMatches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${trainerData.Stats.Wins}</span>
              <span class="stat-label">Перемоги</span>
            </div>
            <div class="stat-card draws">
              <span class="stat-value">${trainerData.Stats.Draws}</span>
              <span class="stat-label">Нічиї</span>
            </div>
            <div class="stat-card losses">
              <span class="stat-value">${trainerData.Stats.Losses}</span>
              <span class="stat-label">Поразки</span>
            </div>
          </div>
        </div>
      </section>
    `;
  }

  private calculateAge(birthDate: string): number {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const m = today.getMonth() - birth.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    return age;
  }
  private getInfoTrainer(login: string): any {
    
    const trainer = trainers.find(t => t.login === login);
    const teams = trainer ? Teams.filter(team => trainer.Teams.includes(team.name)) : [];
    const organiz = trainer ? organizations.filter(o => trainer.Organizations.includes(o.NameOrganization)) : [];
    var TrainerProfile =
    {
      login: login,
      FirsName: trainer?.FirsName,
      LastName: trainer?.LastName,
      DataBirth: trainer?.DataBirth,
      Teams: teams ?? [],
      Organizations:organiz ?? [],
      Photo: trainer?.Photo ?? "",
      Stats: trainer?.stats ?? undefined
    }
  }
  private getResultIcon(result: string): string {
    const icons: Record<string, string> = {
      'win': '✔',
      'draw': '➖',
      'loss': '✘'
    };
    return icons[result] || '?';
  }
}