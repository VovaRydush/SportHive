import { eventCatalogApi } from "../api/eventCatalogApi";
import type { EventCatalogItemDto, EventMatchCatalogDto } from "../api/eventCatalogTypes";
import "./tournamentsPage.css";

export class UserProfilePage {
  private container: HTMLElement;
  private login: string;
  private role: string;

  constructor(containerId: string, login?: string, role?: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
    this.login = login || localStorage.getItem("login") || "";
    this.role = role || localStorage.getItem("role") || localStorage.getItem("userRole") || "Guest";
  }

  async render() {
    this.container.innerHTML = `
      <section class="score-page">
        <header class="score-header">
          <div>
            <span class="score-kicker">${this.escapeHtml(this.role)}</span>
            <h1>${this.escapeHtml(this.login || "Профіль")}</h1>
            <p>Профіль показує участь у матчах, статистику, ролі та активність у SportHive.</p>
          </div>
        </header>
        <div id="profile-root" class="score-card muted-card">Завантаження профілю...</div>
      </section>
    `;

    const root = document.getElementById("profile-root");
    if (!root) return;

    try {
      const events = await eventCatalogApi.getEvents({});
      const matches = this.findMatches(events);

      root.className = "detail-card";
      root.innerHTML = `
        <div class="detail-hero">
          <span class="event-status">${this.escapeHtml(this.role)}</span>
          <h2>${this.escapeHtml(this.login || "Користувач")}</h2>
          <p>${this.roleDescription()}</p>
          <div class="detail-meta">
            <span>Матчів: ${matches.length}</span>
            <span>Live: ${matches.filter(m => m.status === "Live").length}</span>
            <span>Завершено: ${matches.filter(m => m.status === "Finished").length}</span>
          </div>
        </div>
        <div class="detail-content">
          ${this.renderRoleBlocks(matches)}
        </div>
      `;
    } catch {
      root.innerHTML = "Не вдалося завантажити профіль";
    }
  }

  private findMatches(events: EventCatalogItemDto[]) {
    const result: EventMatchCatalogDto[] = [];
    events.forEach(event => {
      (event.matches || []).forEach(match => {
        const text = `${match.firstParticipant} ${match.secondParticipant} ${match.firstLogin || ""} ${match.secondLogin || ""} ${match.loginJudge || ""}`.toLowerCase();
        if (this.login && text.includes(this.login.toLowerCase())) result.push(match);
      });
    });
    return result;
  }

  private roleDescription() {
    if (this.role === "Athlete") return "Спортсмен: матчі, результати, участь у турнірах і статистика виступів.";
    if (this.role === "Trainer") return "Тренер: команди, спортсмени, матчі команд і результати.";
    if (this.role === "Judge") return "Суддя: призначені матчі, live-дані та фінальні результати.";
    if (this.role === "Organization") return "Організація: заходи, команди, судді, тренери та управління турнірами.";
    return "Перегляд SportHive.";
  }

  private renderRoleBlocks(matches: EventMatchCatalogDto[]) {
    return `
      <div class="feed-summary">
        <div class="summary-item"><span>Матчів</span><b>${matches.length}</b></div>
        <div class="summary-item live"><span>Live</span><b>${matches.filter(x => x.status === "Live").length}</b></div>
        <div class="summary-item"><span>Очікують</span><b>${matches.filter(x => x.status === "Upcoming").length}</b></div>
        <div class="summary-item"><span>Завершено</span><b>${matches.filter(x => x.status === "Finished").length}</b></div>
      </div>

      <div class="detail-match-list">
        ${
          matches.length
            ? matches.map(match => `
              <div class="detail-match-row">
                <div>
                  <span class="match-round">${this.escapeHtml(match.matchType)} · Round ${match.tour}</span>
                  <h4>${this.escapeHtml(match.firstParticipant)} — ${this.escapeHtml(match.secondParticipant)}</h4>
                  <p>${this.escapeHtml(match.status)} · ${this.escapeHtml(match.score || "Результат ще не внесено")}</p>
                </div>
                <div class="detail-score"><b>${this.escapeHtml(match.score || "vs")}</b></div>
              </div>
            `).join("")
            : `<div class="empty-state compact">Матчів для цього профілю поки не знайдено</div>`
        }
      </div>
    `;
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
