import { eventCatalogApi } from "../api/eventCatalogApi";
import type { EventCatalogItemDto, EventMatchCatalogDto, EventStandingDto } from "../api/eventCatalogTypes";
import { NotificationKarina } from "./Notification";
import { SportLiveMatchPanel } from "./SportLiveMatchPanel";
import "./tournamentsPage.css";

type StatusFilter = "all" | "Live" | "Finished" | "Upcoming";

export class TournamentsPage {
  private container: HTMLElement;
  private events: EventCatalogItemDto[] = [];
  private selectedEvent?: EventCatalogItemDto;
  private selectedSport = "all";
  private statusFilter: StatusFilter = "all";
  private selectedSystem = "all";
  private searchText = "";
  private activeTab: "matches" | "table" | "bracket" = "matches";

  constructor(containerId: string) {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="score-page">
        <div class="score-top">
          <div class="score-title">
            <span class="score-kicker">SportHive scores</span>
            <h1>Заходи, матчі та результати</h1>
          </div>
          <button id="score-refresh" class="score-btn score-btn-dark" type="button">Оновити</button>
        </div>

        <div id="sports-strip" class="sports-strip">
          <button class="sport-chip active" type="button">Завантаження...</button>
        </div>

        <div class="score-filters">
          <div class="score-datebar">
            <button class="date-btn active" type="button">Усі дати</button>
            <button class="date-btn" type="button">Сьогодні</button>
            <button class="date-btn" type="button">Найближчі</button>
          </div>

          <div class="score-statusbar">
            <button class="status-pill active" data-status="all" type="button">Усі</button>
            <button class="status-pill" data-status="Live" type="button">Live</button>
            <button class="status-pill" data-status="Finished" type="button">Завершені</button>
            <button class="status-pill" data-status="Upcoming" type="button">Очікують</button>
          </div>
        </div>

        <div class="score-layout">
          <aside class="score-sidebar">
            <div class="score-card score-search-card">
              <input id="event-search" class="score-search" placeholder="Пошук команди, заходу, спорту..." />
            </div>

            <div class="score-card">
              <div class="card-head">
                <h3>Види спорту</h3>
              </div>
              <div id="sports-list" class="sidebar-list"></div>
            </div>

            <div class="score-card">
              <div class="card-head">
                <h3>Системи</h3>
              </div>
              <select id="system-filter" class="score-select">
                <option value="all">Усі системи</option>
                <option value="RoundRobin">Round Robin</option>
                <option value="GroupStage">Group Stage</option>
                <option value="PlayOff">PlayOff</option>
                <option value="OlympicSystem">Olympic</option>
                <option value="KnockoutSystem">Knockout</option>
                <option value="SwissSystem">Swiss</option>
                <option value="DoubleElimination">Double Elimination</option>
                <option value="QualificationByStandards">Qualification</option>
              </select>
            </div>
          </aside>

          <main class="score-feed">
            <div id="feed-summary" class="feed-summary"></div>
            <div id="events-feed" class="events-feed">
              <div class="score-card muted-card">Завантаження заходів...</div>
            </div>
          </main>

          <aside id="event-detail" class="score-detail">
            <div class="score-card muted-card">
              Обери захід або матч. Тут буде детальна інформація, матчі, таблиця та сітка.
            </div>
          </aside>
        </div>
      </section>
    `;

    this.bindBaseEvents();
    await this.loadEvents();
  }

  private bindBaseEvents() {
    document.getElementById("score-refresh")?.addEventListener("click", () => this.loadEvents(true));

    const search = document.getElementById("event-search") as HTMLInputElement | null;
    let searchTimer: number | undefined;

    search?.addEventListener("input", () => {
      window.clearTimeout(searchTimer);
      searchTimer = window.setTimeout(() => {
        this.searchText = search.value.trim().toLowerCase();
        this.renderAll();
      }, 220);
    });

    document.querySelectorAll<HTMLButtonElement>(".status-pill").forEach(button => {
      button.addEventListener("click", () => {
        document.querySelectorAll(".status-pill").forEach(x => x.classList.remove("active"));
        button.classList.add("active");
        this.statusFilter = (button.dataset.status as StatusFilter) || "all";
        this.renderAll();
      });
    });

    const system = document.getElementById("system-filter") as HTMLSelectElement | null;

    system?.addEventListener("change", () => {
      this.selectedSystem = system.value || "all";
      this.renderAll();
    });
  }

  private async loadEvents(keepSelected = false) {
    try {
      const previousId = this.selectedEvent?.idEvent;
      this.events = await eventCatalogApi.getEvents({});

      if (keepSelected && previousId) {
        const refreshed = this.events.find(e => e.idEvent === previousId);

        if (refreshed) {
          this.selectedEvent = await eventCatalogApi.getEvent(refreshed.idEvent);
        }
      }

      this.renderAll();

      if (this.selectedEvent) {
        this.renderDetail(this.selectedEvent);
      }
    } catch (error) {
      console.error(error);
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося завантажити заходи", "error");

      const feed = document.getElementById("events-feed");
      if (feed) {
        feed.innerHTML = `<div class="score-card error-card">Не вдалося завантажити заходи</div>`;
      }
    }
  }

  private renderAll() {
    this.renderSportsStrip();
    this.renderSportsList();
    this.renderSummary();
    this.renderFeed();
  }

  private getSports() {
    const map = new Map<string, number>();

    this.events.forEach(event => {
      const sport = event.typeSport || "Інше";
      map.set(sport, (map.get(sport) || 0) + 1);
    });

    return Array.from(map.entries()).sort((a, b) => a[0].localeCompare(b[0]));
  }

  private renderSportsStrip() {
    const root = document.getElementById("sports-strip");
    if (!root) return;

    const sports = this.getSports();

    root.innerHTML = `
      <button class="sport-chip ${this.selectedSport === "all" ? "active" : ""}" data-sport="all" type="button">
        <span>🏆</span> Усі
      </button>
      ${sports.map(([sport]) => `
        <button class="sport-chip ${this.selectedSport === sport ? "active" : ""}" data-sport="${this.escapeAttr(sport)}" type="button">
          <span>${this.sportIcon(sport)}</span> ${this.escapeHtml(this.sportName(sport))}
        </button>
      `).join("")}
    `;

    root.querySelectorAll<HTMLButtonElement>(".sport-chip").forEach(button => {
      button.addEventListener("click", () => {
        this.selectedSport = button.dataset.sport || "all";
        this.renderAll();
      });
    });
  }

  private renderSportsList() {
    const root = document.getElementById("sports-list");
    if (!root) return;

    const sports = this.getSports();

    root.innerHTML = `
      <button class="sidebar-item ${this.selectedSport === "all" ? "active" : ""}" data-sport="all" type="button">
        <span><b>🏆</b> Усі види</span>
        <strong>${this.events.length}</strong>
      </button>
      ${sports.map(([sport, count]) => `
        <button class="sidebar-item ${this.selectedSport === sport ? "active" : ""}" data-sport="${this.escapeAttr(sport)}" type="button">
          <span><b>${this.sportIcon(sport)}</b> ${this.escapeHtml(this.sportName(sport))}</span>
          <strong>${count}</strong>
        </button>
      `).join("")}
    `;

    root.querySelectorAll<HTMLButtonElement>(".sidebar-item").forEach(button => {
      button.addEventListener("click", () => {
        this.selectedSport = button.dataset.sport || "all";
        this.renderAll();
      });
    });
  }

  private filteredEvents() {
    return this.events.filter(event => {
      if (this.selectedSport !== "all" && event.typeSport !== this.selectedSport) return false;
      if (this.statusFilter !== "all" && event.status !== this.statusFilter) return false;
      if (this.selectedSystem !== "all" && event.system !== this.selectedSystem) return false;

      if (this.searchText) {
        const matchText = event.matches
          ?.map(match => `${match.firstParticipant} ${match.secondParticipant} ${match.score || ""}`)
          .join(" ") || "";

        const text = `${event.nameEvent} ${event.description} ${event.typeSport} ${event.system} ${matchText}`.toLowerCase();

        if (!text.includes(this.searchText)) return false;
      }

      return true;
    });
  }

  private renderSummary() {
    const root = document.getElementById("feed-summary");
    if (!root) return;

    const events = this.filteredEvents();
    const live = events.reduce((sum, event) => sum + (event.liveMatches || 0), 0);
    const matches = events.reduce((sum, event) => sum + (event.totalMatches || 0), 0);
    const finished = events.reduce((sum, event) => sum + (event.finishedMatches || 0), 0);

    root.innerHTML = `
      <div class="summary-item">
        <span>Заходів</span>
        <b>${events.length}</b>
      </div>
      <div class="summary-item">
        <span>Матчів</span>
        <b>${matches}</b>
      </div>
      <div class="summary-item live">
        <span>Live</span>
        <b>${live}</b>
      </div>
      <div class="summary-item">
        <span>Завершено</span>
        <b>${finished}</b>
      </div>
    `;
  }

  private renderFeed() {
    const root = document.getElementById("events-feed");
    if (!root) return;

    const events = this.filteredEvents();

    if (!events.length) {
      root.innerHTML = `
        <div class="score-card empty-state">
          <h3>Нічого не знайдено</h3>
          <p>Зміни спорт, статус, систему або пошук.</p>
        </div>
      `;
      return;
    }

    const grouped = this.groupBySport(events);

    root.innerHTML = Array.from(grouped.entries()).map(([sport, sportEvents]) => `
      <section class="competition-card">
        <header class="competition-header">
          <div>
            <span class="competition-country">${this.sportIcon(sport)} ${this.escapeHtml(this.sportName(sport))}</span>
            <h2>${this.escapeHtml(sportEvents.length === 1 ? sportEvents[0].nameEvent : `${sportEvents.length} заходів`)}</h2>
          </div>
          <span class="competition-count">${sportEvents.reduce((sum, e) => sum + (e.totalMatches || 0), 0)} матчів</span>
        </header>

        ${sportEvents.map(event => this.renderEventBlock(event)).join("")}
      </section>
    `).join("");

    root.querySelectorAll<HTMLButtonElement>(".open-event-detail").forEach(button => {
      button.addEventListener("click", () => this.openEvent(Number(button.dataset.id)));
    });

    root.querySelectorAll<HTMLButtonElement>(".open-match-live").forEach(button => {
      button.addEventListener("click", () => {
        const type = button.dataset.type || "team";
        const id = Number(button.dataset.id);

        if (!id) return;

        this.openLiveMatch(type, id);
      });
    });
  }

  private renderEventBlock(event: EventCatalogItemDto) {
    const visibleMatches = this.pickVisibleMatches(event.matches || []);

    return `
      <article class="event-block ${this.selectedEvent?.idEvent === event.idEvent ? "selected" : ""}">
        <button class="event-main-line open-event-detail" data-id="${event.idEvent}" type="button">
          <div class="event-name-line">
            <span class="event-status ${event.status.toLowerCase()}">${this.statusLabel(event.status)}</span>
            <strong>${this.escapeHtml(event.nameEvent)}</strong>
          </div>
          <div class="event-right">
            <span>${this.escapeHtml(event.system)}</span>
            <b>${event.finishedMatches}/${event.totalMatches}</b>
          </div>
        </button>

        <div class="event-match-preview">
          ${
            visibleMatches.length
              ? visibleMatches.map(match => this.renderMatchLine(match)).join("")
              : `<div class="match-line disabled"><span>Матчів поки немає</span></div>`
          }
        </div>
      </article>
    `;
  }

  private renderMatchLine(match: EventMatchCatalogDto) {
    return `
      <div class="match-line">
        <button class="match-teams open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
          <span class="match-minute">${match.status === "Live" ? "LIVE" : `R${match.tour}`}</span>
          <span class="team-name">${this.escapeHtml(match.firstParticipant)}</span>
          <strong class="match-score">${this.scorePart(match.score, 0)}</strong>
          <span class="team-name">${this.escapeHtml(match.secondParticipant)}</span>
          <strong class="match-score">${this.scorePart(match.score, 1)}</strong>
        </button>
        <button class="match-action open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
          Деталі
        </button>
      </div>
    `;
  }

  private pickVisibleMatches(matches: EventMatchCatalogDto[]) {
    const sorted = matches.slice().sort((a, b) => {
      const statusRank = (status: string) => status === "Live" ? 0 : status === "Upcoming" ? 1 : 2;
      return statusRank(a.status) - statusRank(b.status) || a.tour - b.tour || a.matchId - b.matchId;
    });

    return sorted.slice(0, 4);
  }

  private async openEvent(idEvent: number) {
    try {
      this.selectedEvent = await eventCatalogApi.getEvent(idEvent);
      this.activeTab = "matches";
      this.renderDetail(this.selectedEvent);
      this.renderFeed();
    } catch (error) {
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося відкрити захід", "error");
    }
  }

  private renderDetail(event: EventCatalogItemDto) {
    const root = document.getElementById("event-detail");
    if (!root) return;

    root.innerHTML = `
      <div class="detail-card">
        <div class="detail-hero">
          <span class="event-status ${event.status.toLowerCase()}">${this.statusLabel(event.status)}</span>
          <h2>${this.escapeHtml(event.nameEvent)}</h2>
          <p>${this.escapeHtml(event.description || "Опис відсутній")}</p>
          <div class="detail-meta">
            <span>${this.sportIcon(event.typeSport)} ${this.escapeHtml(this.sportName(event.typeSport))}</span>
            <span>${this.escapeHtml(event.system)}</span>
            <span>${this.formatDate(event.dataStart)}</span>
          </div>
        </div>

        <div class="detail-tabs">
          <button class="detail-tab ${this.activeTab === "matches" ? "active" : ""}" data-tab="matches" type="button">Матчі</button>
          <button class="detail-tab ${this.activeTab === "table" ? "active" : ""}" data-tab="table" type="button">Таблиця</button>
          <button class="detail-tab ${this.activeTab === "bracket" ? "active" : ""}" data-tab="bracket" type="button">Сітка</button>
        </div>

        <div id="detail-content" class="detail-content">
          ${this.renderDetailContent(event)}
        </div>
      </div>
    `;

    root.querySelectorAll<HTMLButtonElement>(".detail-tab").forEach(button => {
      button.addEventListener("click", () => {
        this.activeTab = (button.dataset.tab as "matches" | "table" | "bracket") || "matches";
        this.renderDetail(event);
      });
    });

    root.querySelectorAll<HTMLButtonElement>(".open-match-live").forEach(button => {
      button.addEventListener("click", () => {
        const type = button.dataset.type || "team";
        const id = Number(button.dataset.id);

        if (!id) return;

        this.openLiveMatch(type, id);
      });
    });
  }

  private renderDetailContent(event: EventCatalogItemDto) {
    if (this.activeTab === "table") return this.renderStanding(event.standings || []);
    if (this.activeTab === "bracket") return this.renderBracket(event);
    return this.renderAllMatches(event.matches || []);
  }

  private renderAllMatches(matches: EventMatchCatalogDto[]) {
    if (!matches.length) return `<div class="empty-state compact">Матчів поки немає</div>`;

    return `
      <div class="detail-match-list">
        ${matches.map(match => `
          <div class="detail-match-row">
            <div>
              <span class="match-round">${match.matchType} · Round ${match.tour}${match.group ? ` · Group ${match.group}` : ""}</span>
              <h3>${this.escapeHtml(match.firstParticipant)} — ${this.escapeHtml(match.secondParticipant)}</h3>
              <p>${this.escapeHtml(match.locationName || "Локацію не вказано")} · Суддя: ${this.escapeHtml(match.loginJudge || "-")}</p>
            </div>
            <div class="detail-score">
              <b>${this.escapeHtml(match.score || "vs")}</b>
              <button class="score-btn open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
                ${match.canEdit ? "Внести" : "Деталі"}
              </button>
            </div>
          </div>
        `).join("")}
      </div>
    `;
  }

  private renderStanding(rows: EventStandingDto[]) {
    if (!rows.length) return `<div class="empty-state compact">Таблиця буде після завершених матчів</div>`;

    const sorted = rows.slice().sort((a, b) =>
      b.points - a.points ||
      b.scoreDiff - a.scoreDiff ||
      b.scoreFor - a.scoreFor ||
      a.participant.localeCompare(b.participant)
    );

    return `
      <div class="standing-table-wrap">
        <table class="score-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Учасник</th>
              <th>PL</th>
              <th>W</th>
              <th>D</th>
              <th>L</th>
              <th>+/-</th>
              <th>P</th>
            </tr>
          </thead>
          <tbody>
            ${sorted.map((row, index) => `
              <tr>
                <td>${index + 1}</td>
                <td class="participant-cell">${this.escapeHtml(row.participant)}</td>
                <td>${row.played}</td>
                <td>${row.wins}</td>
                <td>${row.draws}</td>
                <td>${row.losses}</td>
                <td>${row.scoreDiff}</td>
                <td><b>${row.points}</b></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>
    `;
  }

  private renderBracket(event: EventCatalogItemDto) {
    if (!event.bracket?.length) return `<div class="empty-state compact">Сітка ще не сформована</div>`;

    return `
      <div class="compact-bracket">
        ${event.bracket.map(round => `
          <section class="round-card">
            <header>
              <strong>Round ${round.tour}</strong>
              <span>${round.group ? `Group ${round.group}` : round.bracketCode}</span>
            </header>
            ${round.matches.map(match => `
              <button class="bracket-match open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
                <span>${this.escapeHtml(match.firstParticipant)}</span>
                <b>${this.escapeHtml(match.score || "vs")}</b>
                <span>${this.escapeHtml(match.secondParticipant)}</span>
              </button>
            `).join("")}
          </section>
        `).join("")}
      </div>
    `;
  }

  private openLiveMatch(matchType: string, matchId: number) {
    new SportLiveMatchPanel("app", matchType, matchId).render();
  }

  private groupBySport(events: EventCatalogItemDto[]) {
    const map = new Map<string, EventCatalogItemDto[]>();

    events.forEach(event => {
      const sport = event.typeSport || "Інше";
      const list = map.get(sport) || [];
      list.push(event);
      map.set(sport, list);
    });

    return map;
  }

  private scorePart(score: string | null | undefined, index: 0 | 1) {
    if (!score) return "-";

    const normalized = score.replace(/ /g, "");
    const parts = normalized.split(/[:\-]/);

    return parts[index] || (index === 0 ? score : "");
  }

  private statusLabel(status: string) {
    if (status === "Live") return "LIVE";
    if (status === "Finished") return "FT";
    if (status === "Upcoming") return "NS";
    return status || "-";
  }

  private sportIcon(sport: string) {
    const key = (sport || "").toLowerCase();

    if (key.includes("football") || key.includes("фут")) return "⚽";
    if (key.includes("basket") || key.includes("бас")) return "🏀";
    if (key.includes("tennis") || key.includes("тен")) return "🎾";
    if (key.includes("hockey") || key.includes("хок")) return "🏒";
    if (key.includes("volley") || key.includes("вол")) return "🏐";
    if (key.includes("box") || key.includes("бокс")) return "🥊";
    if (key.includes("chess") || key.includes("шах")) return "♟️";
    if (key.includes("check")) return "🔲";
    if (key.includes("base")) return "⚾";

    return "🏆";
  }

  private sportName(sport: string) {
    return sport || "Інше";
  }

  private formatDate(value?: string | null) {
    if (!value) return "-";

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) return "-";

    return date.toLocaleDateString("uk-UA", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
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
