import { commandApi } from "../api/commandApi";
import type { AthleteTeamInfo, TeamInfoDto, TeamMatchInfo } from "../api/commandTypes";
import { resolvePhotoUrl, initials } from "../api/media";
import { UserProfilePage } from "./UserProfile";
import "./teamPage.css";

export class TeamPageLook {
  private container: HTMLElement;
  private teamName: string;
  private teamData?: TeamInfoDto;

  constructor(containerId: string, nameTeam: string) {
    const element = document.getElementById(containerId);

    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
    this.teamName = nameTeam;
  }

  async render() {
    this.container.innerHTML = `
      <section class="team-view-page">
        <div class="team-loading">Завантаження команди...</div>
      </section>
    `;

    try {
      this.teamData = await commandApi.getTeam(this.teamName);
      this.renderPage();
    } catch (error) {
      this.container.innerHTML = `
        <section class="team-view-page">
          <button id="team-back" class="team-view-btn secondary" type="button">← Назад</button>
          <div class="team-empty">${this.escape(error instanceof Error ? error.message : "Не вдалося завантажити команду")}</div>
        </section>
      `;
      document.getElementById("team-back")?.addEventListener("click", () => window.history.back());
    }
  }

  private renderPage() {
    if (!this.teamData) return;

    const team = this.teamData;
    const name = this.pick(team, ["nameTeam", "NameTeam"]) || this.teamName;
    const sport = this.pick(team, ["typeSport", "TypeSport"]);
    const trainerLogin = this.pick(team, ["trainerLogin", "TrainerLogin"]);
    const trainerName = [
      this.pick(team, ["trainerFirstName", "TrainerFirstName"]),
      this.pick(team, ["trainerLastName", "TrainerLastName"]),
    ].filter(Boolean).join(" ") || trainerLogin || "-";

    const teamPhoto = this.pick(team, ["photoTeam", "PhotoTeam", "teamPhoto", "TeamPhoto"]);
    const athletes = this.array<AthleteTeamInfo>(team.athletes ?? team.Athletes);
    const organizations = this.array<any>(team.organizations ?? team.Organizations);
    const matches = this.array<TeamMatchInfo>(team.matches ?? team.Matches);
    const stats = team.stats ?? this.buildStats(matches, name);
    const image = resolvePhotoUrl(teamPhoto, "command");

    this.container.innerHTML = `
      <section class="team-view-page">
        <button id="team-back" class="team-view-btn secondary" type="button">← Назад</button>

        <header class="team-view-hero">
          <div class="team-view-logo">
            ${image ? `<img src="${this.attr(image)}" alt="${this.attr(name)}" />` : `<span>${this.escape(initials(name))}</span>`}
            <b>КОМАНДА</b>
          </div>

          <div class="team-view-main">
            <span class="team-kicker">SportHive · команда</span>
            <h1>${this.escape(name)}</h1>
            <div class="team-meta">
              <span>🏅 ${this.escape(sport || "-")}</span>
              <button class="link-like" type="button" data-user-login="${this.attr(trainerLogin)}" data-user-role="Trainer">
                Тренер: ${this.escape(trainerName)}
              </button>
              <span>Спортсменів: ${athletes.length}</span>
            </div>
          </div>
        </header>

        <section class="team-view-stats">
          <div><b>${stats.totalMatches}</b><span>Матчів</span></div>
          <div><b>${stats.finishedMatches}</b><span>Завершено</span></div>
          <div><b>${stats.wins}</b><span>Перемог</span></div>
          <div><b>${stats.losses}</b><span>Поразок</span></div>
        </section>

        <section class="team-view-grid">
          <div class="team-view-panel">
            <h2>Спортсмени</h2>
            ${this.renderAthletes(athletes)}
          </div>

          <div class="team-view-panel">
            <h2>Організації</h2>
            ${this.renderOrganizations(organizations)}
          </div>
        </section>

        <section class="team-view-panel">
          <h2>Матчі команди</h2>
          ${this.renderMatches(matches)}
        </section>
      </section>
    `;

    document.getElementById("team-back")?.addEventListener("click", () => window.history.back());

    this.container.querySelectorAll<HTMLButtonElement>("[data-user-login]").forEach(button => {
      button.addEventListener("click", () => {
        const login = button.dataset.userLogin || "";
        const role = (button.dataset.userRole || "Athlete") as "Athlete" | "Trainer" | "Judge";

        if (login) new UserProfilePage("app", login, role).render();
      });
    });
  }

