import { NotificationKarina } from "./Notification";
import './matchEditModal.css';
export class MatchEditModal {
  private modalContainer: HTMLElement;

  constructor() {
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'edit-modal-container';
    document.body.appendChild(this.modalContainer);
  }

  async show(matchData: {
    id: string;
    currentLocation: string;
    currentDate: string;
    availableLocations: string[];
  }) {
    this.modalContainer.innerHTML = `
      <div class="edit-modal">
        <div class="modal-header">
          <h2>Редагування матчу</h2>
          <button class="close-btn">&times;</button>
        </div>
        
        <form id="editMatchForm" class="edit-form">
          <div class="form-group">
            <label for="matchDate">Дата та час проведення</label>
            <input 
              type="datetime-local" 
              id="matchDate" 
              value="${this.formatDateTimeForInput(matchData.currentDate)}"
              required
            >
          </div>
          
          <div class="form-group">
            <label for="matchLocation">Локація</label>
            <select id="matchLocation" required>
              ${matchData.availableLocations.map(location => `
                <option 
                  value="${location}" 
                  ${location === matchData.currentLocation ? 'selected' : ''}
                >
                  ${location}
                </option>
              `).join('')}
            </select>
          </div>
          
          <div class="form-actions">
            <button type="button" class="btn btn-outline cancel-btn">Скасувати</button>
            <button type="submit" class="btn btn-primary">Зберегти зміни</button>
          </div>
        </form>
      </div>
    `;

    this.addEventListeners(matchData.id);
    this.modalContainer.style.display = 'flex';
  }

  private formatDateTimeForInput(dateTimeString: string): string {
    const date = new Date(dateTimeString);
    const isoString = date.toISOString();
    return isoString.substring(0, isoString.length - 1);
  }

  private addEventListeners(matchId: string) {
    // Закриття модального вікна
    const closeModal = () => this.close();
    
    this.modalContainer.querySelector('.close-btn')?.addEventListener('click', closeModal);
    this.modalContainer.querySelector('.cancel-btn')?.addEventListener('click', closeModal);

    // Відправка форми
    this.modalContainer.querySelector('#editMatchForm')?.addEventListener('submit', async (e) => {
      e.preventDefault();
      
      const formData = {
        id: matchId,
        date: (document.getElementById('matchDate') as HTMLInputElement).value,
        location: (document.getElementById('matchLocation') as HTMLSelectElement).value
      };

      try {
        // Тут буде запит до API
        const response = await fetch('/api/matches/update', {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(formData)
        });

        if (response.ok) {
          const notification = new NotificationKarina();
          notification.show('Зміни успішно збережено!','success');
          this.close();
          location.reload(); // Оновити сторінку для відображення змін
        } else {
          throw new Error('Помилка при збереженні змін');
        }
      } catch (error) {
        console.error('Error:', error);
        const notification = new NotificationKarina();
        notification.show('Сталася помилка при збереженні змін','error');
      }
    });
  }

  close() {
    this.modalContainer.style.display = 'none';
  }
}