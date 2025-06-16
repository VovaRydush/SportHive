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
    // Мок дані для прикладу
    const mockData = {
      sports: ["Футбол", "Баскетбол", "Волейбол", "Теніс", "Хокей"],
      selectionSystems: [
        { value: "RoundRobin", name: "Круговий турнір" },
        { value: "PlayOff", name: "Плейоф" },
        { value: "GroupStage", name: "Груповий етап" },
        { value: "SwissSystem", name: "Швейцарська система" },
        { value: "OlympicSystem", name: "Олімпійська система" }
      ],
      participants: [
        { id: "team1", name: "Динамо", type: "team", sport: "Футбол" },
        { id: "team2", name: "Скіфи", type: "team", sport: "Футбол" },
        { id: "athlete1", name: "Олександр Іваненко", type: "athlete", sport: "Футбол" },
        { id: "athlete2", name: "Марія Петренко", type: "athlete", sport: "Баскетбол" }
      ]
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
                <div class="participants-list" id="availableParticipants">
                  ${mockData.participants.map(p => `
                    <div class="participant-card" data-id="${p.id}" data-type="${p.type}">
                      <label>
                        <input type="checkbox" name="participants" value="${p.id}">
                        <span>${p.name} (${p.sport})</span>
                      </label>
                    </div>
                  `).join('')}
                </div>
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

    
  }

}


// Стилі для сторінки
const createEventStyles = document.createElement('style');
createEventStyles.textContent = `
  .create-event-page {
    max-width: 1200px;
    margin: 2rem auto;
    padding: 2rem;
    font-family: 'Montserrat', sans-serif;
    color: #111;
    background: #fff;
    border-radius: 0;
    box-shadow: 0 0 0 1px rgba(0,0,0,0.1);
  }

  .create-event-page h1 {
    font-size: 2.5rem;
    font-weight: 800;
    margin-bottom: 2rem;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    border-bottom: 2px solid #111;
    padding-bottom: 0.5rem;
  }

  .event-form {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .form-section {
    border: 2px solid #111;
    padding: 1.5rem;
  }

  .form-section h2 {
    font-size: 1.5rem;
    font-weight: 700;
    margin: 0 0 1rem 0;
    border-bottom: 1px solid #ddd;
    padding-bottom: 0.5rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
  }

  .form-group label {
    font-weight: 600;
    font-size: 1rem;
  }

  .form-group input,
  .form-group select,
  .form-group textarea {
    padding: 0.8rem;
    border: 2px solid #111;
    font-size: 1rem;
  }

  .form-group textarea {
    resize: vertical;
    min-height: 100px;
  }

  .date-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
  }

  .photo-preview {
    margin-top: 1rem;
  }

  .photo-preview img {
    max-width: 200px;
    max-height: 200px;
    border: 2px solid #111;
  }

  .search-box {
    display: flex;
    gap: 0.5rem;
    margin-bottom: 1rem;
  }

  .search-box input {
    flex: 1;
    padding: 0.8rem;
    border: 2px solid #111;
  }

  .search-box button {
    padding: 0 1.5rem;
    background: #111;
    color: white;
    border: none;
    font-weight: 600;
    cursor: pointer;
  }

  .participants-container {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
  }

  .participants-list {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    max-height: 300px;
    overflow-y: auto;
    padding: 1rem;
    background: #f8f8f8;
    border: 1px solid #ddd;
  }

  .participant-card {
    padding: 0.8rem;
    border: 1px solid #ddd;
    background: white;
  }

  .participant-card label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    cursor: pointer;
  }

  .selected-participant {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.8rem;
    border: 1px solid #111;
    background: white;
  }

  .remove-btn {
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    padding: 0 0.5rem;
  }

  .form-actions {
    display: flex;
    justify-content: flex-end;
  }

  .btn {
    padding: 1rem 2rem;
    font-weight: 700;
    border: 2px solid #111;
    cursor: pointer;
    text-transform: uppercase;
    transition: all 0.2s;
    font-size: 1rem;
  }

  .btn-primary {
    background: #111;
    color: white;
  }

  .btn:hover {
    transform: translateY(-2px);
    box-shadow: 4px 4px 0 0 #111;
  }

  @media (max-width: 900px) {
    .participants-container {
      grid-template-columns: 1fr;
    }
    
    .date-grid {
      grid-template-columns: 1fr;
    }
  }

  @media (max-width: 600px) {
    .create-event-page {
      padding: 1rem;
    }
    
    .search-box {
      flex-direction: column;
    }
  }
`;
document.head.appendChild(createEventStyles);