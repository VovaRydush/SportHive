import { authApi } from "../api/authApi";
import { eventApi } from "../api/eventApi";
import type { SelectionSystem } from "../api/eventTypes";
import { NotificationKarina } from "./Notification";
import { EventRuntimePage } from "./EventRuntimePage";
import "./eventPage.css";

type Participant = { id: string; name: string; type: "team" | "individual" | "extreme" };

export class CreateEventPage {
  private container: HTMLElement;
  private selected: Participant[] = [];

  constructor(containerId: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    const sports = [
      { value: "Football", label: "Футбол" },
      { value: "Basketball", label: "Баскетбол" },
      { value: "Volleyball", label: "Волейбол" },
      { value: "Tennis", label: "Теніс" },
      { value: "TableTennis", label: "Настільний теніс" },
      { value: "Hockey", label: "Хокей" },
      { value: "Boxing", label: "Бокс" },
      { value: "Wrestling", label: "Боротьба" },
      { value: "Chess", label: "Шахи" },
      { value: "Checkers", label: "Шашки" },
      { value: "Badminton", label: "Бадмінтон" },
      { value: "Baseball", label: "Бейсбол" },
    ];

    const systems = [
      { value: 1, name: "RoundRobin / кругова система" },
      { value: 2, name: "PlayOff" },
      { value: 4, name: "SwissSystem / швейцарська" },
      { value: 5, name: "QualificationByStandards / нормативи" },
      { value: 6, name: "GroupStage / групи" },
      { value: 8, name: "OlympicSystem" },
      { value: 9, name: "KnockoutSystem" },
      { value: 10, name: "DoubleElimination" },
    ];

    this.container.innerHTML = `
      <section class="create-event-page">
        <h1>Створення заходу</h1>
        <form id="createEventForm" class="event-form">
          <div class="form-section">
            <h2>Основна інформація</h2>
            <div class="form-group"><label>Назва заходу</label><input id="eventName" required maxlength="100" /></div>
            <div class="form-group"><label>Вид спорту</label><select id="sportType" required>${sports.map(s => `<option value="${s.value}">${s.label}</option>`).join("")}</select></div>
            <div class="form-group"><label>Система відбору</label><select id="systemType" required>${systems.map(s => `<option value="${s.value}">${s.name}</option>`).join("")}</select></div>
            <div class="form-group"><label>Тип учасників</label><select id="participantType"><option value="team">Команди</option><option value="individual">Спортсмени</option><option value="extreme">Нормативи / заїзд / рейтинг</option></select></div>
            <div class="form-group"><label>Логін судді</label><input id="loginJudge" placeholder="необов'язково" /></div>
            <div class="form-group"><label>Опис</label><textarea id="eventDescription" rows="4"></textarea></div>
          </div>
          <div class="form-section"><h2>Дата</h2><div class="date-grid"><div class="form-group"><label>Початок</label><input type="date" id="startDate" required /></div><div class="form-group"><label>Кінець</label><input type="date" id="endDate" /></div></div></div>
          <div class="form-section">
            <h2>Учасники</h2>
            <p class="hint">Для команд введи точну назву команди й натисни “Додати команду”. Для спортсменів працює пошук по ПІБ.</p>
            <div class="search-box"><input id="participantSearch" placeholder="Назва команди або ПІБ спортсмена" /><button type="button" id="searchBtn">Пошук</button><button type="button" id="addTeamBtn">Додати команду</button></div>
            <div id="availableParticipants" class="participants-list"></div>
            <h3>Обрані учасники</h3><div id="selectedParticipants" class="participants-list"></div>
          </div>
          <div class="form-actions"><button type="submit" class="btn btn-primary" id="createEventBtn">Створити захід і сітку</button></div>
        </form>
      </section>`;

    this.bindEvents();
    this.renderSelected();
  }

