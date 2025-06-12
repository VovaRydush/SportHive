import './athleteProf.css'
export class UserProfile {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  render() {
    this.container.innerHTML = `
      <section class="user-profile">
        <div class="profile-header">
          <div class="profile-photo">
            <img src="https://images.unsplash.com/photo-1508214751196-bcfd4ca60f91?auto=format&fit=crop&w=200&q=80" alt="Фото спортсмена">
            <div class="sport-badge">⚽ Футбол</div>
          </div>
          
          <div class="profile-main">
            <h1>Олександр Іваненко</h1>
            <div class="profile-details">
              <div class="detail-block">
                <h3>Особисті дані</h3>
                <p><strong>Логін:</strong> ivan_sport</p>
                <p><strong>Дата народження:</strong> 12.06.1995</p>
                <p><strong>Місто:</strong> Київ</p>
              </div>
              <div class="detail-block">
                <h3>Контакти</h3>
                <p><strong>Email:</strong> ivan.sport@gmail.com</p>
                <p><strong>Телефон:</strong> +380 99 123 4567</p>
              </div>
            </div>
          </div>
        </div>

        <div class="stats-section">
          <div class="stats-summary">
            <h2>Статистика</h2>
            <div class="stats-grid">
              <div class="stat-card total">
                <span class="stat-value">42</span>
                <span class="stat-label">Матчів</span>
              </div>
              <div class="stat-card wins">
                <span class="stat-value">30</span>
                <span class="stat-label">Перемоги</span>
              </div>
              <div class="stat-card draws">
                <span class="stat-value">7</span>
                <span class="stat-label">Нічиї</span>
              </div>
              <div class="stat-card losses">
                <span class="stat-value">5</span>
                <span class="stat-label">Поразки</span>
              </div>
            </div>
          </div>

          <div class="performance-section">
            <h2>Продуктивність</h2>
            <div class="performance-stats">
              <div class="performance-card">
                <span class="performance-value">24</span>
                <span class="performance-label">Голи</span>
              </div>
              <div class="performance-card">
                <span class="performance-value">18</span>
                <span class="performance-label">Асисти</span>
              </div>
              <div class="performance-card">
                <span class="performance-value">87%</span>
                <span class="performance-label">Точність передач</span>
              </div>
            </div>
          </div>
        </div>

        <div class="matches-section">
          <h2>Останні матчі</h2>
          <div class="matches-list">
            <div class="match-card win">
              <div class="match-result">W</div>
              <div class="match-teams">
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/FC_Barcelona_%28crest%29.svg/50px-FC_Barcelona_%28crest%29.svg.png" alt="Барселона">
                  <span>Барселона</span>
                </div>
                <div class="match-score">3 : 1</div>
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/5/56/Real_Madrid_CF.svg/50px-Real_Madrid_CF.svg.png" alt="Реал Мадрид">
                  <span>Реал Мадрид</span>
                </div>
              </div>
              <div class="match-date">12.05.2025</div>
            </div>

            <div class="match-card loss">
              <div class="match-result">L</div>
              <div class="match-teams">
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/FC_Barcelona_%28crest%29.svg/50px-FC_Barcelona_%28crest%29.svg.png" alt="Барселона">
                  <span>Барселона</span>
                </div>
                <div class="match-score">0 : 2</div>
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/f/f4/Atletico_Madrid_2017_logo.svg/50px-Atletico_Madrid_2017_logo.svg.png" alt="Атлетіко">
                  <span>Атлетіко</span>
                </div>
              </div>
              <div class="match-date">05.05.2025</div>
            </div>

            <div class="match-card draw">
              <div class="match-result">D</div>
              <div class="match-teams">
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/6/6a/FC_Barcelona_%28crest%29.svg/50px-FC_Barcelona_%28crest%29.svg.png" alt="Барселона">
                  <span>Барселона</span>
                </div>
                <div class="match-score">1 : 1</div>
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/c/c3/Sevilla_FC_logo.svg/50px-Sevilla_FC_logo.svg.png" alt="Севілья">
                  <span>Севілья</span>
                </div>
              </div>
              <div class="match-date">30.04.2025</div>
            </div>
          </div>
        </div>
      </section>
    `;
  }
}
