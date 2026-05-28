import { authApi } from "../api/authApi";
import { NotificationKarina } from "./Notification";
import "./reg.css";

export class PasswordRecoveryModal {
  private modal: HTMLElement;
  private email = "";
  private login = "";
  private code = "";

  constructor() {
    this.modal = document.createElement("div");
    this.modal.className = "modal";
    this.modal.style.display = "none";
    document.body.appendChild(this.modal);

    this.renderEmailStep();
  }

  public show() {
    this.modal.style.display = "flex";
  }

  public close() {
    this.modal.style.display = "none";
  }

  private clear() {
    this.modal.innerHTML = "";
  }

  private notify(message: string, type: "success" | "error" | "info" = "info") {
    new NotificationKarina().show(message, type);
  }

  private renderEmailStep() {
    this.clear();

    this.modal.innerHTML = `
      <div class="modal-content">
        <h2>Відновлення паролю</h2>

        <form id="recovery-email-form" class="modal-form">
          <p style="margin: 0 0 12px 0;">
            Введіть email та login акаунта. На email прийде код підтвердження.
          </p>

          <label for="recovery-email">Email:</label>
          <input
            type="email"
            id="recovery-email"
            name="email"
            autocomplete="email"
            required
          />

          <label for="recovery-login">Логін:</label>
          <input
            type="text"
            id="recovery-login"
            name="login"
            autocomplete="username"
            required
          />

          <button type="submit" id="recovery-email-btn">Надіслати код</button>
          <button type="button" id="recovery-close-btn">Закрити</button>
        </form>
      </div>
    `;

    this.modal
      .querySelector("#recovery-email-form")
      ?.addEventListener("submit", (event) => this.handleSendEmail(event));

    this.modal
      .querySelector("#recovery-close-btn")
      ?.addEventListener("click", () => this.close());
  }

  private async handleSendEmail(event: Event) {
    event.preventDefault();

    const emailInput = document.getElementById("recovery-email") as HTMLInputElement | null;
    const loginInput = document.getElementById("recovery-login") as HTMLInputElement | null;
    const button = document.getElementById("recovery-email-btn") as HTMLButtonElement | null;

    const email = emailInput?.value.trim() || "";
    const login = loginInput?.value.trim() || "";

    if (!email || !login) {
      this.notify("Введіть email і логін.", "info");
      return;
    }

    try {
      if (button) {
        button.disabled = true;
        button.textContent = "Відправка...";
      }

      await authApi.sendRecoveryEmail(email);

      this.email = email;
      this.login = login;

      localStorage.setItem("recoveryEmail", email);
      localStorage.setItem("recoveryLogin", login);

      this.notify("Код відновлення відправлено на email.", "success");
      this.renderCodeStep();
    } catch (error) {
      this.notify(
        error instanceof Error ? error.message : "Не вдалося відправити код.",
        "error"
      );
    } finally {
      if (button) {
        button.disabled = false;
        button.textContent = "Надіслати код";
      }
    }
  }

  private renderCodeStep() {
    this.clear();

    this.modal.innerHTML = `
      <div class="modal-content">
        <h2>Підтвердження коду</h2>

        <form id="recovery-code-form" class="modal-form">
          <p style="margin: 0 0 12px 0;">
            Введіть код, який прийшов на <b>${this.escapeHtml(this.email)}</b>.
          </p>

          <label for="recovery-code">Код:</label>
          <input
            type="text"
            id="recovery-code"
            name="code"
            autocomplete="one-time-code"
            required
          />

          <button type="submit" id="recovery-code-btn">Підтвердити код</button>
          <button type="button" id="recovery-back-email-btn">Назад</button>
        </form>
      </div>
    `;

    this.modal
      .querySelector("#recovery-code-form")
      ?.addEventListener("submit", (event) => this.handleCheckCode(event));

    this.modal
      .querySelector("#recovery-back-email-btn")
      ?.addEventListener("click", () => this.renderEmailStep());
  }

