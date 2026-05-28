import { commandApi } from "../api/commandApi";
import { authApi } from "../api/authApi";
import type { NewSatatusAthlete } from "../api/commandTypes";
import { AthconsteProf, match, Organization, organizations, Stats, Teams, Trainer, trainers, users } from "./db";
import { NotificationKarina } from "./Notification";
import "./teamPage.css";

interface Team {
  TeamName: string;
  TeamPhoto: string;
  LoginTrainer: string;
  Trainer: Partial<Trainer>;
  TeamAthletes: Array<AthconsteProf & { athleteStatus?: string }>;
  TypeSport: string;
  Stats?: Stats;
  OrganizationTeam: Organization[];
  eMatchesTeams?: EMatchesTeam[];
}

interface EMatchesTeam {
  Match: Match;
}

interface Match {
  Date: string;
  Team1: string;
  Team2: string;
  Score?: string;
  Status: "upcoming" | "live" | "finished";
}

export class TeamPageLook {
  private container: HTMLElement;
  private teamName: string;
  private teamData!: Team;

  constructor(containerId: string, nameTeam: string) {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

    this.container = element;
    this.teamName = nameTeam;
  }

  async render() {
    this.container.innerHTML = `<div class="team-page"><div class="empty-state">Завантаження команди...</div></div>`;

    try {
      this.teamData = await this.getInfoTeam(this.teamName);
      this.renderHtml();
      this.bindAthleteActions();
    } catch (error) {
      console.error("Помилка завантаження команди:", error);
      new NotificationKarina().show(
        error instanceof Error ? error.message : `Team ${this.teamName} not found`,
        "error"
      );

      this.container.innerHTML = `
        <div class="team-page">
          <div class="empty-state">Команду не знайдено або бекенд недоступний.</div>
        </div>
      `;
    }
  }

  private renderHtml() {
    const canManage =
      localStorage.getItem("userRole") === "Trainer" ||
      localStorage.getItem("role") === "Trainer" ||
      this.teamData.LoginTrainer === localStorage.getItem("login");

    this.container.innerHTML = `
      <div class="team-page">
        <section class="team-header">
          <div class="team-photo">
            <img
              src="${this.escapeHtml(this.teamData.TeamPhoto || "https://placehold.co/200x200?text=Team")}"
              alt="${this.escapeHtml(this.teamData.TeamName)}"
            />
          </div>

          <div class="team-info">
            <h1>${this.escapeHtml(this.teamData.TeamName)}</h1>
            <div class="sport-type">${this.getSportIcon(this.teamData.TypeSport)} ${this.escapeHtml(this.teamData.TypeSport)}</div>

            ${
              this.teamData.OrganizationTeam.length > 0
                ? `
                  <div class="organizations">
                    <h3>Організації:</h3>
                    <div class="organization-logos">
                      ${this.teamData.OrganizationTeam.map((org) => `
                        <img src="${this.escapeHtml(org.photo || "https://placehold.co/60x60?text=Org")}" title="${this.escapeHtml(org.NameOrganization)}" />
                      `).join("")}
                    </div>
                  </div>
                `
                : ""
            }
          </div>
        </section>

        <section class="trainer-section">
          <h2>Тренер</h2>
          <div class="trainer-card">
            <img
              class="trainer-photo"
              src="${this.escapeHtml(this.teamData.Trainer.Photo || "https://placehold.co/150x150?text=Trainer")}"
              alt="Trainer"
            />
            <div class="trainer-info">
              <h3>${this.escapeHtml(`${this.teamData.Trainer.FirsName ?? ""} ${this.teamData.Trainer.LastName ?? ""}`.trim() || this.teamData.LoginTrainer)}</h3>
              <p>Логін: ${this.escapeHtml(this.teamData.LoginTrainer)}</p>
              <p>Спорт: ${this.escapeHtml(this.teamData.Trainer.SportType || this.teamData.TypeSport)}</p>
            </div>
          </div>
        </section>

        <section class="athletes-section">
          <h2>Гравці</h2>

          <div class="athletes-grid">
            ${
              this.teamData.TeamAthletes.length
                ? this.teamData.TeamAthletes.map((athlete) => `
                  <div class="athlete-card" data-login="${this.escapeHtml(athlete.login)}">
                    ${
                      canManage
                        ? `
                          <div class="athlete-actions">
                            <button class="btn-icon btn-edit" data-action="status" title="Змінити статус">✎</button>
                            <button class="btn-icon btn-delete" data-action="remove" title="Видалити">×</button>
                          </div>
                        `
                        : ""
                    }

                    <img
                      class="athlete-photo"
                      src="${this.escapeHtml(athlete.photo || "https://placehold.co/120x120?text=User")}"
                      alt="${this.escapeHtml(athlete.name)}"
                    />

                    <div class="athlete-info">
                      <h3>${this.escapeHtml(athlete.name || athlete.login)}</h3>
                      <p>Логін: ${this.escapeHtml(athlete.login)}</p>
                      <p>Позиція: ${this.escapeHtml(athlete.Position || "Не вказано")}</p>
                      <p>Статус: ${this.escapeHtml(athlete.athleteStatus || "Active")}</p>
                    </div>
                  </div>
                `).join("")
                : `<div class="empty-state">Гравців поки немає</div>`
            }
          </div>
        </section>

        <section class="matches-section">
          <h2>Статистика команди</h2>

          <div class="team-stats">
            <div><b>${this.teamData.Stats?.TotalMatches || 0}</b><span>Зіграно матчів</span></div>
            <div><b>${this.teamData.Stats?.Wins || 0}</b><span>Перемоги</span></div>
            <div><b>${this.teamData.Stats?.Losses || 0}</b><span>Поразки</span></div>
            <div><b>${this.teamData.Stats?.Draws || 0}</b><span>Нічиї</span></div>
          </div>
        </section>

        <section class="matches-section">
          <h2>Матчі</h2>

          <div class="matches-tabs">
            <button class="tab-button active" type="button">Майбутні</button>
            <button class="tab-button" type="button">Завершені</button>
          </div>

          <div class="matches-list">
            ${
              this.teamData.eMatchesTeams
                ?.filter((m) => m.Match.Status === "upcoming")
                .map((item) => this.renderMatchCard(item.Match, "upcoming"))
                .join("") || `<div class="empty-state">Немає майбутніх матчів</div>`
            }
          </div>

          <div class="matches-list hidden">
            ${
              this.teamData.eMatchesTeams
                ?.filter((m) => m.Match.Status === "finished")
                .map((item) => this.renderMatchCard(item.Match, "finished"))
                .join("") || `<div class="empty-state">Немає завершених матчів</div>`
            }
          </div>
        </section>
      </div>
    `;
  }

