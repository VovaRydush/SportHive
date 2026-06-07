import { eventCatalogApi } from "../api/eventCatalogApi";
import type { EventCatalogItemDto, EventMatchCatalogDto, EventStandingDto } from "../api/eventCatalogTypes";
import { NotificationKarina } from "./Notification";
import { SportLiveMatchPanel } from "./SportLiveMatchPanel";
import "./tournamentsPage.css";

type StatusFilter = "all" | "Live" | "Finished" | "Upcoming";
type DetailTab = "matches" | "table" | "bracket";

export class TournamentsPage {
  private container: HTMLElement;
  private events: EventCatalogItemDto[] = [];
  private selectedEvent?: EventCatalogItemDto;
  private selectedSport = "all";
  private statusFilter: StatusFilter = "all";
  private selectedSystem = "all";
  private searchText = "";
  private activeTab: DetailTab = "matches";

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="score-page">
        <header class="score-header">
          <div>
            <span class="score-kicker">SportHive</span>
            <h1>Матч-центр</h1>
            <p>Турніри згруповані по видах спорту. Матчі показані компактно: час, статус, учасники, рахунок і дія.</p>
          </div>
          <button id="score-refresh" class="score-btn score-btn-dark" type="button">Оновити</button>
        </header>

        <nav id="sports-strip" class="sports-strip"></nav>

        <div class="score-toolbar">
          <div class="score-statusbar">
            <button class="status-pill active" data-status="all" type="button">Усі</button>
            <button class="status-pill" data-status="Live" type="button">Live</button>
            <button class="status-pill" data-status="Upcoming" type="button">Очікують</button>
            <button class="status-pill" data-status="Finished" type="button">Завершені</button>
          </div>

          <input id="event-search" class="score-search" placeholder="Пошук заходу, команди, спортсмена..." />
        </div>

        <div class="score-layout">
          <aside class="score-sidebar">
            <div class="score-card">
              <div class="card-head"><h3>Види спорту</h3></div>
              <div id="sports-list" class="sidebar-list"></div>
            </div>

            <div class="score-card">
              <div class="card-head"><h3>Система</h3></div>
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
              <div class="score-card muted-card">Завантаження...</div>
            </div>
          </main>

          <aside id="event-detail" class="score-detail">
            <div class="score-card muted-card">
              Обери захід. Тут буде детальна інформація, матчі, таблиця та сітка.
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
    let timer: number | undefined;

    search?.addEventListener("input", () => {
      window.clearTimeout(timer);
      timer = window.setTimeout(() => {
        this.searchText = search.value.trim().toLowerCase();
        this.renderAll();
      }, 200);
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
        const stillExists = this.events.find(event => event.idEvent === previousId);
        if (stillExists) this.selectedEvent = await eventCatalogApi.getEvent(previousId);
      }

      this.renderAll();
      if (this.selectedEvent) this.renderDetail(this.selectedEvent);
    } catch (error) {
      console.error(error);
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося завантажити турніри", "error");
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
          <span>${this.sportIcon(sport)}</span> ${this.escapeHtml(sport)}
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
          <span><b>${this.sportIcon(sport)}</b> ${this.escapeHtml(sport)}</span>
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
        const matches = event.matches
          ?.map(match => `${match.firstParticipant} ${match.secondParticipant} ${match.score || ""} ${match.winner || ""}`)
          .join(" ") || "";

        const text = `${event.nameEvent} ${event.description || ""} ${event.typeSport} ${event.system} ${matches}`.toLowerCase();
        if (!text.includes(this.searchText)) return false;
      }

