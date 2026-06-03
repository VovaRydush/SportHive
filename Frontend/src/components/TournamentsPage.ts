import { eventCatalogApi } from "../api/eventCatalogApi";
import type { EventCatalogItemDto, EventMatchCatalogDto } from "../api/eventCatalogTypes";
import { NotificationKarina } from "./Notification";
import "./tournamentsPage.css";

export class TournamentsPage {
  private container: HTMLElement;
  private events: EventCatalogItemDto[] = [];
  private selectedEvent?: EventCatalogItemDto;
  private searchTimer?: number;
  private bracketScale = 1;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="tournaments-page">
        <div class="tournaments-hero">
          <div>
            <h1>Турніри SportHive</h1>
            <p>Усі турніри, матчі, сітка, таблиця та права доступу в одному місці.</p>
          </div>
          <button id="reload-tournaments" class="primary-btn" type="button">Оновити</button>
        </div>

        <div class="tournament-filters">
          <input id="tournament-search" type="text" placeholder="Швидкий пошук по назві, спорту, системі..." />
          <select id="tournament-status">
            <option value="all">Усі статуси</option>
            <option value="Upcoming">Upcoming</option>
            <option value="Live">Live</option>
            <option value="Finished">Finished</option>
          </select>
          <select id="tournament-system">
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

        <div class="tournament-layout">
          <aside id="tournaments-list" class="tournaments-list">
            <div class="loading-card">Завантаження турнірів...</div>
          </aside>

          <main id="tournament-details" class="tournament-details">
            <div class="empty-card">Оберіть турнір зі списку</div>
          </main>
        </div>
      </section>
    `;

    this.bindBaseEvents();
    await this.loadEvents();
  }

  private bindBaseEvents() {
    document.getElementById("reload-tournaments")?.addEventListener("click", () => this.loadEvents());

    const search = document.getElementById("tournament-search") as HTMLInputElement | null;
    const status = document.getElementById("tournament-status") as HTMLSelectElement | null;
    const system = document.getElementById("tournament-system") as HTMLSelectElement | null;

    search?.addEventListener("input", () => {
      window.clearTimeout(this.searchTimer);
      this.searchTimer = window.setTimeout(() => this.loadEvents(), 350);
    });

    status?.addEventListener("change", () => this.loadEvents());
    system?.addEventListener("change", () => this.loadEvents());
  }

  private async loadEvents() {
    const list = document.getElementById("tournaments-list");
    if (list) list.innerHTML = `<div class="loading-card">Завантаження турнірів...</div>`;

    const search = (document.getElementById("tournament-search") as HTMLInputElement | null)?.value || "";
    const status = (document.getElementById("tournament-status") as HTMLSelectElement | null)?.value || "all";
    const system = (document.getElementById("tournament-system") as HTMLSelectElement | null)?.value || "all";

    try {
      this.events = await eventCatalogApi.getEvents({ search, status, system });

      this.renderList();

      if (this.events.length) {
        const id = this.selectedEvent?.idEvent;
        const next = this.events.find(e => e.idEvent === id) || this.events[0];
        await this.openEvent(next.idEvent);
      } else {
        this.renderDetailsEmpty("Турнірів не знайдено");
      }
    } catch (error) {
      console.error("Помилка завантаження турнірів:", error);
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося завантажити турніри", "error");
      if (list) list.innerHTML = `<div class="empty-card error">Не вдалося завантажити турніри</div>`;
    }
  }

  private renderList() {
    const list = document.getElementById("tournaments-list");
    if (!list) return;

    if (!this.events.length) {
      list.innerHTML = `<div class="empty-card">Турнірів немає</div>`;
      return;
    }

    list.innerHTML = this.events.map(ev => `
      <button class="tournament-list-card ${this.selectedEvent?.idEvent === ev.idEvent ? "active" : ""}" data-id="${ev.idEvent}" type="button">
        <span class="status-dot ${ev.status.toLowerCase()}"></span>
        <div>
          <strong>${this.escapeHtml(ev.nameEvent)}</strong>
          <small>${this.escapeHtml(ev.typeSport)} · ${this.escapeHtml(ev.system)}</small>
          <small>${ev.finishedMatches}/${ev.totalMatches} матчів завершено</small>
        </div>
      </button>
    `).join("");

    list.querySelectorAll<HTMLButtonElement>(".tournament-list-card").forEach(btn => {
      btn.addEventListener("click", () => this.openEvent(Number(btn.dataset.id)));
    });
  }

  private async openEvent(idEvent: number) {
    try {
      this.bracketScale = 1;
      this.selectedEvent = await eventCatalogApi.getEvent(idEvent);
      this.renderList();
      this.renderDetails(this.selectedEvent);
    } catch (error) {
      console.error("Помилка відкриття турніру:", error);
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося відкрити турнір", "error");
    }
  }

  private renderDetails(ev: EventCatalogItemDto) {
    const root = document.getElementById("tournament-details");
    if (!root) return;

    root.innerHTML = `
      <div class="event-detail-header">
        <img src="${this.escapeHtml(ev.eventPhoto || "https://placehold.co/900x280?text=Tournament")}" alt="${this.escapeHtml(ev.nameEvent)}" />
        <div>
          <span class="event-status ${ev.status.toLowerCase()}">${this.escapeHtml(ev.status)}</span>
          <h2>${this.escapeHtml(ev.nameEvent)}</h2>
          <p>${this.escapeHtml(ev.description || "")}</p>
          <div class="event-meta">
            <span>${this.escapeHtml(ev.typeSport)}</span>
            <span>${this.escapeHtml(ev.system)}</span>
            <span>${this.formatDate(ev.dataStart)}</span>
            <span>${this.escapeHtml(ev.accessLevel)}</span>
          </div>

