import { NotificationKarina } from "./Notification";
import './createTeam.css';
export class CreateTeamModal {
  private modalContainer: HTMLElement;

  constructor() {
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'team-modal-container';
    document.body.appendChild(this.modalContainer);
  }

  async show() {
    // Мок дані спортсменів
    const athletes = [
      { login: "athlete1", name: "Олександр Іваненко", sport: "Футбол" },
      { login: "athlete2", name: "Марія Петренко", sport: "Баскетбол" },
      { login: "athlete3", name: "Іван Сидоренко", sport: "Футбол" }
    ];

    this.modalContainer.innerHTML = `
      <div class="team-modal">
        <div class="modal-header">
          <h2>Створити нову команду</h2>
          <button class="close-btn">&times;</button>
        </div>
        
        <form id="createTeamForm" class="team-form">
          <div class="form-group">
            <label for="teamName">Назва команди</label>
            <input type="text" id="teamName" required>
          </div>
          
          <div class="form-group">
            <label for="sportType">Вид спорту</label>
            <select id="sportType" required>
              <option value="Футбол">Футбол</option>
              <option value="Баскетбол">Баскетбол</option>
              <option value="Волейбол">Волейбол</option>
            </select>
          </div>
          
          <div class="form-group">
            <label for="teamPhoto">Логотип команди</label>
            <input type="file" id="teamPhoto" accept="image/*">
            <div class="photo-preview" id="photoPreview"></div>
          </div>
          
          <div class="form-group">
            <label>Пошук спортсменів</label>
            <div class="search-box">
              <input type="text" id="athleteSearch" placeholder="Введіть ім'я або логін">
              <button type="button" class="search-btn">Пошук</button>
            </div>
          </div>
          
          <div class="athletes-list">
            <h3>Доступні спортсмени</h3>
            <div class="athletes-grid" id="availableAthletes">
              ${athletes.map(athlete => `
                <div class="athlete-card" data-login="${athlete.login}">
                  <label>
                    <input type="checkbox" name="selectedAthletes" value="${athlete.login}">
                    <span>${athlete.name} (${athlete.sport})</span>
                  </label>
                </div>
              `).join('')}
            </div>
          </div>
          
          <div class="selected-athletes">
            <h3>Обрані спортсмени</h3>
            <div class="selected-list" id="selectedAthletesList"></div>
          </div>
          
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">Створити команду</button>
          </div>
        </form>
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

    // Попередній перегляд фото
    const photoInput = this.modalContainer.querySelector('#teamPhoto') as HTMLInputElement;
    const photoPreview = this.modalContainer.querySelector('#photoPreview') as HTMLElement;
    
    photoInput.addEventListener('change', (e) => {
      const file = (e.target as HTMLInputElement).files?.[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = (event) => {
          photoPreview.innerHTML = `<img src="${event.target?.result}" alt="Попередній перегляд">`;
        };
        reader.readAsDataURL(file);
      }
    });

    // Пошук спортсменів
    const searchInput = this.modalContainer.querySelector('#athleteSearch') as HTMLInputElement;
    const searchBtn = this.modalContainer.querySelector('.search-btn') as HTMLElement;
    
    searchBtn.addEventListener('click', () => {
      this.searchAthletes(searchInput.value);
    });

    // Вибір спортсменів
    const checkboxes = this.modalContainer.querySelectorAll('input[name="selectedAthletes"]');
    checkboxes.forEach(checkbox => {
      checkbox.addEventListener('change', (e) => {
        this.updateSelectedAthletes();
      });
    });

    // Відправка форми
    const form = this.modalContainer.querySelector('#createTeamForm') as HTMLFormElement;
    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      
      const formData = new FormData();
      formData.append('nameTeam', (document.getElementById('teamName') as HTMLInputElement).value);
      formData.append('typeSport', (document.getElementById('sportType') as HTMLSelectElement).value);
      
      const photoInput = document.getElementById('teamPhoto') as HTMLInputElement;
      if (photoInput.files?.[0]) {
        formData.append('photo', photoInput.files[0]);
      }

      const selectedAthletes = Array.from(
  document.querySelectorAll<HTMLInputElement>('input[name="selectedAthletes"]:checked')
).map(el => el.value);
      
      formData.append('athletes', JSON.stringify(selectedAthletes));

      try {
        // Тут буде запит до API
        const response = await fetch('/api/teams/create', {
          method: 'POST',
          body: formData
        });

        if (response.ok) {
          const notification = new NotificationKarina();
          notification.show('Команду успішно створено!','success');
          this.close();
        } else {
          throw new Error('Помилка при створенні команди');
        }
      } catch (error) {
        console.error('Error:', error);
        const notification = new NotificationKarina();
        notification.show('Сталася помилка при створенні команди','error');
      }
    });
  }

  private async searchAthletes(query: string) {
    // Тут буде запит до API для пошуку спортсменів
    console.log('Пошук спортсменів:', query);
    // Оновити список availableAthletes
  }

  private updateSelectedAthletes() {
    const selectedList = this.modalContainer.querySelector('#selectedAthletesList') as HTMLElement;
    const selected = Array.from(
     document.querySelectorAll('input[name="selectedAthletes"]:checked') as NodeListOf<HTMLInputElement>
).map(el => {
  const card = el.closest('.athlete-card') as HTMLElement;
  return card.querySelector('span')?.textContent || '';
});

    selectedList.innerHTML = selected.map(name => `
      <div class="selected-athlete">${name}</div>
    `).join('') || '<p>Не обрано жодного спортсмена</p>';
  }

  close() {
    this.modalContainer.style.display = 'none';
  }
}

