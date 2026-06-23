import { LoginModal } from './LoginModalWin';
import { RegistrationModal } from './RegistrationModal';
import { HomePage } from './HomePage';
import { UserProfile } from './AthleteProfile';
import { TrainerProfile } from './TrainerProfile';
import { OrganizationProfile } from './OrganizationProfile';
import { CreateEventPage } from './EventPage';
import { CreateTeamModal } from './CreateTeam';
import { JudgeModal } from './JudgeOperate';
import { TournamentsPage } from './TournamentsPage';
import { StatisticsDashboard } from './StatisticsDashboard';

export class NavigationManager {
  private navElement: HTMLElement | null;

  constructor(navId: string = 'nav-buttons') {
    this.navElement = document.getElementById(navId);
  }

  public init() {
    if (!this.navElement) return;

    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('userRole') || localStorage.getItem('role');

    if (!token) {
      this.renderGuestNav();
    } else {
      this.renderUserNav(role);
    }

    new HomePage('app').render();
  }

  private renderGuestNav() {
    this.navElement!.innerHTML = `
      <button class="nav-btn" id="homePagest">Головна</button>
      <button class="nav-btn" id="tournaments-btn">Турніри</button>
      <button class="nav-btn" id="statistics-btn">Статистика</button>
      <button class="nav-btn" id="login-btn">Увійти</button>
      <button class="nav-btn" id="register-btn">Зареєструватись</button>
    `;

    document.getElementById('homePagest')?.addEventListener('click', () => new HomePage('app').render());
    document.getElementById('tournaments-btn')?.addEventListener('click', () => new TournamentsPage('app').render());
    document.getElementById('statistics-btn')?.addEventListener('click', () => new StatisticsDashboard('app', 'global').render());
    document.getElementById('login-btn')?.addEventListener('click', () => new LoginModal().show());
    document.getElementById('register-btn')?.addEventListener('click', () => new RegistrationModal());
  }

  private renderUserNav(role: string | null) {
    let content = `

      <button class="nav-btn" id="tournaments-btn">Турніри</button>
      <button class="nav-btn" id="statistics-btn">Статистика</button>
    `;

    switch (role) {
      case 'Organization':
        content += `
          <button class="nav-btn" id="org-statistics-btn">Статистика організації</button>
          <button class="nav-btn" id="event-btn">Створити захід</button>
          <button class="nav-btn" id="profil-btn">Профіль Організації</button>
        `;
        break;
      case 'Trainer':
        content += `
          <button class="nav-btn" id="team-btn">Створити команду</button>
          <button class="nav-btn" id="profi-btn">Мій кабінет</button>
        `;
        break;
      case 'Athlete':
      case 'Judge':
        content += `<button class="nav-btn" id="profile-btn">Профіль</button>`;
        break;
      default:
        content += `<button class="nav-btn">N/A</button>`;
    }

    this.navElement!.innerHTML = `
      ${content}
      <button class="nav-btn" id="logout-btn">Вийти</button>
    `;

    const login = localStorage.getItem('login') || '';

    document.getElementById('homePagest')?.addEventListener('click', () => new HomePage('app').render());
    document.getElementById('tournaments-btn')?.addEventListener('click', () => new TournamentsPage('app').render());
    document.getElementById('statistics-btn')?.addEventListener('click', () => new StatisticsDashboard('app', 'global').render());
    document.getElementById('org-statistics-btn')?.addEventListener('click', () => new StatisticsDashboard('app', 'organization', login).render());
    document.getElementById('judge-btn')?.addEventListener('click', () => new JudgeModal().show('1','5'));
    document.getElementById('team-btn')?.addEventListener('click', () => new CreateTeamModal().show());
    document.getElementById('profile-btn')?.addEventListener('click', () => new UserProfile('app', login).render());
    document.getElementById('profi-btn')?.addEventListener('click', () => new TrainerProfile('app', login).render());
    document.getElementById('profil-btn')?.addEventListener('click', () => new OrganizationProfile('app').render());
    document.getElementById('event-btn')?.addEventListener('click', () => new CreateEventPage('app').render());

    document.getElementById('logout-btn')?.addEventListener('click', () => {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('userRole');
      localStorage.removeItem('role');
      localStorage.removeItem('login');
      location.reload();
    });
  }
}
