import { authApi } from "../api/authApi";
import { commandApi } from "../api/commandApi";
import type { SearchAthleteResult, TeamAthleteDto } from "../api/commandTypes";
import { NotificationKarina } from "./Notification";
import "./createTeam.css";

type SelectedAthlete = SearchAthleteResult & {
  status: string;
};

export class CreateTeamModal {
  private modalContainer: HTMLElement;
  private searchResults: SearchAthleteResult[] = [];
  private selectedAthletes: SelectedAthlete[] = [];
  private searchTimer?: number;

  constructor() {
    this.modalContainer = document.createElement("div");
    this.modalContainer.className = "team-modal-container";
    document.body.appendChild(this.modalContainer);
  }

  async show() {
    this.modalContainer.innerHTML = `
      <div class="team-modal">
        <div class="modal-header">
          <h2>Створити нову команду</h2>
          <button class="close-btn" type="button">×</button>
        </div>

        <form id="createTeamForm" class="team-form">
          <div class="form-group">
            <label for="teamName">Назва команди</label>
            <input id="teamName" name="teamName" type="text" required />
          </div>

          <div class="form-group">
            <label for="trainerLogin">Логін тренера</label>
            <input
              id="trainerLogin"
              name="trainerLogin"
              type="text"
              value="${this.escapeHtml(localStorage.getItem("login") || "")}"
              required
            />
          </div>

          <div class="form-group">
            <label for="sportType">Вид спорту</label>
            <select id="sportType" name="sportType" required>
              <option value="">Оберіть вид спорту</option>
              <option value="Football">Футбол</option>
              <option value="Basketball">Баскетбол</option>
              <option value="Volleyball">Волейбол</option>
              <option value="Tennis">Теніс</option>
              <option value="Boxing">Бокс</option>
              <option value="Wrestling">Боротьба</option>
              <option value="Hockey">Хокей</option>
              <option value="Baseball">Бейсбол</option>
              <option value="Chess">Шахи</option>
              <option value="Checkers">Шашки</option>
              <option value="Badminton">Бадмінтон</option>
              <option value="TableTennis">Настільний теніс</option>
            </select>
          </div>

          <div class="form-group">
            <label for="teamPhoto">Логотип команди</label>
            <input id="teamPhoto" name="teamPhoto" type="file" accept="image/*" />
            <div id="photoPreview" class="photo-preview"></div>
          </div>

          <div class="form-group">
            <label for="athleteSearch">Пошук спортсменів</label>
            <div class="search-box">
              <input
                id="athleteSearch"
                type="text"
                placeholder="Введіть ім'я або прізвище"
                autocomplete="off"
              />
              <button class="search-btn" type="button">Пошук</button>
            </div>
          </div>

          <section class="athletes-list">
            <h3>Доступні спортсмени</h3>
            <div id="athleteSearchMessage" class="team-form-message"></div>
            <div id="availableAthletes" class="athletes-grid"></div>
          </section>

          <section class="selected-athletes">
            <h3>Обрані спортсмени</h3>
            <div id="selectedAthletesList" class="selected-list">
              <span class="selected-athlete empty">Не обрано жодного спортсмена</span>
            </div>
          </section>

          <div class="form-actions">
            <button class="btn btn-primary" type="submit" id="createTeamBtn">
              Створити команду
            </button>
          </div>
        </form>
      </div>
    `;

    this.addEventListeners();
    this.modalContainer.style.display = "flex";
  }

