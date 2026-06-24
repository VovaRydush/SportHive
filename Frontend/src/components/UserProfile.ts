import { photoOrInitialsHtml, resolvePhotoUrl, escapeHtml, escapeAttr } from "../api/media";
import { userProfileApi, type ProfileMatch, type ProfileTeam, type UserProfileData } from "../api/userProfileApi";
import type { UserRole } from "../api/authToken";
import { extractScore, formatMatchDateTime, getEffectiveMatchStatus, statusUa } from "../api/matchStatus";
import { TeamPageLook } from "./TeamPage";
import "./userProfile.css";
import "./photoUi.css";

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
          <button id="profile-back" class="profile-back">← Назад</button>
          <div class="profile-error">${escapeHtml(error instanceof Error ? error.message : "Не вдалося завантажити профіль")}</div>
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
        <button id="profile-back" class="profile-back">← Назад</button>

        <header class="profile-hero">
          <div class="profile-avatar-wrap">
            ${photoOrInitialsHtml(image, data.fullName || data.login, "profile-avatar", "auth")}
            <span class="profile-role-badge">${this.roleLabel(data.role)}</span>
          </div>

          <div class="profile-main-info">
            <span class="profile-kicker">SportHive · ${this.roleLabel(data.role)}</span>
            <h1>${escapeHtml(data.fullName || data.login)}</h1>

            <div class="profile-meta">
              <span>Логін: <b>${escapeHtml(data.login)}</b></span>
              ${data.mail ? `<span>Email: <b>${escapeHtml(data.mail)}</b></span>` : ""}
              ${data.typeSport ? `<span>Спорт: <b>${escapeHtml(data.typeSport)}</b></span>` : ""}
              ${data.dataBirth ? `<span>Дата народження: <b>${this.date(data.dataBirth)}</b></span>` : ""}
              ${data.age ? `<span>Вік: <b>${data.age}</b></span>` : ""}
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

    this.container.querySelectorAll<HTMLElement>("[data-team]").forEach(button => {
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

      if ((isFirst && parts[0] > parts[1]) || (!isFirst && second.length > 0 && parts[1] > parts[0])) {
        wins++;
      }
    }

    return wins;
  }

  private renderTeams(teams: ProfileTeam[]) {
    const list = Array.isArray(teams) ? teams : [];

    if (!list.length) {
      return `<div class="profile-empty">Команд поки немає</div>`;
    }

    return `
      <div class="profile-list">
        ${list.map(team => `
          <button class="profile-list-item clickable photo-card-row" type="button" data-team="${escapeAttr(team.teamName)}">
            ${photoOrInitialsHtml(team.photoTeam, team.teamName, "list-photo", "command")}
            <div>
              <b>${escapeHtml(team.teamName)}</b>
              <span>${escapeHtml(team.typeSport || "-")}</span>
              <small>Тренер: ${escapeHtml(team.loginTrainer || "-")} · Спортсменів: ${team.athletesCount ?? 0}</small>
            </div>
          </button>
        `).join("")}
      </div>
    `;
  }

  private renderOrganizations() {
    const list = this.data?.organizations || [];

    if (!list.length) {
      return `<div class="profile-empty">Організацій поки немає</div>`;
    }

    return `
      <div class="profile-list">
        ${list.map(org => `
          <article class="profile-list-item photo-card-row">
            ${photoOrInitialsHtml(org.profilePhoto, org.nameOrganization || org.loginOrganization, "list-photo", "auth")}
            <div>
              <b>${escapeHtml(org.nameOrganization || org.loginOrganization)}</b>
              <span>${escapeHtml(org.loginOrganization)}</span>
              <small>${escapeHtml(org.country || "")}</small>
            </div>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMatches(matches: ProfileMatch[]) {
    const list = Array.isArray(matches) ? matches : [];

    if (!list.length) {
      return `<div class="profile-empty">Матчів поки немає</div>`;
    }

    return `
      <div class="profile-match-list">
        ${list.map(match => {
          const status = getEffectiveMatchStatus(match);
          const score = extractScore(match) || match.score || "vs";

          return `
            <article class="profile-match-card">
              <div>
                <b>${escapeHtml(match.nameEvent || "Матч")}</b>
                <span>${statusUa(status)}</span>
              </div>
              <p>${escapeHtml(match.firstSide || "-")} vs ${escapeHtml(match.secondSide || "-")}</p>
              <small>${formatMatchDateTime(match)} · Score: ${escapeHtml(score)} · R${match.tour || "-"}${match.group ? ` · Group ${match.group}` : ""}</small>
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
}

export default UserProfilePage;
