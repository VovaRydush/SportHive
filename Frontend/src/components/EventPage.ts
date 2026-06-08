import { authApi } from "../api/authApi";
import { eventInvitationApi, type EventInviteParticipant, type EventInviteParticipantType } from "../api/eventInvitationApi";
import { AUTH_API_URL } from "../api/http";
import { NotificationKarina } from "./Notification";
import "./eventPage.css";

type JudgeOption = {
  login: string;
  name: string;
  subtitle?: string;
};

export class CreateEventPage {
  private container: HTMLElement;
  private selectedParticipants: EventInviteParticipant[] = [];
  private selectedJudge: JudgeOption | null = null;
  private participantSearchTimer?: number;
  private judgeSearchTimer?: number;

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `
      <section class="event-create-page">
        <h1>Створення заходу</h1>

        <form id="createEventForm" class="event-form" novalidate>
          <section class="event-section">
            <h2>Основна інформація</h2>

            <label>Назва заходу</label>
            <input id="eventName" type="text" placeholder="Наприклад: Football Spring Cup" required />

            <label>Вид спорту</label>
            <select id="sportType">
              <option value="Football">Футбол</option>
              <option value="Basketball">Баскетбол</option>
              <option value="Volleyball">Волейбол</option>
              <option value="Tennis">Теніс</option>
              <option value="TableTennis">Настільний теніс</option>
              <option value="Hockey">Хокей</option>
              <option value="Boxing">Бокс</option>
              <option value="Wrestling">Боротьба</option>
              <option value="Chess">Шахи</option>
              <option value="Checkers">Шашки</option>
              <option value="Badminton">Бадмінтон</option>
              <option value="Baseball">Бейсбол</option>
            </select>

            <label>Система відбору</label>
            <select id="systemType">
              <option value="1">RoundRobin / кругова система</option>
              <option value="2">PlayOff</option>
              <option value="4">SwissSystem / швейцарська</option>
              <option value="5">QualificationByStandards / нормативи</option>
              <option value="6">GroupStage / групи</option>
              <option value="8">OlympicSystem</option>
              <option value="9">KnockoutSystem</option>
              <option value="10">DoubleElimination</option>
            </select>

            <label>Тип учасників</label>
            <select id="participantType">
              <option value="team">Команди</option>
              <option value="individual">Спортсмени</option>
            </select>

            <label>Опис</label>
            <textarea id="eventDescription" placeholder="Короткий опис заходу"></textarea>
          </section>

          <section class="event-section">
            <h2>Дата</h2>
            <label>Початок</label>
            <input id="startDate" type="date" required />
            <label>Кінець</label>
            <input id="endDate" type="date" />
          </section>

          <section class="event-section">
            <h2>Суддя</h2>
            <p class="event-hint">Суддю можна вибрати тільки з тих, хто вже привʼязаний до вашої організації.</p>

            <div class="adaptive-search">
              <input id="judgeSearch" type="text" placeholder="Почни вводити ПІБ або login судді..." autocomplete="off" />
              <div id="judgeResults" class="adaptive-results" hidden></div>
            </div>

            <div id="selectedJudge" class="selected-items muted-selection">Суддю ще не вибрано</div>
          </section>

          <section class="event-section">
            <h2>Учасники</h2>
            <p id="participantHint" class="event-hint">
              Пошук покаже всі команди відповідного виду спорту. Командам прийде email-запрошення організації-власнику.
            </p>

            <div class="adaptive-search">
              <input id="participantSearch" type="text" placeholder="Пошук учасників..." autocomplete="off" />
              <div id="participantResults" class="adaptive-results" hidden></div>
            </div>

            <h3>Обрані учасники для запрошення</h3>
            <div id="selectedParticipants" class="selected-items"></div>

            <div class="event-quorum-info">
              <b>Правило підтвердження:</b>
              Захід створиться автоматично, коли 70% або більше запрошених приймуть участь.
              На відповідь є 24 години. Якщо менше 70% приймуть запрошення — всім прийде повідомлення, що захід не відбудеться.
            </div>
          </section>

          <button class="primary-btn" id="createEventBtn" type="submit">Надіслати запрошення на участь</button>
        </form>
      </section>
    `;

