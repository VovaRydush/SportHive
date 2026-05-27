import { organizations, trainers, users } from "./db";
import { NotificationKarina } from "./Notification";
import { authApi } from "../api/authApi";
import "./reg.css";

export class RegistrationModal {
  private container: HTMLElement;
  private userEmail = "";
  private userLogin = "";
  private userRole = "";

  constructor() {
    this.container = document.createElement("div");
    this.container.classList.add("modal");
    document.body.appendChild(this.container);
    this.renderStep1();
  }

  private clear() {
    this.container.innerHTML = "";
  }

  private notify(message: string, type: "success" | "error" | "info" = "info") {
    const notification = new NotificationKarina();
    notification.show(message, type);
  }

  private renderStep1() {
    this.clear();

    this.container.innerHTML = `
      <div class="modal-content">
        <h2>Реєстрація</h2>

        <div class="modal-form">
          <label for="reg-email">Email:</label>
          <input type="email" id="reg-email" required />

          <label for="reg-login">Логін:</label>
          <input type="text" id="reg-login" required />

          <label for="reg-password">Пароль:</label>
          <input type="password" id="reg-password" required />

          <label for="reg-role">Оберіть роль:</label>
          <select id="reg-role" required>
            <option value="">Оберіть роль</option>
            <option value="Organization">Організація</option>
            <option value="Trainer">Тренер</option>
            <option value="Athlete">Спортсмен</option>
            <option value="Judge">Суддя</option>
          </select>

          <button id="next-step">Далі</button>
        </div>
      </div>
    `;

    this.container.querySelector("#next-step")?.addEventListener("click", async () => {
      const email = (document.getElementById("reg-email") as HTMLInputElement).value.trim();
      const login = (document.getElementById("reg-login") as HTMLInputElement).value.trim();
      const password = (document.getElementById("reg-password") as HTMLInputElement).value.trim();
      const role = (document.getElementById("reg-role") as HTMLSelectElement).value;

      if (!email || !login || !password || !role) {
        this.notify("Будь ласка, заповніть всі поля!", "info");
        return;
      }

      try {
        const button = document.getElementById("next-step") as HTMLButtonElement;
        button.disabled = true;
        button.textContent = "Реєстрація...";

        await authApi.register({
          email,
          login,
          password,
          role,
        });

        this.userEmail = email;
        this.userLogin = login;
        this.userRole = role;

        localStorage.setItem("email", email);
        localStorage.setItem("login", login);
        localStorage.setItem("userRole", role);
        localStorage.setItem("role", role);

        this.notify("Код підтвердження відправлено на email.", "success");
        this.renderStep2();
      } catch (error) {
        this.notify(error instanceof Error ? error.message : "Помилка реєстрації", "error");

        const button = document.getElementById("next-step") as HTMLButtonElement | null;
        if (button) {
          button.disabled = false;
          button.textContent = "Далі";
        }
      }
    });
  }

  private renderStep2() {
    this.clear();

    this.container.innerHTML = `
      <div class="modal-content">
        <h2>Підтвердження Email</h2>

        <div class="modal-form">
          <p>Ми надіслали код підтвердження на вашу пошту</p>

          <label for="confirm-code">Код:</label>
          <input type="text" id="confirm-code" required />

          <button id="verify-code">Підтвердити</button>
        </div>
      </div>
    `;

    this.container.querySelector("#verify-code")?.addEventListener("click", async () => {
      const code = (document.getElementById("confirm-code") as HTMLInputElement).value.trim();

      if (!code) {
        this.notify("Введіть код підтвердження", "info");
        return;
      }

      try {
        const button = document.getElementById("verify-code") as HTMLButtonElement;
        button.disabled = true;
        button.textContent = "Перевірка...";

        await authApi.verify({
          email: this.userEmail || localStorage.getItem("email") || "",
          code,
        });

        this.notify("Email підтверджено.", "success");
        this.renderProfileForm();
      } catch (error) {
        this.notify(error instanceof Error ? error.message : "Помилка підтвердження коду", "error");

        const button = document.getElementById("verify-code") as HTMLButtonElement | null;
        if (button) {
          button.disabled = false;
          button.textContent = "Підтвердити";
        }
      }
    });
  }

  private renderProfileForm() {
    this.clear();

    const isPerson =
      this.userRole === "Athlete" ||
      this.userRole === "Trainer" ||
      this.userRole === "Judge";

    const personFields = `
      <label for="first-name">Ім'я:</label>
      <input type="text" id="first-name" required />

      <label for="last-name">Прізвище:</label>
      <input type="text" id="last-name" required />

      <label for="birth-date">Дата народження:</label>
      <input type="date" id="birth-date" required />

      <label for="user-photo">Фото профілю:</label>
      <input type="file" id="user-photo" accept="image/*" />

      <label for="sport-type">Оберіть вид спорту:</label>
      <select id="sport-type" required>
        <option value="">Оберіть вид спорту</option>
        <option value="Boxing">Бокс</option>
        <option value="Wrestling">Боротьба</option>
        <option value="TableTennis">Настільний теніс</option>
        <option value="Tennis">Теніс</option>
        <option value="Badminton">Бадмінтон</option>
        <option value="Checkers">Шашки</option>
        <option value="Chess">Шахи</option>
        <option value="Football">Футбол</option>
        <option value="Basketball">Баскетбол</option>
        <option value="Volleyball">Волейбол</option>
        <option value="Hockey">Хокей</option>
        <option value="Baseball">Бейсбол</option>
      </select>
    `;

    const organizationFields = `
      <label for="org-name">Назва організації:</label>
      <input type="text" id="org-name" required />

      <label for="org-type">Тип організації:</label>
      <input type="text" id="org-type" required />

      <label for="org-country">Країна:</label>
      <input type="text" id="org-country" required />

      <label for="org-photo">Фото організації:</label>
      <input type="file" id="org-photo" accept="image/*" />

      <label for="org-desc">Опис:</label>
      <textarea id="org-desc"></textarea>
    `;

    this.container.innerHTML = `
      <div class="modal-content">
        <h2>Заповніть профіль (${this.userRole})</h2>

        <div class="modal-form">
          ${isPerson ? personFields : organizationFields}
          <button id="submit-profile">Завершити</button>
        </div>
      </div>
    `;

    this.container.querySelector("#submit-profile")?.addEventListener("click", async () => {
      if (isPerson) {
        await this.submitPersonProfile();
      } else {
        await this.submitOrganizationProfile();
      }
    });
  }