  private bindEvents() {
    document.getElementById("searchBtn")?.addEventListener("click", () => this.searchParticipant());
    document.getElementById("addTeamBtn")?.addEventListener("click", () => this.addTeamByName());
    document.getElementById("createEventForm")?.addEventListener("submit", (e) => this.submit(e));
  }

  private async searchParticipant() {
    const type = (document.getElementById("participantType") as HTMLSelectElement).value as any;
    const query = (document.getElementById("participantSearch") as HTMLInputElement).value.trim();
    const container = document.getElementById("availableParticipants")!;
    if (!query) return;
    if (type === "team") { this.addTeamByName(); return; }
    container.innerHTML = "Пошук...";
    try {
      const response = await authApi.searchAthlete(query);
      const list = Array.isArray(response) ? response : [response];
      container.innerHTML = list.map((x: any) => {
        const login = x.login || x.Login;
        const name = x.fullName || x.FullName || x.name || login;
        return `<div class="participant-card"><span>${this.escapeHtml(name)} (${this.escapeHtml(login)})</span><button type="button" data-login="${this.escapeHtml(login)}" data-name="${this.escapeHtml(name)}">Додати</button></div>`;
      }).join("") || "Нічого не знайдено";
      container.querySelectorAll<HTMLButtonElement>("button[data-login]").forEach(btn => btn.addEventListener("click", () => this.addParticipant(btn.dataset.login!, btn.dataset.name!, type)));
    } catch (error) { container.innerHTML = error instanceof Error ? error.message : "Помилка пошуку"; }
  }

  private addTeamByName() {
    const input = document.getElementById("participantSearch") as HTMLInputElement;
    const name = input.value.trim();
    if (!name) return;
    this.addParticipant(name, name, "team");
    input.value = "";
  }

  private addParticipant(id: string, name: string, type: "team" | "individual" | "extreme") {
    if (this.selected.some(x => x.id === id)) return;
    this.selected.push({ id, name, type });
    this.renderSelected();
  }

  private renderSelected() {
    const container = document.getElementById("selectedParticipants");
    if (!container) return;
    container.innerHTML = this.selected.length ? this.selected.map((p, i) => `<div class="participant-card"><span>${this.escapeHtml(p.name)}</span><button type="button" data-i="${i}">×</button></div>`).join("") : `<div class="empty-state">Учасників ще не вибрано</div>`;
    container.querySelectorAll<HTMLButtonElement>("button[data-i]").forEach(btn => btn.addEventListener("click", () => { this.selected.splice(Number(btn.dataset.i), 1); this.renderSelected(); }));
  }

  private async submit(e: Event) {
    e.preventDefault();
    const notify = new NotificationKarina();
    const nameEvent = (document.getElementById("eventName") as HTMLInputElement).value.trim();
    const participantType = (document.getElementById("participantType") as HTMLSelectElement).value as "team" | "individual" | "extreme";
    if (this.selected.length < 1) { notify.show("Додай учасників", "info"); return; }
    try {
      const workspace = await eventApi.createEvent({
        nameEvent,
        systems: Number((document.getElementById("systemType") as HTMLSelectElement).value) as SelectionSystem,
        typeSport: (document.getElementById("sportType") as HTMLSelectElement).value,
        participantType,
        participants: this.selected.map(x => x.id),
        dataStart: (document.getElementById("startDate") as HTMLInputElement).value,
        dataEnd: (document.getElementById("endDate") as HTMLInputElement).value || null,
        loginJudge: (document.getElementById("loginJudge") as HTMLInputElement).value.trim() || null,
        description: (document.getElementById("eventDescription") as HTMLTextAreaElement).value.trim(),
        generateMatches: true,
      });
      notify.show("Захід і сітку створено", "success");
      new EventRuntimePage("app", workspace.nameEvent).render();
    } catch (error) { notify.show(error instanceof Error ? error.message : "Помилка створення заходу", "error"); }
  }

  private escapeHtml(value: unknown) {
    return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
  }}
