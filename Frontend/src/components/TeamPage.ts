import { commandApi } from "../api/commandApi";
import type { AthleteTeamInfo, TeamInfoDto, TeamMatchInfo } from "../api/commandTypes";
import { UserProfilePage } from "./UserProfile";
import "./teamPage.css";

export class TeamPageLook {
  private container: HTMLElement;
  private teamName: string;
  private teamData?: TeamInfoDto;

  constructor(containerId: string, nameTeam: string) {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

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
          <div class="team-empty">${this.escapeHtml(error instanceof Error ? error.message : "Не вдалося завантажити команду")}</div>
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

    const athletes = this.array<AthleteTeamInfo>(team.athletes ?? team.Athletes);
    const orgs = this.array<any>(team.organizations ?? team.Organizations);
    const matches = this.array<TeamMatchInfo>(team.matches ?? team.Matches);
    const stats = team.stats ?? this.buildStats(matches, name);

    this.container.innerHTML = `
      <section class="team-view-page">
        <button id="team-back" class="team-view-btn secondary" type="button">← Назад</button>

        <header class="team-view-hero">
          <div class="team-view-logo">
            ${this.pick(team, ["photoTeam", "PhotoTeam"])
              ? `<img src="${this.escapeAttr(commandApi.getPhotoUrl(this.pick(team, ["photoTeam", "PhotoTeam"])))}" alt="" />`
              : `<span>${this.escapeHtml(name[0] || "T")}</span>`}
            <b>КОМАНДА</b>
          </div>

          <div class="team-view-main">
            <span class="team-kicker">SportHive · команда</span>
            <h1>${this.escapeHtml(name)}</h1>

            <div class="team-meta">
              <span>🏅 ${this.escapeHtml(sport || "-")}</span>
              <button class="link-like" type="button" data-user-login="${this.escapeAttr(trainerLogin)}" data-user-role="Trainer">
                Тренер: ${this.escapeHtml(trainerName)}
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
            ${this.renderOrganizations(orgs)}
          </div>
        </section>

        <section class="team-view-panel">
          <h2>Матчі команди</h2>
          ${this.renderMatches(matches)}
        </section>
      </section>
    `;

    document.getElementById("team-back")?.addEventListener("click", () => window.history.back());

    this.container.querySelectorAll<HTMLButtonElement>("[data-user-login]").forEach(btn => {
      btn.addEventListener("click", () => {
        const login = btn.dataset.userLogin || "";
        const role = (btn.dataset.userRole || "Athlete") as "Athlete" | "Trainer" | "Judge";

        if (login) new UserProfilePage("app", login, role).render();
      });
    });
  }

  private renderAthletes(athletes: AthleteTeamInfo[]) {
    if (!athletes.length) return `<div class="team-empty">Спортсменів поки немає</div>`;

    return `
      <div class="team-list">
        ${athletes.map(a => {
          const login = this.pick(a, ["login", "Login"]);
          const fullName =
            this.pick(a, ["fullName", "FullName"]) ||
            [this.pick(a, ["firsName", "FirsName"]), this.pick(a, ["lastName", "LastName"])].filter(Boolean).join(" ") ||
            login;

          return `
            <button class="team-list-item clickable" type="button" data-user-login="${this.escapeAttr(login)}" data-user-role="Athlete">
              <b>${this.escapeHtml(fullName || login)}</b>
              <span>${this.escapeHtml(login)}</span>
              <small>${this.escapeHtml(this.pick(a, ["athleteStatus", "AthleteStatus"]) || "Active")}</small>
            </button>
          `;
        }).join("")}
      </div>
    `;
  }

  private renderOrganizations(orgs: any[]) {
    if (!orgs.length) return `<div class="team-empty">Організацій поки немає</div>`;

    return `
      <div class="team-list">
        ${orgs.map(org => `
          <article class="team-list-item">
            <b>${this.escapeHtml(this.pick(org, ["nameOrganization", "NameOrganization"]) || this.pick(org, ["loginOrganization", "LoginOrganization"]))}</b>
            <span>${this.escapeHtml(this.pick(org, ["loginOrganization", "LoginOrganization"]))}</span>
            <small>${this.escapeHtml(this.pick(org, ["country", "Country"]) || "")}</small>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderMatches(matches: TeamMatchInfo[]) {
    if (!matches.length) return `<div class="team-empty">Матчів поки немає</div>`;

    return `
      <div class="team-match-list">
        ${matches.map(m => `
          <article class="team-match-card">
            <span>${this.escapeHtml(m.nameEvent || "Матч")}</span>
            <b>${this.escapeHtml(m.firstTeam || "-")} vs ${this.escapeHtml(m.secondTeam || "-")}</b>
            <small>
              ${m.dataMatch ? this.date(m.dataMatch) : "-"}
              · ${this.escapeHtml(m.score || "score -")}
              · ${this.status(m.statusMatch)}
              · R${m.tour ?? "-"}
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

    for (const m of matches) {
      if (m.statusMatch !== 2) continue;
      finishedMatches++;

      const parts = String(m.score || "").split(":").map(x => Number(x));
      if (parts.length !== 2 || parts.some(Number.isNaN)) continue;

      const isFirst = m.firstTeam === teamName;

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
