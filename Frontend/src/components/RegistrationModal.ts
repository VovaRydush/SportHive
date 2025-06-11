import './reg.css';
export class RegistrationModal {
    private container: HTMLElement;

    private currentStep = 1;

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
  <option value="" disabled selected>Виберіть роль</option>
  <option value="organization">Організація</option>
  <option value="coach">Тренер</option>
  <option value="athlete">Спортсмен</option>
</select>
    <button id="next-step">Далі</button>
  </div>
`;
        this.container.querySelector('#next-step')?.addEventListener('click', () => {
            // TODO: Валідація
            this.renderStep2();
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
        this.container.querySelector('#verify-code')?.addEventListener('click', () => {
            // TODO: Перевірка коду
            this.renderStep3();
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
    }
}
