import { eventCatalogApi } from "../api/eventCatalogApi";
import { NotificationKarina } from "./Notification";
import { SportLiveMatchPanel } from "./SportLiveMatchPanel";
import { StatisticsDashboard } from "./StatisticsDashboard";
import {
  canEnterMatchData,
  extractScore,
  formatMatchDateTime,
  formatMatchTime,
  getEffectiveMatchStatus,
  readinessLabel,
  statusLabel,
} from "../api/matchStatus";
import {
  buildStandingsFromMatches,
  calculateEventStats,
  normalizeEventForUi,
  normalizeMatchForUi,
  type UiStandingRow,
} from "../api/tournamentResultUtils";
import "./tournamentsPage.css";
import "./tournamentsPageStats.css";

type CatalogEvent = Record<string, any>;
type CatalogMatch = Record<string, any>;
type StatusFilter = "all" | "Live" | "Finished" | "Upcoming";
type DetailTab = "matches" | "table" | "bracket";

export class TournamentsPage {
  private container: HTMLElement;
  private events: CatalogEvent[] = [];
  private selectedEvent?: CatalogEvent;
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
      <section class="score-page tournaments-page">
        <header class="score-header">
          <div>
            <span class="score-kicker">SportHive · Match Center</span>
            <h1>Турніри</h1>
            <p>Переглядай заходи, матчі, статуси, таблиці та сітки в одному місці.</p>
          </div>
          <button id="score-refresh-top" class="score-btn score-btn-dark" type="button">Оновити</button>
        </header>

        <div id="tournament-stat-band" class="tournament-stat-band"></div>
        <div id="sports-strip" class="sports-strip"></div>

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
            <section class="score-card">
              <div class="card-head"><h3>Види спорту</h3></div>
              <div id="sports-list" class="sidebar-list"></div>
            </section>

            <section class="score-card">
              <div class="card-head"><h3>Система</h3></div>
              <select id="system-filter" class="score-select">
                <option value="all">Усі системи</option>
                <option value="RoundRobin">RoundRobin</option>
                <option value="GroupStage">GroupStage</option>
                <option value="PlayOff">PlayOff</option>
                <option value="OlympicSystem">OlympicSystem</option>
                <option value="Knockout">Knockout</option>
                <option value="KnockoutSystem">KnockoutSystem</option>
                <option value="SwissSystem">SwissSystem</option>
                <option value="DoubleElimination">DoubleElimination</option>
                <option value="Qualification">Qualification</option>
                <option value="QualificationByStandards">QualificationByStandards</option>
              </select>
            </section>
          </aside>

          <main class="score-feed">
            <div id="feed-summary" class="feed-summary"></div>
            <div id="events-feed" class="events-feed">
              <div class="muted-card">Завантаження турнірів...</div>
            </div>
          </main>

