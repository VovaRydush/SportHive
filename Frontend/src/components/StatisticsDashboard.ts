import { statisticsApi, type LeaderboardRowDto, type StatisticsDashboardDto, type StatisticsFilter } from "../api/statisticsApi";
import { userProfileApi, type ProfileOrganization } from "../api/userProfileApi";
import "./statisticsDashboard.css";

type Scope = "global" | "organization" | "member";
type Tab = "athletes" | "teams" | "organizations" | "judges";

export class StatisticsDashboard {
  private container: HTMLElement;
  private scope: Scope;
  private organizationLogin?: string;
  private memberOrganizations: ProfileOrganization[] = [];
  private data?: StatisticsDashboardDto;
  private activeTab: Tab = "athletes";
  private filter: StatisticsFilter = {
    season: "all",
    sport: "all",
    country: "all",
    system: "all",
    sortBy: "points",
    direction: "desc",
  };

  constructor(containerId = "app", scope: Scope = "global", organizationLogin?: string) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element ${containerId} not found`);

    this.container = element;
    this.scope = scope;
    this.organizationLogin = organizationLogin;
  }

  async render() {
    this.container.innerHTML = `<section class="stats-page"><div class="stats-card">Завантаження статистики...</div></section>`;

    try {
      if (this.scope === "member") {
        const profile = await userProfileApi.getMe();
        this.memberOrganizations = profile.organizations || [];

        if (!this.organizationLogin && this.memberOrganizations.length) {
          this.organizationLogin = this.memberOrganizations[0].loginOrganization;
        }
      }

      await this.load();
    } catch (error) {
      this.container.innerHTML = `
        <section class="stats-page">
          <div class="stats-card stats-empty">${this.escape(error instanceof Error ? error.message : "Не вдалося завантажити статистику")}</div>
        </section>
      `;
    }
  }

  private async load() {
    if ((this.scope === "organization" || this.scope === "member") && this.organizationLogin) {
      this.data = await statisticsApi.getOrganization(this.organizationLogin, this.filter);
    } else {
      this.data = await statisticsApi.getGlobal(this.filter);
    }

    this.paint();
  }

  private paint() {
    const d = this.data;
    if (!d) return;

    const rows = this.rowsForTab();

    this.container.innerHTML = `
      <section class="stats-page">
        <header class="stats-hero">
          <span class="stats-kicker">SportHive Analytics</span>
          <h1>${this.title()}</h1>
          <p>Лідери, сезони, країни, спорт, системи відбору, організації, команди, судді та спортсмени.</p>
        </header>

        ${this.renderOrganizationPicker()}

        <section class="stats-filters">
          <label>
            Сезон
            <select id="stats-season">
              <option value="all">Усі сезони</option>
              ${d.seasons.map(x => `<option value="${this.attr(x)}" ${this.filter.season === x ? "selected" : ""}>${this.escape(x)}</option>`).join("")}
            </select>
          </label>

          <label>
            Країна
            <select id="stats-country">
              <option value="all">Усі країни</option>
              ${(d.countries || []).map(x => `<option value="${this.attr(x)}" ${this.filter.country === x ? "selected" : ""}>${this.escape(x)}</option>`).join("")}
            </select>
          </label>

          <label>
            Спорт
            <select id="stats-sport">
              <option value="all">Усі види</option>
              ${d.sports.map(x => `<option value="${this.attr(x)}" ${this.filter.sport === x ? "selected" : ""}>${this.escape(x)}</option>`).join("")}
            </select>
          </label>

          <label>
            Сортування
            <select id="stats-sort">
              <option value="points" ${this.filter.sortBy === "points" ? "selected" : ""}>Очки</option>
              <option value="wins" ${this.filter.sortBy === "wins" ? "selected" : ""}>Перемоги</option>
              <option value="winrate" ${this.filter.sortBy === "winrate" ? "selected" : ""}>Win Rate</option>
              <option value="played" ${this.filter.sortBy === "played" ? "selected" : ""}>Матчі</option>
              <option value="scoreDiff" ${this.filter.sortBy === "scoreDiff" ? "selected" : ""}>Різниця</option>
              <option value="country" ${this.filter.sortBy === "country" ? "selected" : ""}>Країна</option>
              <option value="name" ${this.filter.sortBy === "name" ? "selected" : ""}>Назва</option>
            </select>
          </label>
        </section>

