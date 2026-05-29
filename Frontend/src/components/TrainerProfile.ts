import { trainerApi } from "../api/trainerApi";
import type { TrainerProfileDto, TrainerTeamDto } from "../api/trainerTypes";
import { commandApi } from "../api/commandApi";
import { NotificationKarina } from "./Notification";
import { CreateTeamModal } from "./CreateTeam";
import { TeamPageLook } from "./TeamPage";
import "./trainerProfile.css";

export class TrainerProfile {
  private container: HTMLElement;
  private loginTrainer: string;
  private trainerData?: TrainerProfileDto;

  constructor(containerId: string, loginTrainers: string) {
    this.loginTrainer = loginTrainers;

    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="user-profile">
        <div class="profile-loading">Завантаження профілю тренера...</div>
      </section>
    `;

    try {
      this.trainerData = await trainerApi.getTrainerProfile(this.loginTrainer);
      this.renderProfile(this.trainerData);
      this.bindEvents();
    } catch (error) {
      console.error("Помилка завантаження профілю тренера:", error);

      new NotificationKarina().show(
        error instanceof Error ? error.message : "Не вдалося завантажити профіль тренера.",
        "error"
      );

      this.container.innerHTML = `
        <section class="user-profile">
          <div class="empty-state">
            Профіль тренера не знайдено або ComandService недоступний.
          </div>
        </section>
      `;
    }
  }

  private renderProfile(trainerData: TrainerProfileDto) {
    const fullName = `${trainerData.firsName ?? ""} ${trainerData.lastName ?? ""}`.trim() || trainerData.login;
    const isMyProfile = trainerData.login === localStorage.getItem("login");

    this.container.innerHTML = `
      <section class="user-profile">
        <div class="profile-header">
          <div class="profile-photo">
            <img
              src="${this.escapeHtml(this.getPhotoUrl(trainerData.photo))}"
              alt="Фото тренера"
              onerror="this.src='https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=300&q=80'"
            />
            <div class="sport-badge">🏆 Тренер</div>
          </div>

          <div class="profile-main">
            <div class="trainer-title-row">
              <div>
                <h1>${this.escapeHtml(fullName)}</h1>
                <div class="trainer-login">Логін: ${this.escapeHtml(trainerData.login)}</div>
              </div>

              ${
                isMyProfile
                  ? `<button id="trainer-create-team-btn" class="trainer-action-btn" type="button">+ Створити команду</button>`
                  : ""
              }
            </div>

            <div class="profile-details">
              <div class="detail-block">
                <h3>Особисті дані</h3>
                <p><strong>Дата народження:</strong> ${this.formatDate(trainerData.dataBirth)}</p>
                <p><strong>Вік:</strong> ${this.calculateAge(trainerData.dataBirth)} років</p>
                <p><strong>Роль:</strong> Trainer</p>
              </div>

              <div class="detail-block">
                <h3>Команди</h3>
                ${
                  trainerData.teams.length
                    ? trainerData.teams.map(team => `
                      <p>
                        <strong>${this.escapeHtml(team.nameTeam)}:</strong>
                        ${this.escapeHtml(team.typeSport)}
                        <span class="muted">(${team.athletesCount} спортсменів)</span>
                      </p>
                    `).join("")
                    : `<p class="muted">Команд поки немає</p>`
                }
              </div>

              <div class="detail-block">
                <h3>Організації</h3>
                ${
                  trainerData.organizations.length
                    ? trainerData.organizations.map(org => `
                      <p>
                        <strong>${this.escapeHtml(org.nameOrganization)}:</strong>
                        ${this.escapeHtml(org.typeOrganozation)}
                        <span class="muted">${this.escapeHtml(org.country)}</span>
                      </p>
                    `).join("")
                    : `<p class="muted">Організацій поки немає</p>`
                }
              </div>
            </div>
          </div>
        </div>

        <div class="stats-section">
          <h2>Статистика тренера</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${trainerData.stats?.teamsCount ?? 0}</span>
              <span class="stat-label">Команд</span>
            </div>
            <div class="stat-card total">
              <span class="stat-value">${trainerData.stats?.athletesCount ?? 0}</span>
              <span class="stat-label">Спортсменів</span>
            </div>
            <div class="stat-card total">
              <span class="stat-value">${trainerData.stats?.organizationsCount ?? 0}</span>
              <span class="stat-label">Організацій</span>
            </div>
            <div class="stat-card total">
              <span class="stat-value">${trainerData.stats?.totalMatches ?? 0}</span>
              <span class="stat-label">Матчів</span>
            </div>
          </div>
        </div>

        <div class="trainer-teams-section">
          <h2>Команди тренера</h2>

          <div class="trainer-teams-grid">
            ${
              trainerData.teams.length
                ? trainerData.teams.map(team => this.renderTeamCard(team)).join("")
                : `<div class="empty-state">У тренера ще немає команд</div>`
            }
          </div>
        </div>
      </section>
    `;
  }

  private renderTeamCard(team: TrainerTeamDto) {
    return `
      <article class="trainer-team-card">
        <div class="trainer-team-photo">
          <img
            src="${this.escapeHtml(this.getTeamPhotoUrl(team.photoTeam))}"
            alt="${this.escapeHtml(team.nameTeam)}"
            onerror="this.src='https://placehold.co/300x180?text=Team'"
          />
        </div>

        <div class="trainer-team-body">
          <h3>${this.escapeHtml(team.nameTeam)}</h3>
          <p>${this.getSportIcon(team.typeSport)} ${this.escapeHtml(team.typeSport)}</p>
          <p class="muted">${team.athletesCount} спортсменів</p>

          <button
            class="trainer-open-team-btn"
            type="button"
            data-team="${this.escapeHtml(team.nameTeam)}"
          >
            Відкрити команду
          </button>
        </div>
      </article>
    `;
  }

  private bindEvents() {
    document.getElementById("trainer-create-team-btn")?.addEventListener("click", () => {
      new CreateTeamModal().show();
    });

    this.container.querySelectorAll<HTMLButtonElement>(".trainer-open-team-btn").forEach(button => {
      button.addEventListener("click", () => {
        const teamName = button.dataset.team;

        if (!teamName) return;

        new TeamPageLook("app", teamName).render();
      });
    });
  }

  private getPhotoUrl(photo?: string | null) {
    if (!photo) return "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=300&q=80";
    if (photo.startsWith("http") || photo.startsWith("data:")) return photo;

    return photo;
  }

  private getTeamPhotoUrl(photo?: string | null) {
    if (!photo) return "https://placehold.co/300x180?text=Team";
    if (photo.startsWith("http") || photo.startsWith("data:")) return photo;

    return commandApi.getPhotoUrl(photo);
  }

  private calculateAge(birthDate: string): number {
    const today = new Date();
    const birth = new Date(birthDate);

    if (Number.isNaN(birth.getTime())) return 0;

    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }

    return age;
  }

  private formatDate(value: string) {
    const date = new Date(value);

    if (Number.isNaN(date.getTime())) return "-";

    return date.toLocaleDateString("uk-UA");
  }

  private getSportIcon(sportType: string): string {
    const icons: Record<string, string> = {
      Football: "⚽",
      "Футбол": "⚽",
      Basketball: "🏀",
      "Баскетбол": "🏀",
      Volleyball: "🏐",
      "Волейбол": "🏐",
      Tennis: "🎾",
      "Теніс": "🎾",
      Boxing: "🥊",
      "Бокс": "🥊",
      Wrestling: "🤼",
      "Боротьба": "🤼",
      Hockey: "🏒",
      "Хокей": "🏒",
      Baseball: "⚾",
      "Бейсбол": "⚾",
      Chess: "♟️",
      "Шахи": "♟️",
      Checkers: "⛀",
      "Шашки": "⛀",
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