          <aside id="event-detail" class="score-detail">
            <div class="detail-card">
              <div class="empty-state">
                <h3>Обери захід</h3>
                <p>Тут буде детальна інформація, матчі, таблиця та сітка.</p>
              </div>
            </div>
          </aside>
        </div>
      </section>
    `;

    this.bindBaseEvents();
    await this.loadEvents();
  }

  private bindBaseEvents() {
    document.getElementById("score-refresh-top")?.addEventListener("click", () => this.loadEvents(true));

    const search = document.getElementById("event-search") as HTMLInputElement | null;
    let timer: number | undefined;

    search?.addEventListener("input", () => {
      window.clearTimeout(timer);
      timer = window.setTimeout(() => {
        this.searchText = search.value.trim().toLowerCase();
        this.renderAll();
      }, 200);
    });

    document.querySelectorAll<HTMLElement>(".status-pill").forEach(button => {
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
      const events = await eventCatalogApi.getEvents({});
      this.events = (events || []).map(event => this.normalizeEvent(event as CatalogEvent));

      if (keepSelected && previousId) {
        const stillExists = this.events.find(event => Number(event.idEvent) === Number(previousId));
        if (stillExists) this.selectedEvent = this.normalizeEvent(await eventCatalogApi.getEvent(previousId) as CatalogEvent);
      }

      this.renderAll();

      if (this.selectedEvent) this.renderDetail(this.selectedEvent);
    } catch (error) {
      const message = error instanceof Error ? error.message : "Не вдалося завантажити турніри";
      const root = document.getElementById("events-feed");
      if (root) root.innerHTML = `<div class="error-card">${this.escapeHtml(message)}</div>`;
      new NotificationKarina().show(message, "error");
    }
  }

  private normalizeEvent(event: CatalogEvent): CatalogEvent {
    return normalizeEventForUi(event);
  }

  private renderAll() {
    this.renderTournamentStatBand();
    this.renderSportsStrip();
    this.renderSportsList();
    this.renderSummary();
    this.renderFeed();
  }

  private renderTournamentStatBand() {
    const root = document.getElementById("tournament-stat-band");
    if (!root) return;

    const filtered = this.filteredEvents();
    const allStats = filtered.map(calculateEventStats);
    const matches = allStats.reduce((sum, x) => sum + x.totalMatches, 0);
    const finished = allStats.reduce((sum, x) => sum + x.finishedMatches, 0);
    const live = allStats.reduce((sum, x) => sum + x.liveMatches, 0);
    const sports = new Set(filtered.map(x => x.typeSport).filter(Boolean)).size;
    const completion = matches ? Math.round((finished / matches) * 100) : 0;

    root.innerHTML = `
      <button id="open-global-statistics" class="tournament-stat-card" type="button">
        <b>${completion}%</b>
        <span>Завершення матчів</span>
      </button>
      <button id="open-global-statistics-2" class="tournament-stat-card" type="button">
        <b>${sports}</b>
        <span>Видів спорту</span>
      </button>
      <button id="open-global-statistics-3" class="tournament-stat-card live" type="button">
        <b>${live}</b>
        <span>Live зараз</span>
      </button>
      <button id="open-global-statistics-4" class="tournament-stat-card" type="button">
        <b>Analytics</b>
        <span>Відкрити повну статистику</span>
      </button>
    `;

    root.querySelectorAll("button").forEach(button => {
      button.addEventListener("click", () => new StatisticsDashboard("app", "global").render());
    });
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
      <button class="sport-chip ${this.selectedSport === "all" ? "active" : ""}" data-sport="all" type="button">🏆 Усі</button>
      ${sports.map(([sport]) => `
        <button class="sport-chip ${this.selectedSport === sport ? "active" : ""}" data-sport="${this.escapeAttr(sport)}" type="button">
          ${this.sportIcon(sport)} ${this.escapeHtml(sport)}
        </button>
      `).join("")}
    `;

    root.querySelectorAll<HTMLElement>(".sport-chip").forEach(button => {
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
        <span>🏆 Усі види</span>
        <strong>${this.events.length}</strong>
      </button>
      ${sports.map(([sport, count]) => `
        <button class="sidebar-item ${this.selectedSport === sport ? "active" : ""}" data-sport="${this.escapeAttr(sport)}" type="button">
          <span>${this.sportIcon(sport)} ${this.escapeHtml(sport)}</span>
          <strong>${count}</strong>
        </button>
      `).join("")}
    `;

    root.querySelectorAll<HTMLElement>(".sidebar-item").forEach(button => {
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
          ?.map((match: CatalogMatch) => `${match.firstParticipant} ${match.secondParticipant} ${match.score || ""} ${match.winner || ""}`)
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
    const matches = events.reduce((sum, event) => sum + calculateEventStats(event).totalMatches, 0);
    const live = events.reduce((sum, event) => sum + calculateEventStats(event).liveMatches, 0);
    const finished = events.reduce((sum, event) => sum + calculateEventStats(event).finishedMatches, 0);

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
        <div class="empty-state">
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
            <h2>${this.escapeHtml(sport)}</h2>
          </div>
          <span class="competition-count">${sportEvents.reduce((sum, event) => sum + calculateEventStats(event).totalMatches, 0)} матчів</span>
        </header>
        ${sportEvents.map(event => this.renderEventBlock(event)).join("")}
      </section>
    `).join("");

    root.querySelectorAll<HTMLElement>(".event-main-line").forEach(button => {
      button.addEventListener("click", () => this.openEvent(Number(button.dataset.id)));
    });

    root.querySelectorAll<HTMLElement>(".match-teams, .match-action").forEach(button => {
      button.addEventListener("click", event => {
        event.stopPropagation();
        const id = Number(button.dataset.id);
        const type = button.dataset.type || "team";
        if (!id) return;
        this.openLiveMatch(type, id);
      });
    });

    root.querySelectorAll<HTMLElement>(".show-all-matches").forEach(button => {
      button.addEventListener("click", event => {
        event.stopPropagation();
        this.openEvent(Number(button.dataset.id));
      });
    });
  }

