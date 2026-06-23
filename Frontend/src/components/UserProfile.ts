import { resolvePhotoUrl, initials } from "../api/media";
import { userProfileApi, type ProfileMatch, type ProfileTeam, type UserProfileData } from "../api/userProfileApi";
import type { UserRole } from "../api/authToken";
import { TeamPageLook } from "./TeamPage";
import "./userProfile.css";

export class UserProfilePage {
  private container: HTMLElement;
  private login?: string;
  private role?: UserRole;
  private selfMode: boolean;
  private data?: UserProfileData;

  constructor(containerId: string = "app", login?: string, role?: UserRole) {
    const element = document.getElementById(containerId);

    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
    this.login = login;
    this.role = role;
    this.selfMode = !login;
  }

  async render() {
    this.container.innerHTML = `
      <section class="user-profile-page">
        <div class="profile-loading">Завантаження профілю...</div>
      </section>
    `;

    try {
      this.data = this.selfMode
        ? await userProfileApi.getMe()
        : await userProfileApi.get(this.login || "", this.role || "Athlete");

      this.renderPage();
    } catch (error) {
      this.container.innerHTML = `
        <section class="user-profile-page">
          <button id="profile-back" class="profile-btn secondary" type="button">← Назад</button>
          <div class="profile-empty">${this.escape(error instanceof Error ? error.message : "Не вдалося завантажити профіль")}</div>
        </section>
      `;
      document.getElementById("profile-back")?.addEventListener("click", () => window.history.back());
    }
  }

  async show() {
    await this.render();
  }

  private renderPage() {
    if (!this.data) return;

    const data = this.data;
    const image = resolvePhotoUrl(data.profilePhoto, "auth");
    const allMatches = this.allMatches();

    this.container.innerHTML = `
      <section class="user-profile-page">
        <div class="profile-top-actions">
          <button id="profile-back" class="profile-btn secondary" type="button">← Назад</button>
          ${this.selfMode ? `<span class="profile-own-badge">Мій профіль</span>` : ""}
        </div>

        <header class="profile-hero">
          <div class="profile-avatar">
            ${image ? `<img src="${this.attr(image)}" alt="${this.attr(data.fullName)}" />` : `<span>${this.escape(initials(data.fullName || data.login))}</span>`}
            <b>${this.roleLabel(data.role)}</b>
          </div>

          <div class="profile-main">
            <span class="profile-kicker">SportHive · ${this.roleLabel(data.role)}</span>
            <h1>${this.escape(data.fullName || data.login)}</h1>
            <div class="profile-meta">
              <span>Логін: ${this.escape(data.login)}</span>
              ${data.mail ? `<span>Email: ${this.escape(data.mail)}</span>` : ""}
              ${data.typeSport ? `<span>Спорт: ${this.escape(data.typeSport)}</span>` : ""}
              ${data.dataBirth ? `<span>Дата народження: ${this.date(data.dataBirth)}</span>` : ""}
              ${data.age ? `<span>Вік: ${data.age}</span>` : ""}
            </div>
          </div>
        </header>

        <section class="profile-stats">
          <div><b>${data.stats.totalMatches}</b><span>Матчів</span></div>
          <div><b>${data.stats.finishedMatches}</b><span>Завершено</span></div>
          <div><b>${data.stats.wins}</b><span>Перемог</span></div>
          <div><b>${data.stats.teamsCount}</b><span>Команд</span></div>
        </section>

        <section class="profile-grid">
          <div class="profile-panel">
            <h2>Команди</h2>
            ${this.renderTeams(data.teams)}
          </div>

          <div class="profile-panel">
            <h2>Організації</h2>
            ${this.renderOrganizations()}
          </div>
        </section>

        <section class="profile-panel">
          <h2>${data.role === "Judge" ? "Матчі судді" : "Матчі"}</h2>
          ${this.renderMatches(data.role === "Judge" && data.judgedMatches.length ? data.judgedMatches : allMatches)}
        </section>

        <section class="profile-grid">
          <div class="profile-panel">
            <h2>Очікуються</h2>
            ${this.renderMatches(data.upcomingMatches)}
          </div>

          <div class="profile-panel">
            <h2>Завершені</h2>
            ${this.renderMatches(data.finishedMatches)}
          </div>
        </section>
      </section>
    `;

    document.getElementById("profile-back")?.addEventListener("click", () => window.history.back());

    this.container.querySelectorAll<HTMLButtonElement>("[data-team]").forEach(button => {
      button.addEventListener("click", () => {
        const team = button.dataset.team;
        if (team) new TeamPageLook("app", team).render();
      });
    });
  }