  private renderAthletes(athletes: AthleteTeamInfo[]) {
    if (!athletes.length) return `<div class="team-empty">Спортсменів поки немає</div>`;

    return `
      <div class="team-list">
        ${athletes.map(athlete => {
          const login = this.pick(athlete, ["login", "Login"]);
          const fullName =
            this.pick(athlete, ["fullName", "FullName"]) ||
            [this.pick(athlete, ["firsName", "FirsName"]), this.pick(athlete, ["lastName", "LastName"])].filter(Boolean).join(" ") ||
            login;
          const photo = resolvePhotoUrl(this.pick(athlete, ["photo", "Photo", "profilePhoto", "ProfilePhoto"]), "auth");

          return `
            <button class="team-list-item clickable" type="button" data-user-login="${this.attr(login)}" data-user-role="Athlete">
              <span class="mini-avatar">${photo ? `<img src="${this.attr(photo)}" alt="" />` : this.escape(initials(fullName || login))}</span>
              <b>${this.escape(fullName || login)}</b>
              <span>${this.escape(login)}</span>
              <small>${this.escape(this.pick(athlete, ["athleteStatus", "AthleteStatus"]) || "Active")}</small>
            </button>
          `;
        }).join("")}
      </div>
    `;
  }

  private renderOrganizations(organizations: any[]) {
    if (!organizations.length) return `<div class="team-empty">Організацій поки немає</div>`;

    return `
      <div class="team-list">
        ${organizations.map(organization => `
          <article class="team-list-item">
            <b>${this.escape(this.pick(organization, ["nameOrganization", "NameOrganization"]) || this.pick(organization, ["loginOrganization", "LoginOrganization"]))}</b>
            <span>${this.escape(this.pick(organization, ["loginOrganization", "LoginOrganization"]))}</span>
            <small>${this.escape(this.pick(organization, ["country", "Country"]) || "")}</small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMatches(matches: TeamMatchInfo[]) {
    if (!matches.length) return `<div class="team-empty">Матчів поки немає</div>`;

    return `
      <div class="team-match-list">
        ${matches.map(match => `
          <article class="team-match-card">
            <span>${this.escape(match.nameEvent || "Матч")}</span>
            <b>${this.escape(match.firstTeam || "-")} vs ${this.escape(match.secondTeam || "-")}</b>
            <small>
              ${match.dataMatch ? this.date(match.dataMatch) : "-"}
              · ${this.escape(match.score || "score -")}
              · ${this.status(match.statusMatch)}
              · R${match.tour ?? "-"}
            </small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private buildStats(matches: TeamMatchInfo[], teamName: string) {
    let wins = 0;
    let losses = 0;
    let draws = 0;
    let finishedMatches = 0;

    for (const match of matches) {
      if (match.statusMatch !== 2) continue;

      finishedMatches++;

      const parts = String(match.score || "").split(":").map(item => Number(item));

      if (parts.length !== 2 || parts.some(Number.isNaN)) continue;

      const isFirst = match.firstTeam === teamName;

      if (parts[0] === parts[1]) draws++;
      else if ((isFirst && parts[0] > parts[1]) || (!isFirst && parts[1] > parts[0])) wins++;
      else losses++;
    }

    return {
      totalMatches: matches.length,
      finishedMatches,
      wins,
      losses,
      draws,
    };
  }

  private pick(obj: any, keys: string[]) {
    for (const key of keys) {
      const value = obj?.[key];
      if (value !== undefined && value !== null && String(value).trim() !== "") return String(value);
    }

    return "";
  }

  private array<T>(value: T[] | undefined | null) {
    return Array.isArray(value) ? value : [];
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
