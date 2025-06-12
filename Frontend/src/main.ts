import { RegistrationModal } from './components/RegistrationModal';
import { LoginModal } from './components/LoginModalWin';
import { UserProfile } from './components/AthleteProfile';

document.addEventListener('DOMContentLoaded', () => {
    const nav = document.getElementById('nav-buttons');
    if (!nav) return;

    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('userRole');

    if (!token) {

        nav.innerHTML = `
  <button class="nav-btn" id="login-btn">Увійти</button>
  <button class="nav-btn" id="register-btn">Зареєструватись</button>
`;

        // Додаємо обробники тільки після того, як кнопки зʼявилися в DOM
        document.getElementById('login-btn')?.addEventListener('click', () => {
            localStorage.setItem("userRole", "Athlete");
            const loginModal = new LoginModal();
            loginModal.show();
        });

        document.getElementById('register-btn')?.addEventListener('click', () => {
            new RegistrationModal();
        });
    }

    else {
        let content = '';
        console.log(role);
        switch (role) {
            case 'Organization': content = '<button class="nav-btn" href="#admin">Адмін-панель</button>';
                break;
            case 'Trainer': content = '<button class="nav-btn" href="#dashboard">Мій кабінет</button>';
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
                btn.addEventListener('click', () => {
                    console.log("vfvf");
                    const profile = new UserProfile("app");
                    profile.render();
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
});