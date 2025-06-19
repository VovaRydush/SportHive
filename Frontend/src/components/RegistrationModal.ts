import { organizations, trainers, users } from './db';
import { HomePage } from './HomePage';
import { NotificationKarina } from './Notification';
import './reg.css';

export class RegistrationModal {
    private container: HTMLElement;
    private currentStep = 1;
    private userEmail: string = '';
    private userRole: string = '';
    private Role: string = '';

    constructor() {
        this.container = document.createElement('div');
        this.container.classList.add('modal');
        document.body.appendChild(this.container);
        this.renderStep1();
    }

    private clear() {
        this.container.innerHTML = '';
    }

    private renderStep1() {
        this.clear();
        this.container.innerHTML = `
      <div class="modal-content">
        <h2>Реєстрація</h2>
        <input type="email" placeholder="Email" id="reg-email" />
        <input type="text" placeholder="Логін" id="reg-login" />
        <input type="password" placeholder="Пароль" id="reg-password" />
        <select id="reg-role">
          <option value="" disabled selected>Оберіть роль</option>
          <option value="Organization">Організація</option>
          <option value="Trainer">Тренер</option>
          <option value="Athlete">Спортсмен</option>
        </select>
        <button id="next-step">Далі</button>
      </div>
    `;
        document.getElementById('reg-role')!.addEventListener('change', () => {
            this.Role = (document.getElementById('reg-role') as HTMLSelectElement).value;
            localStorage.setItem('userRole', this.Role);
        });
        this.container.querySelector('#next-step')?.addEventListener('click', async () => {
            const email = (document.getElementById('reg-email') as HTMLInputElement).value.trim();
            const login = (document.getElementById('reg-login') as HTMLInputElement).value.trim();
            localStorage.setItem('login', login);
            localStorage.setItem('email', email);
            const password = (document.getElementById('reg-password') as HTMLInputElement).value.trim();

            if (!email || !login || !password || !this.Role) {
                const notification = new NotificationKarina();
                notification.show('Будь ласка, заповніть всі поля!', 'info');
                return;
            }

            try {
                await this.register({ email, login, password, role: this.Role });
                this.userEmail = email;
                this.userRole = this.Role;
                this.renderStep2();
            } catch (error: any) {
                const notification = new NotificationKarina();
                notification.show(`Помилка: ${error.message}`, 'error');
            }
        });
    }

    private renderStep2() {
        this.clear();
        this.container.innerHTML = `
      <div class="modal-content">
        <h2>Підтвердження Email</h2>
        <p>Ми надіслали код підтвердження на вашу пошту</p>
        <input type="text" placeholder="Введіть код" id="confirm-code" />
        <button id="verify-code">Підтвердити</button>
      </div>
    `;

        const confirmInput = this.container.querySelector<HTMLInputElement>('#confirm-code');
        this.container.querySelector('#verify-code')?.addEventListener('click', async () => {
            if (!confirmInput) return;

            const code = confirmInput.value.trim();
            if (!code) {
                const notification = new NotificationKarina();
                notification.show('Введіть код підтвердження', 'info');
                return;
            }

            await this.verifyCode(this.userEmail, code);
        });
    }

    private renderProfileForm() {
        this.clear();

        const isPerson = this.userRole === 'Athlete' || this.userRole === 'Trainer';
        const isOrg = this.userRole === 'Organization';

        const commonFields = isPerson
            ? `
        <input type="text" placeholder="Ім'я" id="first-name" />
        <input type="text" placeholder="Прізвище" id="last-name" />
        <input type="date" placeholder="Дата народження" id="birth-date" />
        <input type="file" id="user-photo" />
        <select id="sport-type">
          <option disabled selected>Оберіть вид спорту</option>
          <optgroup label="Індивідуальні">
            <option value="Box">Бокс</option>
            <option value="Struggle">Боротьба</option>
            <option value="CortMatch">Настільний теніс</option>
            <option value="CortMatch">Теніс</option>
            <option value="CortMatch">Бадмінтон</option>
            <option value="Checkers">Шашки</option>
            <option value="Chess">Шахи</option>
          </optgroup>
          <optgroup label="Командні">
            <option value="Football">Футбол</option>
            <option value="Basketball">Баскетбол</option>
            <option value="Volleyball">Волейбол</option>
            <option value="Hockey">Хокей</option>
            <option value="Baseball">Бейсбол</option>
          </optgroup>
        </select>
      `
            : `
        <input type="text" placeholder="Назва організації" id="org-name" />
        <input type="text" placeholder="Тип організації" id="org-type" />
        <textarea placeholder="Опис організації" id="org-desc"></textarea>
        <input type="date" placeholder="Дата заснування" id="org-founded" />
        <input type="text" placeholder="Країна" id="org-country" />
        <input type="file" id="org-photo" accept="image/*"  />
      `;

        this.container.innerHTML = `
      <div class="modal-content">
        <h2>Заповніть профіль (${this.userRole})</h2>
        ${commonFields}
        <button id="submit-profile">Завершити</button>
      </div>
    `;

        this.container.querySelector('#submit-profile')?.addEventListener('click', () => {
            if (this.userRole === 'Athlete' || this.userRole === 'Trainer' || this.userRole === 'Judge') {
                this.submitAthleteOrTrainerProfile();
            } else if (this.userRole === 'Organization') {
                this.submitOrganizationProfile();
            }
            this.container.remove();
        });
    }

