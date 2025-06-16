import { NotificationKarina } from "./Notification";
import './createMatch.css'
export class CreateMatchPage {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    // Мок дані для прикладу
    const data = {
      teams: [
        { id: "team1", name: "Динамо", sportType: "Футбол" },
        { id: "team2", name: "Скіфи", sportType: "Футбол" }
      ],
      judges: [
        { id: "judge1", name: "Іван Петренко", category: "Міжнародна" },
        { id: "judge2", name: "Олена Сидорова", category: "Національна" }
      ],
      events: [
        { id: "event1", name: "Чемпіонат міста 2023" },
        { id: "event2", name: "Кубок ліги" }
      ],
      systems: [
        { value: "RoundRobin", name: "Круговий турнір" },
        { value: "PlayOff", name: "Плейоф" },
        { value: "GroupStage", name: "Груповий етап" }
      ],
      sports: ["Футбол", "Баскетбол", "Волейбол"]
    };

    this.container.innerHTML = `
      <section class="create-match-page">
        <h1>Створення нового матчу</h1>
        
        <form id="createMatchForm" class="match-form">
          <div class="form-section">
            <h2>Основні параметри</h2>
            
            <div class="form-group">
              <label for="sportType">Вид спорту</label>
              <select id="sportType" required>
                ${data.sports.map(sport => `
                  <option value="${sport}">${sport}</option>
                `).join('')}
              </select>
            </div>
            
            <div class="form-group">
              <label for="event">Подія/Турнір</label>
              <select id="event" required>
                <option value="">Виберіть подію</option>
                ${data.events.map(event => `
                  <option value="${event.id}">${event.name}</option>
                `).join('')}
              </select>
            </div>
            
            <div class="form-group">
              <label for="system">Система проведення</label>
              <select id="system" required>
                ${data.systems.map(sys => `
                  <option value="${sys.value}">${sys.name}</option>
                `).join('')}
              </select>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Учасники</h2>
            
            <div class="form-group">
              <label for="team1">Команда 1</label>
              <select id="team1" required>
                <option value="">Виберіть команду</option>
                ${data.teams.map(team => `
                  <option value="${team.id}" data-sport="${team.sportType}">${team.name}</option>
                `).join('')}
              </select>
            </div>
            
            <div class="form-group">
              <label for="team2">Команда 2</label>
              <select id="team2" required>
                <option value="">Виберіть команду</option>
                ${data.teams.map(team => `
                  <option value="${team.id}" data-sport="${team.sportType}">${team.name}</option>
                `).join('')}
              </select>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Дата та місце</h2>
            
            <div class="form-group">
              <label for="matchDate">Дата проведення</label>
              <input type="date" id="matchDate" required>
            </div>
            
            <div class="form-group">
              <label for="matchTime">Час проведення</label>
              <input type="time" id="matchTime" required>
            </div>
            
            <div class="form-group">
              <label for="location">Місце проведення</label>
              <input type="text" id="location" required>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Суддівство</h2>
            
            <div class="form-group">
              <label for="judge">Головний суддя</label>
              <select id="judge" required>
                <option value="">Виберіть суддю</option>
                ${data.judges.map(judge => `
                  <option value="${judge.id}">${judge.name} (${judge.category})</option>
                `).join('')}
              </select>
            </div>
          </div>
          
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">Створити матч</button>
          </div>
        </form>
      </section>
    `;

    this.addEventListeners();
  }

  private addEventListeners() {
    // Фільтрація команд за видом спорту
    const sportTypeSelect = document.getElementById('sportType') as HTMLSelectElement;
    sportTypeSelect.addEventListener('change', () => {
      const selectedSport = sportTypeSelect.value;
      this.filterTeamsBySport(selectedSport);
    });

    // Відправка форми
    const form = document.getElementById('createMatchForm') as HTMLFormElement;
    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      
      const formData = {
        team1Id: (document.getElementById('team1') as HTMLSelectElement).value,
        team2Id: (document.getElementById('team2') as HTMLSelectElement).value,
        sportType: (document.getElementById('sportType') as HTMLSelectElement).value,
        eventId: (document.getElementById('event') as HTMLSelectElement).value,
        system: (document.getElementById('system') as HTMLSelectElement).value,
        date: (document.getElementById('matchDate') as HTMLInputElement).value,
        time: (document.getElementById('matchTime') as HTMLInputElement).value,
        location: (document.getElementById('location') as HTMLInputElement).value,
        judgeId: (document.getElementById('judge') as HTMLSelectElement).value
      };

      try {
        const response = await fetch('/api/matches/create', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(formData)
        });

        if (response.ok) {
          const notification = new NotificationKarina();
          notification.show('Матч успішно створено!','success');

          form.reset();
        } else {
          throw new Error('Помилка при створенні матчу');
        }
      } catch (error) {
        console.error('Error:', error);
        const notification = new NotificationKarina();
        notification.show('Сталася помилка при створенні матчу','error');
      }
    });
  }

  private filterTeamsBySport(sportType: string) {
    const team1Select = document.getElementById('team1') as HTMLSelectElement;
    const team2Select = document.getElementById('team2') as HTMLSelectElement;
    
    // Оновити список команд для обох виборів
    [team1Select, team2Select].forEach(select => {
      Array.from(select.options).forEach(option => {
        if (option.value === "") return;
        option.style.display = option.dataset.sport === sportType ? "" : "none";
      });
      
      // Скинути вибір, якщо команда не підходить
      if (select.value && (select.selectedOptions[0] as HTMLOptionElement).dataset.sport !== sportType) {
        select.value = "";
      }
    });
  }
}