    this.bindEvents();
    this.renderSelectedParticipants();
    this.renderSelectedJudge();
    this.updateParticipantMode();
  }

  private bindEvents() {
    document.getElementById("createEventForm")?.addEventListener("submit", e => this.submit(e));

    const participantType = document.getElementById("participantType") as HTMLSelectElement | null;
    const sportType = document.getElementById("sportType") as HTMLSelectElement | null;
    const participantSearch = document.getElementById("participantSearch") as HTMLInputElement | null;
    const judgeSearch = document.getElementById("judgeSearch") as HTMLInputElement | null;

    participantType?.addEventListener("change", () => {
      this.selectedParticipants = [];
      this.renderSelectedParticipants();
      this.updateParticipantMode();
      this.clearParticipantResults();
      if (participantSearch) participantSearch.value = "";
    });

    sportType?.addEventListener("change", () => {
      this.selectedParticipants = [];
      this.renderSelectedParticipants();
      this.clearParticipantResults();
      if (participantSearch) participantSearch.value = "";
    });

    participantSearch?.addEventListener("input", () => {
      window.clearTimeout(this.participantSearchTimer);
      this.participantSearchTimer = window.setTimeout(() => this.searchParticipant(), 250);
    });

    judgeSearch?.addEventListener("input", () => {
      window.clearTimeout(this.judgeSearchTimer);
      this.judgeSearchTimer = window.setTimeout(() => this.searchJudge(), 250);
    });
  }

  private updateParticipantMode() {
    const hint = document.getElementById("participantHint");
    const input = document.getElementById("participantSearch") as HTMLInputElement | null;
    const type = this.getParticipantType();

    if (type === "team") {
      if (hint) {
        hint.textContent = "Пошук показує всі команди відповідного виду спорту. Запрошення прийде на email організації, якій належить команда.";
      }
      if (input) input.placeholder = "Знайти команду по назві...";
    } else {
      if (hint) {
        hint.textContent = "Пошук показує спортсменів відповідного виду спорту. Спортсмен не обовʼязково має бути привʼязаний до організації.";
      }
      if (input) input.placeholder = "Знайти спортсмена по ПІБ або login...";
    }
  }

  private async searchParticipant() {
    const input = document.getElementById("participantSearch") as HTMLInputElement | null;
    const root = document.getElementById("participantResults");

    if (!input || !root) return;

    const query = input.value.trim();
    if (query.length < 1) {
      root.hidden = true;
      root.innerHTML = "";
      return;
    }

    const type = this.getParticipantType();
    const sport = (document.getElementById("sportType") as HTMLSelectElement).value;

    root.hidden = false;
    root.innerHTML = `<div class="adaptive-empty">Пошук...</div>`;

    try {
      const list = await eventInvitationApi.searchParticipants(type, sport, query);
      const selectedIds = new Set(this.selectedParticipants.map(x => x.id));
      const filtered = list.filter(x => !selectedIds.has(x.id));

      if (!filtered.length) {
        root.innerHTML = `<div class="adaptive-empty">Нічого не знайдено по спорту ${this.escapeHtml(sport)}</div>`;
        return;
      }

      root.innerHTML = filtered.map(item => `
        <button type="button" class="adaptive-option" data-id="${this.escapeAttr(item.id)}">
          <strong>${this.escapeHtml(item.name)}</strong>
          <small>${this.escapeHtml(item.subtitle || "")}</small>
        </button>
      `).join("");

      root.querySelectorAll<HTMLButtonElement>("button[data-id]").forEach(button => {
        button.addEventListener("click", () => {
          const item = filtered.find(x => x.id === button.dataset.id);
          if (!item) return;

          this.selectedParticipants.push(item);
          input.value = "";
          root.hidden = true;
          root.innerHTML = "";
          this.renderSelectedParticipants();
        });
      });
    } catch (error) {
      root.innerHTML = `<div class="adaptive-empty error">${this.escapeHtml(error instanceof Error ? error.message : "Помилка пошуку")}</div>`;
    }
  }

  private async searchJudge() {
    const input = document.getElementById("judgeSearch") as HTMLInputElement | null;
    const root = document.getElementById("judgeResults");

    if (!input || !root) return;

    const query = input.value.trim();
    if (query.length < 2) {
      root.hidden = true;
      root.innerHTML = "";
      return;
    }

    root.hidden = false;
    root.innerHTML = `<div class="adaptive-empty">Пошук суддів...</div>`;

    const judges = await this.searchOrganizationJudges(query);

    if (!judges.length) {
      root.innerHTML = `<div class="adaptive-empty">Суддів не знайдено</div>`;
      return;
    }

    root.innerHTML = judges.map(judge => `
      <button type="button" class="adaptive-option" data-login="${this.escapeAttr(judge.login)}">
        <strong>${this.escapeHtml(judge.name)}</strong>
        <small>${this.escapeHtml(judge.subtitle || judge.login)}</small>
      </button>
    `).join("");

    root.querySelectorAll<HTMLButtonElement>("button[data-login]").forEach(button => {
      button.addEventListener("click", () => {
        const judge = judges.find(x => x.login === button.dataset.login);
        if (!judge) return;

        this.selectedJudge = judge;
        input.value = "";
        root.hidden = true;
        root.innerHTML = "";
        this.renderSelectedJudge();
      });
    });
  }

  private async searchOrganizationJudges(query: string): Promise<JudgeOption[]> {
    const org = this.getOrganizationLogin();
    if (!org) return [];

    const token = localStorage.getItem("accessToken") || localStorage.getItem("token") || "";
    const response = await fetch(
      `${AUTH_API_URL}/organization-linked/judges?loginOrganization=${encodeURIComponent(org)}&query=${encodeURIComponent(query)}`,
      {
        headers: {
          Accept: "application/json",
          ...(token ? { Authorization: `Bearer ${token.replace(/^"(.+)"$/, "$1").replace(/^Bearer\s+/i, "")}` } : {}),
        },
      }
    );

    if (!response.ok) return [];

    const data = await response.json();

    return (Array.isArray(data) ? data : []).map((x: any) => ({
      login: String(x.login || x.Login || ""),
      name: String(x.fullName || x.FullName || x.name || x.Name || x.login || x.Login || ""),
      subtitle: "Суддя організації",
    })).filter(x => x.login);
  }

  private renderSelectedJudge() {
    const root = document.getElementById("selectedJudge");
    if (!root) return;

    if (!this.selectedJudge) {
      root.classList.add("muted-selection");
      root.innerHTML = "Суддю ще не вибрано";
      return;
    }

    root.classList.remove("muted-selection");
    root.innerHTML = `
      <span class="selected-chip">
        <b>${this.escapeHtml(this.selectedJudge.name)}</b>
        <small>${this.escapeHtml(this.selectedJudge.login)}</small>
        <button type="button" id="removeJudge">×</button>
      </span>
    `;

    document.getElementById("removeJudge")?.addEventListener("click", () => {
      this.selectedJudge = null;
      this.renderSelectedJudge();
    });
  }

  private renderSelectedParticipants() {
    const root = document.getElementById("selectedParticipants");
    if (!root) return;

    if (!this.selectedParticipants.length) {
      root.innerHTML = `<p class="muted-selection">Учасників ще не вибрано</p>`;
      return;
    }

    root.innerHTML = this.selectedParticipants.map((p, index) => `
      <span class="selected-chip">
        <b>${this.escapeHtml(p.name)}</b>
        <small>${this.escapeHtml(p.subtitle || p.id)}</small>
        <button type="button" data-remove="${index}">×</button>
      </span>
    `).join("");

    root.querySelectorAll<HTMLButtonElement>("[data-remove]").forEach(btn => {
      btn.addEventListener("click", () => {
        this.selectedParticipants.splice(Number(btn.dataset.remove), 1);
        this.renderSelectedParticipants();
      });
    });
  }

  private async submit(event: Event) {
    event.preventDefault();

    const notify = new NotificationKarina();
    const button = document.getElementById("createEventBtn") as HTMLButtonElement | null;

    const nameEvent = (document.getElementById("eventName") as HTMLInputElement).value.trim();
    const typeSport = (document.getElementById("sportType") as HTMLSelectElement).value;
    const systems = Number((document.getElementById("systemType") as HTMLSelectElement).value);
    const participantType = this.getParticipantType();
    const dataStart = (document.getElementById("startDate") as HTMLInputElement).value;
    const dataEnd = (document.getElementById("endDate") as HTMLInputElement).value || null;
    const description = (document.getElementById("eventDescription") as HTMLTextAreaElement).value.trim();

    if (!nameEvent) {
      notify.show("Введи назву заходу", "error");
      return;
    }

    if (!dataStart) {
      notify.show("Вкажи дату початку", "error");
      return;
    }

    if (this.selectedParticipants.length < 2) {
      notify.show("Обери мінімум 2 учасники", "error");
      return;
    }

    try {
      if (button) button.disabled = true;

      const result = await eventInvitationApi.createInvitation({
        nameEvent,
        typeSport,
        systems,
        participantType,
        participants: this.selectedParticipants.map(x => x.id),
        dataStart,
        dataEnd,
        description,
        loginJudge: this.selectedJudge?.login || null,
        createdByOrganization: this.getOrganizationLogin(),
      });

      notify.show(
        `Запрошення надіслано. Потрібно ${result.requiredCount}/${result.totalInvited} підтверджень до ${new Date(result.deadlineAt).toLocaleString("uk-UA")}`,
        "success"
      );
    } catch (error) {
      notify.show(error instanceof Error ? error.message : "Не вдалося створити запрошення", "error");
    } finally {
      if (button) button.disabled = false;
    }
  }

  private getParticipantType(): EventInviteParticipantType {
    return (document.getElementById("participantType") as HTMLSelectElement).value as EventInviteParticipantType;
  }

  private clearParticipantResults() {
    const root = document.getElementById("participantResults");
    if (!root) return;
    root.hidden = true;
    root.innerHTML = "";
  }

  private getOrganizationLogin() {
    return (
      localStorage.getItem("organizationLogin") ||
      localStorage.getItem("loginOrganization") ||
      localStorage.getItem("login") ||
      localStorage.getItem("userLogin") ||
      ""
    ).replace(/^"(.+)"$/, "$1");
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
