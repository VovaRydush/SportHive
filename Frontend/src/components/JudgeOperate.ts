import { MatchAssignment } from "./MatchMenage";
import { NotificationKarina } from "./Notification";
import { judges, match, organizations } from "./db";
import './judgeOperate.css';
export class JudgeModal {
  private modalContainer: HTMLElement;
  private existingJudges: any[] =[];
  constructor() {
    this.modalContainer = document.createElement('div');
    this.modalContainer.className = 'judge-modal-container';
    document.body.appendChild(this.modalContainer);
  }

  async show(organizationId: string, matchId?: string) {
    await this.findJudge(organizationId);
    this.modalContainer.innerHTML = `
      <div class="judge-modal">
        <div class="modal-header">
          <h2>${matchId ? 'Призначити суддю' : 'Створити нового суддю'}</h2>
          <button class="close-btn">&times;</button>
        </div>
        
        <div class="modal-tabs">
          <button class="tab-btn active" data-tab="create">Новий суддя</button>
          <button class="tab-btn" data-tab="existing">Існуючі судді</button>
        </div>
        
        <div class="tab-content active" data-tab="create">
          <form id="createJudgeForm" class="judge-form">
            <div class="form-group">
              <label for="firstName">Логін</label>
              <input type="text" id="login" required>
            </div>
          <div class="form-group">
              <label for="firstName">Ім'я</label>
              <input type="text" id="firstName" required>
            </div>
            
            <div class="form-group">
              <label for="lastName">Прізвище</label>
              <input type="text" id="lastName" required>
            </div>
            
            <div class="form-group">
              <label for="birthDate">Дата народження</label>
              <input type="date" id="birthDate" required>
            </div>
            
            <div class="form-group">
              <label for="category">Категорія</label>
              <select id="category" required>
                <option value="Міжнародна">Міжнародна</option>
                <option value="Національна">Національна</option>
                <option value="Регіональна">Регіональна</option>
              </select>
            </div>
            
            <div class="form-group">
              <label for="photo">Фото</label>
              <input type="file" id="photo" accept="image/*">
            </div>
            
            <button type="submit" class="btn btn-primary">
              ${matchId ? 'Створити та призначити' : 'Створити суддю'}
            </button>
          </form>
        </div>
        
        <div class="tab-content" data-tab="existing">
          <div class="judges-list">
            ${this.existingJudges.map(judge => `
              <div class="judge-card" data-login="${judge.login}">
                <div class="judge-info">
                  <h3>${judge.FirsName} ${judge.LastName}</h3>
                  <p>Категорія: ${judge.Category}</p>
                </div>
                ${matchId ? `
                  <button class="btn btn-outline assign-btn" id="boo" data-login="${judge.login}">
                    Призначити
                  </button>
                ` : '0'}
              </div>
            `).join('')}
          </div>
        </div>
      </div>
    `;
    document.getElementById('boo')?.addEventListener('click', () => {
      const loginModal = new MatchAssignment('app');
      loginModal.show();
    });
    // Додаємо обробники подій
    this.addEventListeners(organizationId, matchId);
    this.modalContainer.style.display = 'flex';
  }

  private addEventListeners(organizationId: string, matchId?: string) {
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

    // Відправка форми створення судді
    document.getElementById('createJudgeForm')?.addEventListener('submit', async (e) => {
      e.preventDefault();

      const formData = new FormData();
      formData.append('firstName', (document.getElementById('firstName') as HTMLInputElement).value);
      formData.append('lastName', (document.getElementById('lastName') as HTMLInputElement).value);
      formData.append('birthDate', (document.getElementById('birthDate') as HTMLInputElement).value);
      formData.append('category', (document.getElementById('category') as HTMLSelectElement).value);
      formData.append('organizationId', organizationId);

      const photoInput = document.getElementById('photo') as HTMLInputElement;
      if (photoInput.files?.[0]) {
        formData.append('photo', photoInput.files[0]);
      }
      if (matchId) {
        var profilePhoto = (document.getElementById('photo') as HTMLInputElement).files?.[0];
        var base64String;
        var loginJ = (document.getElementById('login') as HTMLInputElement).value;
        if (profilePhoto) {
          const reader = new FileReader();
          reader.onload = function () {
            base64String = reader.result as string;
            localStorage.setItem('userPhoto', base64String);
          };

        }
        judges.push({
          login: loginJ,
          Photo: base64String || "",
          FirsName: (document.getElementById('firstName') as HTMLInputElement).value,
          LastName: (document.getElementById('lastName') as HTMLInputElement).value,
          Category: (document.getElementById('category') as HTMLSelectElement).value
        });
        await this.assignJudgeToMatch(loginJ, matchId);
        await this.addJudgeOrganiz(organizationId, loginJ);
      }
      this.close();

      const notification = new NotificationKarina();
      notification.show('Суддю успішно створено' + (matchId ? ' та призначено' : ''), 'success');
    });

    // Призначення існуючого судді
    document.querySelectorAll('.assign-btn').forEach(btn => {
      btn.addEventListener('click', async () => {
        const judgeLogin = (btn as HTMLElement).dataset.login;
        if (judgeLogin && matchId) {
          await this.assignJudgeToMatch(judgeLogin, matchId);
          await this.addJudgeOrganiz(organizationId, judgeLogin);
          this.close();
        }
      });
    });
  }
  private async findJudge(nameOrganization: string)
  {
    const org = organizations.find(o => o.NameOrganization === nameOrganization);
  if (!org) {
    console.error('Організацію не знайдено');
    return;
  }
  const judgeLogins = org.OrganizationJudge;

  // 3. Знайти суддів за логінами
  this.existingJudges = judges
    .filter(j => judgeLogins.includes(j.login))
    .map(j => ({
      login: j.login,
      FirsName: j.FirsName,
      LastName: j.LastName,
      Category: j.Category
    }));
  }
  private async addJudgeOrganiz(nameOrganization: string, judgeLogin: string) {
    const foundMatch = organizations.find(m => m.NameOrganization === nameOrganization);

    if (!foundMatch) {
      console.error(`Матч з id ${nameOrganization} не знайдено`);
      return;
    }
    foundMatch.OrganizationJudge.push(judgeLogin);
  }
  private async assignJudgeToMatch(judgeLogin: string, matchId: string) {
    const foundMatch = match.find(m => m.idMatch === matchId);

    if (!foundMatch) {
      console.error(`Матч з id ${matchId} не знайдено`);
      return;
    }

    foundMatch.loginJudge = judgeLogin;
    /*
    try {
      const response = await fetch(`/api/matches/${matchId}/assign-judge`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ judgeLogin })
      });

      if (!response.ok) {
        throw new Error('Помилка при призначенні судді');
      }
    } catch (error) {
      console.error('Error:', error);
      throw error;
    }*/
  }

  close() {
    this.modalContainer.style.display = 'none';
  }
}