          ${
            ev.canManageEvent && ev.status !== "Finished" && ev.system !== "QualificationByStandards"
              ? `<div class="event-actions">
                  <button id="generate-next-round-btn" class="primary-btn" type="button">Згенерувати наступний раунд</button>
                  <button id="rebuild-event-btn" class="secondary-btn" type="button">Перерахувати</button>
                </div>`
              : ""
          }
        </div>
      </div>

      <div class="event-summary-grid">
        <div class="summary-card"><b>${ev.totalMatches}</b><span>Усього матчів</span></div>
        <div class="summary-card live"><b>${ev.liveMatches}</b><span>Live</span></div>
        <div class="summary-card finished"><b>${ev.finishedMatches}</b><span>Завершені</span></div>
        <div class="summary-card upcoming"><b>${ev.upcomingMatches}</b><span>Очікують</span></div>
      </div>

      <div class="event-tabs">
        <button class="event-tab active" data-tab="matches" type="button">Матчі</button>
        <button class="event-tab" data-tab="bracket" type="button">Вертикальна сітка</button>
        <button class="event-tab" data-tab="standings" type="button">Таблиця</button>
      </div>

      <section id="tab-matches" class="event-tab-content active">
        ${this.renderMatches(ev.matches)}
      </section>

      <section id="tab-bracket" class="event-tab-content">
        ${this.renderBracket(ev)}
      </section>

