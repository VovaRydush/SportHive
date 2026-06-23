import { LoginModal } from './LoginModalWin';
import { RegistrationModal } from './RegistrationModal';
import { HomePage } from './HomePage';
import { OrganizationProfile } from './OrganizationProfile';
import { CreateEventPage } from './EventPage';
import { CreateTeamModal } from './CreateTeam';
import { TournamentsPage } from './TournamentsPage';
import { UserProfilePage } from './UserProfile';
import { clearAuthStorage, getAccessToken, getCurrentRole } from '../api/authToken';

export class NavigationManager {
  private navElement: HTMLElement | null;

  constructor(navId: string = 'nav-buttons') {
    this.navElement = document.getElementById(navId);
  }

  public init() {
    if (!this.navElement) return;

    if (!getAccessToken()) this.renderGuestNav();
    else this.renderUserNav();

    new HomePage('app').render();
  }

  private button(id: string, title: string) {
    return `<button class="nav-btn" id="${id}" type="button">${title}</button>`;
  }

  private renderGuestNav() {
    this.navElement!.innerHTML = [
     
      this.button('tournaments-btn', 'Турніри'),
      this.button('login-btn', 'Увійти'),
      this.button('register-btn', 'Зареєструватись'),
    ].join('');

    document.getElementById('homePagest')?.addEventListener('click', () => new HomePage('app').render());
    document.getElementById('tournaments-btn')?.addEventListener('click', () => new TournamentsPage('app').render());
    document.getElementById('login-btn')?.addEventListener('click', () => new LoginModal().show());
    document.getElementById('register-btn')?.addEventListener('click', () => new RegistrationModal());
  }

  private renderUserNav() {
    const role = getCurrentRole();
    const items: string[] = [this.button('tournaments-btn', 'Турніри')];

    if (role === 'Organization') {
      items.push(this.button('event-btn', 'Створити захід'));
      items.push(this.button('profile-btn', 'Профіль організації'));
    }

    if (role === 'Trainer') {
      items.push(this.button('team-btn', 'Створити команду'));
      items.push(this.button('profile-btn', 'Мій профіль'));
    }

    if (role === 'Athlete' || role === 'Judge') {
      items.push(this.button('profile-btn', 'Мій профіль'));
    }

    items.push(this.button('logout-btn', 'Вийти'));

    this.navElement!.innerHTML = items.join('');

    document.getElementById('homePagest')?.addEventListener('click', () => new HomePage('app').render());
    document.getElementById('tournaments-btn')?.addEventListener('click', () => new TournamentsPage('app').render());
    document.getElementById('team-btn')?.addEventListener('click', () => new CreateTeamModal().show());
    document.getElementById('event-btn')?.addEventListener('click', () => new CreateEventPage('app').render());
    document.getElementById('profile-btn')?.addEventListener('click', () => {
      if (role === 'Organization') new OrganizationProfile('app').render();
      else new UserProfilePage('app').render();
    });
    document.getElementById('logout-btn')?.addEventListener('click', () => {
      clearAuthStorage();
      location.reload();
    });
  }
}