  private async submitPersonProfile() {
    const firstName = (document.getElementById("first-name") as HTMLInputElement).value.trim();
    const lastName = (document.getElementById("last-name") as HTMLInputElement).value.trim();
    const birthDate = (document.getElementById("birth-date") as HTMLInputElement).value;
    const sportType = (document.getElementById("sport-type") as HTMLSelectElement).value;
    const profilePhoto = (document.getElementById("user-photo") as HTMLInputElement).files?.[0] || null;
    const savedLogin = this.userLogin || localStorage.getItem("login") || "";

    if (!firstName || !lastName || !birthDate || !sportType) {
      this.notify("Заповніть всі обов'язкові поля профілю.", "info");
      return;
    }

    if (!savedLogin) {
      this.notify("Не знайдено логін користувача. Зареєструйтесь ще раз.", "error");
      return;
    }

    try {
      const button = document.getElementById("submit-profile") as HTMLButtonElement;
      button.disabled = true;
      button.textContent = "Збереження...";

      console.log("PROFILE PAYLOAD:", {
        fistName: firstName,
        lastName,
        login: savedLogin,
        dateBirhsday: new Date(birthDate).toISOString(),
        typeSport: sportType,
        hasPhoto: Boolean(profilePhoto),
      });

      await authApi.completeProfile({
        fistName: firstName,
        lastName,
        login: savedLogin,
        dateBirhsday: new Date(birthDate).toISOString(),
        profilePhoto,
        typeSport: sportType,
      });

      localStorage.setItem("sport", sportType);

      if (profilePhoto) {
        const reader = new FileReader();
        reader.onload = () => {
          localStorage.setItem("userPhoto", String(reader.result || ""));
        };
        reader.readAsDataURL(profilePhoto);
      }

      if (this.userRole === "Athlete") {
        users.push({
          name: `${firstName} ${lastName}`,
          sport: sportType,
          photo: "",
          stats: "",
          Team: "",
          DataBirth: birthDate,
          login: savedLogin,
          Position: "",
          Matches: [],
          dataMathes: sportType,
        });
      }

      if (this.userRole === "Trainer") {
        trainers.push({
          FirsName: firstName,
          LastName: lastName,
          SportType: sportType,
          Photo: "",
          Teams: [],
          Organizations: [],
          DataBirth: birthDate,
          login: savedLogin,
          Position: "",
          Matches: [],
          stats: undefined,
        });
      }

      this.notify("Профіль успішно заповнено!", "success");
      this.container.remove();
      window.location.reload();
    } catch (error) {
      this.notify(error instanceof Error ? error.message : "Помилка при збереженні профілю", "error");
      console.error("Помилка при відправці профілю:", error);

      const button = document.getElementById("submit-profile") as HTMLButtonElement | null;
      if (button) {
        button.disabled = false;
        button.textContent = "Завершити";
      }
    }
  }

  private async submitOrganizationProfile() {
    const name = (document.getElementById("org-name") as HTMLInputElement).value.trim();
    const type = (document.getElementById("org-type") as HTMLInputElement).value.trim();
    const country = (document.getElementById("org-country") as HTMLInputElement).value.trim();
    const description = (document.getElementById("org-desc") as HTMLTextAreaElement).value.trim();
    const photo = (document.getElementById("org-photo") as HTMLInputElement).files?.[0] || null;
    const email = this.userEmail || localStorage.getItem("email") || "";
    const savedLogin = this.userLogin || localStorage.getItem("login") || "";

    if (!name || !type || !country) {
      this.notify("Заповніть назву, тип і країну організації.", "info");
      return;
    }

    try {
      const button = document.getElementById("submit-profile") as HTMLButtonElement;
      button.disabled = true;
      button.textContent = "Збереження...";

      await authApi.completeOrganizationProfile({
        nameOrganization: name,
        typeOrganozation: type,
        email,
        country,
        description,
        profilePhoto: photo,
      });

      organizations.push({
        login: savedLogin,
        NameOrganization: name,
        TypeOrganozation: type,
        Description: description,
        Country: country,
        Teams: [],
        OrganizationJudge: [],
        OrganizationTrainer: [],
        Events: [],
        photo: "",
      });

      this.notify("Профіль організації успішно заповнено!", "success");
      this.container.remove();
      window.location.reload();
    } catch (error) {
      this.notify(error instanceof Error ? error.message : "Помилка при збереженні організації", "error");
      console.error("Помилка при відправці організації:", error);

      const button = document.getElementById("submit-profile") as HTMLButtonElement | null;
      if (button) {
        button.disabled = false;
        button.textContent = "Завершити";
      }
    }
  }
}
