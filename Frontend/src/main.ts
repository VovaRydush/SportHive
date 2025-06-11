import { RegistrationModal } from './components/RegistrationModal';


document.addEventListener('DOMContentLoaded', () => {
    const nav = document.getElementById('nav-buttons');
    if (!nav) return;

    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('userRole');

    if (!token) {
        // Не авторизований — показати кнопки входу/реєстрації
        nav.innerHTML = ` <button class="btn" id="login-btn" >Увійти</button> <button class="btn" id="register-btn" >Зареєструватись</button> `;

        document.getElementById('login-btn')?.addEventListener('click', () => {
            // Показати модальне вікно входу (додамо пізніше)
            console.log('login');
        });

        document.getElementById('register-btn')?.addEventListener('click', () => {
            new RegistrationModal();
        });
    }

    else {
        // Авторизований — показати ім'я або меню користувача за роллю
        let content = '';

        switch (role) {
            case 'admin': content = '<a class="btn" href="#admin">Адмін-панель</a>';
                break;
            case 'organizer': content = '<a class="btn" href="#dashboard">Мій кабінет</a>';
                break;
            case 'user': default: content = '<a class="btn" href="#profile">Профіль</a>';
                break;
        }

        nav.innerHTML = `
  ${content}
  <button class="btn" id="logout-btn">Вийти</button>
`;


        document.getElementById('logout-btn')?.addEventListener('click', () => {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('userRole');
            location.reload();
        });
    }
});