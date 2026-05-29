import {
  allEvents,
  Event,
  judges,
  organizations,
  trainers,
  users,
} from "./db";
import { authApi } from "../api/authApi";
import { commandApi } from "../api/commandApi";
import { TeamPageLook } from "./TeamPage";
import { NotificationKarina } from "./Notification";
import { CreateTeamModal } from "./CreateTeam";
import "./organizationProfile.css";

type EmployeeRole = "Athlete" | "Judge" | "Trainer";

type SearchPerson = {
  login: string;
  fullName: string;
  role?: string;
  sport?: string;
  photo?: string;
  raw: any;
};

interface OrganizationView {
  login: string;
  photo: string;
  NameOrganization: string;
  TypeOrganozation: string;
  Description: string;
  Country: string;
  Teams: any[];
  OrganizationJudge: any[];
  OrganizationTrainer: any[];
  OrganizationAthlete: any[];
  Events: Event[];
}

export class OrganizationProfile {
  private container: HTMLElement;
  private orgData!: OrganizationView;
  private searchResults: SearchPerson[] = [];
  private selectedRole: EmployeeRole = "Athlete";
  private searchTimer?: number;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

    this.container = element;
  }

  async render() {
    this.orgData = this.setDataOrganiz();

    try {
      const backendTeams = await commandApi.getTeamsByOrganization(this.orgData.login);
      this.orgData.Teams = backendTeams || [];
    } catch (error) {
      console.warn("Не вдалося завантажити команди організації з бекенду:", error);
    }

    this.container.innerHTML = `
      <div class="organization-profile">
        <section class="org-header">
          <div class="org-logo-container">
            <img
              class="org-logo"
              src="${this.escapeHtml(this.orgData.photo || "https://placehold.co/250x250?text=SportHive")}"
              alt="Лого організації"
            />
            <div class="org-type-badge">${this.escapeHtml(this.orgData.TypeOrganozation || "Організація")}</div>
          </div>

          <div class="org-main-info">
            <div class="org-title-row">
              <h1 class="org-title">${this.escapeHtml(this.orgData.NameOrganization || "Моя організація")}</h1>

              <button id="create-team-btn" class="create-team-profile-btn" type="button">
                + Створити команду
              </button>
            </div>

            <div class="org-meta">
              <span>${this.getCountryFlag(this.orgData.Country)} ${this.escapeHtml(this.orgData.Country || "Україна")}</span>
              <span>Логін: ${this.escapeHtml(this.orgData.login)}</span>
            </div>

            <div class="org-description-block">
              <h3>Про організацію</h3>
              <p class="org-description">
                ${this.escapeHtml(this.orgData.Description || "Опис організації поки не заповнений.")}
              </p>
            </div>
          </div>
        </section>

        ${this.renderEmployeeManager()}

        <section>
          <h2 class="section-title">Наші команди</h2>
          <div class="teams-grid">
            ${
              this.orgData.Teams.length
                ? this.orgData.Teams.map((team) => `
                  <div class="team-card organization-team-card" data-team-name="${this.escapeHtml(team.name || team.Name || team.nameTeam || team.NameTeam || team.NameComand || "")}">
                    <div class="team-header">
                      <span class="team-sport-icon">${this.getSportIcon(team.sport || team.SportType || team.typeSport || "")}</span>
                      <h3 class="team-name">${this.escapeHtml(team.name || team.Name || team.nameTeam || team.NameTeam || team.NameComand || "Команда")}</h3>
                    </div>
                    <div class="team-details">
                      <p>Вид спорту: ${this.escapeHtml(team.sport || team.SportType || team.typeSport || team.TypeSport || "-")}</p>
                      <p>Тренер: ${this.escapeHtml(team.trainerLogin || team.TrainerLogin || "-")}</p>
                      <button class="employee-btn open-team-page-btn" type="button">Переглянути команду</button>
                    </div>
                  </div>
                `).join("")
                : `<div class="empty-state">Команд поки немає. Натисни “Створити команду”, щоб додати першу.</div>`
            }
          </div>
        </section>

        <section class="org-staff-section">
          <div>
            <h2 class="section-title">Судді</h2>
            <div class="staff-list" id="judges-list">
              ${this.renderJudgesList()}
            </div>
          </div>

          <div>
            <h2 class="section-title">Тренери</h2>
            <div class="staff-list" id="trainers-list">
              ${this.renderTrainersList()}
            </div>
          </div>
        </section>

        <section>
          <h2 class="section-title">Атлети організації</h2>
          <div class="staff-list" id="athletes-list">
            ${this.renderAthletesList()}
          </div>
        </section>

        <section>
          <h2 class="section-title">Останні події</h2>
          <div class="events-timeline">
            ${
              this.orgData.Events.length
                ? this.orgData.Events.map((event) => `
                  <div class="event-item">
                    <div class="event-date">
                      ${new Date(event.date).toLocaleDateString("uk-UA", {
                        day: "numeric",
                        month: "long",
                      })}
                    </div>
                    <div class="event-content">
                      <h3 class="event-title">${this.escapeHtml(event.NameEvent)}</h3>
                      <p class="event-description">${this.escapeHtml(event.description)}</p>
                      <div class="event-meta">📍 ${this.escapeHtml(event.location)}</div>
                    </div>
                  </div>
                `).join("")
                : `<div class="empty-state">Подій поки немає</div>`
            }
          </div>
        </section>
      </div>
    `;

    this.bindCreateTeamButton();
    this.bindEmployeeManagerEvents();
    this.bindTeamCards();
  }

  private bindTeamCards() {
    this.container.querySelectorAll<HTMLElement>(".organization-team-card").forEach((card) => {
      card.addEventListener("click", (event) => {
        const target = event.target as HTMLElement;
        if (!target.closest(".open-team-page-btn") && target.tagName !== "BUTTON") {
          return;
        }

        const teamName = card.dataset.teamName || "";
        if (teamName) {
          new TeamPageLook("app", teamName).render();
        }
      });
    });
  }

  private bindCreateTeamButton() {
    const button = document.getElementById("create-team-btn");

    button?.addEventListener("click", () => {
      new CreateTeamModal().show();
    });
  }

  private renderEmployeeManager() {
    return `
      <section class="employee-manager">
        <div class="employee-manager-header">
          <div>
            <h2 class="section-title employee-title">Додати учасника в організацію</h2>
            <p class="employee-subtitle">
              Знайди атлета, суддю або тренера і додай його до організації.
            </p>
          </div>
        </div>

        <div class="employee-form">
          <div class="employee-field">
            <label for="employee-role">Кого додати</label>
            <select id="employee-role">
              <option value="Athlete">Атлета</option>
              <option value="Judge">Суддю</option>
              <option value="Trainer">Тренера</option>
            </select>
          </div>

          <div class="employee-field employee-search-field">
            <label for="employee-search">Пошук по ПІБ</label>
            <input
              id="employee-search"
              type="text"
              placeholder="Наприклад: Іван Петренко"
              autocomplete="off"
            />
          </div>

          <button id="employee-search-btn" class="employee-btn" type="button">
            Знайти
          </button>
        </div>

        <div id="employee-message" class="employee-message"></div>
        <div id="employee-results" class="employee-results"></div>
      </section>
    `;
  }

  private bindEmployeeManagerEvents() {
    const roleSelect = document.getElementById("employee-role") as HTMLSelectElement | null;
    const searchInput = document.getElementById("employee-search") as HTMLInputElement | null;
    const searchButton = document.getElementById("employee-search-btn") as HTMLButtonElement | null;

    roleSelect?.addEventListener("change", () => {
      this.selectedRole = roleSelect.value as EmployeeRole;
      this.renderSearchResults();
    });

    searchButton?.addEventListener("click", () => {
      this.searchPeople();
    });

    searchInput?.addEventListener("input", () => {
      window.clearTimeout(this.searchTimer);
      this.searchTimer = window.setTimeout(() => {
        if (searchInput.value.trim().length >= 2) {
          this.searchPeople();
        } else {
          this.searchResults = [];
          this.renderSearchResults();
        }
      }, 450);
    });

    searchInput?.addEventListener("keydown", (event) => {
      if (event.key === "Enter") {
        event.preventDefault();
        this.searchPeople();
      }
    });
  }

  private async searchPeople() {
    const input = document.getElementById("employee-search") as HTMLInputElement | null;
    const button = document.getElementById("employee-search-btn") as HTMLButtonElement | null;

    const query = input?.value.trim() || "";

    if (!query) {
      this.showEmployeeMessage("Введіть ім'я або прізвище для пошуку.", "info");
      return;
    }

    try {
      if (button) {
        button.disabled = true;
        button.textContent = "Пошук...";
      }

      this.showEmployeeMessage("", "info");

      const response = await authApi.searchAthlete(query);
      const list = Array.isArray(response) ? response : [response];

      this.searchResults = list
        .map((item) => this.normalizeSearchResult(item))
        .filter((item) => Boolean(item.login));

      if (!this.searchResults.length) {
        this.showEmployeeMessage("Нічого не знайдено.", "info");
      }

      this.renderSearchResults();
    } catch (error) {
      console.error("Помилка пошуку:", error);
      this.searchResults = [];
      this.renderSearchResults();
      this.showEmployeeMessage(
        error instanceof Error ? error.message : "Помилка пошуку користувача.",
        "error"
      );
    } finally {
      if (button) {
        button.disabled = false;
        button.textContent = "Знайти";
      }
    }
  }

  private normalizeSearchResult(item: any): SearchPerson {
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
      "Користувач";

    const login =
      item?.login ||
      item?.Login ||
      item?.loginEntyty ||
      item?.LoginEntyty ||
      item?.userName ||
      item?.UserName ||
      item?.email ||
      item?.Email ||
      "";

    return {
      login,
      fullName,
      role: item?.role || item?.Role,
      sport: item?.sport || item?.Sport || item?.typeSport || item?.TypeSport || item?.SportType,
      photo: item?.photo || item?.Photo || item?.profilePhoto || item?.ProfilePhoto,
      raw: item,
    };
  }

  private renderSearchResults() {
    const resultsContainer = document.getElementById("employee-results");

    if (!resultsContainer) return;

    if (!this.searchResults.length) {
      resultsContainer.innerHTML = "";
      return;
    }

    resultsContainer.innerHTML = this.searchResults
      .map((person, index) => `
        <div class="employee-result-card">
          <div class="employee-result-avatar">
            ${
              person.photo
                ? `<img src="${this.escapeHtml(person.photo)}" alt="${this.escapeHtml(person.fullName)}" />`
                : `<span>${this.escapeHtml(person.fullName.slice(0, 1).toUpperCase())}</span>`
            }
          </div>

          <div class="employee-result-info">
            <h3>${this.escapeHtml(person.fullName)}</h3>
            <p>Логін: ${this.escapeHtml(person.login)}</p>
            ${person.sport ? `<p>Спорт: ${this.escapeHtml(person.sport)}</p>` : ""}
            ${person.role ? `<p>Роль: ${this.escapeHtml(person.role)}</p>` : ""}
          </div>

          <button class="employee-add-btn" data-index="${index}" type="button">
            Додати як ${this.getRoleLabel(this.selectedRole)}
          </button>
        </div>
      `)
      .join("");

    resultsContainer.querySelectorAll<HTMLButtonElement>(".employee-add-btn")
      .forEach((button) => {
        button.addEventListener("click", async () => {
          const index = Number(button.dataset.index);
          const person = this.searchResults[index];

          if (person) {
            await this.linkEmployee(person, button);
          }
        });
      });
  }

  private async linkEmployee(person: SearchPerson, button: HTMLButtonElement) {
    const orgLogin = this.orgData.login || localStorage.getItem("login") || "";

    if (!orgLogin) {
      this.showEmployeeMessage("Не знайдено логін організації.", "error");
      return;
    }

    try {
      button.disabled = true;
      button.textContent = "Додавання...";

      await authApi.linkEmployee({
        loginOrganization: orgLogin,
        role: this.selectedRole,
        loginEntyty: person.login,
      });

      this.updateLocalOrganization(person);

      new NotificationKarina().show(
        `${person.fullName} додано як ${this.getRoleLabel(this.selectedRole)}.`,
        "success"
      );

      this.showEmployeeMessage(`${person.fullName} успішно додано в організацію.`, "success");

      await this.render();
    } catch (error) {
      console.error("Помилка додавання учасника:", error);

      this.showEmployeeMessage(
        error instanceof Error ? error.message : "Не вдалося додати учасника.",
        "error"
      );

      button.disabled = false;
      button.textContent = `Додати як ${this.getRoleLabel(this.selectedRole)}`;
    }
  }

  private updateLocalOrganization(person: SearchPerson) {
    const orgLogin = this.orgData.login || localStorage.getItem("login") || "";
    const organization: any = organizations.find((o: any) => o.login === orgLogin);

    if (!organization) return;

    if (this.selectedRole === "Judge") {
      if (!organization.OrganizationJudge) organization.OrganizationJudge = [];
      if (!organization.OrganizationJudge.includes(person.login)) {
        organization.OrganizationJudge.push(person.login);
      }
    }

    if (this.selectedRole === "Trainer") {
      if (!organization.OrganizationTrainer) organization.OrganizationTrainer = [];
      if (!organization.OrganizationTrainer.includes(person.login)) {
        organization.OrganizationTrainer.push(person.login);
      }
    }

    if (this.selectedRole === "Athlete") {
      if (!organization.OrganizationAthlete) organization.OrganizationAthlete = [];
      if (!organization.OrganizationAthlete.includes(person.login)) {
        organization.OrganizationAthlete.push(person.login);
      }
    }
  }

  private renderJudgesList() {
    if (!this.orgData.OrganizationJudge.length) {
      return `<div class="empty-state">Суддів поки немає</div>`;
    }

    return this.orgData.OrganizationJudge.map((judge: any) => `
      <div class="staff-card">
        <h3 class="staff-name">${this.escapeHtml(this.getFullName(judge))}</h3>
        <p class="staff-category">${this.escapeHtml(judge.Category || judge.category || "Суддя")}</p>
        <p class="staff-category">Логін: ${this.escapeHtml(judge.login || judge.Login || "")}</p>
      </div>
    `).join("");
  }

  private renderTrainersList() {
    if (!this.orgData.OrganizationTrainer.length) {
      return `<div class="empty-state">Тренерів поки немає</div>`;
    }

    return this.orgData.OrganizationTrainer.map((trainer: any) => `
      <div class="staff-card">
        <h3 class="staff-name">${this.escapeHtml(this.getFullName(trainer))}</h3>
        <p class="staff-sport">${this.getSportIcon(trainer.SportType || trainer.typeSport || "")} ${this.escapeHtml(trainer.SportType || trainer.typeSport || "-")}</p>
        <p class="staff-category">Логін: ${this.escapeHtml(trainer.login || trainer.Login || "")}</p>
      </div>
    `).join("");
  }

  private renderAthletesList() {
    if (!this.orgData.OrganizationAthlete.length) {
      return `<div class="empty-state">Атлетів поки немає</div>`;
    }

    return this.orgData.OrganizationAthlete.map((athlete: any) => `
      <div class="staff-card">
        <h3 class="staff-name">${this.escapeHtml(this.getFullName(athlete))}</h3>
        <p class="staff-sport">${this.getSportIcon(athlete.sport || athlete.typeSport || "")} ${this.escapeHtml(athlete.sport || athlete.typeSport || "-")}</p>
        <p class="staff-category">Логін: ${this.escapeHtml(athlete.login || athlete.Login || "")}</p>
      </div>
    `).join("");
  }

  private getFullName(entity: any) {
    return (
      entity?.name ||
      entity?.Name ||
      entity?.fullName ||
      entity?.FullName ||
      `${entity?.FirsName || entity?.firsName || entity?.fistName || entity?.FistName || ""} ${entity?.LastName || entity?.lastName || ""}`.trim() ||
      entity?.login ||
      entity?.Login ||
      "Користувач"
    );
  }

  private setDataOrganiz(): OrganizationView {
    const login = localStorage.getItem("login") ?? "";
    const organization: any = organizations.find((o: any) => o.login === login);

    const judgeLogins: string[] = organization?.OrganizationJudge ?? [];
    const trainerLogins: string[] = organization?.OrganizationTrainer ?? [];
    const athleteLogins: string[] = organization?.OrganizationAthlete ?? [];

    const organizationJudge = judges.filter((j: any) => judgeLogins.includes(j.login));
    const organizationTrainer = trainers.filter((t: any) => trainerLogins.includes(t.login));
    const organizationAthlete = users.filter((u: any) => athleteLogins.includes(u.login));

    const events = allEvents.filter((e: any) => (organization?.Events ?? []).includes(e.NameEvent));

    return {
      login,
      photo: organization?.photo ?? "",
      NameOrganization: organization?.NameOrganization ?? "",
      TypeOrganozation: organization?.TypeOrganozation ?? "",
      Description: organization?.Description ?? "",
      Country: organization?.Country ?? "",
      Teams: organization?.Teams ?? [],
      OrganizationJudge: organizationJudge,
      OrganizationTrainer: organizationTrainer,
      OrganizationAthlete: organizationAthlete,
      Events: events ?? [],
    };
  }

  private showEmployeeMessage(message: string, type: "success" | "error" | "info") {
    const element = document.getElementById("employee-message");

    if (!element) return;

    element.textContent = message;
    element.className = `employee-message ${message ? type : ""}`;
  }

  private getRoleLabel(role: EmployeeRole) {
    const labels: Record<EmployeeRole, string> = {
      Athlete: "атлета",
      Judge: "суддю",
      Trainer: "тренера",
    };

    return labels[role];
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      "Футбол": "⚽",
      Football: "⚽",
      "Баскетбол": "🏀",
      Basketball: "🏀",
      "Гімнастика": "🤸",
      Gymnastics: "🤸",
      "Теніс": "🎾",
      Tennis: "🎾",
      "Волейбол": "🏐",
      Volleyball: "🏐",
      "Бокс": "🥊",
      Boxing: "🥊",
      "Боротьба": "🤼",
      Wrestling: "🤼",
      "Хокей": "🏒",
      Hockey: "🏒",
      "Бейсбол": "⚾",
      Baseball: "⚾",
      Chess: "♟️",
      "Шахи": "♟️",
    };

    return icons[sportType] || "🏅";
  }

  private getCountryFlag(country: string): string {
    const flags: Record<string, string> = {
      "Україна": "🇺🇦",
      "США": "🇺🇸",
      "Німеччина": "🇩🇪",
    };

    return flags[country] || "🌍";
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