  private async handleCheckCode(event: Event) {
    event.preventDefault();

    const codeInput = document.getElementById("recovery-code") as HTMLInputElement | null;
    const button = document.getElementById("recovery-code-btn") as HTMLButtonElement | null;

    const code = codeInput?.value.trim() || "";

    if (!code) {
      this.notify("Введіть код.", "info");
      return;
    }

    try {
      if (button) {
        button.disabled = true;
        button.textContent = "Перевірка...";
      }

      await authApi.checkRecoveryCode({
        email: this.email,
        code,
      });

      this.code = code;
      localStorage.setItem("recoveryCode", code);

      this.notify("Код підтверджено.", "success");
      this.renderPasswordStep();
    } catch (error) {
      this.notify(
        error instanceof Error ? error.message : "Невірний код підтвердження.",
        "error"
      );
    } finally {
      if (button) {
        button.disabled = false;
        button.textContent = "Підтвердити код";
      }
    }
  }

  private renderPasswordStep() {
    this.clear();

    this.modal.innerHTML = `
      <div class="modal-content">
        <h2>Новий пароль</h2>

        <form id="recovery-password-form" class="modal-form">
          <p style="margin: 0 0 12px 0;">
            Пароль буде змінено для логіну: <b>${this.escapeHtml(this.login)}</b>
          </p>

          <label for="new-password">Новий пароль:</label>
          <input
            type="password"
            id="new-password"
            name="password"
            autocomplete="new-password"
            required
          />

          <label for="repeat-password">Повторіть пароль:</label>
          <input
            type="password"
            id="repeat-password"
            name="repeat-password"
            autocomplete="new-password"
            required
          />

          <button type="submit" id="recovery-password-btn">Змінити пароль</button>
          <button type="button" id="recovery-back-code-btn">Назад</button>
        </form>
      </div>
    `;

    this.modal
      .querySelector("#recovery-password-form")
      ?.addEventListener("submit", (event) => this.handleChangePassword(event));

    this.modal
      .querySelector("#recovery-back-code-btn")
      ?.addEventListener("click", () => this.renderCodeStep());
  }

  private async handleChangePassword(event: Event) {
    event.preventDefault();

    const passwordInput = document.getElementById("new-password") as HTMLInputElement | null;
    const repeatInput = document.getElementById("repeat-password") as HTMLInputElement | null;
    const button = document.getElementById("recovery-password-btn") as HTMLButtonElement | null;

    const password = passwordInput?.value.trim() || "";
    const repeatPassword = repeatInput?.value.trim() || "";

    if (!password || !repeatPassword) {
      this.notify("Заповніть обидва поля паролю.", "info");
      return;
    }

    if (password.length < 6) {
      this.notify("Пароль має містити мінімум 6 символів.", "info");
      return;
    }

    if (password !== repeatPassword) {
      this.notify("Паролі не співпадають.", "error");
      return;
    }

    if (!this.login) {
      this.notify("Не знайдено логін користувача. Почніть відновлення заново.", "error");
      this.renderEmailStep();
      return;
    }

    try {
      if (button) {
        button.disabled = true;
        button.textContent = "Збереження...";
      }

      await authApi.changePassword({
        email: this.email,
        login: this.login,
        password,
        role: "",
      });

      localStorage.removeItem("recoveryEmail");
      localStorage.removeItem("recoveryLogin");
      localStorage.removeItem("recoveryCode");

      this.notify("Пароль успішно змінено. Тепер увійдіть з новим паролем.", "success");

      this.email = "";
      this.login = "";
      this.code = "";

      this.close();
      this.renderEmailStep();
    } catch (error) {
      this.notify(
        error instanceof Error ? error.message : "Не вдалося змінити пароль.",
        "error"
      );
    } finally {
      if (button) {
        button.disabled = false;
        button.textContent = "Змінити пароль";
      }
    }
  }

 private escapeHtml(value: unknown) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}
}