  private bindAthleteActions() {
    this.container.querySelectorAll<HTMLButtonElement>("[data-action='remove']").forEach((button) => {
      button.addEventListener("click", async () => {
        const card = button.closest(".athlete-card") as HTMLElement | null;
        const loginAthlete = card?.dataset.login || "";

        if (!loginAthlete) return;

        await this.removeAthlete(loginAthlete);
      });
    });

    this.container.querySelectorAll<HTMLButtonElement>("[data-action='status']").forEach((button) => {
      button.addEventListener("click", async () => {
        const card = button.closest(".athlete-card") as HTMLElement | null;
        const loginAthlete = card?.dataset.login || "";

        if (!loginAthlete) return;

        const newStatus = prompt("Новий статус спортсмена:", "Active");

        if (!newStatus) return;

        await this.changeAthleteStatus({
          nameTeam: this.teamData.TeamName,
          loginAthlete,
          newStatus,
        });
      });
    });
  }

  private async removeAthlete(loginAthlete: string) {
    if (!confirm("Видалити спортсмена з команди?")) return;

    try {
      await commandApi.removeAthlet({
        nameTeam: this.teamData.TeamName,
        loginAthlete,
        newStatus: "",
      });

      new NotificationKarina().show("Спортсмена видалено з команди.", "success");
      await this.render();
    } catch (error) {
      new NotificationKarina().show(
        error instanceof Error ? error.message : "Не вдалося видалити спортсмена.",
        "error"
      );
    }
  }

  private async changeAthleteStatus(data: NewSatatusAthlete) {
    try {
      await commandApi.changeStatusAthlet(data);
      new NotificationKarina().show("Статус спортсмена змінено.", "success");
      await this.render();
    } catch (error) {
      new NotificationKarina().show(
        error instanceof Error ? error.message : "Не вдалося змінити статус.",
        "error"
      );
    }
  }

  private renderMatchCard(match: Match, type: "upcoming" | "finished") {
    return `
      <div class="match-card ${type}">
        <div class="match-teams">
          <span>${this.escapeHtml(match.Team1)}</span>
          <span>vs</span>
          <span>${this.escapeHtml(match.Team2)}</span>
        </div>

        ${match.Score ? `<div class="match-score">${this.escapeHtml(match.Score)}</div>` : ""}

        <div class="match-date">
          ${new Date(match.Date).toLocaleDateString("uk-UA")}
        </div>
      </div>
    `;
  }

  private async getInfoTeam(nameTeam: string): Promise<Team> {
    try {
      const teamResponse = await commandApi.getTeam(nameTeam);
      const athletesResponse = await commandApi.getAthletesTeam(nameTeam);

      return this.normalizeBackendTeam(teamResponse, athletesResponse, nameTeam);
    } catch (error) {
      console.warn("Не вдалося отримати команду з API, fallback на локальні дані:", error);
      return this.getLocalTeam(nameTeam);
    }
  }

