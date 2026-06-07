import { authApi } from "../api/authApi";
import { eventApi } from "../api/eventApi";
import { AUTH_API_URL, COMMAND_API_URL } from "../api/http";
import type { SelectionSystem } from "../api/eventTypes";
import { NotificationKarina } from "./Notification";
import { EventRuntimePage } from "./EventRuntimePage";
import { authHeaders, getAccessToken } from "../api/authToken";
import "./eventPage.css";

type ParticipantType = "team" | "individual" | "extreme";

type Participant = {
  id: string;
  name: string;
  type: ParticipantType;
  subtitle?: string;
};

type JudgeOption = {
  login: string;
  name: string;
  subtitle?: string;
  photo?: string;
};

export class CreateEventPage {
  private container: HTMLElement;
  private selected: Participant[] = [];
  private selectedJudge: JudgeOption | null = null;
  private judgeSearchTimer?: number;
  private participantSearchTimer?: number;

  constructor(containerId: string) {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

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
      <section class="event-create-page">
        <h1>Створення заходу</h1>

        <form id="createEventForm" class="event-form">
          <section class="event-section">
            <h2>Основна інформація</h2>

            <label for="eventName">Назва заходу</label>
            <input id="eventName" type="text" placeholder="Наприклад: Football Spring Cup" required />

            <label for="sportType">Вид спорту</label>
            <select id="sportType">
              ${sports.map(s => `<option value="${this.escapeAttr(s.value)}">${this.escapeHtml(s.label)}</option>`).join("")}
            </select>

            <label for="systemType">Система відбору</label>
            <select id="systemType">
              ${systems.map(s => `<option value="${s.value}">${this.escapeHtml(s.name)}</option>`).join("")}
            </select>

            <label for="participantType">Тип учасників</label>
            <select id="participantType">
              <option value="team">Команди</option>
              <option value="individual">Спортсмени</option>
              <option value="extreme">Нормативи / заїзд / рейтинг</option>
            </select>

            <label for="eventDescription">Опис</label>
            <textarea id="eventDescription" placeholder="Короткий опис заходу"></textarea>
          </section>

          <section class="event-section">
            <h2>Дата</h2>

            <label for="startDate">Початок</label>
            <input id="startDate" type="date" required />

            <label for="endDate">Кінець</label>
            <input id="endDate" type="date" />
          </section>

          <section class="event-section">
            <h2>Суддя</h2>
            <p class="event-hint">
              Суддю можна вибрати тільки з тих, хто вже привʼязаний до вашої організації.
            </p>

            <div class="adaptive-search">
              <input id="judgeSearch" type="text" placeholder="Почни вводити ПІБ або login судді..." autocomplete="off" />
              <div id="judgeResults" class="adaptive-results" hidden></div>
            </div>

            <div id="selectedJudge" class="selected-items muted-selection">
              Суддю ще не вибрано
            </div>
          </section>

          <section class="event-section">
            <h2>Учасники</h2>
            <p id="participantHint" class="event-hint">
              Для команд пошук працює по командах організації. Для спортсменів — по спортсменах. Для нормативів можна додати введений текст.
            </p>

            <div class="adaptive-search">
              <input id="participantSearch" type="text" placeholder="Пошук учасника..." autocomplete="off" />
              <button id="addExtremeBtn" type="button" class="secondary-btn" hidden>Додати</button>
              <div id="participantResults" class="adaptive-results" hidden></div>
            </div>

            <h3>Обрані учасники</h3>
            <div id="selectedParticipants" class="selected-items"></div>
          </section>

          <button class="primary-btn" type="submit">Створити захід і сітку</button>
        </form>
      </section>
    `;

    this.bindEvents();
    this.renderSelected();
    this.renderSelectedJudge();
    this.updateParticipantMode();
  }

  private bindEvents() {
    document.getElementById("createEventForm")?.addEventListener("submit", (e) => this.submit(e));

    const participantType = document.getElementById("participantType") as HTMLSelectElement | null;
    const participantSearch = document.getElementById("participantSearch") as HTMLInputElement | null;
    const judgeSearch = document.getElementById("judgeSearch") as HTMLInputElement | null;

    participantType?.addEventListener("change", () => {
      this.updateParticipantMode();
      this.clearParticipantResults();
      if (participantSearch) participantSearch.value = "";
    });

    participantSearch?.addEventListener("input", () => {
      window.clearTimeout(this.participantSearchTimer);
      this.participantSearchTimer = window.setTimeout(() => this.searchParticipant(), 250);
    });

    participantSearch?.addEventListener("keydown", (event) => {
      if (event.key === "Enter") {
        event.preventDefault();

        const type = this.getParticipantType();
        if (type === "extreme") {
          this.addExtremeParticipant();
        }
      }
    });

    document.getElementById("addExtremeBtn")?.addEventListener("click", () => this.addExtremeParticipant());

    judgeSearch?.addEventListener("input", () => {
      window.clearTimeout(this.judgeSearchTimer);
      this.judgeSearchTimer = window.setTimeout(() => this.searchJudge(), 250);
    });
  }

  private updateParticipantMode() {
    const type = this.getParticipantType();
    const hint = document.getElementById("participantHint");
    const input = document.getElementById("participantSearch") as HTMLInputElement | null;
    const addExtremeBtn = document.getElementById("addExtremeBtn") as HTMLButtonElement | null;

    if (type === "team") {
      if (hint) hint.textContent = "Пошук показує тільки команди, привʼязані до вашої організації.";
      if (input) input.placeholder = "Почни вводити назву команди...";
      if (addExtremeBtn) addExtremeBtn.hidden = true;
    }

    if (type === "individual") {
      if (hint) hint.textContent = "Пошук спортсменів працює по ПІБ або login.";
      if (input) input.placeholder = "Почни вводити ПІБ або login спортсмена...";
      if (addExtremeBtn) addExtremeBtn.hidden = true;
    }

    if (type === "extreme") {
      if (hint) hint.textContent = "Для нормативів / рейтингу / заїздів можна додати учасника вручну.";
      if (input) input.placeholder = "Назва учасника / заїзду / нормативу...";
      if (addExtremeBtn) addExtremeBtn.hidden = false;
    }
  }

  private async searchJudge() {
    const input = document.getElementById("judgeSearch") as HTMLInputElement | null;
    const container = document.getElementById("judgeResults");

    if (!input || !container) return;

    const query = input.value.trim();

    if (query.length < 2) {
      container.hidden = true;
      container.innerHTML = "";
      return;
    }

    container.hidden = false;
    container.innerHTML = `<div class="adaptive-empty">Пошук суддів...</div>`;

    const judges = await this.searchOrganizationJudges(query);

    if (!judges.length) {
      container.innerHTML = `
        <div class="adaptive-empty">
          Суддів не знайдено. Перевір, чи суддя привʼязаний до вашої організації.
        </div>
      `;
      return;
    }

    container.innerHTML = judges.map(judge => `
      <button type="button" class="adaptive-option" data-login="${this.escapeAttr(judge.login)}">
        <strong>${this.escapeHtml(judge.name)}</strong>
        <small>${this.escapeHtml(judge.subtitle || judge.login)}</small>
      </button>
    `).join("");

    container.querySelectorAll<HTMLButtonElement>("button[data-login]").forEach(btn => {
      btn.addEventListener("click", () => {
        const judge = judges.find(x => x.login === btn.dataset.login);
        if (!judge) return;

        this.selectedJudge = judge;
        input.value = "";
        container.hidden = true;
        container.innerHTML = "";
        this.renderSelectedJudge();
      });
    });
  }

  private async searchParticipant() {
    const type = this.getParticipantType();
    const input = document.getElementById("participantSearch") as HTMLInputElement | null;
    const container = document.getElementById("participantResults");

    if (!input || !container) return;

    const query = input.value.trim();

    if (type === "extreme") {
      container.hidden = true;
      container.innerHTML = "";
      return;
    }

    if (query.length < 2) {
      container.hidden = true;
      container.innerHTML = "";
      return;
    }

    container.hidden = false;
    container.innerHTML = `<div class="adaptive-empty">Пошук...</div>`;

    const list = type === "team"
      ? await this.searchOrganizationTeams(query)
      : await this.searchAthletes(query);

    if (!list.length) {
      container.innerHTML = `
        <div class="adaptive-empty">
          Нічого не знайдено.
          ${type === "team" ? "Команда має бути привʼязана до організації." : ""}
        </div>
      `;
      return;
    }

    container.innerHTML = list.map(item => `
      <button type="button" class="adaptive-option" data-id="${this.escapeAttr(item.id)}">
        <strong>${this.escapeHtml(item.name)}</strong>
        ${item.subtitle ? `<small>${this.escapeHtml(item.subtitle)}</small>` : ""}
      </button>
    `).join("");

    container.querySelectorAll<HTMLButtonElement>("button[data-id]").forEach(btn => {
      btn.addEventListener("click", () => {
        const item = list.find(x => x.id === btn.dataset.id);
        if (!item) return;

        this.addParticipant(item.id, item.name, type, item.subtitle);
        input.value = "";
        container.hidden = true;
        container.innerHTML = "";
      });
    });
  }

  private addExtremeParticipant() {
    const input = document.getElementById("participantSearch") as HTMLInputElement | null;
    if (!input) return;

    const name = input.value.trim();
    if (!name) return;

    this.addParticipant(name, name, "extreme", "Ручний учасник для нормативу / рейтингу");
    input.value = "";
  }

  private addParticipant(id: string, name: string, type: ParticipantType, subtitle?: string) {
    if (this.selected.some(x => x.id === id && x.type === type)) return;

    this.selected.push({ id, name, type, subtitle });
    this.renderSelected();
  }

  private renderSelectedJudge() {
    const container = document.getElementById("selectedJudge");
    if (!container) return;

    if (!this.selectedJudge) {
      container.classList.add("muted-selection");
      container.innerHTML = "Суддю ще не вибрано";
      return;
    }

    container.classList.remove("muted-selection");
    container.innerHTML = `
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

  private renderSelected() {
    const container = document.getElementById("selectedParticipants");
    if (!container) return;

    container.innerHTML = this.selected.length
      ? this.selected.map((p, i) => `
        <span class="selected-chip">
          <b>${this.escapeHtml(p.name)}</b>
          <small>${this.escapeHtml(this.participantLabel(p.type))}${p.subtitle ? ` · ${this.escapeHtml(p.subtitle)}` : ""}</small>
          <button type="button" data-i="${i}">×</button>
        </span>
      `).join("")
      : `<p class="muted-selection">Учасників ще не вибрано</p>`;

    container.querySelectorAll<HTMLButtonElement>("button[data-i]").forEach(btn => {
      btn.addEventListener("click", () => {
        this.selected.splice(Number(btn.dataset.i), 1);
        this.renderSelected();
      });
    });
  }

  private async submit(e: Event) {
  e.preventDefault();

  const notify = new NotificationKarina();
  const nameEvent = (document.getElementById("eventName") as HTMLInputElement).value.trim();
  const participantType = this.getParticipantType();

  if (!nameEvent) {
    notify.show("Введи назву заходу", "info");
    return;
  }

  if (this.selected.length < 1) {
    notify.show("Додай учасників", "info");
    return;
  }

  const token = getAccessToken();

  if (!token) {
    notify.show("Токен авторизації не знайдено. Вийди з акаунта і зайди знову як організація.", "error");
    return;
  }

  try {
    const body = {
      nameEvent,
      systems: Number((document.getElementById("systemType") as HTMLSelectElement).value),
      typeSport: (document.getElementById("sportType") as HTMLSelectElement).value,
      participantType,
      participants: this.selected.map(x => x.id),
      dataStart: (document.getElementById("startDate") as HTMLInputElement).value,
      dataEnd: (document.getElementById("endDate") as HTMLInputElement).value || null,
      loginJudge: this.selectedJudge?.login || null,
      description: (document.getElementById("eventDescription") as HTMLTextAreaElement).value.trim(),
      generateMatches: true,
    };

    const response = await fetch("http://localhost:5042/stage3/events", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...authHeaders(),
      },
      body: JSON.stringify(body),
    });

    if (response.status === 401) {
      throw new Error("401 Unauthorized: EventService не прийняв токен. Перелогінься як Organization і перевір, що токен зберігся в localStorage.");
    }

    if (response.status === 403) {
      throw new Error("403 Forbidden: токен є, але роль не має доступу. Перевір роль у JWT — має бути Organization або Trainer.");
    }

    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || `HTTP error ${response.status}`);
    }

    const workspace = await response.json();

    notify.show("Захід і сітку створено", "success");
    new EventRuntimePage("app", workspace.nameEvent).render();
  } catch (error) {
    notify.show(error instanceof Error ? error.message : "Помилка створення заходу", "error");
  }
}


  private async searchOrganizationJudges(query: string): Promise<JudgeOption[]> {
    const org = this.getOrganizationLogin();
    if (!org) return [];

    const data = await this.safeGet<any[]>(
      `${AUTH_API_URL}/organization-linked/judges?loginOrganization=${encodeURIComponent(org)}&query=${encodeURIComponent(query)}`
    );

    return (data || []).map(x => ({
      login: String(x.login || x.Login || ""),
      name: String(x.fullName || x.FullName || x.name || x.Name || x.login || x.Login || ""),
      subtitle: "Суддя організації",
      photo: x.profilePhoto || x.ProfilePhoto,
    })).filter(x => x.login);
  }

  private async searchOrganizationTeams(query: string): Promise<Participant[]> {
    const org = this.getOrganizationLogin();
    if (!org) return [];

    const organizationTeams = await this.safeGet<any[]>(
      `${AUTH_API_URL}/organization-linked/teams?loginOrganization=${encodeURIComponent(org)}&query=${encodeURIComponent(query)}`
    );

    if (organizationTeams?.length) {
      return organizationTeams.map(x => {
        const name = String(x.teamName || x.TeamName || x.name || x.Name || "");
        return {
          id: name,
          name,
          type: "team" as const,
          subtitle: String(x.typeSport || x.TypeSport || "Команда організації"),
        };
      }).filter(x => x.id);
    }

    const commandTeams = await this.safeGet<any[]>(
      `${COMMAND_API_URL}/teams/search?query=${encodeURIComponent(query)}`
    );

    return (commandTeams || []).map(x => {
      const name = String(x.teamName || x.TeamName || x.name || x.Name || "");
      return {
        id: name,
        name,
        type: "team" as const,
        subtitle: String(x.typeSport || x.TypeSport || "Команда"),
      };
    }).filter(x => x.id);
  }

  private async searchAthletes(query: string): Promise<Participant[]> {
    const org = this.getOrganizationLogin();

    if (org) {
      const linked = await this.safeGet<any[]>(
        `${AUTH_API_URL}/organization-linked/athletes?loginOrganization=${encodeURIComponent(org)}&query=${encodeURIComponent(query)}`
      );

      if (linked?.length) {
        return linked.map(x => {
          const login = String(x.login || x.Login || "");
          const name = String(x.fullName || x.FullName || x.name || x.Name || login);
          return {
            id: login,
            name,
            type: "individual" as const,
            subtitle: String(x.typeSport || x.TypeSport || "Спортсмен організації"),
          };
        }).filter(x => x.id);
      }
    }

    try {
      const response = await authApi.searchAthlete(query);
      const list = Array.isArray(response) ? response : [response];

      return list.map((x: any) => {
        const login = String(x.login || x.Login || "");
        const name = String(x.fullName || x.FullName || x.name || x.Name || login);
        return {
          id: login,
          name,
          type: "individual" as const,
          subtitle: String(x.typeSport || x.TypeSport || "Спортсмен"),
        };
      }).filter(x => x.id);
    } catch {
      return [];
    }
  }

  private async safeGet<T>(url: string): Promise<T | null> {
    try {
      const token = localStorage.getItem("accessToken");

      const response = await fetch(url, {
        method: "GET",
        headers: {
          "Accept": "application/json",
          ...(token ? { "Authorization": `Bearer ${token.replace(/^"(.+)"$/, "$1")}` } : {}),
        },
      });

      if (!response.ok) return null;

      return await response.json() as T;
    } catch {
      return null;
    }
  }

  private clearParticipantResults() {
    const container = document.getElementById("participantResults");
    if (!container) return;

    container.hidden = true;
    container.innerHTML = "";
  }

  private getParticipantType(): ParticipantType {
    return (document.getElementById("participantType") as HTMLSelectElement).value as ParticipantType;
  }

  private getOrganizationLogin() {
    return (
      localStorage.getItem("organizationLogin") ||
      localStorage.getItem("loginOrganization") ||
      localStorage.getItem("login") ||
      ""
    );
  }

  private participantLabel(type: ParticipantType) {
    if (type === "team") return "Команда";
    if (type === "individual") return "Спортсмен";
    return "Норматив / рейтинг";
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