      <section id="tab-standings" class="event-tab-content">
        ${this.renderStandings(ev)}
      </section>
    `;

    this.bindDetailsEvents(ev);
  }

  private bindDetailsEvents(ev: EventCatalogItemDto) {
    document.querySelectorAll<HTMLButtonElement>(".event-tab").forEach(tab => {
      tab.addEventListener("click", () => {
        document.querySelectorAll(".event-tab").forEach(x => x.classList.remove("active"));
        document.querySelectorAll(".event-tab-content").forEach(x => x.classList.remove("active"));

        tab.classList.add("active");
        document.getElementById(`tab-${tab.dataset.tab}`)?.classList.add("active");
      });
    });

    document.getElementById("generate-next-round-btn")?.addEventListener("click", async () => {
      try {
        this.selectedEvent = await eventCatalogApi.generateNextRound(ev.idEvent);
        this.renderDetails(this.selectedEvent);
        new NotificationKarina().show("Наступний раунд згенеровано", "success");
      } catch (error) {
        new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося згенерувати раунд", "error");
      }
    });

    document.getElementById("rebuild-event-btn")?.addEventListener("click", async () => {
      try {
        this.selectedEvent = await eventCatalogApi.rebuild(ev.idEvent);
        this.renderDetails(this.selectedEvent);
        new NotificationKarina().show("Турнір перераховано", "success");
      } catch (error) {
        new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося перерахувати", "error");
      }
    });

    document.getElementById("bracket-zoom-in")?.addEventListener("click", () => this.changeBracketZoom(0.1));
    document.getElementById("bracket-zoom-out")?.addEventListener("click", () => this.changeBracketZoom(-0.1));
    document.getElementById("bracket-zoom-reset")?.addEventListener("click", () => this.resetBracketZoom());

    document.querySelectorAll<HTMLButtonElement>(".submit-result-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        const matchType = btn.dataset.type || "";
        const matchId = Number(btn.dataset.id);
        const score = prompt("Рахунок / результат. Наприклад 2:1");
        if (!score) return;

        const winner = prompt("Переможець. Введи назву команди або login спортсмена. Можна залишити пустим, якщо рахуємо по score.") || "";

        try {
          await eventCatalogApi.submitResult({
            matchType,
            matchId,
            score,
            winner,
            notes: "",
            finishMatch: true,
          });

          await this.openEvent(ev.idEvent);
          new NotificationKarina().show("Результат збережено", "success");
        } catch (error) {
          new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося зберегти результат", "error");
        }
      });
    });
  }

  private changeBracketZoom(delta: number) {
    this.bracketScale = Math.min(1.6, Math.max(0.55, Number((this.bracketScale + delta).toFixed(2))));
    this.applyBracketZoom();
  }

  private resetBracketZoom() {
    this.bracketScale = 1;
    this.applyBracketZoom();
  }

  private applyBracketZoom() {
    const canvas = document.getElementById("bracket-canvas") as HTMLElement | null;
    const label = document.getElementById("bracket-zoom-value");

    if (!canvas) return;

    canvas.style.transform = `scale(${this.bracketScale})`;

    if (label) {
      label.textContent = `${Math.round(this.bracketScale * 100)}%`;
    }
  }

  private renderMatches(matches: EventMatchCatalogDto[]) {
    if (!matches.length) return `<div class="empty-card">Матчів поки немає</div>`;

    return `
      <div class="matches-list">
        ${matches.map(m => `
          <article class="match-row ${m.status.toLowerCase()}">
            <div class="match-main">
              <span class="match-type">${this.escapeHtml(m.matchType)} · Round ${m.tour}${m.group ? ` · Group ${m.group}` : ""}</span>
              <h3>${this.escapeHtml(m.firstParticipant)} <span>vs</span> ${this.escapeHtml(m.secondParticipant)}</h3>
              <p>${this.escapeHtml(m.locationName || "-")} · ${this.formatDate(m.dataMatch)} · ${this.escapeHtml(String(m.timeMatch || ""))}</p>
              ${m.score ? `<p class="score-line">Score: ${this.escapeHtml(m.score)} ${m.winner ? `· Winner: ${this.escapeHtml(m.winner)}` : ""}</p>` : ""}
              <p class="access-line">${this.escapeHtml(m.accessReason)} · Judge: ${this.escapeHtml(m.loginJudge || "-")}</p>
            </div>
            <div class="match-side">
              <span class="match-status ${m.status.toLowerCase()}">${this.escapeHtml(m.status)}</span>
              ${m.canEdit ? `<button class="submit-result-btn primary-btn" data-type="${m.matchType}" data-id="${m.matchId}" type="button">Внести результат</button>` : `<span class="view-only">Тільки перегляд</span>`}
            </div>
          </article>
        `).join("")}
      </div>
    `;
  }

  private renderBracket(ev: EventCatalogItemDto) {
    if (!ev.bracket.length) return `<div class="empty-card">Сітка ще не сформована</div>`;

    const rounds = ev.bracket
      .slice()
      .sort((a, b) => a.tour - b.tour || (a.group ?? 0) - (b.group ?? 0));

    return `
      <div class="bracket-panel vertical-bracket-panel">
        <div class="bracket-toolbar">
          <div>
            <h3>Вертикальна сітка турніру</h3>
            <p>Раунди йдуть зверху вниз. Переможці підсвічені, а стрілки показують прохід у наступний раунд.</p>
          </div>
          <div class="bracket-zoom-controls">
            <button id="bracket-zoom-out" type="button">−</button>
            <span id="bracket-zoom-value">100%</span>
            <button id="bracket-zoom-in" type="button">+</button>
            <button id="bracket-zoom-reset" type="button">Reset</button>
          </div>
        </div>