      return true;
    });
  }

  private renderSummary() {
    const root = document.getElementById("feed-summary");
    if (!root) return;

    const events = this.filteredEvents();
    const matches = events.reduce((sum, event) => sum + (event.totalMatches || 0), 0);
    const live = events.reduce((sum, event) => sum + (event.liveMatches || 0), 0);
    const finished = events.reduce((sum, event) => sum + (event.finishedMatches || 0), 0);

    root.innerHTML = `
      <div class="summary-item"><span>Заходів</span><b>${events.length}</b></div>
      <div class="summary-item"><span>Матчів</span><b>${matches}</b></div>
      <div class="summary-item live"><span>Live</span><b>${live}</b></div>
      <div class="summary-item"><span>Завершено</span><b>${finished}</b></div>
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
            <span class="competition-country">${this.sportIcon(sport)} ${this.escapeHtml(sport)}</span>
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
      button.addEventListener("click", event => {
        event.stopPropagation();
        const id = Number(button.dataset.id);
        const type = button.dataset.type || "team";
        if (!id) return;
        this.openLiveMatch(type, id);
      });
    });
  }

  private renderEventBlock(event: EventCatalogItemDto) {
    const matches = this.pickVisibleMatches(event.matches || []);

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
            matches.length
              ? matches.map(match => this.renderMatchLine(match)).join("")
              : `<div class="match-line disabled"><span>Матчів поки немає</span></div>`
          }
          ${(event.matches?.length || 0) > matches.length ? `
            <button class="show-all-matches open-event-detail" data-id="${event.idEvent}" type="button">
              Показати всі матчі (${event.matches.length})
            </button>
          ` : ""}
        </div>
      </article>
    `;
  }

  private renderMatchLine(match: EventMatchCatalogDto) {
    const firstScore = this.scorePart(match.score, 0);
    const secondScore = this.scorePart(match.score, 1);
    const canEnter = this.canEnterMatchData(match);
    const actionText = canEnter ? "Внести" : "Деталі";

    return `
      <div class="match-line ${match.status.toLowerCase()}">
        <button class="match-teams open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
          <span class="match-time ${match.status.toLowerCase()}">${this.matchTimeLabel(match)}</span>
          <span class="team-name first">${this.escapeHtml(match.firstParticipant)}</span>
          <strong class="match-score first-score">${this.escapeHtml(firstScore)}</strong>
          <span class="team-name second">${this.escapeHtml(match.secondParticipant)}</span>
          <strong class="match-score second-score">${this.escapeHtml(secondScore)}</strong>
        </button>
        <button class="match-action open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
          ${actionText}
        </button>
      </div>
    `;
  }

  private pickVisibleMatches(matches: EventMatchCatalogDto[]) {
    const sorted = matches.slice().sort((a, b) => {
      const rank = (status: string) => status === "Live" ? 0 : status === "Upcoming" ? 1 : 2;
      return rank(a.status) - rank(b.status) || a.tour - b.tour || a.matchId - b.matchId;
    });
    return sorted.slice(0, 5);
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
            <span>${this.sportIcon(event.typeSport)} ${this.escapeHtml(event.typeSport)}</span>
            <span>${this.escapeHtml(event.system)}</span>
            <span>${this.formatDate(event.dataStart)}</span>
          </div>
        </div>

        <div class="detail-tabs">
          <button class="detail-tab ${this.activeTab === "matches" ? "active" : ""}" data-tab="matches" type="button">Матчі</button>
          <button class="detail-tab ${this.activeTab === "table" ? "active" : ""}" data-tab="table" type="button">Таблиця</button>
          <button class="detail-tab ${this.activeTab === "bracket" ? "active" : ""}" data-tab="bracket" type="button">Сітка</button>
        </div>

        <div class="detail-content">
          ${this.renderDetailContent(event)}
        </div>
      </div>
    `;

    root.querySelectorAll<HTMLButtonElement>(".detail-tab").forEach(button => {
      button.addEventListener("click", () => {
        this.activeTab = (button.dataset.tab as DetailTab) || "matches";
        this.renderDetail(event);
      });
    });

    root.querySelectorAll<HTMLButtonElement>(".open-match-live").forEach(button => {
      button.addEventListener("click", () => {
        const id = Number(button.dataset.id);
        const type = button.dataset.type || "team";
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

    const byRound = this.groupMatchesByRound(matches);

    return Array.from(byRound.entries()).map(([round, roundMatches]) => `
      <section class="detail-round">
        <h3>${this.escapeHtml(round)}</h3>
        <div class="detail-match-list">
          ${roundMatches.map(match => `
            <div class="detail-match-row">
              <div>
                <span class="match-round">${match.matchType}${match.group ? ` · Group ${match.group}` : ""}</span>
                <h4>${this.escapeHtml(match.firstParticipant)} — ${this.escapeHtml(match.secondParticipant)}</h4>
                <p>${this.matchDateTimeLabel(match)} · Суддя: ${this.escapeHtml(match.loginJudge || "-")}</p>
                <small>${this.readinessLabel(match)}</small>
              </div>
              <div class="detail-score">
                <b>${this.escapeHtml(match.score || "vs")}</b>
                <button class="score-btn open-match-live" data-type="${match.matchType}" data-id="${match.matchId}" type="button">
                  ${this.canEnterMatchData(match) ? "Внести" : "Перегляд"}
                </button>
              </div>
            </div>
          `).join("")}
        </div>
      </section>
    `).join("");
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
              <th>#</th><th>Учасник</th><th>PL</th><th>W</th><th>D</th><th>L</th><th>SF</th><th>SA</th><th>+/-</th><th>P</th>
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
                <td>${row.scoreFor}</td>
                <td>${row.scoreAgainst}</td>
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

  private groupMatchesByRound(matches: EventMatchCatalogDto[]) {
    const map = new Map<string, EventMatchCatalogDto[]>();

    matches
      .slice()
      .sort((a, b) => a.tour - b.tour || (a.group || 0) - (b.group || 0) || a.matchId - b.matchId)
      .forEach(match => {
        const key = `Round ${match.tour}${match.group ? ` · Group ${match.group}` : ""}`;
        const list = map.get(key) || [];
        list.push(match);
        map.set(key, list);
      });

    return map;
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

  private canEnterMatchData(match: EventMatchCatalogDto) {
    if (!match.canEdit) return false;
    if (match.status === "Finished") return false;
    if (match.status === "Live") return true;

    const date = this.getMatchDate(match);
    if (!date) return false;

    return date.getTime() <= Date.now();
  }

  private readinessLabel(match: EventMatchCatalogDto) {
    if (match.status === "Finished") return "Матч завершений. Доступний перегляд результату.";
    if (this.canEnterMatchData(match)) return "Час матчу настав. Суддя може вносити live-дані та результат.";
    return "Матч ще очікується. Доступна тільки загальна інформація.";
  }

  private matchTimeLabel(match: EventMatchCatalogDto) {
    if (match.status === "Live") return "LIVE";
    if (match.status === "Finished") return "FT";

    const date = this.getMatchDate(match);
    if (!date) return `R${match.tour}`;

    return date.toLocaleTimeString("uk-UA", { hour: "2-digit", minute: "2-digit" });
  }

  private matchDateTimeLabel(match: EventMatchCatalogDto) {
    const date = this.getMatchDate(match);
    if (!date) return "Дата/час не вказані";

    return date.toLocaleString("uk-UA", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  }

  private getMatchDate(match: EventMatchCatalogDto) {
    if (!match.dataMatch) return null;

    const datePart = String(match.dataMatch).split("T")[0];
    const timeRaw = String(match.timeMatch || "00:00:00");
    const timePart = timeRaw.includes(".") ? timeRaw.split(".")[0] : timeRaw;
    const date = new Date(`${datePart}T${timePart}`);

    if (Number.isNaN(date.getTime())) {
      const fallback = new Date(String(match.dataMatch));
      return Number.isNaN(fallback.getTime()) ? null : fallback;
    }

    return date;
  }

  private scorePart(score: string | null | undefined, index: 0 | 1) {
    if (!score) return "-";
    const parts = score.replace(/ /g, "").split(/[:\\-]/);
    return parts[index] || "-";
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

  private openLiveMatch(matchType: string, matchId: number) {
    new SportLiveMatchPanel("app", matchType, matchId).render();
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
