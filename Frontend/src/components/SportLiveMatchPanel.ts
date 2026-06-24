import { HubConnection } from "@microsoft/signalr";
import { sportRulesApi } from "../api/sportRulesApi";
import type { SportMatchStateDto } from "../api/sportRulesTypes";
import { NotificationKarina } from "./Notification";
import "./sportLiveMatchPanel.css";

export class SportLiveMatchPanel {
  private container: HTMLElement;
  private matchType: string;
  private matchId: number;
  private state?: SportMatchStateDto;
  private connection?: HubConnection;
  private pollingTimer?: number;
  private isEditing = false;
  private lastRenderedVersion = "";

  constructor(containerId: string, matchType: string, matchId: number) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);

    this.container = element;
    this.matchType = matchType;
    this.matchId = matchId;
  }

  async render() {
    this.container.innerHTML = `<section class="sport-live-panel"><div class="empty-card">Завантаження матчу...</div></section>`;

    try {
      this.state = await sportRulesApi.getMatchState(this.matchType, this.matchId);
      this.connectLiveUpdates();
      this.renderState(true);
    } catch (error) {
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося завантажити матч", "error");
    }
  }

  destroy() {
    if (this.pollingTimer) window.clearInterval(this.pollingTimer);
    this.connection?.stop();
  }

  private connectLiveUpdates() {
    this.connection?.stop();

    this.connection = sportRulesApi.createConnection(this.matchType, this.matchId, (state) => {
      this.handleIncomingState(state);
    });

    if (this.pollingTimer) window.clearInterval(this.pollingTimer);

    this.pollingTimer = window.setInterval(async () => {
      if (this.isEditing || this.isAnyFormFocused()) return;

      try {
        const state = await sportRulesApi.getMatchState(this.matchType, this.matchId);
        this.handleIncomingState(state);
      } catch {
        // silent fallback
      }
    }, 8000);
  }

  private handleIncomingState(state: SportMatchStateDto) {
    this.state = state;

    if (this.isEditing || this.isAnyFormFocused()) {
      this.updateReadonlyBlocksOnly();
      return;
    }

    this.renderState(false);
  }

  private makeVersion(state: SportMatchStateDto) {
    return JSON.stringify({
      score: state.score,
      winner: state.winner,
      status: state.status,
      timeline: state.timeline?.length ?? 0,
      stats: state.stats,
      canEdit: state.canEdit,
    });
  }

  private renderState(force = false) {
    if (!this.state) return;

    const version = this.makeVersion(this.state);

    if (!force && version === this.lastRenderedVersion) return;

    this.lastRenderedVersion = version;

    const s = this.state;

    this.container.innerHTML = `
      <section class="sport-live-panel">

        <header class="sport-live-header">
          <div>
            <span class="sport-badge">${this.escapeHtml(s.rules.displayName)}</span>
            <h1>${this.escapeHtml(s.firstParticipant)} <small>vs</small> ${this.escapeHtml(s.secondParticipant)}</h1>
            <p>${this.escapeHtml(s.status)} · ${this.escapeHtml(s.matchType)} #${s.matchId}</p>
          </div>

          <div class="score-board" id="live-score-board">
            <b>${this.escapeHtml(s.score || "0:0")}</b>
            <span>${s.winner ? `Winner: ${this.escapeHtml(s.winner)}` : "Winner not set"}</span>
          </div>
        </header>

        ${
          s.validationMessages?.length
            ? `<div class="validation-box">${s.validationMessages.map(x => `<p>${this.escapeHtml(x)}</p>`).join("")}</div>`
            : ""
        }

        <div class="sport-grid">
          <section class="sport-card">
            <h2>Live подія</h2>
            ${s.canEdit ? this.renderLiveForm(s) : `<div class="readonly-box">Тут можна переглядати live-дані.</div>`}
          </section>

          <section class="sport-card">
            <h2>Фінальний результат</h2>
            ${s.canEdit ? this.renderFinalForm(s) : `<div class="readonly-box">Перегляд фінального результату.</div>`}
          </section>
        </div>

        <section class="sport-card">
          <h2>Timeline матчу</h2>
          <div id="timeline-container">${this.renderTimeline(s)}</div>
        </section>

        <section class="sport-card">
          <h2>Статистика</h2>
          <div id="stats-container">${this.renderStats(s)}</div>
        </section>
      </section>
    `;

    this.bindEvents();
  }

  private updateReadonlyBlocksOnly() {
    if (!this.state) return;

    const score = document.getElementById("live-score-board");
    const timeline = document.getElementById("timeline-container");
    const stats = document.getElementById("stats-container");

    if (score) {
      score.innerHTML = `
        <b>${this.escapeHtml(this.state.score || "0:0")}</b>
        <span>${this.state.winner ? `Winner: ${this.escapeHtml(this.state.winner)}` : "Winner not set"}</span>
      `;
    }

    if (timeline) timeline.innerHTML = this.renderTimeline(this.state);
    if (stats) stats.innerHTML = this.renderStats(this.state);
  }

  private isAnyFormFocused() {
    const active = document.activeElement as HTMLElement | null;
    return !!active?.closest("#live-event-form, #final-result-form");
  }

  private bindFormEditGuards(form: HTMLFormElement | null) {
    if (!form) return;

    form.addEventListener("focusin", () => {
      this.isEditing = true;
    });

    form.addEventListener("focusout", () => {
      window.setTimeout(() => {
        this.isEditing = this.isAnyFormFocused();
      }, 150);
    });

    form.addEventListener("input", () => {
      this.isEditing = true;
    });
  }

  private renderLiveForm(s: SportMatchStateDto) {
    return `
      <form id="live-event-form" class="sport-form" autocomplete="off">
        <label>Тип події</label>
        <select name="type">
          ${s.rules.liveEvents.map(event => `<option value="${this.escapeHtml(event)}">${this.escapeHtml(s.rules.eventLabels[event] || this.label(event))}</option>`).join("")}
        </select>

        <label>Учасник</label>
        <select name="participant">
          <option value="${this.escapeHtml(s.firstParticipant)}">${this.escapeHtml(s.firstParticipant)}</option>
          <option value="${this.escapeHtml(s.secondParticipant)}">${this.escapeHtml(s.secondParticipant)}</option>
        </select>

        <label>Гравець / спортсмен</label>
        <input name="player" placeholder="Наприклад: Іван Петренко" />

        <div class="form-row">
          <div>
            <label>Хвилина / час</label>
            <input name="minute" type="number" min="0" placeholder="45" />
          </div>
          <div>
            <label>Період / сет / раунд</label>
            <input name="period" type="number" min="0" max="${s.rules.maxPeriods || 99}" placeholder="1" />
          </div>
        </div>

        <label>Значення</label>
        <input name="value" placeholder="Наприклад: +1, yellow, KO" />

        <label>Нотатка</label>
        <textarea name="notes" placeholder="Коментар судді"></textarea>

        <button class="primary-btn" type="submit">Додати live-подію</button>
      </form>
    `;
  }

  private renderFinalForm(s: SportMatchStateDto) {
    return `
      <form id="final-result-form" class="sport-form" autocomplete="off">
        <label>Рахунок</label>
        <input name="score" value="${this.escapeHtml(s.score || "")}" placeholder="${this.escapeHtml(s.rules.scorePatternHint || "2:1")}" required />

        <label>Переможець</label>
        <select name="winner">
          <option value="">Автоматично по рахунку / нічия</option>
          <option value="${this.escapeHtml(s.firstParticipant)}" ${s.winner === s.firstParticipant ? "selected" : ""}>${this.escapeHtml(s.firstParticipant)}</option>
          <option value="${this.escapeHtml(s.secondParticipant)}" ${s.winner === s.secondParticipant ? "selected" : ""}>${this.escapeHtml(s.secondParticipant)}</option>
        </select>

        <div class="dynamic-fields">
          ${s.rules.resultFields
            .filter(field => !["score", "winner"].includes(field))
            .map(field => `
              <div>
                <label>${this.escapeHtml(s.rules.fieldLabels[field] || this.label(field))}</label>
                <input name="stat_${this.escapeHtml(field)}" value="${this.escapeHtml(s.stats[field] || "")}" />
              </div>
            `)
            .join("")}
        </div>

        <label>Нотатки</label>
        <textarea name="notes">${this.escapeHtml(s.stats["notes"] || "")}</textarea>

        <button class="primary-btn" type="submit">Зберегти фінальний результат</button>
      </form>
    `;
  }

  private renderTimeline(s: SportMatchStateDto) {
    if (!s.timeline.length) return `<div class="readonly-box">Live-подій ще немає</div>`;

    return `
      <div class="timeline-list">
        ${s.timeline
          .slice()
          .reverse()
          .map(item => `
            <article class="timeline-item">
              <b>${this.escapeHtml(s.rules.eventLabels[item.type] || this.label(item.type))}</b>
              <span>${this.escapeHtml(item.participant)}</span>
              ${item.player ? `<small>Гравець: ${this.escapeHtml(item.player)}</small>` : ""}
              ${item.minute !== null && item.minute !== undefined ? `<small>Час: ${item.minute}'</small>` : ""}
              ${item.period !== null && item.period !== undefined ? `<small>Період: ${item.period}</small>` : ""}
              ${item.value ? `<small>Значення: ${this.escapeHtml(item.value)}</small>` : ""}
              ${item.createdBy ? `<small>Вніс: ${this.escapeHtml(item.createdBy)}</small>` : ""}
              ${item.notes ? `<p>${this.escapeHtml(item.notes)}</p>` : ""}
            </article>
          `)
          .join("")}
      </div>
    `;
  }

 private renderStats(s: SportMatchStateDto) {
  if (!Object.keys(s.stats).length) {
    return `<div class="readonly-box">Статистика ще не внесена</div>`;
  }

  const items: string[] = [];

  for (const key in s.stats) {
    const value = s.stats[key];

    items.push(`
      <div>
        <span>${this.label(key)}</span>
        <b>${this.escapeHtml(value)}</b>
      </div>
    `);
  }

  return `
    <div class="stats-list">
      ${items.join("")}
    </div>
  `;
}

  private bindEvents() {
    const liveForm = document.getElementById("live-event-form") as HTMLFormElement | null;
    const finalForm = document.getElementById("final-result-form") as HTMLFormElement | null;

    this.bindFormEditGuards(liveForm);
    this.bindFormEditGuards(finalForm);

    document.getElementById("back-to-tournaments-btn")?.addEventListener("click", async () => {
      this.destroy();
      const mod = await import("./TournamentsPage");
      new mod.TournamentsPage("app").render();
    });

    liveForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      this.isEditing = false;

      const form = new FormData(liveForm);

      try {
        this.state = await sportRulesApi.addLiveEvent({
          matchType: this.matchType,
          matchId: this.matchId,
          type: String(form.get("type") || ""),
          participant: String(form.get("participant") || ""),
          player: String(form.get("player") || ""),
          minute: this.toNumber(form.get("minute")),
          period: this.toNumber(form.get("period")),
          value: String(form.get("value") || ""),
          notes: String(form.get("notes") || ""),
        });

        liveForm.reset();
        this.renderState(true);
        new NotificationKarina().show("Live-подію додано", "success");
      } catch (error) {
        this.isEditing = true;
        new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося додати подію", "error");
      }
    });

    finalForm?.addEventListener("submit", async (event) => {
      event.preventDefault();
      this.isEditing = false;

      const form = new FormData(finalForm);
      const stats: Record<string, string> = {};

      form.forEach((value, key) => {
        if (key.startsWith("stat_")) stats[key.replace("stat_", "")] = String(value || "");
      });

      try {
        this.state = await sportRulesApi.submitFinalResult({
          matchType: this.matchType,
          matchId: this.matchId,
          score: String(form.get("score") || ""),
          winner: String(form.get("winner") || ""),
          stats,
          notes: String(form.get("notes") || ""),
          finishMatch: true,
        });

        this.renderState(true);
        new NotificationKarina().show("Фінальний результат збережено", "success");
      } catch (error) {
        this.isEditing = true;
        new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося зберегти результат", "error");
      }
    });
  }

 private label(value: string) {
  return value
    .replace(/_/g, " ")
    .replace(/\b\w/g, letter => letter.toUpperCase());
}

  private toNumber(value: FormDataEntryValue | null) {
    const text = String(value || "");
    const number = Number(text);
    return Number.isFinite(number) && text !== "" ? number : undefined;
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