        <div class="vertical-bracket-viewport">
          <div id="bracket-canvas" class="vertical-bracket-map" style="transform: scale(${this.bracketScale});">
            ${rounds.map((round, roundIndex) => `
              <section class="vertical-round" data-round="${round.tour}">
                <div class="vertical-round-header">
                  <div>
                    <span class="round-kicker">Round ${round.tour}</span>
                    <h4>${round.group ? `Group ${round.group}` : this.escapeHtml(round.bracketCode)}</h4>
                  </div>
                  <span class="round-count">${round.matches.length} матчів</span>
                </div>

                <div class="vertical-round-grid">
                  ${round.matches.map((match, matchIndex) => this.renderVerticalBracketMatch(match, roundIndex, matchIndex)).join("")}
                </div>

                ${roundIndex < rounds.length - 1 ? `
                  <div class="round-flow">
                    <span></span>
                    <b>переможці проходять нижче</b>
                    <span></span>
                  </div>
                ` : ""}
              </section>
            `).join("")}
          </div>
        </div>

        <div class="bracket-legend">
          <span><i class="legend-dot upcoming"></i> Upcoming</span>
          <span><i class="legend-dot live"></i> Live</span>
          <span><i class="legend-dot finished"></i> Finished</span>
          <span><i class="legend-line vertical"></i> прохід у наступний раунд</span>
        </div>
      </div>
    `;
  }

  private renderVerticalBracketMatch(match: EventMatchCatalogDto, roundIndex: number, matchIndex: number) {
    const winner = match.winner || this.resolveWinnerByScore(match);
    const firstWinner = winner && this.isWinner(match.firstParticipant, match.firstLogin, winner);
    const secondWinner = winner && this.isWinner(match.secondParticipant, match.secondLogin, winner);

    return `
      <article class="vertical-match-card ${match.status.toLowerCase()} ${winner ? "has-winner" : ""}">
        <div class="vertical-match-top">
          <span>${this.escapeHtml(match.matchType)} #${match.matchId}</span>
          <b>${this.escapeHtml(match.status)}</b>
        </div>

        <div class="vertical-match-body">
          <div class="vertical-player ${firstWinner ? "winner" : ""}">
            <span class="seed-number">${matchIndex * 2 + 1}</span>
            <strong>${this.escapeHtml(match.firstParticipant)}</strong>
          </div>

          <div class="vertical-score">
            <span>${this.escapeHtml(match.score || "vs")}</span>
          </div>

          <div class="vertical-player ${secondWinner ? "winner" : ""}">
            <span class="seed-number">${matchIndex * 2 + 2}</span>
            <strong>${this.escapeHtml(match.secondParticipant)}</strong>
          </div>
        </div>

        <div class="vertical-match-footer">
          <span>${this.escapeHtml(match.locationName || "-")}</span>
          ${winner ? `<em>Переможець: ${this.escapeHtml(winner)}</em>` : `<em>Очікує результат</em>`}
        </div>

