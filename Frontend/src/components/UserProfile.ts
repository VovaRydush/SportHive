import { resolvePhotoUrl, initials } from "../api/media";
import { userProfileApi, type ProfileMatch, type ProfileTeam, type UserProfileData } from "../api/userProfileApi";
import type { UserRole } from "../api/authToken";
import { extractScore, formatMatchDateTime, getEffectiveMatchStatus, statusUa } from "../api/matchStatus";
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
          <div class="profile-empty">
            ${this.escape(error instanceof Error ? error.message : "Не вдалося завантажити профіль")}
          </div>
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
    const allMatches = this.normalizeMatches(this.allMatches());
    const finishedMatches = allMatches.filter(match => getEffectiveMatchStatus(match) === "Finished");
    const activeMatches = allMatches.filter(match => getEffectiveMatchStatus(match) !== "Finished");

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
          <div><b>${allMatches.length}</b><span>Матчів</span></div>
          <div><b>${finishedMatches.length}</b><span>Завершено</span></div>
          <div><b>${this.countWins(finishedMatches, data.login, data.fullName)}</b><span>Перемог</span></div>
          <div><b>${data.teams.length}</b><span>Команд</span></div>
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
          ${this.renderMatches(allMatches)}
        </section>

        <section class="profile-grid">
          <div class="profile-panel">
            <h2>Очікуються / Live</h2>
            ${this.renderMatches(activeMatches)}
          </div>

          <div class="profile-panel">
            <h2>Завершені</h2>
            ${this.renderMatches(finishedMatches)}
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

  private normalizeMatches(matches: ProfileMatch[]) {
    return matches.map(match => ({
      ...match,
      score: extractScore(match) || match.score || "",
      statusMatch: this.statusNumber(match),
    }));
  }

  private statusNumber(match: ProfileMatch) {
    const status = getEffectiveMatchStatus(match);

    if (status === "Finished") return 2;
    if (status === "Live") return 1;
    return 0;
  }

  private countWins(matches: ProfileMatch[], login: string, fullName: string) {
    let wins = 0;

    for (const match of matches) {
      const score = extractScore(match) || match.score || "";
      const parts = score.split(":").map(Number);

      if (parts.length !== 2 || parts.some(Number.isNaN) || parts[0] === parts[1]) continue;

      const first = String(match.firstSide || "");
      const second = String(match.secondSide || "");
      const isFirst = first === login || first === fullName;

      if ((isFirst && parts[0] > parts[1]) || (!isFirst && second.length > 0 && parts[1] > parts[0])) wins++;
    }

    return wins;
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
        ${list.map(match => {
          const status = getEffectiveMatchStatus(match);
          const score = extractScore(match) || match.score || "vs";

          return `
            <article class="profile-match">
              <div class="match-row">
                <span>${this.escape(match.nameEvent || "Матч")}</span>
                <strong class="status ${status.toLowerCase()}">${statusUa(status)}</strong>
              </div>
              <b>${this.escape(match.firstSide || "-")} vs ${this.escape(match.secondSide || "-")}</b>
              <small>
                ${formatMatchDateTime(match)}
                · Score: ${this.escape(score)}
                · R${match.tour || "-"}
                ${match.group ? ` · Group ${match.group}` : ""}
              </small>
            </article>
          `;
        }).join("")}
      </div>
    `;
  }

  private roleLabel(role: string) {
    if (role === "Organization") return "Організація";
    if (role === "Trainer") return "Тренер";
    if (role === "Judge") return "Суддя";
    return "Спортсмен";
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