  private renderEventBlock(event: CatalogEvent) {
    const eventUi = this.normalizeEvent(event);
    const stats = calculateEventStats(eventUi);
    const matches = this.pickVisibleMatches(eventUi.matches || []);
    const selectedClass = Number(this.selectedEvent?.idEvent) === Number(eventUi.idEvent) ? "selected" : "";

    return `
      <article class="event-block ${selectedClass}">
        <button class="event-main-line" data-id="${eventUi.idEvent}" type="button">
          <span class="event-name-line">
            <span class="event-status ${String(stats.status).toLowerCase()}">${statusLabel(stats.status)}</span>
            <strong>${this.escapeHtml(eventUi.nameEvent)}</strong>
          </span>
          <span class="event-right">
            <span>${this.escapeHtml(eventUi.system || "-")}</span>
            <b>${stats.finishedMatches}/${stats.totalMatches}</b>
          </span>
        </button>

        <div class="event-match-preview">
          ${matches.length ? matches.map(match => this.renderMatchLine(match)).join("") : `
            <div class="match-line disabled">Матчів поки немає</div>
          `}
        </div>

        ${(eventUi.matches?.length || 0) > matches.length ? `
          <button class="show-all-matches" data-id="${eventUi.idEvent}" type="button">Показати всі матчі (${eventUi.matches.length})</button>
        ` : ""}
      </article>
    `;
  }

  private renderMatchLine(matchInput: CatalogMatch) {
    const match = normalizeMatchForUi(matchInput);
    const status = getEffectiveMatchStatus(match);
    const score = extractScore(match) || match.score || "";
    const firstScore = this.scorePart(score, 0);
    const secondScore = this.scorePart(score, 1);
    const actionText = canEnterMatchData(match) ? "Внести" : "Деталі";

    return `
      <div class="match-line ${status.toLowerCase()}">
        <button class="match-teams" data-type="${this.escapeAttr(match.matchType)}" data-id="${match.matchId}" type="button">
          <span class="match-time ${status.toLowerCase()}">${formatMatchTime(match)}</span>
          <span class="team-name first">${this.escapeHtml(match.firstParticipant || "-")}</span>
          <span class="match-score first-score">${this.escapeHtml(firstScore)}</span>
          <span class="team-name second">${this.escapeHtml(match.secondParticipant || "-")}</span>
          <span class="match-score second-score">${this.escapeHtml(secondScore)}</span>
        </button>
        <button class="match-action" data-type="${this.escapeAttr(match.matchType)}" data-id="${match.matchId}" type="button">${actionText}</button>
      </div>
    `;
  }

  private pickVisibleMatches(matches: CatalogMatch[]) {
    const sorted = matches.map(normalizeMatchForUi).sort((a, b) => {
      const rank = (status: string) => status === "Live" ? 0 : status === "Upcoming" ? 1 : 2;
      return rank(a.status) - rank(b.status) || Number(a.tour || 0) - Number(b.tour || 0) || Number(a.matchId || 0) - Number(b.matchId || 0);
    });

    return sorted.slice(0, 5);
  }

  private async openEvent(idEvent: number) {
    try {
      this.selectedEvent = this.normalizeEvent(await eventCatalogApi.getEvent(idEvent) as CatalogEvent);
      this.activeTab = "matches";
      this.renderDetail(this.selectedEvent);
      this.renderFeed();
    } catch (error) {
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося відкрити захід", "error");
    }
  }

