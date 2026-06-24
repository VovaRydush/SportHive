import { commandApi } from "../api/commandApi";
import type { AthleteTeamInfo, TeamInfoDto, TeamMatchInfo } from "../api/commandTypes";
import { photoOrInitialsHtml, resolvePhotoUrl, escapeHtml, escapeAttr } from "../api/media";
import { UserProfilePage } from "./UserProfile";
import {
  openTeamEditor,
  removeTeamAthlete,
} from "./OrganizationManagementPanels";
import "./teamPage.css";
import "./managementPanel.css";
import "./photoUi.css";

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
      <section class="team-page">
        <div class="team-loading">Завантаження команди...</div>
      </section>
    `;

    try {
      this.teamData = await commandApi.getTeam(this.teamName);
      this.renderPage();
    } catch (error) {
      this.container.innerHTML = `
        <section class="team-page">
          <button id="team-back" class="team-back">← Назад</button>
          <div class="team-error">${escapeHtml(error instanceof Error ? error.message : "Не вдалося завантажити команду")}</div>
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
    const trainerPhoto = this.pick(team, ["trainerPhoto", "TrainerPhoto", "trainerPhotp", "TrainerPhotp"]);
    const athletes = this.array(team.athletes ?? team.Athletes);
    const organizations = this.array(team.organizations ?? team.Organizations);
    const matches = this.array(team.matches ?? team.Matches);
    const stats = team.stats ?? this.buildStats(matches, name);
    const image = resolvePhotoUrl(teamPhoto, "command");

    this.container.innerHTML = `
      <section class="team-page">
        <button id="team-back" class="team-back">← Назад</button>

        <header class="team-hero">
          ${photoOrInitialsHtml(image, name, "team-photo", "command")}

          <div class="team-info">
            <span class="team-kicker">SportHive · команда</span>
            <h1>${escapeHtml(name)}</h1>

            <div class="photo-card-row team-trainer-row">
              ${photoOrInitialsHtml(trainerPhoto, trainerName, "list-photo", "auth")}
              <p>${escapeHtml(sport || "-")} · Тренер: ${escapeHtml(trainerName)} · Спортсменів: ${athletes.length}</p>
            </div>

            <div class="team-manage-buttons">
              <button id="edit-team-btn" class="manage-small-btn" type="button">Редагувати команду</button>
            </div>
          </div>
        </header>

        <section class="team-stats">
          <div><b>${stats.totalMatches}</b><span>Матчів</span></div>
          <div><b>${stats.finishedMatches}</b><span>Завершено</span></div>
          <div><b>${stats.wins}</b><span>Перемог</span></div>
          <div><b>${stats.losses}</b><span>Поразок</span></div>
        </section>

        <section class="team-grid">
          <div class="team-panel">
            <h2>Спортсмени</h2>
            ${this.renderAthletes(athletes, name)}
          </div>

          <div class="team-panel">
            <h2>Організації</h2>
            ${this.renderOrganizations(organizations)}
          </div>
        </section>

        <section class="team-panel">
          <h2>Матчі команди</h2>
          ${this.renderMatches(matches)}
        </section>
      </section>
    `;

    document.getElementById("team-back")?.addEventListener("click", () => window.history.back());

    document.getElementById("edit-team-btn")?.addEventListener("click", () => {
      openTeamEditor({
        teamName: name,
        typeSport: sport,
        loginTrainer: trainerLogin,
      }, () => this.render());
    });

    this.container.querySelectorAll<HTMLElement>("[data-user-login]").forEach(button => {
      button.addEventListener("click", () => {
        const login = button.dataset.userLogin || "";
        const role = (button.dataset.userRole || "Athlete") as "Athlete" | "Trainer" | "Judge";

        if (login) new UserProfilePage("app", login, role).render();
      });
    });

    this.container.querySelectorAll<HTMLElement>("[data-remove-team-athlete]").forEach(button => {
      button.addEventListener("click", event => {
        event.stopPropagation();
        const login = button.dataset.removeTeamAthlete || "";

        if (login) removeTeamAthlete(name, login, () => this.render());
      });
    });
  }

  private renderAthletes(athletes: AthleteTeamInfo[], teamName: string) {
    if (!athletes.length) {
      return `<div class="team-empty">Спортсменів поки немає</div>`;
    }

    return `
      <div class="team-list">
        ${athletes.map(athlete => {
          const login = this.pick(athlete, ["login", "Login"]);
          const fullName = this.pick(athlete, ["fullName", "FullName"]) ||
            [this.pick(athlete, ["firsName", "FirsName"]), this.pick(athlete, ["lastName", "LastName"])].filter(Boolean).join(" ") ||
            login;
          const photo = this.pick(athlete, ["profilePhoto", "ProfilePhoto", "photo", "Photo"]);

          return `
            <article class="team-list-item">
              <button class="team-athlete-main photo-card-row" type="button" data-user-login="${escapeAttr(login)}" data-user-role="Athlete">
                ${photoOrInitialsHtml(photo, fullName || login, "team-member-avatar", "auth")}
                <div>
                  <b>${escapeHtml(fullName || login)}</b>
                  <span>${escapeHtml(login)}</span>
                  <small>${escapeHtml(this.pick(athlete, ["athleteStatus", "AthleteStatus"]) || "Active")}</small>
                </div>
              </button>

              <div class="member-actions">
                <button class="remove-small-btn" type="button" data-remove-team-athlete="${escapeAttr(login)}">Видалити</button>
              </div>
            </article>
          `;
        }).join("")}
      </div>
    `;
  }

  private renderOrganizations(organizations: any[]) {
    if (!organizations.length) {
      return `<div class="team-empty">Організацій поки немає</div>`;
    }

    return `
      <div class="team-list">
        ${organizations.map(org => {
          const name = this.pick(org, ["nameOrganization", "NameOrganization"]) || this.pick(org, ["loginOrganization", "LoginOrganization"]);
          const login = this.pick(org, ["loginOrganization", "LoginOrganization"]);
          const photo = this.pick(org, ["profilePhoto", "ProfilePhoto", "photo", "Photo"]);

          return `
            <article class="team-list-item photo-card-row">
              ${photoOrInitialsHtml(photo, name || login, "list-photo", "auth")}
              <div>
                <b>${escapeHtml(name)}</b>
                <span>${escapeHtml(login)}</span>
                <small>${escapeHtml(this.pick(org, ["country", "Country"]) || "")}</small>
              </div>
            </article>
          `;
        }).join("")}
      </div>
    `;
  }

  private renderMatches(matches: TeamMatchInfo[]) {
    if (!matches.length) {
      return `<div class="team-empty">Матчів поки немає</div>`;
    }

    return `
      <div class="team-match-list">
        ${matches.map(match => `
          <article class="team-match-card">
            <b>${escapeHtml(match.nameEvent || "Матч")}</b>
            <span>${escapeHtml(match.firstTeam || "-")} vs ${escapeHtml(match.secondTeam || "-")}</span>
            <small>${match.dataMatch ? this.date(match.dataMatch) : "-"} · ${escapeHtml(match.score || "score -")} · ${this.status(match.statusMatch)} · R${match.tour ?? "-"}</small>
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

    return { totalMatches: matches.length, finishedMatches, wins, losses, draws };
  }

  private pick(obj: any, keys: string[]) {
    for (const key of keys) {
      const value = obj?.[key];

      if (value !== undefined && value !== null && String(value).trim() !== "") {
        return String(value);
      }
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
}
