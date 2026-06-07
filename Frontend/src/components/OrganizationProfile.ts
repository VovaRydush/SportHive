import { organizationApi, type OrgRole, type OrgSearchUser, type OrganizationProfileData } from "../api/organizationApi";
import { NotificationKarina } from "./Notification";
import { CreateTeamModal } from "./CreateTeam";
import { TeamPageLook } from "./TeamPage";
import { UserProfilePage } from "./UserProfile";
import "./organizationProfile.css";

export class OrganizationProfile {
  private container: HTMLElement;
  private data?: OrganizationProfileData;
  private selectedUser: OrgSearchUser | null = null;
  private searchTimer?: number;

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);

    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="org-page">
        <div class="org-loading">Завантаження профілю організації...</div>
      </section>
    `;

    await this.load();
  }

  async show() {
    await this.render();
  }

  private async load() {
    const notify = new NotificationKarina();

    try {
      this.data = await organizationApi.getProfile();
      this.renderPage();
    } catch (error) {
      notify.show(error instanceof Error ? error.message : "Не вдалося завантажити організацію", "error");
      this.container.innerHTML = `<section class="org-page"><div class="org-empty">Не вдалося завантажити організацію</div></section>`;
    }
  }

  private renderPage() {
    if (!this.data) return;

    const org = this.data.organization;
    const athleteCount = Object.values(this.data.athletesBySport || {}).reduce((sum, arr) => sum + arr.length, 0);

    this.container.innerHTML = `
      <section class="org-page">
        <header class="org-hero">
          <div class="org-avatar">
            ${org.profilePhoto ? `<img src="${this.escapeAttr(org.profilePhoto)}" alt="" />` : `<span>SportHive</span>`}
            <b>ОРГАНІЗАЦІЯ</b>
          </div>

          <div class="org-main">
            <h1>${this.escapeHtml(org.nameOrganization || "Моя організація")}</h1>
            <div class="org-meta">
              <span>🌍 ${this.escapeHtml(org.country || "-")}</span>
              <span>Логін: ${this.escapeHtml(org.login)}</span>
              <span>Тип: ${this.escapeHtml(org.typeOrganization || "-")}</span>
            </div>

            <div class="org-about">
              <b>Про організацію</b>
              <p>${this.escapeHtml(org.description || "Опис організації поки не заповнений.")}</p>
            </div>
          </div>

          <div class="org-actions">
            <button id="create-team-btn" class="org-btn dark" type="button">+ Створити команду</button>
          </div>
        </header>

        <section class="org-stats">
          <div><b>${this.data.teams.length}</b><span>Команди</span></div>
          <div><b>${this.data.judges.length}</b><span>Судді</span></div>
          <div><b>${this.data.trainers.length}</b><span>Тренери</span></div>
          <div><b>${athleteCount}</b><span>Спортсмени</span></div>
        </section>

        <section class="org-panel">
          <div class="org-panel-head">
            <div>
              <h2>Додати учасника в організацію</h2>
              <p>Запрошення прийде користувачу на пошту. Після прийняття організація отримає email-повідомлення.</p>
            </div>
          </div>

          <div class="invite-grid">
            <label>
              Кого додати
              <select id="invite-role">
                <option value="Trainer">Тренера</option>
                <option value="Judge">Суддю</option>
                <option value="Athlete">Спортсмена</option>
              </select>
            </label>

            <label class="invite-search-wrap">
              Пошук і вибір
              <input id="invite-search" placeholder="Знайти за ПІБ, login або поштою..." autocomplete="off" />
              <div id="invite-results" class="invite-results" hidden></div>
            </label>

            <button id="send-invite-btn" class="org-btn dark" type="button">Надіслати запрошення</button>
          </div>

          <div id="selected-invite-user" class="selected-invite-user muted">
            Користувача ще не вибрано
          </div>
        </section>

        <section class="org-section">
          <div class="section-title">
            <h2>Наші команди</h2>
          </div>
          ${this.renderTeams()}
        </section>

        <section class="org-grid-two">
          <div class="org-section">
            <div class="section-title">
              <h2>Судді</h2>
            </div>
            ${this.renderMembers(this.data.judges, "Judge", "Суддів поки немає")}
          </div>

          <div class="org-section">
            <div class="section-title">
              <h2>Тренери</h2>
            </div>
            ${this.renderMembers(this.data.trainers, "Trainer", "Тренерів поки немає")}
          </div>
        </section>

        <section class="org-section">
          <div class="section-title">
            <h2>Атлети організації</h2>
          </div>
          ${this.renderAthletesBySport()}
        </section>

        <section class="org-section">
          <div class="section-title">
            <h2>Запрошення</h2>
          </div>
          ${this.renderInvitations()}
        </section>

        <section class="org-section">
          <div class="section-title">
            <h2>Останні події</h2>
          </div>
          ${this.renderEvents()}
        </section>
      </section>
    `;

    this.bind();
  }

  private bind() {
    document.getElementById("create-team-btn")?.addEventListener("click", () => {
      new CreateTeamModal("app").show();
    });

    this.container.querySelectorAll<HTMLButtonElement>("[data-team-name]").forEach(btn => {
      btn.addEventListener("click", () => {
        const name = btn.dataset.teamName;
        if (name) new TeamPageLook("app", name).render();
      });
    });

    this.container.querySelectorAll<HTMLButtonElement>("[data-profile-login]").forEach(btn => {
      btn.addEventListener("click", () => {
        const login = btn.dataset.profileLogin;
        const role = btn.dataset.profileRole as "Athlete" | "Trainer" | "Judge";

        if (login && role) new UserProfilePage("app", login, role).render();
      });
    });

    const role = document.getElementById("invite-role") as HTMLSelectElement | null;
    const search = document.getElementById("invite-search") as HTMLInputElement | null;

    role?.addEventListener("change", () => {
      this.selectedUser = null;
      this.renderSelectedUser();
      if (search) search.value = "";
      this.hideResults();
    });

    search?.addEventListener("input", () => {
      window.clearTimeout(this.searchTimer);
      this.searchTimer = window.setTimeout(() => this.searchUsers(), 250);
    });

    document.getElementById("send-invite-btn")?.addEventListener("click", () => this.sendInvite());
  }

  private async searchUsers() {
    const role = (document.getElementById("invite-role") as HTMLSelectElement | null)?.value as OrgRole;
    const input = document.getElementById("invite-search") as HTMLInputElement | null;
    const root = document.getElementById("invite-results");

    if (!input || !root) return;

    const query = input.value.trim();

    if (query.length < 2) {
      this.hideResults();
      return;
    }

    root.hidden = false;
    root.innerHTML = `<div class="invite-empty">Пошук...</div>`;

    try {
      const users = await organizationApi.searchUsers(role, query);

      if (!users.length) {
        root.innerHTML = `<div class="invite-empty">Користувачів не знайдено або вони вже додані/запрошені</div>`;
        return;
      }

      root.innerHTML = users.map(u => `
        <button class="invite-result" data-login="${this.escapeAttr(u.login)}" type="button">
          <span class="invite-avatar">${this.escapeHtml((u.fullName || u.login)[0] || "?")}</span>
          <span>
            <b>${this.escapeHtml(u.fullName || u.login)}</b>
            <small>${this.escapeHtml(u.mail || "")}${u.typeSport ? ` · ${this.escapeHtml(u.typeSport)}` : ""}</small>
          </span>
        </button>
      `).join("");

      root.querySelectorAll<HTMLButtonElement>(".invite-result").forEach(btn => {
        btn.addEventListener("click", () => {
          const user = users.find(x => x.login === btn.dataset.login);
          if (!user) return;

          this.selectedUser = user;
          input.value = "";
          this.hideResults();
          this.renderSelectedUser();
        });
      });
    } catch (error) {
      root.innerHTML = `<div class="invite-empty error">${this.escapeHtml(error instanceof Error ? error.message : "Помилка пошуку")}</div>`;
    }
  }

  private async sendInvite() {
    const notify = new NotificationKarina();

    if (!this.selectedUser) {
      notify.show("Спочатку обери користувача", "info");
      return;
    }

    try {
      await organizationApi.sendInvitation(this.selectedUser.login, this.selectedUser.role);
      notify.show("Запрошення надіслано на пошту", "success");
      this.selectedUser = null;
      this.renderSelectedUser();
      await this.load();
    } catch (error) {
      notify.show(error instanceof Error ? error.message : "Не вдалося надіслати запрошення", "error");
    }
  }

  private renderSelectedUser() {
    const root = document.getElementById("selected-invite-user");
    if (!root) return;

    if (!this.selectedUser) {
      root.innerHTML = "Користувача ще не вибрано";
      root.classList.add("muted");
      return;
    }

    root.classList.remove("muted");
    root.innerHTML = `
      <span class="org-chip">
        <b>${this.escapeHtml(this.selectedUser.fullName || this.selectedUser.login)}</b>
        <small>${this.escapeHtml(this.selectedUser.role)} · ${this.escapeHtml(this.selectedUser.mail || this.selectedUser.login)}</small>
        <button id="clear-selected-user" type="button">×</button>
      </span>
    `;

    document.getElementById("clear-selected-user")?.addEventListener("click", () => {
      this.selectedUser = null;
      this.renderSelectedUser();
    });
  }

  private hideResults() {
    const root = document.getElementById("invite-results");
    if (!root) return;
    root.hidden = true;
    root.innerHTML = "";
  }

  private renderTeams() {
    if (!this.data?.teams.length) return `<div class="org-empty">Команд поки немає</div>`;

    return `
      <div class="teams-grid">
        ${this.data.teams.map(t => `
          <button class="team-card clickable-card" type="button" data-team-name="${this.escapeAttr(t.teamName)}">
            <div class="team-cover">${this.escapeHtml(t.teamName[0] || "T")}</div>
            <h3>${this.escapeHtml(t.teamName)}</h3>
            <p>Вид спорту: ${this.escapeHtml(t.typeSport || "-")}</p>
            <p>Тренер: ${this.escapeHtml(t.loginTrainer || "-")}</p>
            <p>Спортсменів: ${t.athletesCount}</p>
            <small>Переглянути команду →</small>
          </button>
        `).join("")}
      </div>
    `;
  }

  private renderMembers(items: any[], role: "Trainer" | "Judge", empty: string) {
    if (!items.length) return `<div class="org-empty">${this.escapeHtml(empty)}</div>`;

    return `
      <div class="member-list">
        ${items.map(m => `
          <button class="member-card clickable-card" type="button" data-profile-login="${this.escapeAttr(m.login)}" data-profile-role="${role}">
            <b>${this.escapeHtml(m.fullName || m.login)}</b>
            <span>${this.escapeHtml(m.login)}</span>
            ${m.mail ? `<small>${this.escapeHtml(m.mail)}</small>` : ""}
            <em>Переглянути профіль →</em>
          </button>
        `).join("")}
      </div>
    `;
  }

  private renderAthletesBySport() {
    const groups = this.data?.athletesBySport || {};
    const sports = Object.keys(groups).sort();

    if (!sports.length) return `<div class="org-empty">Атлетів поки немає</div>`;

    return `
      <div class="sport-athlete-groups">
        ${sports.map(sport => `
          <section class="sport-group">
            <h3>${this.escapeHtml(sport)} <span>${groups[sport].length}</span></h3>
            <div class="member-list">
              ${groups[sport].map(a => `
                <button class="member-card clickable-card" type="button" data-profile-login="${this.escapeAttr(a.login)}" data-profile-role="Athlete">
                  <b>${this.escapeHtml(a.fullName || a.login)}</b>
                  <span>${this.escapeHtml(a.login)}</span>
                  ${a.mail ? `<small>${this.escapeHtml(a.mail)}</small>` : ""}
                  <em>Переглянути профіль →</em>
                </button>
              `).join("")}
            </div>
          </section>
        `).join("")}
      </div>
    `;
  }

  private renderInvitations() {
    const invitations = this.data?.invitations || [];

    if (!invitations.length) return `<div class="org-empty">Запрошень поки немає</div>`;

    return `
      <div class="invite-list">
        ${invitations.map(i => `
          <article class="invite-card ${this.escapeAttr(i.status.toLowerCase())}">
            <b>${this.escapeHtml(i.targetLogin)}</b>
            <span>${this.escapeHtml(i.targetRole)} · ${this.escapeHtml(i.targetEmail)}</span>
            <small>${this.escapeHtml(i.status)} · ${this.formatDate(i.createdAt)}</small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderEvents() {
    const events = this.data?.recentEvents || [];

    if (!events.length) return `<div class="org-empty">Подій поки немає</div>`;

    return `
      <div class="event-list">
        ${events.map(e => `
          <article class="org-event-card">
            <b>${this.escapeHtml(e.nameEvent)}</b>
            <span>${this.escapeHtml(e.typeSport)} · ${this.escapeHtml(e.systems)}</span>
            <small>${this.formatDate(e.dataStart)} · ${e.finishedMatches}/${e.totalMatches} матчів завершено</small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private formatDate(value: string) {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? "-" : date.toLocaleDateString("uk-UA");
  }

   private escapeHtml(value: unknown) {
    return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
  }

  private escapeAttr(value: unknown) {
    return this.escapeHtml(value).replace(/`/g, "&#096;");
  }
}

export default OrganizationProfile;
