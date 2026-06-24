import {
  organizationApi,
  type OrgInvite,
  type OrgMember,
  type OrgRole,
  type OrgSearchUser,
  type OrgTeam,
  type OrganizationProfileData,
} from "../api/organizationApi";
import { eventCatalogApi } from "../api/eventCatalogApi";
import { NotificationKarina } from "./Notification";
import { CreateTeamModal } from "./CreateTeam";
import { TeamPageLook } from "./TeamPage";
import { UserProfilePage } from "./UserProfile";
import {
  calculateEventStats,
  formatFinishedCounter,
  normalizeEventForUi,
} from "../api/tournamentResultUtils";
import {
  openOrganizationRosterEditor,
  openTeamEditor,
  removeOrganizationMember,
} from "./OrganizationManagementPanels";
import "./organizationProfile.css";

type AnyObj = Record<string, any>;

export class OrganizationProfile {
  private container: HTMLElement;
  private data?: OrganizationProfileData;
  private catalogEvents: AnyObj[] = [];
  private selectedUser: OrgSearchUser | null = null;
  private searchTimer?: number;

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="organization-profile">
        <div class="profile-loading">Завантаження профілю організації...</div>
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
      const [profile, events] = await Promise.all([
        organizationApi.getProfile(),
        eventCatalogApi.getEvents({}).catch(() => []),
      ]);

