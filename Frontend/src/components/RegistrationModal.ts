import './reg.css';

export class RegistrationModal {
  private container: HTMLElement;
  private currentStep = 1;
  private userEmail: string = '';

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

    this.container.querySelector('#next-step')?.addEventListener('click', async () => {
      const email = (document.getElementById('reg-email') as HTMLInputElement).value.trim();
      const login = (document.getElementById('reg-login') as HTMLInputElement).value.trim();
      const password = (document.getElementById('reg-password') as HTMLInputElement).value.trim();
      const role = (document.getElementById('reg-role') as HTMLSelectElement).value;

      if (!email || !login || !password || !role) {
        alert('Будь ласка, заповніть всі поля!');
        return;
      }

      try {
        await this.register({ email, login, password, role });
        this.userEmail = email; // 👈 зберігаємо email для другого кроку
        this.renderStep2();
      } catch (error: any) {
        alert(`Помилка: ${error.message}`);
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
        alert('Введіть код підтвердження');
        return;
      }

      await this.verifyCode(this.userEmail, code); // 👈 використовуємо збережений email
    });
  }

  private renderStep3() {
    this.clear();
    this.container.innerHTML = `
      <div class="modal-content">
        <h2>Успішна реєстрація!</h2>
        <p>Тепер ви можете увійти у свій обліковий запис.</p>
        <button id="close-modal">Закрити</button>
      </div>
    `;

    this.container.querySelector('#close-modal')?.addEventListener('click', () => {
      this.container.remove();
    });

    setTimeout(() => {
      this.container.remove();
    }, 5000);
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
        this.renderStep3();
      } else {
        const errorData = await response.json();
        alert(errorData.detail || 'Помилка підтвердження коду');
      }
    } catch (error) {
      alert('Помилка мережі при підтвердженні коду');
    }
  }
}