    private async register(user: {
        email: string;
        login: string;
        password: string;
        role: string;
    }) {
        const response = await fetch('http://localhost:5154/registr', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(user),
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.detail || 'Сталася помилка при реєстрації');
        }
    }

    private async verifyCode(email: string, code: string): Promise<void> {
        try {
            const response = await fetch('http://localhost:5154/verify', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, code }),
            });

            if (response.ok) {
                this.renderProfileForm();

            } else {
                const errorData = await response.json();
                const notification = new NotificationKarina();
                notification.show(errorData.detail || 'Помилка підтвердження коду', 'error');
            }
        } catch (error) {
            const notification = new NotificationKarina();
            notification.show('Помилка мережі при підтвердженні коду', 'error');
        }
    }
    private async submitAthleteOrTrainerProfile(): Promise<void> {
        try {
            const formData = new FormData();

            const firstName = (document.getElementById('first-name') as HTMLInputElement).value;
            const lastName = (document.getElementById('last-name') as HTMLInputElement).value;
            const login = localStorage.getItem('login') || '';
            const profilePhoto = (document.getElementById('user-photo') as HTMLInputElement).files?.[0];
            const sportType = (document.getElementById('sport-type') as HTMLSelectElement).value;
            localStorage.setItem('sport', sportType);
            const dateBirhsday = (document.getElementById('birth-date') as HTMLInputElement).value;

            formData.append('FistName', firstName);
            formData.append('LastName', lastName);
            formData.append('Login', login);
            formData.append('dateBirhsday', dateBirhsday);
            if (profilePhoto) formData.append('ProfilePhoto', profilePhoto);
            formData.append('TypeSport', sportType);

            console.log(login);

            const response = await fetch('http://localhost:5154/complite-profile', {
                method: 'POST',
                body: formData,
            });

            if (!response.ok) {
                const contentType = response.headers.get('content-type');

                if (contentType && contentType.includes('application/json')) {
                    const error = await response.json();
                    throw new Error(error.detail || 'Помилка при збереженні профілю');
                } else {
                    const text = await response.text();
                    throw new Error(`Сервер повернув не JSON: ${text.slice(0, 100)}...`);
                }
            }
            var userRole = localStorage.getItem('userRole');
            var sportik = localStorage.getItem('sport');
            var base64String;
            if (profilePhoto) {
                const reader = new FileReader();
                reader.onload = function () {
                    base64String = reader.result as string;
                    localStorage.setItem('userPhoto', base64String);
                };
            }
            if (userRole === "Athlete") {
                users.push({
                    name: firstName + " " + lastName,
                    sport: sportik || "",
                    photo: base64String || "",
                    stats: "",
                    Team: "",
                    DataBirth: dateBirhsday,
                    login: login,
                    Position: "",
                    Matches: [],
                    dataMathes: sportik || ""
                });
            }
            if (userRole = "Trainer") {
                trainers.push({
                    FirsName: firstName,
                    LastName: lastName,
                    SportType: sportik || "",
                    Photo: base64String || "",
                    Teams: [],
                    Organizations: [],
                    DataBirth: dateBirhsday,
                    login: login,
                    Position: "",
                    Matches: [],
                    stats: undefined
                });
            }
            const notification = new NotificationKarina();
            notification.show('Профіль успішно заповнено!', 'success');
        } catch (err: any) {
            console.error('Помилка при відправці профілю:', err);
            const notification = new NotificationKarina();
            notification.show(err.message || 'Невідома помилка', 'error');
        }


    }

    private async submitOrganizationProfile(): Promise<void> {
        try {
            const formData = new FormData();

            const name = (document.getElementById('org-name') as HTMLInputElement).value;
            const type = (document.getElementById('org-type') as HTMLInputElement).value;
            const email = localStorage.getItem('email') || '';
            const country = (document.getElementById('org-country') as HTMLInputElement).value;
            const description = (document.getElementById('org-desc') as HTMLTextAreaElement).value;
            const photo = (document.getElementById('org-photo') as HTMLInputElement).files?.[0];

            formData.append('NameOrganization', name);
            formData.append('TypeOrganozation', type);
            formData.append('Email', email);
            formData.append('Country', country);
            formData.append('Description', description);
            if (photo) formData.append('ProfilePhoto', photo);

            const response = await fetch('http://localhost:5154/complite-profile-organization', {
                method: 'POST',
                body: formData,
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.detail || 'Помилка при збереженні організації');
            }
            var login1 = localStorage.getItem('login');
            organizations.push({
                login: login1 || "",
                NameOrganization: name,
                TypeOrganozation: type,
                Description: description,
                Country: country,
                Teams: [],
                OrganizationJudge: [],
                OrganizationTrainer: [],
                Events: [],
                photo: ''
            });
            const notification = new NotificationKarina();
            notification.show('Профіль організації успішно заповнено!', 'success');
        } catch (err: any) {
            console.error('Помилка при відправці організації:', err);
            const notification = new NotificationKarina();
            notification.show(err.message || 'Невідома помилка', 'error');
        }
    }

}