      this.data = profile;
      this.catalogEvents = (events || []).map(event => normalizeEventForUi(event as AnyObj));
      this.renderPage();
    } catch (error) {
      notify.show(error instanceof Error ? error.message : "Не вдалося завантажити організацію", "error");
      this.container.innerHTML = `<section class="organization-profile"><div class="empty-state">Не вдалося завантажити організацію</div></section>`;
    }
  }

  private renderPage() {
    if (!this.data) return;

    const org = this.data.organization;
    const athleteCount = Object.values(this.data.athletesBySport || {}).reduce((sum, arr) => sum + arr.length, 0);
    const events = this.resolveOrganizationEvents();

    this.container.innerHTML = `
      <section class="organization-profile">
        <header class="org-header">
          <div class="org-logo-container">
            ${org.profilePhoto
              ? `<img class="org-logo" src="${this.escapeAttr(org.profilePhoto)}" alt="${this.escapeAttr(org.nameOrganization)}" />`
              : `<div class="org-logo org-logo-placeholder">SportHive</div>`
            }
            <span class="org-type-badge">${this.escapeHtml(org.typeOrganization || "Організація")}</span>
          </div>

          <div class="org-main-info">
            <span class="org-label">SportHive · Організація</span>
            <h1 class="org-title">${this.escapeHtml(org.nameOrganization || "Моя організація")}</h1>

            <div class="org-meta">
              <span>Логін: <b>${this.escapeHtml(org.login || "-")}</b></span>
              <span>Країна: <b>${this.escapeHtml(org.country || "-")}</b></span>
              <span>Тип: <b>${this.escapeHtml(org.typeOrganization || "-")}</b></span>
            </div>

            <div class="org-description-block">
              <h3>Про організацію</h3>
              <p class="org-description">${this.escapeHtml(org.description || "Опис організації поки не заповнений.")}</p>
            </div>

            <div class="org-actions">
              <button id="create-team-btn" class="employee-btn" type="button">+ Створити команду</button>
              <button id="manage-org-roster-btn" class="employee-btn" type="button">Керувати складом</button>
            </div>
          </div>
        </header>

        <section class="org-stats">
          <div class="org-stat-card"><b>${this.data.teams.length}</b><span>Команди</span></div>
          <div class="org-stat-card"><b>${this.data.judges.length}</b><span>Судді</span></div>
          <div class="org-stat-card"><b>${this.data.trainers.length}</b><span>Тренери</span></div>
          <div class="org-stat-card"><b>${athleteCount}</b><span>Спортсмени</span></div>
        </section>

        <section class="employee-manager">
          <h2 class="employee-title">Додати учасника через запрошення</h2>
          <p class="employee-subtitle">Запрошення прийде користувачу на пошту. Для прямого додавання/видалення натисни “Керувати складом”.</p>

          <div class="employee-form">
            <div class="employee-field">
              <label for="invite-role">Кого додати</label>
              <select id="invite-role">
                <option value="Trainer">Тренера</option>
                <option value="Judge">Суддю</option>
                <option value="Athlete">Спортсмена</option>
              </select>
            </div>

            <div class="employee-field">
              <label for="invite-search">Пошук і вибір</label>
              <input id="invite-search" placeholder="Введи імʼя, email або login..." autocomplete="off" />
            </div>

            <button id="send-invite-btn" class="employee-btn" type="button">Надіслати запрошення</button>
          </div>

          <div id="invite-results" class="employee-results" hidden></div>
          <div id="selected-invite-user" class="selected-user muted">Користувача ще не вибрано</div>
        </section>

        <section class="org-section">
          <h2 class="section-title">Наші команди</h2>
          ${this.renderTeams(this.data.teams)}
        </section>

        <section class="org-staff-section">
          <div class="org-section">
            <h2 class="section-title">Судді</h2>
            ${this.renderMembers(this.data.judges, "Judge", "Суддів поки немає")}
          </div>

          <div class="org-section">
            <h2 class="section-title">Тренери</h2>
            ${this.renderMembers(this.data.trainers, "Trainer", "Тренерів поки немає")}
          </div>
        </section>

        <section class="org-section">
          <h2 class="section-title">Атлети організації</h2>
          ${this.renderAthletesBySport()}
        </section>

        <section class="org-section">
          <h2 class="section-title">Останні події</h2>
          ${this.renderEvents(events)}
        </section>

        <section class="org-section">
          <h2 class="section-title">Запрошення</h2>
          ${this.renderInvitations(this.data.invitations)}
        </section>
      </section>
    `;

    this.bind();
  }

  private bind() {
    document.getElementById("create-team-btn")?.addEventListener("click", () => new CreateTeamModal("app").show());

    document.getElementById("manage-org-roster-btn")?.addEventListener("click", () => {
      const login = this.data?.organization.login || organizationApi.getLogin();
      openOrganizationRosterEditor(login, () => this.load());
    });

    this.container.querySelectorAll<HTMLElement>("[data-team-name]").forEach(button => {
      button.addEventListener("click", () => {
        const name = button.dataset.teamName;
        if (name) new TeamPageLook("app", name).render();
      });
    });

    this.container.querySelectorAll<HTMLElement>("[data-edit-team]").forEach(button => {
      button.addEventListener("click", event => {
        event.stopPropagation();
        const teamName = button.dataset.editTeam || "";
        const team = this.data?.teams.find(x => x.teamName === teamName);

        if (!team) return;

        openTeamEditor({
          teamName: team.teamName,
          typeSport: team.typeSport,
          loginTrainer: team.loginTrainer,
        }, () => this.load());
      });
    });

    this.container.querySelectorAll<HTMLElement>("[data-profile-login]").forEach(button => {
      button.addEventListener("click", () => {
        const login = button.dataset.profileLogin;
        const role = button.dataset.profileRole as "Athlete" | "Trainer" | "Judge";

        if (login && role) new UserProfilePage("app", login, role).render();
      });
    });

    this.container.querySelectorAll<HTMLElement>("[data-remove-member]").forEach(button => {
      button.addEventListener("click", event => {
        event.stopPropagation();
        const login = button.dataset.removeMember || "";
        const role = button.dataset.removeRole || "";
        const orgLogin = this.data?.organization.login || organizationApi.getLogin();

        if (!login || !role) return;

        removeOrganizationMember(orgLogin, role, login, () => this.load());
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
    root.innerHTML = `<div class="empty-state">Пошук...</div>`;

    try {
      const users = await organizationApi.searchUsers(role, query);

      if (!users.length) {
        root.innerHTML = `<div class="empty-state">Користувачів не знайдено або вони вже додані/запрошені</div>`;
        return;
      }

      root.innerHTML = users.map(user => `
        <button class="employee-result-card invite-result" type="button" data-login="${this.escapeAttr(user.login)}">
          <div class="employee-result-avatar">
            ${user.profilePhoto
              ? `<img src="${this.escapeAttr(user.profilePhoto)}" alt="${this.escapeAttr(user.fullName || user.login)}" />`
              : this.escapeHtml((user.fullName || user.login)[0] || "?")
            }
          </div>
          <div class="employee-result-info">
            <h3>${this.escapeHtml(user.fullName || user.login)}</h3>
            <p>${this.escapeHtml(user.mail || "")}</p>
            <p>${this.escapeHtml(user.role)}${user.typeSport ? ` · ${this.escapeHtml(user.typeSport)}` : ""}</p>
          </div>
          <span class="employee-add-btn">Обрати</span>
        </button>
      `).join("");

      root.querySelectorAll<HTMLElement>(".invite-result").forEach(button => {
        button.addEventListener("click", () => {
          const user = users.find(item => item.login === button.dataset.login);
          if (!user) return;

          this.selectedUser = user;
          input.value = "";
          this.hideResults();
          this.renderSelectedUser();
        });
      });
    } catch (error) {
      root.innerHTML = `<div class="empty-state">${this.escapeHtml(error instanceof Error ? error.message : "Помилка пошуку")}</div>`;
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
      <b>${this.escapeHtml(this.selectedUser.fullName || this.selectedUser.login)}</b>
      <span>${this.escapeHtml(this.selectedUser.role)} · ${this.escapeHtml(this.selectedUser.mail || this.selectedUser.login)}</span>
      <button id="clear-selected-user" type="button">×</button>
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

  private renderTeams(teams: OrgTeam[]) {
    if (!teams.length) return `<div class="empty-state">Команд поки немає</div>`;

    return `
      <div class="teams-grid">
        ${teams.map(team => `
          <article class="team-card">
            <button class="team-card-button" type="button" data-team-name="${this.escapeAttr(team.teamName)}">
              <header class="team-header">
                <span class="team-sport-icon">${this.sportIcon(team.typeSport)}</span>
                <h3 class="team-name">${this.escapeHtml(team.teamName)}</h3>
              </header>
              <div class="team-details">
                <p><b>Вид спорту:</b> ${this.escapeHtml(team.typeSport || "-")}</p>
                <p><b>Тренер:</b> ${this.escapeHtml(team.loginTrainer || "-")}</p>
                <p><b>Спортсменів:</b> ${team.athletesCount}</p>
                <span class="team-link">Переглянути команду →</span>
              </div>
            </button>

            <div class="team-manage-buttons">
              <button class="manage-small-btn" type="button" data-edit-team="${this.escapeAttr(team.teamName)}">Редагувати команду</button>
            </div>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMembers(items: OrgMember[], role: "Trainer" | "Judge", empty: string) {
    if (!items.length) return `<div class="empty-state">${this.escapeHtml(empty)}</div>`;

    return `
      <div class="staff-list">
        ${items.map(member => `
          <div class="staff-card">
            <button class="staff-main-button" type="button" data-profile-login="${this.escapeAttr(member.login)}" data-profile-role="${role}">
              <div class="staff-avatar">
                ${member.profilePhoto
                  ? `<img src="${this.escapeAttr(member.profilePhoto)}" alt="${this.escapeAttr(member.fullName || member.login)}" />`
                  : this.escapeHtml((member.fullName || member.login)[0] || "?")
                }
              </div>
              <div>
                <h3 class="staff-name">${this.escapeHtml(member.fullName || member.login)}</h3>
                <p class="staff-category">${this.escapeHtml(member.login)}</p>
                ${member.mail ? `<p class="staff-sport">${this.escapeHtml(member.mail)}</p>` : ""}
                ${member.typeSport ? `<p class="staff-sport">${this.escapeHtml(member.typeSport)}</p>` : ""}
                <span class="staff-link">Переглянути профіль →</span>
              </div>
            </button>

            <div class="member-actions">
              <button class="remove-small-btn" type="button" data-remove-member="${this.escapeAttr(member.login)}" data-remove-role="${role}">Видалити</button>
            </div>
          </div>
        `).join("")}
      </div>
    `;
  }

  private renderAthletesBySport() {
    const groups = this.data?.athletesBySport || {};
    const sports = Object.keys(groups).sort();

    if (!sports.length) return `<div class="empty-state">Атлетів поки немає</div>`;

    return `
      <div class="athlete-sport-groups">
        ${sports.map(sport => `
          <section class="athlete-sport-group">
            <h3>${this.escapeHtml(sport)} <span>${groups[sport].length}</span></h3>
            <div class="staff-list">
              ${groups[sport].map(athlete => `
                <div class="staff-card">
                  <button class="staff-main-button" type="button" data-profile-login="${this.escapeAttr(athlete.login)}" data-profile-role="Athlete">
                    <div class="staff-avatar">
                      ${athlete.profilePhoto
                        ? `<img src="${this.escapeAttr(athlete.profilePhoto)}" alt="${this.escapeAttr(athlete.fullName || athlete.login)}" />`
                        : this.escapeHtml((athlete.fullName || athlete.login)[0] || "?")
                      }
                    </div>
                    <div>
                      <h3 class="staff-name">${this.escapeHtml(athlete.fullName || athlete.login)}</h3>
                      <p class="staff-category">${this.escapeHtml(athlete.login)}</p>
                      ${athlete.mail ? `<p class="staff-sport">${this.escapeHtml(athlete.mail)}</p>` : ""}
                      <span class="staff-link">Переглянути профіль →</span>
                    </div>
                  </button>

                  <div class="member-actions">
                    <button class="remove-small-btn" type="button" data-remove-member="${this.escapeAttr(athlete.login)}" data-remove-role="Athlete">Видалити</button>
                  </div>
                </div>
              `).join("")}
            </div>
          </section>
        `).join("")}
      </div>
    `;
  }

  private resolveOrganizationEvents() {
    const recentEvents = this.data?.recentEvents || [];
    const byKey = new Map<string, AnyObj>();

    for (const event of this.catalogEvents) {
      const canManage = Boolean(event.canManageEvent || event.accessLevel === "Manage");
      const matchesRecent = recentEvents.some(recent => this.eventKey(recent) === this.eventKey(event) || this.sameEventNameDate(recent, event));
      if (canManage || matchesRecent) byKey.set(this.eventKey(event), normalizeEventForUi(event));
    }

    for (const recent of recentEvents) {
      const matched = this.catalogEvents.find(event => this.sameEventNameDate(recent, event));
      byKey.set(this.eventKey(recent), normalizeEventForUi(matched || recent));
    }

    return Array.from(byKey.values())
      .sort((a, b) => new Date(b.dataStart || b.DataStart || 0).getTime() - new Date(a.dataStart || a.DataStart || 0).getTime())
      .slice(0, 8);
  }

  private renderEvents(events: AnyObj[]) {
    if (!events.length) return `<div class="empty-state">Подій поки немає</div>`;

    return `
      <div class="events-timeline">
        ${events.map(eventInput => {
          const event = normalizeEventForUi(eventInput);
          const stats = calculateEventStats(event);
          const name = event.nameEvent || event.NameEvent || "-";
          const sport = event.typeSport || event.TypeSport || "-";
          const system = event.system || event.systems || event.Systems || "-";
          const date = this.formatDate(event.dataStart || event.DataStart);

          return `
            <article class="event-item">
              <div class="event-date">${date}</div>
              <div class="event-content">
                <h3 class="event-title">${this.escapeHtml(name)}</h3>
                <p class="event-description">${this.escapeHtml(sport)} · ${this.escapeHtml(system)}</p>
                <div class="event-meta">
                  <span>${stats.totalMatches} матчів</span>
                  <span>${formatFinishedCounter(event)} завершено</span>
                  <span>${stats.liveMatches} live</span>
                </div>
              </div>
            </article>
          `;
        }).join("")}
      </div>
    `;
  }

  private renderInvitations(invitations: OrgInvite[]) {
    if (!invitations.length) return `<div class="empty-state">Запрошень поки немає</div>`;

    return `
      <div class="invitation-list">
        ${invitations.map(invitation => `
          <article class="invitation-card ${this.escapeAttr(invitation.status.toLowerCase())}">
            <h3>${this.escapeHtml(invitation.targetLogin)}</h3>
            <p>${this.escapeHtml(invitation.targetRole)} · ${this.escapeHtml(invitation.targetEmail)}</p>
            <span>${this.escapeHtml(invitation.status)} · ${this.formatDate(invitation.createdAt)}</span>
          </article>
        `).join("")}
      </div>
    `;
  }

  private eventKey(event: AnyObj) {
    const id = event.idEvent || event.IdEvent;
    if (id) return `id:${id}`;
    return `${String(event.nameEvent || event.NameEvent || "").toLowerCase()}|${this.formatDate(event.dataStart || event.DataStart)}`;
  }

  private sameEventNameDate(first: AnyObj, second: AnyObj) {
    const firstName = String(first.nameEvent || first.NameEvent || "").trim().toLowerCase();
    const secondName = String(second.nameEvent || second.NameEvent || "").trim().toLowerCase();

    return Boolean(firstName && secondName && firstName === secondName && this.formatDate(first.dataStart || first.DataStart) === this.formatDate(second.dataStart || second.DataStart));
  }

  private formatDate(value: unknown) {
    if (!value) return "-";
    const date = new Date(String(value));
    return Number.isNaN(date.getTime()) ? "-" : date.toLocaleDateString("uk-UA");
  }

  private sportIcon(sport?: string | null) {
    const value = String(sport || "").toLowerCase();

    if (value.includes("football") || value.includes("фут")) return "⚽";
    if (value.includes("basket") || value.includes("бас")) return "🏀";
    if (value.includes("tennis") || value.includes("тен")) return "🎾";
    if (value.includes("volley") || value.includes("вол")) return "🏐";
    if (value.includes("chess") || value.includes("шах")) return "♟️";
    if (value.includes("box") || value.includes("бокс")) return "🥊";
    if (value.includes("hockey") || value.includes("хок")) return "🏒";
    return "🏆";
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