  private renderDetail(eventInput: CatalogEvent) {
    const event = this.normalizeEvent(eventInput);
    const stats = calculateEventStats(event);
    const root = document.getElementById("event-detail");
    if (!root) return;

    root.innerHTML = `
      <section class="detail-card">
        <div class="detail-hero">
          <span class="event-status ${String(stats.status).toLowerCase()}">${statusLabel(stats.status)}</span>
          <h2>${this.escapeHtml(event.nameEvent)}</h2>
          <p>${this.escapeHtml(event.description || "Опис відсутній")}</p>
          <div class="detail-meta">
            <span>${this.sportIcon(event.typeSport)} ${this.escapeHtml(event.typeSport || "-")}</span>
            <span>${this.escapeHtml(event.system || "-")}</span>
            <span>${this.formatDate(event.dataStart)}</span>
          </div>
        </div>

        <div class="detail-tabs">
          <button class="detail-tab ${this.activeTab === "matches" ? "active" : ""}" data-tab="matches" type="button">Матчі</button>
          <button class="detail-tab ${this.activeTab === "table" ? "active" : ""}" data-tab="table" type="button">Таблиця</button>
          <button class="detail-tab ${this.activeTab === "bracket" ? "active" : ""}" data-tab="bracket" type="button">Сітка</button>
        </div>

        <div class="detail-content">${this.renderDetailContent(event)}</div>
      </section>
    `;

    root.querySelectorAll<HTMLElement>(".detail-tab").forEach(button => {
      button.addEventListener("click", () => {
        this.activeTab = (button.dataset.tab as DetailTab) || "matches";
        this.renderDetail(event);
      });
    });

    root.querySelectorAll<HTMLElement>(".match-action, .bracket-match").forEach(button => {
      button.addEventListener("click", () => {
        const id = Number(button.dataset.id);
        const type = button.dataset.type || "team";
        if (!id) return;
        this.openLiveMatch(type, id);
      });
    });
  }

  private renderDetailContent(event: CatalogEvent) {
    if (this.activeTab === "table") return this.renderStanding(event);
    if (this.activeTab === "bracket") return this.renderBracket(event);
    return this.renderAllMatches(event.matches || []);
  }

  private renderAllMatches(matchesInput: CatalogMatch[]) {
    const matches = (matchesInput || []).map(normalizeMatchForUi);
    if (!matches.length) return `<div class="empty-state compact">Матчів поки немає</div>`;

    const byRound = this.groupMatchesByRound(matches);

    return Array.from(byRound.entries()).map(([round, roundMatches]) => `
      <section class="detail-round">
        <h3>${this.escapeHtml(round)}</h3>
        <div class="detail-match-list">
          ${roundMatches.map(match => {
            const score = extractScore(match) || match.score || "vs";
            const status = getEffectiveMatchStatus(match);

            return `
              <article class="detail-match-row ${status.toLowerCase()}">
                <div>
                  <span class="match-round">${this.escapeHtml(match.matchType || "match")}${match.group ? ` · Group ${match.group}` : ""}</span>
                  <h4>${this.escapeHtml(match.firstParticipant || "-")} — ${this.escapeHtml(match.secondParticipant || "-")}</h4>
                  <p>${formatMatchDateTime(match)} · Суддя: ${this.escapeHtml(match.loginJudge || "-")}</p>
                  <small>${readinessLabel(match)}</small>
                </div>
                <div class="detail-score">
                  <b>${this.escapeHtml(score)}</b>
                  <button class="match-action" data-type="${this.escapeAttr(match.matchType)}" data-id="${match.matchId}" type="button">
                    ${canEnterMatchData(match) ? "Внести" : "Перегляд"}
                  </button>
                </div>
              </article>
            `;
          }).join("")}
        </div>
      </section>
    `).join("");
  }

