import './eventPage.css'
type AthleteSearchResultDto = {
  login: string;
  fullName: string;
  profilePhotoPath: string;
}

export class CreateEventPage {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {

    const mockData = {
      sports: ["Футбол", "Баскетбол", "Волейбол", "Теніс", "Настільний теніс", "Хокей", "Волейбол", "Бокс", "Бородьба"],
      selectionSystems: [
        { value: "RoundRobin", name: "Круговий турнір" },
        { value: "PlayOff", name: "Плейоф" },
        { value: "GroupStage", name: "Груповий етап" },
        { value: "SwissSystem", name: "Швейцарська система" },
        { value: "OlympicSystem", name: "Олімпійська система" }
      ],
    };

    this.container.innerHTML = `
      <section class="create-event-page">
        <h1>Створення нового івенту</h1>
        
        <form id="createEventForm" class="event-form" enctype="multipart/form-data">
          <div class="form-section">
            <h2>Основна інформація</h2>
            
            <div class="form-group">
              <label for="eventName">Назва івенту*</label>
              <input type="text" id="eventName" required maxlength="100">
            </div>
            
            <div class="form-group">
              <label for="sportType">Вид спорту*</label>
              <select id="sportType" required>
                <option value="">Виберіть вид спорту</option>
                ${mockData.sports.map(sport => `
                  <option value="${sport}">${sport}</option>
                `).join('')}
              </select>
            </div>
            
            <div class="form-group">
              <label for="systemType">Система відбору*</label>
              <select id="systemType" required>
                <option value="">Виберіть систему</option>
                ${mockData.selectionSystems.map(sys => `
                  <option value="${sys.value}">${sys.name}</option>
                `).join('')}
              </select>
            </div>
            
            <div class="form-group">
              <label for="eventDescription">Опис івенту</label>
              <textarea id="eventDescription" rows="4" maxlength="300"></textarea>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Дата проведення</h2>
            
            <div class="date-grid">
              <div class="form-group">
                <label for="startDate">Дата початку*</label>
                <input type="date" id="startDate" required>
              </div>
              
              <div class="form-group">
                <label for="endDate">Дата завершення</label>
                <input type="date" id="endDate">
              </div>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Зображення івенту</h2>
            
            <div class="form-group">
              <label for="eventPhoto">Завантажити лого*</label>
              <input type="file" id="eventPhoto" accept="image/*" required>
              <div class="photo-preview" id="photoPreview"></div>
            </div>
          </div>
          
          <div class="form-section">
            <h2>Учасники</h2>
            
            <div class="search-box">
              <input type="text" id="participantSearch" placeholder="Пошук команд або спортсменів...">
              <button type="button" id="searchBtn">Пошук</button>
            </div>
            
            <div class="participants-container">
              <div class="available-participants">
                <h3>Доступні учасники</h3>
                
              </div>
              
              <div class="selected-participants">
                <h3>Обрані учасники</h3>
                <div class="participants-list" id="selectedParticipants"></div>
              </div>
            </div>
          </div>
          
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">Створити івент</button>
          </div>
        </form>
      </section>
    `;
    this.initSearch();
  }
  private initSearch() {
    const searchBtn = document.getElementById('searchBtn') as HTMLButtonElement;
    const searchInput = document.getElementById('participantSearch') as HTMLInputElement;
    const availableParticipants = document.getElementById('availableParticipants') as HTMLDivElement;

    searchBtn.addEventListener('click', async () => {
      const query = searchInput.value.trim();
      if (!query) return;

      try {
        const response = await fetch(`http://localhost:5154/get-search-athlete?fullName=${encodeURIComponent(query)}`);
        const athletes: AthleteSearchResultDto[] = await response.json();

        availableParticipants.innerHTML = '';

        for (const athlete of athletes) {
          const card = document.createElement('div');
          card.className = 'participant-card';
          card.dataset.id = athlete.login;
          card.dataset.type = 'athlete';

          card.innerHTML = `
          <label>
            <input type="checkbox" name="participants" value="${athlete.login}">
            <img src="data:image/jpeg;base64,${athlete.profilePhotoPath}" alt="Photo" class="participant-photo" />
            <span>${athlete.fullName}</span>
          </label>
        `;

          card.querySelector('input')?.addEventListener('change', (e: Event) => {
            const checkbox = e.target as HTMLInputElement;
            this.toggleParticipantSelection(card, checkbox.checked);
          });

          availableParticipants.appendChild(card);
        }

      } catch (error) {
        console.error('Search failed:', error);
      }
    });
  }
  private toggleParticipantSelection(card: HTMLElement, selected: boolean) {
    const selectedContainer = document.getElementById('selectedParticipants')!;
    const clone = card.cloneNode(true) as HTMLElement;

    if (selected) {
      selectedContainer.appendChild(clone);
    } else {
      const id = card.dataset.id;
      const toRemove = selectedContainer.querySelector(`[data-id="${id}"]`);
      if (toRemove) toRemove.remove();
    }
  }


}