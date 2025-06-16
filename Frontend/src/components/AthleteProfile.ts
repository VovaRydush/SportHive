import { TeamIndivMatchRes } from '../logic/TeamIndivMatch';
import './athleteProf.css'
import { UserTeamMatchRender } from './matchinfo/UserTeamMatchRender';
import { AmericanFootballRender } from './sportsInfo/AmericanFootball';
import { BasketballStatsRenderer } from './sportsInfo/BasketballRender';
import { BoxingStatsRenderer } from './sportsInfo/BoxingRender';
import { CheckersChessStatsRenderer } from './sportsInfo/CheckersChessRender';
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
  private login:string;
  constructor(containerId: string,Login:string) {
    this.login = Login;
    const element = document.getElementById(containerId);
    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }
    this.container = element;
  }

  async render() {
    let matches = new UserTeamMatchRender();
    const photoUrl = await this.getUserPhoto(this.login);
    const athleteInfo = await this.getUserInfo(this.login);
    const userMatch = await this.getUserTeamMatchs(this.login);
    //const matchesUser = JSON.parse(userMatch);
    const obj = JSON.parse(athleteInfo);
    console.log(userMatch);
    let statsRenderer: ISportStatsRenderer;
    switch (obj.SportType) {
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
      default:
        statsRenderer = new FootballStatsRenderer();
    }

    this.container.innerHTML = `
      <section class="user-profile">
      <div class="profile-header">
        <div class="profile-photo">
          <img src="${photoUrl}" alt="Фото спортсмена">
          <div class="sport-badge"> ${obj.SportType}</div>
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
                <p><strong>Позиція:</strong> ${obj.Position || "Не відомо"}</p>
                <p><strong>Команда:</strong> ${obj.Team || "Не відомо"}</p>
              </div>
            </div>
          </div>
        </div>
        <div class="stats-section">
        ${statsRenderer.renderStats(obj.SportStats)}
        </div>
      ${matches.render(userMatch.matches)}
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
  private async getUserTeamMatchs(login: string): Promise<any> {
    try {
      let token = localStorage.getItem('accessToken');
      if (token) {
        token = token.replace(/^"(.+)"$/, '$1'); // прибирає лапки, якщо є
      }
      const response = await fetch(`http://localhost:5042/get-team-matchs/${login}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error('Failed to fetch');
      }

      return await response.json();
    } catch (error) {
      console.error('Error fetching user matches:', error);
      return [];
    }
  }

}
