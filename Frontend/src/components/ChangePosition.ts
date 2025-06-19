import { NotificationKarina } from "./Notification";
import './changepos.css'
export class PlayerPositionModal {
  private modalContainer: HTMLElement;

  constructor() {
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'position-modal-container';
    document.body.appendChild(this.modalContainer);
  }

  async show(player: {
    id: string;
    name: string;
    currentPosition: string;
    availablePositions: string[];
    teamId?: string;
  }) {
    this.modalContainer.innerHTML = `
      <div class="position-modal">
        <div class="modal-header">
          <h2>Зміна позиції гравця</h2>
          <button class="close-btn">&times;</button>
        </div>
        
        <div class="player-info">
          <h3>${player.name}</h3>
          <p>Поточна позиція: <strong>${player.currentPosition}</strong></p>
        </div>
        
        <form id="positionForm" class="position-form">
          <div class="form-group">
            <label for="newPosition">Нова позиція</label>
            <select id="newPosition" required>
              ${player.availablePositions.map(pos => `
                <option value="${pos}" ${pos === player.currentPosition ? 'selected' : ''}>
                  ${pos}
                </option>
              `).join('')}
            </select>
          </div>
          
          ${player.teamId ? `
            <div class="form-group">
              <label class="checkbox-label">
                <input type="checkbox" id="updateTeamPosition">
                Оновити позицію у складі команди
              </label>
            </div>
          ` : ''}
          
          <div class="form-actions">
            <button type="button" class="btn btn-outline cancel-btn">Скасувати</button>
            <button type="submit" class="btn btn-primary">Зберегти зміни</button>
          </div>
        </form>
      </div>
    `;

    this.addEventListeners(player);
    this.modalContainer.style.display = 'flex';
  }

  private addEventListeners(player: {
    id: string;
    name: string;
    currentPosition: string;
    availablePositions: string[];
    teamId?: string;
  }) {
    // Закриття модального вікна
    const closeModal = () => this.close();
    
    this.modalContainer.querySelector('.close-btn')?.addEventListener('click', closeModal);
    this.modalContainer.querySelector('.cancel-btn')?.addEventListener('click', closeModal);

    // Відправка форми
    this.modalContainer.querySelector('#positionForm')?.addEventListener('submit', async (e) => {
      e.preventDefault();
      
      const newPosition = (document.getElementById('newPosition') as HTMLSelectElement).value;
      const updateTeamPosition = player.teamId 
        ? (document.getElementById('updateTeamPosition') as HTMLInputElement).checked 
        : false;

      const requestData = {
        playerId: player.id,
        newPosition,
        ...(player.teamId && { teamId: player.teamId, updateTeamPosition })
      };

      try {
        const response = await fetch('/api/players/update-position', {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(requestData)
        });

        if (response.ok) {
          const notification = new NotificationKarina();
          notification.show(`Позицію гравця ${player.name} успішно змінено на ${newPosition}`,'success');
          this.close();
          location.reload(); // Оновити сторінку для відображення змін
        } else {
          throw new Error('Помилка при зміні позиції');
        }
      } catch (error) {
        const notification = new NotificationKarina();
        notification.show('Сталася помилка при зміні позиції','error');
        console.error('Error:', error);
      }
    });
  }
  close() {
    this.modalContainer.style.display = 'none';
  }
}


