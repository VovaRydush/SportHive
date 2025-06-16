import { RegistrationModal } from './components/RegistrationModal';
import { LoginModal } from './components/LoginModalWin';
import { UserProfile } from './components/AthleteProfile';
import { OrganizationProfile } from './components/OrganizationProfile';
import { TrainerProfile } from './components/TrainerProfile';
import { MatchPage } from './components/MatchPage';
import { HomePage } from './components/HomePage';
import { JudgeModal } from './components/JudgeOperate';
import { CreateTeamModal } from './components/CreateTeam';
import { MatchEditModal } from './components/MatchEditModal';
import { PlayerPositionModal } from './components/ChangePosition';
import { CreateEventPage } from './components/EventPage';
import { TeamPageLook } from './components/TeamPage';
import { DeleteConfirmationModal } from './components/ModalConfig';
import { EventPageLook } from './components/EventPageLook';

document.addEventListener('DOMContentLoaded', () => {


    const nav = document.getElementById('nav-buttons');
    if (!nav) return;

    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('userRole');

    if (!token) {

        nav.innerHTML = `
        <button class="nav-btn" id="judge-btn">Управління складом</button>
        <button class="nav-btn" id="teamLook-btn">Команда</button>
        <button class="nav-btn" id="event-btn">Створення заходу</button>
         <button class="nav-btn" id="chenge-btn">Зміна позиції</button>
         <button class="nav-btn" id="rmAthlete-btn">Видалити спортіка</button>
        <button class="nav-btn" id="editmatch-btn">Редагування матчу</button>
        <button class="nav-btn" id="team-btn">Створення команди</button>
        <button class="nav-btn" id="match-btn">Матч</button>
  <button class="nav-btn" id="login-btn">Увійти</button>
  <button class="nav-btn" id="register-btn">Зареєструватись</button>
`;

        document.getElementById('teamLook-btn')?.addEventListener('click', () => {
            const team: any = {}
            const loginModal = new TeamPageLook('app',team);
            loginModal.render();
        });
        document.getElementById('rmAthlete-btn')?.addEventListener('click', () => {
            const loginModal = new DeleteConfirmationModal();
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
        document.getElementById('chenge-btn')?.addEventListener('click', () => {
            // Приклад виклику модального вікна
            const positionModal = new PlayerPositionModal();

            positionModal.show({
                id: 'player123',
                name: 'Олександр Іваненко',
                currentPosition: 'Нападник',
                availablePositions: ['Воротар', 'Захисник', 'Півзахисник', 'Нападник'],
                teamId: 'team456' // Опціонально, якщо гравець у команді
            });
        });
        document.getElementById('editmatch-btn')?.addEventListener('click', () => {
            // Приклад виклику модального вікна
            const editModal = new MatchEditModal();

            editModal.show({
                id: 'match123',
                currentLocation: 'Стадіон "Динамо"',
                currentDate: '2023-12-15T15:00:00',
                availableLocations: [
                    'Стадіон "Динамо"',
                    'Палац спорту',
                    'Спорткомплекс "Олімпійський"'
                ]
            });
        });
        document.getElementById('judge-btn')?.addEventListener('click', () => {
            const loginModal = new JudgeModal();
            loginModal.show("1", "1");
        });
        document.getElementById('match-btn')?.addEventListener('click', () => {
            const loginModal = new MatchPage('app');
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
        localStorage.setItem('userRole', "Organization");
        switch (role) {
            case 'Organization': content = '<button class="nav-btn" id="profil-btn" href="#admin">Профіль Організації</button>';
                break;
            case 'Trainer': content = '<button class="nav-btn" id="profi-btn" href="#dashboard">Мій кабінет</button>';
                break;
            case 'Athlete': content = `<button class="nav-btn" id="profile-btn">Профіль</button>`;
                break;
            default: content = '<button class="nav-btn" id="profile" href="#profile">Профіл</button>';
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