  private addEventListeners() {
    this.modalContainer.querySelector(".close-btn")?.addEventListener("click", () => {
      this.close();
    });

    const photoInput = this.modalContainer.querySelector("#teamPhoto") as HTMLInputElement;
    const photoPreview = this.modalContainer.querySelector("#photoPreview") as HTMLElement;

    photoInput.addEventListener("change", (event) => {
      const file = (event.target as HTMLInputElement).files?.[0];

      if (!file) {
        photoPreview.innerHTML = "";
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        photoPreview.innerHTML = `<img src="${String(reader.result)}" alt="Логотип команди" />`;
      };
      reader.readAsDataURL(file);
    });

    const searchInput = this.modalContainer.querySelector("#athleteSearch") as HTMLInputElement;
    const searchBtn = this.modalContainer.querySelector(".search-btn") as HTMLButtonElement;

    searchBtn.addEventListener("click", () => this.searchAthletes(searchInput.value));

    searchInput.addEventListener("input", () => {
      window.clearTimeout(this.searchTimer);
      this.searchTimer = window.setTimeout(() => {
        if (searchInput.value.trim().length >= 2) {
          this.searchAthletes(searchInput.value);
        }
      }, 450);
    });

    searchInput.addEventListener("keydown", (event) => {
      if (event.key === "Enter") {
        event.preventDefault();
        this.searchAthletes(searchInput.value);
      }
    });

    const form = this.modalContainer.querySelector("#createTeamForm") as HTMLFormElement;
    form.addEventListener("submit", (event) => this.handleSubmit(event));
  }

  private async searchAthletes(query: string) {
    const message = this.modalContainer.querySelector("#athleteSearchMessage") as HTMLElement;
    const container = this.modalContainer.querySelector("#availableAthletes") as HTMLElement;

    const search = query.trim();

    if (!search) {
      message.textContent = "Введіть ім'я або прізвище спортсмена.";
      container.innerHTML = "";
      return;
    }

    try {
      message.textContent = "Пошук...";
      container.innerHTML = "";

      const response = await authApi.searchAthlete(search);
      const list = Array.isArray(response) ? response : [response];

      this.searchResults = list
        .map((item) => this.normalizeAthlete(item))
        .filter((athlete) => Boolean(athlete.login));

      if (!this.searchResults.length) {
        message.textContent = "Спортсменів не знайдено.";
        return;
      }

      message.textContent = "";
      this.renderAvailableAthletes();
    } catch (error) {
      console.error("Помилка пошуку спортсменів:", error);
      message.textContent = error instanceof Error ? error.message : "Помилка пошуку спортсменів.";
    }
  }

  private renderAvailableAthletes() {
    const container = this.modalContainer.querySelector("#availableAthletes") as HTMLElement;

    container.innerHTML = this.searchResults
      .map((athlete) => {
        const checked = this.selectedAthletes.some((a) => a.login === athlete.login) ? "checked" : "";

        return `
          <div class="athlete-card">
            <label>
              <input
                type="checkbox"
                name="selectedAthletes"
                value="${this.escapeHtml(athlete.login)}"
                ${checked}
              />
              <span>${this.escapeHtml(athlete.fullName)} (${this.escapeHtml(athlete.sport || "спорт не вказано")})</span>
            </label>
          </div>
        `;
      })
      .join("");

    container
      .querySelectorAll<HTMLInputElement>('input[name="selectedAthletes"]')
      .forEach((checkbox) => {
        checkbox.addEventListener("change", () => {
          const athlete = this.searchResults.find((a) => a.login === checkbox.value);
          if (!athlete) return;

          if (checkbox.checked) {
            this.addSelectedAthlete(athlete);
          } else {
            this.selectedAthletes = this.selectedAthletes.filter((a) => a.login !== athlete.login);
          }

          this.updateSelectedAthletes();
        });
      });
  }

  private addSelectedAthlete(athlete: SearchAthleteResult) {
    if (this.selectedAthletes.some((a) => a.login === athlete.login)) return;

    this.selectedAthletes.push({
      ...athlete,
      status: "Active",
    });
  }

