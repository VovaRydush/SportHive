import { NavigationManager } from "./NavigationManager";
import { NotificationKarina } from "./Notification";
import { authApi } from "../api/authApi";
import { saveAuth } from "../api/authStorage";
import { PasswordRecoveryModal } from "./PasswordRecoveryModal";
import "./reg.css";

export class LoginModal {
  private modal: HTMLElement;
  private loginInput: HTMLInputElement;
  private passwordInput: HTMLInputElement;
  private submitButton: HTMLButtonElement;
  private recoveryModal: PasswordRecoveryModal;

  constructor() {
    this.recoveryModal = new PasswordRecoveryModal();

    this.modal = document.createElement("div");
    this.modal.className = "modal";

    this.modal.innerHTML = `
      <div class="modal-content">
        <h2>Вхід</h2>

        <form id="login-form" class="modal-form">
          <label for="login">Логін:</label>
          <input type="text" id="login" name="login" autocomplete="username" required />

          <label for="password">Пароль:</label>
          <input type="password" id="password" name="password" autocomplete="current-password" required />

          <button type="submit">Увійти</button>

          <button
            type="button"
            id="forgot-password-btn"
            style="background: transparent; color: #111; border: none; text-decoration: underline; cursor: pointer; padding: 8px 0;"
          >
            Забули пароль?
          </button>
        </form>
      </div>
    `;

    document.body.appendChild(this.modal);

    this.loginInput = this.modal.querySelector("#login")!;
    this.passwordInput = this.modal.querySelector("#password")!;
    this.submitButton = this.modal.querySelector("button[type='submit']")!;

    this.modal
      .querySelector("#login-form")!
      .addEventListener("submit", (event) => this.handleSubmit(event));

    this.modal
      .querySelector("#forgot-password-btn")
      ?.addEventListener("click", () => {
        this.close();
        this.recoveryModal.show();
      });
  }

  private async handleSubmit(event: Event) {
    event.preventDefault();

    const login = this.loginInput.value.trim();
    const password = this.passwordInput.value.trim();
    const notification = new NotificationKarina();

    if (!login || !password) {
      notification.show("Введіть логін і пароль.", "info");
      return;
    }

    try {
      this.submitButton.disabled = true;
      this.submitButton.textContent = "Вхід...";

      const response = await authApi.login({
        email: "",
        login,
        password,
        role: "",
      });

      saveAuth({
        ...response,
        login: response.login || login,
      });

      this.close();

      notification.show("Вхід виконано успішно.", "success");

      const nav = new NavigationManager();
      nav.init();
    } catch (error) {
      notification.show(
        error instanceof Error ? error.message : "Невдалий вхід. Перевір логін і пароль.",
        "error"
      );

      console.error("Помилка входу:", error);
    } finally {
      this.submitButton.disabled = false;
      this.submitButton.textContent = "Увійти";
    }
  }

  public show() {
    this.modal.style.display = "flex";
  }

  public close() {
    this.modal.style.display = "none";
  }
}
