import { eventApi } from "../api/eventApi";
import type { Stage3EventWorkspaceDto, Stage3MatchDto } from "../api/eventTypes";
import { NotificationKarina } from "./Notification";
import "./eventRuntimePage.css";

export class EventRuntimePage {
  private container: HTMLElement;
  private eventName: string;
  private data?: Stage3EventWorkspaceDto;

  constructor(containerId: string, eventName?: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
    this.eventName = eventName || "";
  }

  async render() {
    if (!this.eventName) {
      this.container.innerHTML = `<section class="event-runtime"><h1>Турніри</h1><div class="runtime-search"><input id="runtimeEventName" placeholder="Назва заходу"/><button id="runtimeLoadBtn">Відкрити</button></div></section>`;
      document.getElementById("runtimeLoadBtn")?.addEventListener("click", () => { const name = (document.getElementById("runtimeEventName") as HTMLInputElement).value.trim(); if (name) new EventRuntimePage("app", name).render(); });
      return;
    }
    this.container.innerHTML = `<section class="event-runtime"><div class="empty-state">Завантаження заходу...</div></section>`;
    try { this.data = await eventApi.getWorkspace(this.eventName); this.renderWorkspace(); }
    catch (error) { this.container.innerHTML = `<section class="event-runtime"><div class="empty-state">${error instanceof Error ? error.message : "Не вдалося завантажити захід"}</div></section>`; }
  }

  private renderWorkspace() {
    const d = this.data!;
    this.container.innerHTML = `
      <section class="event-runtime">
        <div class="runtime-header"><div><h1>${this.escapeHtml(d.nameEvent)}</h1><p>${this.escapeHtml(d.typeSport)} · ${this.escapeHtml(d.systemName)}</p></div><button id="runtimeRefreshBtn">Оновити</button></div>
        <div class="runtime-summary"><div><b>${d.summary.participantsCount}</b><span>учасників</span></div><div><b>${d.summary.matchesCount}</b><span>матчів</span></div><div><b>${d.summary.finishedMatches}</b><span>завершено</span></div><div><b>${this.escapeHtml(d.summary.winner || "-")}</b><span>лідер</span></div></div>
        <div class="runtime-layout"><section><h2>Матчі / сітка</h2><div class="matches-board">${d.matches.map(m => this.renderMatch(m)).join("") || `<div class="empty-state">Матчів немає</div>`}</div></section><section><h2>Таблиця</h2>${this.renderStandings()}</section></div>
      </section>`;
    document.getElementById("runtimeRefreshBtn")?.addEventListener("click", () => this.render());
    this.container.querySelectorAll<HTMLButtonElement>("button[data-result]").forEach(btn => btn.addEventListener("click", () => this.openResult(btn.dataset.type!, Number(btn.dataset.id))));
    this.container.querySelectorAll<HTMLButtonElement>("button[data-live]").forEach(btn => btn.addEventListener("click", () => this.setLive(btn.dataset.type!, Number(btn.dataset.id))));
  }

  private renderMatch(m: Stage3MatchDto) {
    return `<article class="runtime-match ${m.status.toLowerCase()}"><div class="match-top"><span>Раунд ${m.tour}${m.group ? ` · Група ${m.group}` : ""}</span><b>${this.escapeHtml(m.status)}</b></div><div class="match-teams"><strong>${this.escapeHtml(m.entity1)}</strong><span>${m.score1} : ${m.score2}</span><strong>${this.escapeHtml(m.entity2)}</strong></div><div class="match-winner">Переможець: ${this.escapeHtml(m.winner || "-")}</div><div class="match-actions"><button data-live="1" data-type="${m.matchType}" data-id="${m.idMatch}">Live</button><button data-result="1" data-type="${m.matchType}" data-id="${m.idMatch}">Внести результат</button></div></article>`;
  }

  private renderStandings() {
    const rows = this.data!.standings.map((s, i) => `<tr><td>${i + 1}</td><td>${this.escapeHtml(s.name)}</td><td>${s.played}</td><td>${s.wins}</td><td>${s.draws}</td><td>${s.losses}</td><td>${s.scoreFor}:${s.scoreAgainst}</td><td>${s.scoreDiff}</td><td>${s.points}</td></tr>`).join("");
    return `<table class="standings"><thead><tr><th>#</th><th>Учасник</th><th>І</th><th>В</th><th>Н</th><th>П</th><th>Рах</th><th>±</th><th>Очки</th></tr></thead><tbody>${rows}</tbody></table>`;
  }

  private async setLive(matchType: string, idMatch: number) {
    try { await eventApi.changeStatus({ matchType, idMatch, status: 0 }); await this.render(); }
    catch (error) { new NotificationKarina().show(error instanceof Error ? error.message : "Помилка статусу", "error"); }
  }

  private async openResult(matchType: string, idMatch: number) {
    const match = this.data!.matches.find(x => x.matchType === matchType && x.idMatch === idMatch);
    if (!match) return;
    const score1 = Number(prompt(`Рахунок для ${match.entity1}`, String(match.score1)) ?? match.score1);
    const score2 = Number(prompt(`Рахунок для ${match.entity2}`, String(match.score2)) ?? match.score2);
    const isDraw = score1 === score2;
    const winner = isDraw ? "" : (score1 > score2 ? match.entity1 : match.entity2);
    try {
      this.data = await eventApi.setResult({ matchType, idMatch, scoreEntity1: score1, scoreEntity2: score2, winner, isDraw, closeMatch: true });
      new NotificationKarina().show("Результат збережено", "success");
      this.renderWorkspace();
    } catch (error) { new NotificationKarina().show(error instanceof Error ? error.message : "Помилка збереження", "error"); }
  }

  private escapeHtml(value: unknown) {
    return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
  }}