  private updateSelectedAthletes() {
    const selectedList = this.modalContainer.querySelector("#selectedAthletesList") as HTMLElement;

    if (!this.selectedAthletes.length) {
      selectedList.innerHTML = `<span class="selected-athlete empty">Не обрано жодного спортсмена</span>`;
      return;
    }

    selectedList.innerHTML = this.selectedAthletes
      .map(
        (athlete) => `
          <span class="selected-athlete">
            ${this.escapeHtml(athlete.fullName)}
            <select data-login="${this.escapeHtml(athlete.login)}" class="athlete-status-select">
              <option value="Active" ${athlete.status === "Active" ? "selected" : ""}>Active</option>
              <option value="Reserve" ${athlete.status === "Reserve" ? "selected" : ""}>Reserve</option>
              <option value="Injured" ${athlete.status === "Injured" ? "selected" : ""}>Injured</option>
            </select>
          </span>
        `
      )
      .join("");

    selectedList.querySelectorAll<HTMLSelectElement>(".athlete-status-select").forEach((select) => {
      select.addEventListener("change", () => {
        const login = select.dataset.login;
        const athlete = this.selectedAthletes.find((a) => a.login === login);
        if (athlete) athlete.status = select.value;
      });
    });
  }

  private async handleSubmit(event: Event) {
    event.preventDefault();

    const notification = new NotificationKarina();
    const button = this.modalContainer.querySelector("#createTeamBtn") as HTMLButtonElement;

    const nameTeam = (this.modalContainer.querySelector("#teamName") as HTMLInputElement).value.trim();
    const loginTrainer = (this.modalContainer.querySelector("#trainerLogin") as HTMLInputElement).value.trim();
    const typeSport = (this.modalContainer.querySelector("#sportType") as HTMLSelectElement).value;
    const photo = (this.modalContainer.querySelector("#teamPhoto") as HTMLInputElement).files?.[0] || null;

    if (!nameTeam || !loginTrainer || !typeSport) {
      notification.show("Заповніть назву команди, тренера та вид спорту.", "info");
      return;
    }

    const athletes: TeamAthleteDto[] = this.selectedAthletes.map((athlete) => ({
      nameTeam,
      loginAthlets: athlete.login,
      athleteStatus: athlete.status || "Active",
    }));

    try {
      button.disabled = true;
      button.textContent = "Створення...";

      await commandApi.createTeam({
        nameTeam,
        loginTrainer,
        typeSport,
        photo,
        athlets: athletes,
      });

      const role = localStorage.getItem("userRole") || localStorage.getItem("role");
      const loginOrganization = localStorage.getItem("login") || "";

      if (role === "Organization" && loginOrganization) {
        await commandApi.linkTeamOrganization({
          loginOrganization,
          nameTeam,
        });
      }

      notification.show("Команду успішно створено!", "success");
      this.close();
    } catch (error) {
      console.error("Помилка створення команди:", error);
      notification.show(
        error instanceof Error ? error.message : "Сталася помилка при створенні команди",
        "error"
      );
    } finally {
      button.disabled = false;
      button.textContent = "Створити команду";
    }
  }

  private normalizeAthlete(item: any): SearchAthleteResult {
    const firstName =
      item?.fistName ||
      item?.FistName ||
      item?.firstName ||
      item?.FirstName ||
      item?.FirsName ||
      item?.firsName ||
      "";

    const lastName = item?.lastName || item?.LastName || "";

    const fullName =
      item?.fullName ||
      item?.FullName ||
      item?.name ||
      item?.Name ||
      `${firstName} ${lastName}`.trim() ||
      "Спортсмен";

    const login =
      item?.login ||
      item?.Login ||
      item?.loginAthlets ||
      item?.LoginAthlets ||
      item?.userName ||
      item?.UserName ||
      "";

    return {
      login,
      fullName,
      sport: item?.sport || item?.Sport || item?.typeSport || item?.TypeSport || item?.SportType,
      photo: item?.photo || item?.Photo || item?.profilePhoto || item?.ProfilePhoto,
      raw: item,
    };
  }

  private escapeHtml(value: unknown) {
    return String(value ?? "")
      .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
  }

  close() {
    this.modalContainer.style.display = "none";
    this.modalContainer.innerHTML = "";
  }
}
