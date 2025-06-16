import { NotificationKarina } from "./Notification";
import './matchMenage.css'
export class MatchAssignment {
  private modalContainer: HTMLElement;
  private matchId: string;

  constructor(matchId: string) {
    this.matchId = matchId;
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'assignment-modal-container';
    document.body.appendChild(this.modalContainer);
  }

  async show() {
    // Мок дані для прикладу
    const matchDetails = {
      team1: "Динамо",
      team2: "Скіфи",
      date: "2023-11-20T15:00:00",
      location: "Стадіон 'Динамо'"
    };

    const availableJudges = [
      { login: "judge1", name: "Іван Петренко", category: "Міжнародна" },
      { login: "judge2", name: "Олена Сидорова", category: "Національна" }
    ];

    const team1Players = [
      { login: "player1", name: "Олександр Іваненко", position: "Нападник" },
      { login: "player2", name: "Михайло Коваль", position: "Воротар" }
    ];

    const team2Players = [
      { login: "player3", name: "Андрій Шевченко", position: "Захисник" },
      { login: "player4", name: "Сергій Ребров", position: "Півзахисник" }
    ];

    this.modalContainer.innerHTML = `
      <div class="assignment-modal">
        <div class="modal-header">
          <h2>Призначення на матч</h2>
          <button class="close-btn">&times;</button>
        </div>

        <div class="match-info">
          <h3>${matchDetails.team1} vs ${matchDetails.team2}</h3>
          <p>${new Date(matchDetails.date).toLocaleDateString()} • ${matchDetails.location}</p>
        </div>

        <div class="assignment-tabs">
          <button class="tab-btn active" data-tab="judges">Судді</button>
          <button class="tab-btn" data-tab="team1">${matchDetails.team1}</button>
          <button class="tab-btn" data-tab="team2">${matchDetails.team2}</button>
        </div>

        <div class="tab-content active" data-tab="judges">
          <h3>Призначити суддю</h3>
          <div class="judges-list">
            ${availableJudges.map(judge => `
              <div class="judge-card">
                <div class="judge-info">
                  <h4>${judge.name}</h4>
                  <p>Категорія: ${judge.category}</p>
                </div>
                <button class="btn assign-btn" data-login="${judge.login}">
                  Призначити
                </button>
              </div>
            `).join('')}
          </div>
        </div>

        <div class="tab-content" data-tab="team1">
          <h3>Склад ${matchDetails.team1}</h3>
          <div class="players-list">
            ${team1Players.map(player => `
              <div class="player-card">
                <div class="player-info">
                  <h4>${player.name}</h4>
                  <p>Позиція: ${player.position}</p>
                </div>
                <button class="btn assign-btn" data-login="${player.login}">
                  Додати до матчу
                </button>
              </div>
            `).join('')}
          </div>
        </div>

        <div class="tab-content" data-tab="team2">
          <h3>Склад ${matchDetails.team2}</h3>
          <div class="players-list">
            ${team2Players.map(player => `
              <div class="player-card">
                <div class="player-info">
                  <h4>${player.name}</h4>
                  <p>Позиція: ${player.position}</p>
                </div>
                <button class="btn assign-btn" data-login="${player.login}">
                  Додати до матчу
                </button>
              </div>
            `).join('')}
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-primary save-btn">Зберегти зміни</button>
        </div>
      </div>
    `;

    this.addEventListeners();
    this.modalContainer.style.display = 'flex';
  }

  private addEventListeners() {
    // Закриття модального вікна
    this.modalContainer.querySelector('.close-btn')?.addEventListener('click', () => {
      this.close();
    });

    // Переключення вкладок
    document.querySelectorAll('.tab-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const tab = (e.target as HTMLElement).dataset.tab;
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
        
        (e.target as HTMLElement).classList.add('active');
        document.querySelector(`.tab-content[data-tab="${tab}"]`)?.classList.add('active');
      });
    });

    // Призначення судді/гравця
    document.querySelectorAll('.assign-btn').forEach(btn => {
      btn.addEventListener('click', async (e) => {
        const login = (e.target as HTMLElement).dataset.login;
        const tabContent = (e.target as HTMLElement).closest('.tab-content');
const tab = (tabContent as HTMLElement)?.dataset.tab;
        
        if (login) {
          try {
            let endpoint = '';
            let successMessage = '';
            
            if (tab === 'judges') {
              endpoint = `/api/matches/${this.matchId}/assign-judge`;
              successMessage = 'Суддю успішно призначено';
            } else if (tab === 'team1' || tab === 'team2') {
              endpoint = `/api/matches/${this.matchId}/assign-player`;
              successMessage = 'Гравця успішно додано';
            }

            const response = await fetch(endpoint, {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ login })
            });

            if (response.ok) {
              const notification = new NotificationKarina();
              notification.show(successMessage,'info');
              (e.target as HTMLElement).textContent = 'Призначено';
              (e.target as HTMLElement).classList.add('assigned');
              (e.target as HTMLButtonElement).disabled = true;
            } else {
              throw new Error('Помилка при призначенні');
            }
          } catch (error) {
            console.error('Error:', error);
            const notification = new NotificationKarina();
            notification.show('Сталася помилка','error');
          }
        }
      });
    });

    // Збереження змін
    document.querySelector('.save-btn')?.addEventListener('click', () => {
      this.close();
      const notification = new NotificationKarina();
      notification.show('Усі зміни збережено','success');
    });
  }

  close() {
    this.modalContainer.style.display = 'none';
  }
}