        ${winner ? `<div class="winner-flow-arrow">↓</div>` : ""}
      </article>
    `;
  }

  private resolveWinnerByScore(match: EventMatchCatalogDto) {
    if (!match.score) return "";

    const parsed = match.score.match(/(-?\d+)\s*[:\-]\s*(-?\d+)/);

    if (!parsed) return "";

    const first = Number(parsed[1]);
    const second = Number(parsed[2]);

    if (first > second) return match.firstParticipant;
    if (second > first) return match.secondParticipant;

    return "";
  }

  private isWinner(name: string, login: string | null | undefined, winner: string) {
    return winner === name || winner === login;
  }

  private renderStandings(ev: EventCatalogItemDto) {
    if (!ev.standings.length) return `<div class="empty-card">Таблиця буде після завершених матчів</div>`;

    const sorted = ev.standings
      .slice()
      .sort((a, b) =>
        a.group - b.group ||
        b.points - a.points ||
        b.scoreDiff - a.scoreDiff ||
        b.scoreFor - a.scoreFor ||
        a.participant.localeCompare(b.participant)
      );

    return `
      <div class="standings-panel">
        <div class="standings-header">
          <div>
            <h3>Турнірна таблиця</h3>
            <p>P — очки, W/D/L — перемоги/нічиї/поразки, SF/SA — забито/пропущено, +/- — різниця.</p>
          </div>
          <div class="standings-badges">
            <span>${ev.standings.length} учасників</span>
            <span>${ev.finishedMatches} завершених матчів</span>
          </div>
        </div>

        <div class="standings-table-wrap">
          <table class="standings-table upgraded">
            <thead>
              <tr>
                <th>#</th>
                <th>Учасник</th>
                <th>Group</th>
                <th title="Played">PL</th>
                <th title="Points">P</th>
                <th title="Wins">W</th>
                <th title="Draws">D</th>
                <th title="Losses">L</th>
                <th title="Score For">SF</th>
                <th title="Score Against">SA</th>
                <th title="Score Difference">+/-</th>
                <th>Форма</th>
              </tr>
            </thead>
            <tbody>
              ${sorted.map((s, index) => `
                <tr class="${index < 2 ? "top-place" : ""}">
                  <td><span class="rank-badge">${index + 1}</span></td>
                  <td>
                    <strong>${this.escapeHtml(s.participant)}</strong>
                    ${s.participantLogin ? `<small>${this.escapeHtml(s.participantLogin)}</small>` : ""}
                  </td>
                  <td>${s.group}</td>
                  <td>${s.played}</td>
                  <td><b>${s.points}</b></td>
                  <td>${s.wins}</td>
                  <td>${s.draws}</td>
                  <td>${s.losses}</td>
                  <td>${s.scoreFor}</td>
                  <td>${s.scoreAgainst}</td>
                  <td class="${s.scoreDiff > 0 ? "positive" : s.scoreDiff < 0 ? "negative" : ""}">${s.scoreDiff}</td>
                  <td>${this.renderFormDots(s.wins, s.draws, s.losses)}</td>
                </tr>
              `).join("")}
            </tbody>
          </table>
        </div>
      </div>
    `;
  }

  private renderFormDots(wins: number, draws: number, losses: number) {
    const dots: string[] = [];
    for (let i = 0; i < Math.min(wins, 5); i++) dots.push(`<i class="form-dot win">W</i>`);
    for (let i = 0; i < Math.min(draws, 5 - dots.length); i++) dots.push(`<i class="form-dot draw">D</i>`);
    for (let i = 0; i < Math.min(losses, 5 - dots.length); i++) dots.push(`<i class="form-dot loss">L</i>`);
    return dots.length ? dots.join("") : `<span class="muted">—</span>`;
  }

  private renderDetailsEmpty(text: string) {
    const root = document.getElementById("tournament-details");
    if (root) root.innerHTML = `<div class="empty-card">${this.escapeHtml(text)}</div>`;
  }

  private formatDate(value?: string | null) {
    if (!value) return "-";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return "-";
    return date.toLocaleDateString("uk-UA");
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
