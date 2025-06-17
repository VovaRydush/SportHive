import { RegistrationModal } from './components/RegistrationModal';
import { LoginModal } from './components/LoginModalWin';
import { UserProfile } from './components/AthleteProfile';
import { OrganizationProfile } from './components/OrganizationProfile';
import { TrainerProfile } from './components/TrainerProfile';
import { HomePage } from './components/HomePage';
import { CreateTeamModal } from './components/CreateTeam';
import { CreateEventPage } from './components/EventPage';
import { BoxingMatchEntry } from './EnterResultsPages/BoxDataEntry';

document.addEventListener('DOMContentLoaded', () => {

    const nav = document.getElementById('nav-buttons');
    if (!nav) return;

    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('userRole');

    if (!token) {

        nav.innerHTML = `
            <button class="nav-btn" id="box-btn">Бокс</button>
            <button class="nav-btn" id="login-btn">Увійти</button>
            <button class="nav-btn" id="register-btn">Зареєструватись</button>
`;
        document.getElementById('box-btn')?.addEventListener('click', () => {
            const loginModal = new BoxingMatchEntry('app');
            loginModal.render();
        });
        document.getElementById('team-btn')?.addEventListener('click', () => {
            const loginModal = new CreateTeamModal();
            loginModal.show();
        });
        document.getElementById('event-btn')?.addEventListener('click', () => {
            const loginModal = new CreateEventPage('app');
            loginModal.render();
        });
        document.getElementById('login-btn')?.addEventListener('click', () => {
            const loginModal = new LoginModal();
            loginModal.show();
        });
        document.getElementById('register-btn')?.addEventListener('click', () => {
            new RegistrationModal();
        });
    }

    else {
        let content = '';
        switch (role) {
            case 'Organization': content = `<button class="nav-btn" id="event-btn">Створити захід</button>
            <button class="nav-btn" id="profil-btn" href="#admin">Профіль Організації</button>
            `;
                break;
            case 'Trainer': content = `
            <button class="nav-btn" id="team-btn">Створити команду</button>
            <button class="nav-btn" id="profi-btn" href="#dashboard">Мій кабінет</button>`;
                break;
            case 'Athlete': content = `<button class="nav-btn" id="profile-btn">Профіль</button>`;
                break;
            default: content = '<button class="nav-btn" id="profile" href="#profile">N/A</button>';
                break;
        }

        const nav1 = document.getElementById('nav-buttons');
        nav1!.innerHTML = content;

        // Після вставки кнопки — тепер елемент точно є
        setTimeout(() => {
            const btn = document.getElementById('profile-btn');
            if (btn) {
                btn.addEventListener('click', async () => {
                    const profile = new UserProfile("app");
                    await profile.render();
                });
            }
        }, 0);
        setTimeout(() => {
            const btn = document.getElementById('profi-btn');
            if (btn) {

                btn.addEventListener('click', async () => {
                    const profile = new TrainerProfile("app");
                    await profile.render();
                });
            }
        }, 0);
        setTimeout(() => {
            const btn = document.getElementById('profil-btn');
            if (btn) {
                btn.addEventListener('click', async () => {
                    const profile = new OrganizationProfile("app");
                    await profile.render();
                });
            }
        }, 0);
        nav.innerHTML = `
  ${content}
  <button class="nav-btn" id="logout-btn">Вийти</button>
`;
        document.getElementById('logout-btn')?.addEventListener('click', () => {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('userRole');
            location.reload();
        });
    }
    let homePage = new HomePage('app');
    homePage.render();
});