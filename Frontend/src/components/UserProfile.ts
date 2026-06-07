import { userProfileApi, type UserProfileData, type UserRole } from "../api/userProfileApi";
import { TeamPageLook } from "./TeamPage";
import "./userProfile.css";

export class UserProfilePage {
  private container: HTMLElement;
  private login: string;
  private role: UserRole;
  private data?: UserProfileData;

  constructor(containerId: string = "app", login: string, role: UserRole) {
    const element = document.getElementById(containerId);

    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
    this.login = login;
    this.role = role;
  }

  async render() {
    this.container.innerHTML = `
      <section class="user-profile-page">
        <div class="profile-loading">Завантаження профілю...</div>
      </section>
    `;

    try {
      this.data = await userProfileApi.get(this.login, this.role);
      this.renderPage();
    } catch (error) {
      this.container.innerHTML = `
        <section class="user-profile-page">
          <div class="profile-empty">${this.escapeHtml(error instanceof Error ? error.message : "Не вдалося завантажити профіль")}</div>
        </section>
      `;
    }
  }

  async show() {
    await this.render();
  }

  private renderPage() {
    if (!this.data) return;

    const d = this.data;

    this.container.innerHTML = `
      <section class="user-profile-page">
        <button id="profile-back" class="profile-btn secondary" type="button">← Назад</button>

        <header class="profile-hero">
          <div class="profile-avatar">
            ${d.profilePhoto ? `<img src="${this.escapeAttr(d.profilePhoto)}" alt="" />` : `<span>${this.escapeHtml((d.fullName || d.login)[0] || "?")}</span>`}
            <b>${this.escapeHtml(this.roleLabel(d.role))}</b>
          </div>

          <div class="profile-main">
            <span class="profile-kicker">SportHive · профіль</span>
            <h1>${this.escapeHtml(d.fullName || d.login)}</h1>

            <div class="profile-meta">
              <span>Логін: ${this.escapeHtml(d.login)}</span>
              ${d.mail ? `<span>Email: ${this.escapeHtml(d.mail)}</span>` : ""}
              ${d.typeSport ? `<span>Спорт: ${this.escapeHtml(d.typeSport)}</span>` : ""}
              ${d.dataBirth ? `<span>Дата народження: ${this.date(d.dataBirth)}</span>` : ""}
              ${d.age ? `<span>Вік: ${d.age}</span>` : ""}
            </div>
          </div>
        </header>

        <section class="profile-stats">
          <div><b>${d.stats?.totalMatches ?? 0}</b><span>Матчів</span></div>
          <div><b>${d.stats?.finishedMatches ?? 0}</b><span>Завершено</span></div>
          <div><b>${d.stats?.wins ?? 0}</b><span>Перемог</span></div>
          <div><b>${d.stats?.losses ?? 0}</b><span>Поразок</span></div>
        </section>

        <section class="profile-grid">
          <div class="profile-panel">
            <h2>Команди</h2>
            ${this.renderTeams()}
          </div>

          <div class="profile-panel">
            <h2>Організації</h2>
            ${this.renderOrganizations()}
          </div>
        </section>

        <section class="profile-panel">
          <h2>Останні матчі</h2>
          ${this.renderMatches()}
        </section>
      </section>
    `;

    document.getElementById("profile-back")?.addEventListener("click", () => window.history.back());

    this.container.querySelectorAll<HTMLButtonElement>("[data-team]").forEach(btn => {
      btn.addEventListener("click", () => {
        const name = btn.dataset.team;
        if (name) new TeamPageLook("app", name).render();
      });
    });
  }

  private renderTeams() {
    const teams = this.data?.teams || [];

    if (!teams.length) return `<div class="profile-empty">Команд поки немає</div>`;

    return `
      <div class="profile-list">
        ${teams.map(team => `
          <button class="profile-list-item clickable" type="button" data-team="${this.escapeAttr(team.teamName)}">
            <b>${this.escapeHtml(team.teamName)}</b>
            <span>${this.escapeHtml(team.typeSport || "-")} · тренер: ${this.escapeHtml(team.loginTrainer || "-")}</span>
            <small>Спортсменів: ${team.athletesCount ?? 0}</small>
          </button>
        `).join("")}
      </div>
    `;
  }

  private renderOrganizations() {
    const orgs = this.data?.organizations || [];

    if (!orgs.length) return `<div class="profile-empty">Організацій поки немає</div>`;

    return `
      <div class="profile-list">
        ${orgs.map(org => `
          <article class="profile-list-item">
            <b>${this.escapeHtml(org.nameOrganization || org.loginOrganization)}</b>
            <span>${this.escapeHtml(org.loginOrganization)}</span>
            ${org.country ? `<small>${this.escapeHtml(org.country)}</small>` : ""}
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMatches() {
    const matches = this.data?.matches || [];

    if (!matches.length) return `<div class="profile-empty">Матчів поки немає</div>`;

    return `
      <div class="profile-match-list">
        ${matches.map(match => `
          <article class="profile-match">
            <span>${this.escapeHtml(match.nameEvent || "Матч")}</span>
            <b>${this.escapeHtml(match.firstSide || "-")} vs ${this.escapeHtml(match.secondSide || "-")}</b>
            <small>
              ${match.dataMatch ? this.date(match.dataMatch) : "-"}
              · ${this.escapeHtml(match.score || "score -")}
              · ${this.status(match.statusMatch)}
            </small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private roleLabel(role: string) {
    if (role === "Trainer") return "Тренер";
    if (role === "Judge") return "Суддя";
    return "Спортсмен";
  }

  private status(status?: number) {
    if (status === 2) return "Finished";
    if (status === 1) return "Live";
    return "Upcoming";
  }

  private date(value: string) {
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
