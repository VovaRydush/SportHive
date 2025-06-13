import './athleteProf.css'
import { AmericanFootballRender } from './sportsInfo/AmericanFootball';
import { BasketballStatsRenderer } from './sportsInfo/BasketballRender';
import { BoxingStatsRenderer } from './sportsInfo/BoxingRender';
import { CheckersChessStatsRenderer } from './sportsInfo/CheckersChessRender';
import { CyclingStatsRenderer } from './sportsInfo/CyclingRender';
import { FootballStatsRenderer } from './sportsInfo/FootballRender';
import { IceHockeyStatsRenderer } from './sportsInfo/IceHockeyRender';
import { ISportStatsRenderer } from './sportsInfo/ISportStatsRenderer';
import { PowerliftingStatsRenderer } from './sportsInfo/PowerliftingRender';
import { RacketSportsStatsRenderer } from './sportsInfo/RacketSportsRender';
import { RowingStatsRenderer } from './sportsInfo/RowingRender';
import { RugbyStatsRenderer } from './sportsInfo/RugbyRender';
import { StruggleStatsRenderer } from './sportsInfo/StruggleRender';
import { SwimmingStatsRenderer } from './sportsInfo/SwimmingRender';
import { VolleyballStatsRenderer } from './sportsInfo/VolleyballRender';
import { WeightliftingStatsRenderer } from './sportsInfo/WeightliftingRender';
export class UserProfile {
  private container: HTMLElement;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    const login = localStorage.getItem('login') ?? "";
    const photoUrl = await this.getUserPhoto(login);
    const athleteInfo = await this.getUserInfo(login);
    const obj = JSON.parse(athleteInfo);
    let statsRenderer : ISportStatsRenderer;
    switch(athleteInfo.SportType) {
      case 'Football':
        statsRenderer = new FootballStatsRenderer();
        break;
      case 'AmericanFootball':
        statsRenderer = new AmericanFootballRender();
        break;
      case 'Basketball':
        statsRenderer = new BasketballStatsRenderer();
        break;
      case 'Box':
        statsRenderer = new BoxingStatsRenderer();
        break;
      case 'Chess':
        statsRenderer = new CheckersChessStatsRenderer();
        break;
      case 'Checkers':
        statsRenderer = new CheckersChessStatsRenderer();
        break;
      case 'Cycling':
        statsRenderer = new CyclingStatsRenderer();
        break;
      case 'Hockey':
        statsRenderer = new IceHockeyStatsRenderer();
        break;
      case 'Powerlifting': 
        statsRenderer = new PowerliftingStatsRenderer();
        break;
      case 'Cort':
        statsRenderer = new RacketSportsStatsRenderer();
        break;
      case 'Rowing':
        statsRenderer = new RowingStatsRenderer();
        break;
      case 'Rugby':
        statsRenderer = new RugbyStatsRenderer();
        break;
      case 'Struggle':
        statsRenderer = new StruggleStatsRenderer();
        break;
      case 'Swimming':
        statsRenderer = new SwimmingStatsRenderer();
        break;
      case 'Volleyball':
        statsRenderer = new VolleyballStatsRenderer();
        break;
      case 'BeachVolleyball':
        statsRenderer = new VolleyballStatsRenderer();
        break;
      case 'Weightlifting':
        statsRenderer = new WeightliftingStatsRenderer();
        break;
      default : 
        statsRenderer = new FootballStatsRenderer();
    }
    
    this.container.innerHTML = `
      <section class="user-profile">
      <div class="profile-header">
        <div class="profile-photo">
          <img src="${photoUrl}" alt="Фото спортсмена">
          <div class="sport-badge">⚽ Футбол</div>
        </div>
          
          <div class="profile-main">
            <h1>${obj.FullName}</h1>
            <div class="profile-details">
              <div class="detail-block">
                <h3>Особисті дані</h3>
                <p><strong>Логін:</strong>${obj.login}</p>
                <p><strong>Дата народження:</strong> 12.06.1995</p>
              </div>
              <div class="detail-block">
                <h3>Статус</h3>
                <p><strong>Позиція:</strong> ${obj.Position}</p>
                <p><strong>Команда:</strong> ${obj.Team || "Не відомо"}</p>
              </div>
            </div>
          </div>
        </div>
        <div class="stats-section">
        ${statsRenderer.renderStats(obj.SportStats)}
        </div>
        <div class="matches-section">
          <h2>Останні матчі</h2>
          <div class="matches-list">
            <div class="match-card win">
              <div class="match-result">W</div>
              <div class="match-teams">
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/ru/thumb/2/24/FC_Barcelona.svg/200px-FC_Barcelona.svg.png" alt="Барселона">
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
                  <img src="https://upload.wikimedia.org/wikipedia/ru/thumb/2/24/FC_Barcelona.svg/200px-FC_Barcelona.svg.png" alt="Барселона">
                  <span>Барселона</span>
                </div>
                <div class="match-score">0 : 2</div>
                <div class="team">
                  <img src="https://www.footballtop.ru/sites/default/files/styles/club_full_200_265/public/photos/clubs/atlytico-madrid.png?itok=6q7wJByq" alt="Атлетіко">
                  <span>Атлетіко</span>
                </div>
              </div>
              <div class="match-date">05.05.2025</div>
            </div>

            <div class="match-card draw">
              <div class="match-result">D</div>
              <div class="match-teams">
                <div class="team">
                  <img src="https://upload.wikimedia.org/wikipedia/ru/thumb/2/24/FC_Barcelona.svg/200px-FC_Barcelona.svg.png" alt="Барселона">
                  <span>Барселона</span>
                </div>
                <div class="match-score">1 : 1</div>
                <div class="team">
                  <img src="https://static.ua-football.com/img/teams/500.png" alt="Севілья">
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
  private async getUserPhoto(login: string): Promise<string> {
    try {
      const response = await fetch(`http://localhost:5154/get-user-photo/${login}`);
      if (!response.ok) {
        throw new Error('Failed to fetch user photo');
      }
      const photoData = await response.json();
      return photoData; // fallback якщо фото немає
    } catch (error) {
      console.error('Error fetching user photo:', error);
      return 'https://via.placeholder.com/200'; // fallback у разі помилки
    }
  }
  private async getUserInfo(login: string): Promise<any> {
    try {
      const response = await fetch(`http://localhost:5154/get-statistic-info/${login}`);
      if (!response.ok) throw new Error('Failed to fetch user info');
      return await response.json();
    } catch (error) {
      console.error('Error fetching user info:', error);
      return null;
    }
  }
}