        <section class="stats-summary">
          <div><b>${d.summary.events}</b><span>Заходів</span></div>
          <div><b>${d.summary.matches}</b><span>Матчів</span></div>
          <div><b>${d.summary.finishedMatches}</b><span>Завершено</span></div>
          <div><b>${d.summary.countries || 0}</b><span>Країн</span></div>
        </section>

        <nav class="stats-tabs">
          <button class="${this.activeTab === "athletes" ? "active" : ""}" data-tab="athletes">Спортсмени</button>
          <button class="${this.activeTab === "teams" ? "active" : ""}" data-tab="teams">Команди</button>
          <button class="${this.activeTab === "organizations" ? "active" : ""}" data-tab="organizations">Організації</button>
          <button class="${this.activeTab === "judges" ? "active" : ""}" data-tab="judges">Судді</button>
        </nav>

        <section class="stats-card">
          ${this.renderTable(rows)}
        </section>
      </section>
    `;

    this.bind();
  }

  private title() {
    if (this.scope === "member") return "Статистика моїх організацій";
    if (this.scope === "organization") return "Статистика організації";
    return "Глобальна статистика";
  }

  private renderOrganizationPicker() {
    if (this.scope !== "member") return "";

    if (!this.memberOrganizations.length) {
      return `<div class="stats-card stats-empty">У тебе поки немає організацій для перегляду статистики.</div>`;
    }

    return `
      <section class="stats-org-picker">
        <label>
          Організація
          <select id="stats-member-org">
            ${this.memberOrganizations.map(org => `
              <option value="${this.attr(org.loginOrganization)}" ${this.organizationLogin === org.loginOrganization ? "selected" : ""}>
                ${this.escape(org.nameOrganization || org.loginOrganization)}${org.country ? ` · ${this.escape(org.country)}` : ""}
              </option>
            `).join("")}
          </select>
        </label>
      </section>
    `;
  }

  private bind() {
    document.getElementById("stats-member-org")?.addEventListener("change", e => {
      this.organizationLogin = (e.target as HTMLSelectElement).value;
      this.load();
    });

    document.getElementById("stats-season")?.addEventListener("change", e => {
      this.filter.season = (e.target as HTMLSelectElement).value;
      this.load();
    });

    document.getElementById("stats-country")?.addEventListener("change", e => {
      this.filter.country = (e.target as HTMLSelectElement).value;
      this.load();
    });

    document.getElementById("stats-sport")?.addEventListener("change", e => {
      this.filter.sport = (e.target as HTMLSelectElement).value;
      this.load();
    });

    document.getElementById("stats-sort")?.addEventListener("change", e => {
      this.filter.sortBy = (e.target as HTMLSelectElement).value;
      this.load();
    });

    this.container.querySelectorAll<HTMLButtonElement>("[data-tab]").forEach(button => {
      button.addEventListener("click", () => {
        this.activeTab = button.dataset.tab as Tab;
        this.paint();
      });
    });
  }

  private rowsForTab() {
    if (!this.data) return [];
    if (this.activeTab === "teams") return this.data.teamLeaders;
    if (this.activeTab === "organizations") return this.data.organizationLeaders;
    if (this.activeTab === "judges") return this.data.judgeLeaders;
    return this.data.athleteLeaders;
  }

  private renderTable(rows: LeaderboardRowDto[]) {
    if (!rows.length) return `<div class="stats-empty">Даних для таблиці поки немає</div>`;

    return `
      <div class="stats-table-wrap">
        <table class="stats-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Учасник</th>
              <th>Тип</th>
              <th>Країна</th>
              <th>Спорт</th>
              <th>PL</th>
              <th>W</th>
              <th>D</th>
              <th>L</th>
              <th>+/-</th>
              <th>Win%</th>
              <th>Pts</th>
            </tr>
          </thead>
          <tbody>
            ${rows.map((row, index) => `
              <tr>
                <td>${index + 1}</td>
                <td>
                  <b>${this.escape(row.name)}</b>
                  ${row.organizationName ? `<small>${this.escape(row.organizationName)}</small>` : ""}
                </td>
                <td>${this.escape(row.type)}</td>
                <td>${this.escape(row.country || "-")}</td>
                <td>${this.escape(row.sport || "-")}</td>
                <td>${row.played}</td>
                <td>${row.wins}</td>
                <td>${row.draws}</td>
                <td>${row.losses}</td>
                <td>${row.scoreDiff}</td>
                <td>${row.winRate}%</td>
                <td><b>${row.points}</b></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>
    `;
  }

  private escape(value: unknown) {
    return String(value ?? "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }

  private attr(value: unknown) {
    return this.escape(value).replace(/`/g, "&#096;");
  }
}