  private normalizeBackendTeam(teamResponse: any, athletesResponse: any, nameTeam: string): Team {
    const team = typeof teamResponse === "string" ? JSON.parse(teamResponse) : teamResponse;
    const athletesRaw = typeof athletesResponse === "string" ? JSON.parse(athletesResponse) : athletesResponse;

    const athletesList = Array.isArray(athletesRaw)
      ? athletesRaw
      : athletesRaw?.athlets || athletesRaw?.Athlets || athletesRaw?.athletes || athletesRaw?.Athletes || [];

    const loginTrainer =
      team?.loginTrainer ||
      team?.LoginTrainer ||
      team?.trainerLogin ||
      team?.TrainerLogin ||
      "";

    const trainer =
      trainers.find((item) => item.login === loginTrainer) ||
      team?.trainer ||
      team?.Trainer ||
      ({} as Trainer);

    const normalizedAthletes = athletesList.map((item: any) => {
      const login =
        item?.login ||
        item?.Login ||
        item?.loginAthlets ||
        item?.LoginAthlets ||
        item?.loginAthlete ||
        item?.LoginAthlete ||
        "";

      const local = users.find((user) => user.login === login);

      return {
        ...(local || {}),
        name:
          local?.name ||
          item?.fullName ||
          item?.FullName ||
          item?.name ||
          item?.Name ||
          login,
        login,
        sport: local?.sport || item?.typeSport || item?.TypeSport || team?.typeSport || team?.TypeSport || "",
        photo: local?.photo || item?.photo || item?.Photo || "",
        stats: local?.stats || "",
        Team: nameTeam,
        DataBirth: local?.DataBirth || item?.dateBirhsday || item?.DataBirth || "",
        Position: local?.Position || item?.position || item?.Position || "",
        Matches: local?.Matches || [],
        dataMathes: local?.dataMathes || "",
        athleteStatus: item?.athleteStatus || item?.AthleteStatus || item?.status || item?.Status || "Active",
      };
    });

    const organizationTeam = organizations.filter((org) =>
      org.Teams?.some((t: any) => (typeof t === "string" ? t === nameTeam : t?.name === nameTeam || t?.Name === nameTeam))
    );

    return {
      TeamName: team?.nameTeam || team?.NameTeam || team?.name || team?.Name || nameTeam,
      TeamPhoto: team?.photo || team?.Photo || team?.logo || team?.Logo || "",
      LoginTrainer: loginTrainer,
      Trainer: trainer,
      TeamAthletes: normalizedAthletes,
      TypeSport: team?.typeSport || team?.TypeSport || team?.sport || team?.Sport || "",
      Stats: team?.stats || team?.Stats,
      OrganizationTeam: organizationTeam,
      eMatchesTeams: this.getLocalMatches(nameTeam),
    };
  }

  private getLocalTeam(nameTeam: string): Team {
    const teamInfo = Teams.find((t) => t.name === nameTeam);

    if (!teamInfo) {
      throw new Error(`Team ${nameTeam} not found`);
    }

    const trainer = trainers.find((t) => t.login === teamInfo.LoginTrainer);
    const teamAthletes = users.filter((a) => teamInfo.AthleteLogins.includes(a.login));
    const organizationTeam = organizations.filter((org) =>
      org.Teams.some((team: any) => (typeof team === "string" ? team === nameTeam : team.name === nameTeam))
    );

    return {
      TeamName: teamInfo.name ?? "",
      TeamPhoto: teamInfo.logo ?? "",
      LoginTrainer: teamInfo.LoginTrainer ?? "",
      Trainer: trainer || ({} as Trainer),
      TeamAthletes: teamAthletes ?? [],
      TypeSport: teamInfo.sport ?? "",
      Stats: teamInfo.stats ?? undefined,
      OrganizationTeam: organizationTeam ?? [],
      eMatchesTeams: this.getLocalMatches(nameTeam),
    };
  }

  private getLocalMatches(nameTeam: string): EMatchesTeam[] {
    return match
      .filter((m) => m.team1 === nameTeam || m.team2 === nameTeam)
      .map((m) => ({
        Match: {
          Date: m.date,
          Team1: m.team1,
          Team2: m.team2,
          Score: m.score,
          Status: this.normalizeMatchStatus(m.status),
        },
      }));
  }

  private normalizeMatchStatus(status: string): "upcoming" | "live" | "finished" {
    if (status === "Завершено" || status === "finished") return "finished";
    if (status === "Live" || status === "live") return "live";
    return "upcoming";
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      "Футбол": "⚽",
      Football: "⚽",
      "Баскетбол": "🏀",
      Basketball: "🏀",
      "Волейбол": "🏐",
      Volleyball: "🏐",
      "Теніс": "🎾",
      Tennis: "🎾",
      "Хокей": "🏒",
      Hockey: "🏒",
      "Бокс": "🥊",
      Boxing: "🥊",
      Wrestling: "🤼",
      Chess: "♟️",
      Checkers: "⛀",
    };

    return icons[sportType] || "🏅";
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
