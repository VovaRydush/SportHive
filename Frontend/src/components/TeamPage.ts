import { commandApi } from "../api/commandApi";
import type { AthleteTeamInfo, NewSatatusAthlete, TeamInfoDto } from "../api/commandTypes";
import { NotificationKarina } from "./Notification";
import "./teamPage.css";

const MANAGER_ROLES = ["Trainer", "Organization"];
const STATUS_OPTIONS = ["Active", "Reserve", "Injured", "Disqualified"];

export class TeamPageLook {
  private container: HTMLElement;
  private teamName: string;
  private teamData!: TeamInfoDto;

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
      this.teamData = await commandApi.getTeam(this.teamName);
      this.renderHtml();
      this.bindAthleteActions();
    } catch (error) {
      console.error("Помилка завантаження команди:", error);
      new NotificationKarina().show(
        error instanceof Error ? error.message : `Команду ${this.teamName} не знайдено`,
        "error"
      );

      this.container.innerHTML = `
        <div class="team-page">
          <div class="empty-state">Команду не знайдено або ComandService недоступний.</div>
        </div>
      `;
    }
  }

  private renderHtml() {
    const nameTeam = this.pick(this.teamData.nameTeam, this.teamData.NameTeam, this.teamName);
    const typeSport = this.pick(this.teamData.typeSport, this.teamData.TypeSport, "-");
    const trainerLogin = this.pick(this.teamData.trainerLogin, this.teamData.TrainerLogin, "-");
    const trainerFirstName = this.pick(this.teamData.trainerFirstName, this.teamData.TrainerFirstName, "");
    const trainerLastName = this.pick(this.teamData.trainerLastName, this.teamData.TrainerLastName, "");
    const teamPhoto = this.normalizePhoto(this.pick(this.teamData.photoTeam, this.teamData.PhotoTeam, ""));
    const trainerPhoto = this.normalizePhoto(this.pick(this.teamData.trainerPhotp, this.teamData.TrainerPhotp, ""));
    const athletes = this.teamData.athletes || this.teamData.Athletes || [];
    const organizations = this.teamData.organizations || this.teamData.Organizations || [];

    const currentRole = localStorage.getItem("userRole") || localStorage.getItem("role") || "";
    const currentLogin = localStorage.getItem("login") || "";
    const canManage = MANAGER_ROLES.includes(currentRole) && (currentRole === "Organization" || trainerLogin === currentLogin);

    this.container.innerHTML = `
      <div class="team-page">
        <section class="team-header">
          <div class="team-photo">
            <img
              src="${this.escapeHtml(teamPhoto || "https://placehold.co/200x200?text=Team")}" 
              alt="${this.escapeHtml(nameTeam)}"
            />
          </div>

          <div class="team-info">
            <h1>${this.escapeHtml(nameTeam)}</h1>
            <div class="sport-type">${this.getSportIcon(typeSport)} ${this.escapeHtml(typeSport)}</div>
            <p><b>Тренер:</b> ${this.escapeHtml(`${trainerFirstName} ${trainerLastName}`.trim() || trainerLogin)}</p>
            <p><b>Login тренера:</b> ${this.escapeHtml(trainerLogin)}</p>
            <p><b>Спортсменів:</b> ${athletes.length}</p>

            ${organizations.length ? `
              <div class="organizations">
                <h3>Організації:</h3>
                <div class="organization-logos team-org-list">
                  ${organizations.map((org) => `
                    <span class="team-org-chip">
                      ${this.escapeHtml(this.pick(org.nameOrganization, org.NameOrganization, org.loginOrganization, org.LoginOrganization, "Організація"))}
                    </span>
                  `).join("")}
                </div>
              </div>
            ` : ""}
          </div>
        </section>

        <section class="trainer-section">
          <h2>Тренер</h2>
          <div class="trainer-card">
            <img
              class="trainer-photo"
              src="${this.escapeHtml(trainerPhoto || "https://placehold.co/150x150?text=Trainer")}" 
              alt="Trainer"
            />
            <div class="trainer-info">
              <h3>${this.escapeHtml(`${trainerFirstName} ${trainerLastName}`.trim() || trainerLogin)}</h3>
              <p>Логін: ${this.escapeHtml(trainerLogin)}</p>
              <p>Спорт: ${this.escapeHtml(typeSport)}</p>
            </div>
          </div>
        </section>

        <section class="athletes-section">
          <div class="team-section-header">
            <h2>Спортсмени команди</h2>
          </div>

          <div class="athletes-grid">
            ${athletes.length ? athletes.map((athlete) => this.renderAthleteCard(athlete, canManage)).join("") : `<div class="empty-state">Спортсменів поки немає</div>`}
          </div>
        </section>
      </div>
    `;
  }

  private renderAthleteCard(athlete: AthleteTeamInfo, canManage: boolean) {
    const login = this.pick(athlete.login, "");
    const firstName = this.pick(athlete.firsName, athlete.FirsName, "");
    const lastName = this.pick(athlete.lastName, athlete.LastName, "");
    const fullName = `${firstName} ${lastName}`.trim() || login;
    const typeSport = this.pick(athlete.typeSport, athlete.TypeSport, "-");
    const status = this.pick(athlete.athleteStatus, athlete.AthleteStatus, "Active");
    const photo = this.normalizePhoto(this.pick(athlete.photo, athlete.Photo, ""));

    return `
      <div class="athlete-card" data-login="${this.escapeHtml(login)}">
        ${canManage ? `
          <div class="athlete-actions">
            <button class="btn-icon btn-edit" data-action="status" title="Змінити статус">✎</button>
            <button class="btn-icon btn-delete" data-action="remove" title="Видалити">×</button>
          </div>
        ` : ""}

        <img
          class="athlete-photo"
          src="${this.escapeHtml(photo || "https://placehold.co/120x120?text=User")}" 
          alt="${this.escapeHtml(fullName)}"
        />

        <div class="athlete-info">
          <h3>${this.escapeHtml(fullName)}</h3>
          <p>Логін: ${this.escapeHtml(login)}</p>
          <p>Спорт: ${this.escapeHtml(typeSport)}</p>
          <p>Статус: <b>${this.escapeHtml(status)}</b></p>
        </div>
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

        const current = card?.querySelector(".athlete-info b")?.textContent || "Active";
        const newStatus = prompt(`Новий статус (${STATUS_OPTIONS.join(" / ")}):`, current);

        if (!newStatus) return;

        await this.changeAthleteStatus({
          nameTeam: this.pick(this.teamData.nameTeam, this.teamData.NameTeam, this.teamName),
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
        nameTeam: this.pick(this.teamData.nameTeam, this.teamData.NameTeam, this.teamName),
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

  private normalizePhoto(photo: string) {
    if (!photo) return "";
    if (photo.startsWith("http") || photo.startsWith("data:")) return photo;
    return commandApi.getPhotoUrl(photo);
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

  private pick(...values: any[]) {
    for (const value of values) {
      if (value !== undefined && value !== null && value !== "") return String(value);
    }
    return "";
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
