import { NotificationKarina } from './Notification';
import './reg.css';
export class LoginModal {
    private modal: HTMLElement;
    private loginInput: HTMLInputElement;
    private passwordInput: HTMLInputElement;
    private submitButton: HTMLButtonElement;

    constructor() {
        this.modal = document.createElement('div');
        this.modal.className = 'modal';

        this.modal.innerHTML = `
			<div class="modal-content">
				<h2>Вхід</h2>
				<form id="login-form" class="modal-form">

					<label for="login">Логін:</label>
					<input type="text" id="login" name="login" required />

					<label for="password">Пароль:</label>
					<input type="password" id="password" name="password" required />

					<button type="submit">Увійти</button>
				</form>
			</div>
		`;

        document.body.appendChild(this.modal);

        this.loginInput = this.modal.querySelector('#login')!;
        this.passwordInput = this.modal.querySelector('#password')!;
        this.submitButton = this.modal.querySelector('button')!;

        this.modal.querySelector('#login-form')!.addEventListener('submit', (e) => this.handleSubmit(e));
    }

    private async handleSubmit(event: Event) {
	event.preventDefault();

	const login = this.loginInput.value.trim();
	const password = this.passwordInput.value.trim();

	try {
		const response = await fetch('http://localhost:5154/login', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				email: '', // якщо бекенд вимагає
				login,
				password,
				role: ''   // якщо бекенд вимагає
			}),
		});

		if (!response.ok) {
			const text = await response.text();
			throw new Error(`Помилка входу: ${text}`);
		}

		const token = await response.text(); // ← просто рядок
		if (token && typeof token === 'string') {
			localStorage.setItem('accessToken', token);
			this.close();
		} else {
			throw new Error('Невірна відповідь: токен не отримано');
		}
	} catch (error) {
		const notification = new NotificationKarina();
        notification.show('Невдалий вхід. Перевір логін і пароль.','error');
		console.error('Помилка входу:', error);
	}
}


    public show() {
        this.modal.style.display = 'flex';
    }

    public close() {
        this.modal.style.display = 'none';
    }
}