  private allMatches() {
    if (!this.data) return [];

    const map = new Map<string, ProfileMatch>();

    [...this.data.matches, ...this.data.judgedMatches].forEach((match, index) => {
      const key = `${match.type}-${match.id}-${match.idEvent}-${index}`;
      map.set(key, match);
    });

    return Array.from(map.values());
  }

  private renderTeams(teams: ProfileTeam[]) {
    const list = Array.isArray(teams) ? teams : [];

    if (!list.length) return `<div class="profile-empty">Команд поки немає</div>`;

    return `
      <div class="profile-list">
        ${list.map(team => `
          <button class="profile-list-item clickable" type="button" data-team="${this.attr(team.teamName)}">
            <b>${this.escape(team.teamName)}</b>
            <span>${this.escape(team.typeSport || "-")}</span>
            <small>Тренер: ${this.escape(team.loginTrainer || "-")} · Спортсменів: ${team.athletesCount ?? 0}</small>
          </button>
        `).join("")}
      </div>
    `;
  }

  private renderOrganizations() {
    const list = this.data?.organizations || [];

    if (!list.length) return `<div class="profile-empty">Організацій поки немає</div>`;

    return `
      <div class="profile-list">
        ${list.map(org => `
          <article class="profile-list-item">
            <b>${this.escape(org.nameOrganization || org.loginOrganization)}</b>
            <span>${this.escape(org.loginOrganization)}</span>
            <small>${this.escape(org.country || "")}</small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMatches(matches: ProfileMatch[]) {
    const list = Array.isArray(matches) ? matches : [];

    if (!list.length) return `<div class="profile-empty">Матчів поки немає</div>`;

    return `
      <div class="profile-match-list">
        ${list.map(match => `
          <article class="profile-match">
            <div class="match-row">
              <span>${this.escape(match.nameEvent || "Матч")}</span>
              <strong class="${this.statusClass(match.statusMatch)}">${this.status(match.statusMatch)}</strong>
            </div>
            <b>${this.escape(match.firstSide || "-")} vs ${this.escape(match.secondSide || "-")}</b>
            <small>
              ${match.dataMatch ? this.date(match.dataMatch) : "-"}
              · ${match.score ? `Score: ${this.escape(match.score)}` : "score -"}
              · R${match.tour || "-"}
              ${match.group ? ` · Group ${match.group}` : ""}
            </small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private roleLabel(role: string) {
    if (role === "Organization") return "Організація";
    if (role === "Trainer") return "Тренер";
    if (role === "Judge") return "Суддя";
    return "Спортсмен";
  }

  private status(status?: number) {
    if (status === 2) return "Finished";
    if (status === 1) return "Live";
    return "Upcoming";
  }

  private statusClass(status?: number) {
    if (status === 2) return "status finished";
    if (status === 1) return "status live";
    return "status upcoming";
  }

  private date(value: string) {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? "-" : date.toLocaleDateString("uk-UA");
  }

  private escape(value: unknown) {
    return String(value ?? "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }

  private attr(value: unknown) {
    return this.escape(value).replace(/`/g, "&#096;");
  }
}

export default UserProfilePage;