  private renderStanding(event: CatalogEvent) {
    const backendRows = Array.isArray(event.standings) ? event.standings : [];
    const computedRows = buildStandingsFromMatches(event.matches || []);
    const rows = computedRows.length ? computedRows : backendRows;

    if (!rows.length) {
      return `<div class="empty-state compact">Таблиця зʼявиться після завершення хоча б одного матчу з рахунком або переможцем</div>`;
    }

    const sorted = rows.slice().sort((a: UiStandingRow, b: UiStandingRow) =>
      Number(a.group || 0) - Number(b.group || 0) ||
      Number(b.points || 0) - Number(a.points || 0) ||
      Number(b.scoreDiff || 0) - Number(a.scoreDiff || 0) ||
      Number(b.scoreFor || 0) - Number(a.scoreFor || 0) ||
      String(a.participant || "").localeCompare(String(b.participant || ""))
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
            ${sorted.map((row: UiStandingRow, index: number) => `
              <tr>
                <td>${index + 1}</td>
                <td class="participant-cell">${this.escapeHtml(row.participant || "-")}</td>
                <td>${row.played || 0}</td>
                <td>${row.wins || 0}</td>
                <td>${row.draws || 0}</td>
                <td>${row.losses || 0}</td>
                <td>${row.scoreFor || 0}</td>
                <td>${row.scoreAgainst || 0}</td>
                <td>${row.scoreDiff || 0}</td>
                <td><b>${row.points || 0}</b></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>
    `;
  }

  private renderBracket(event: CatalogEvent) {
    if (!event.bracket?.length) return `<div class="empty-state compact">Сітка ще не сформована</div>`;

    return `
      <div class="compact-bracket">
        ${event.bracket.map((round: CatalogEvent) => `
          <section class="round-card">
            <header>
              <b>Round ${round.tour}</b>
              <span>${round.group ? `Group ${round.group}` : round.bracketCode || ""}</span>
            </header>
            ${(round.matches || []).map((matchInput: CatalogMatch) => {
              const match = normalizeMatchForUi(matchInput);
              const score = extractScore(match) || match.score || "vs";
              return `
                <button class="bracket-match" data-type="${this.escapeAttr(match.matchType)}" data-id="${match.matchId}" type="button">
                  <span>${this.escapeHtml(match.firstParticipant || "-")}</span>
                  <b>${this.escapeHtml(score)}</b>
                  <span>${this.escapeHtml(match.secondParticipant || "-")}</span>
                </button>
              `;
            }).join("")}
          </section>
        `).join("")}
      </div>
    `;
  }

  private groupMatchesByRound(matches: CatalogMatch[]) {
    const map = new Map<string, CatalogMatch[]>();

    matches
      .slice()
      .sort((a, b) => Number(a.tour || 0) - Number(b.tour || 0) || Number(a.group || 0) - Number(b.group || 0) || Number(a.matchId || 0) - Number(b.matchId || 0))
      .forEach(match => {
        const key = `Round ${match.tour || "-"}${match.group ? ` · Group ${match.group}` : ""}`;
        const list = map.get(key) || [];
        list.push(match);
        map.set(key, list);
      });

    return map;
  }

  private groupBySport(events: CatalogEvent[]) {
    const map = new Map<string, CatalogEvent[]>();

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
    const clean = extractScore({ score }) || score;
    const parts = clean.replace(/ /g, "").split(/[:\-]/);
    return parts[index] || "-";
  }

  private sportIcon(sport: string) {
    const key = (sport || "").toLowerCase();
    if (key.includes("football") || key.includes("фут")) return "⚽";
    if (key.includes("basket") || key.includes("бас")) return "🏀";
    if (key.includes("tennis") || key.includes("тен")) return "🎾";
    if (key.includes("volley") || key.includes("вол")) return "🏐";
    if (key.includes("box") || key.includes("бокс")) return "🥊";
    if (key.includes("chess") || key.includes("шах")) return "♟️";
    if (key.includes("wrest")) return "🤼";
    if (key.includes("swim")) return "🏊";
    if (key.includes("athletics")) return "🏃";
    return "🏆";
  }

  private openLiveMatch(matchType: string, matchId: number) {
    new SportLiveMatchPanel("app", matchType, matchId).render();
  }

  private formatDate(value?: string | null) {
    if (!value) return "-";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return "-";
    return date.toLocaleDateString("uk-UA", { day: "2-digit", month: "2-digit", year: "numeric" });
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
