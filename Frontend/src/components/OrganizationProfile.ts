import { organizationApi } from "../api/organizationApi";
import { eventCatalogApi } from "../api/eventCatalogApi";
import { NotificationKarina } from "./Notification";
import { CreateTeamModal } from "./CreateTeam";
import { TeamPageLook } from "./TeamPage";
import { UserProfilePage } from "./UserProfile";
import { calculateEventStats, formatFinishedCounter, normalizeEventForUi } from "../api/tournamentResultUtils";

type AnyObj = Record<string, any>;

export class OrganizationProfile {
  private container: HTMLElement;
  private data?: AnyObj;
  private catalogEvents: AnyObj[] = [];
  private selectedUser: AnyObj | null = null;

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    this.container.innerHTML = `<section class="organization-profile"><div class="profile-loading">Завантаження профілю організації...</div></section>`;
    try {
      const [profile, events] = await Promise.all([
        organizationApi.getProfile(),
        eventCatalogApi.getEvents({}).catch(() => []),
      ]);
      this.data = profile as AnyObj;
      this.catalogEvents = (events || []).map(event => normalizeEventForUi(event as AnyObj));
      this.renderPage();
    } catch (error) {
      new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося завантажити організацію", "error");
    }
  }

  private renderPage() {
    const d = this.data || {};
    const organization = d.organization || d.profile || d;
    const teams = this.list(d.teams || d.organizationTeams);
    const judges = this.list(d.judges || d.organizationJudges);
    const trainers = this.list(d.trainers || d.organizationTrainers);
    const athletes = this.list(d.athletes || d.organizationAthletes);
    const invitations = this.list(d.invitations || d.organizationInvitations);
    const events = this.resolveOrganizationEvents(d);

    this.container.innerHTML = `
      <section class="organization-profile">
        <header class="org-hero">
          <div>
            <span class="profile-kicker">SportHive · Організація</span>
            <h1>${this.escape(organization.nameOrganization || organization.NameOrganization || organization.login || organization.Login || "Організація")}</h1>
            <p>${this.escape(organization.description || organization.Description || "Опис відсутній")}</p>
            <div class="profile-meta">
              <span>Логін: ${this.escape(organization.login || organization.Login || "-")}</span>
              <span>Країна: ${this.escape(organization.country || organization.Country || "-")}</span>
              <span>Тип: ${this.escape(organization.typeOrganozation || organization.TypeOrganozation || organization.typeOrganization || "-")}</span>
            </div>
          </div>
          <div class="org-actions"><button id="create-team-btn" class="profile-btn primary" type="button">Створити команду</button></div>
        </header>

        <section class="profile-stats">
          <div><b>${teams.length}</b><span>Команд</span></div>
          <div><b>${athletes.length}</b><span>Спортсменів</span></div>
          <div><b>${trainers.length}</b><span>Тренерів</span></div>
          <div><b>${judges.length}</b><span>Суддів</span></div>
        </section>

        <section class="profile-grid">
          <div class="profile-panel"><h2>Команди</h2>${this.renderTeams(teams)}</div>
          <div class="profile-panel"><h2>Судді</h2>${this.renderUsers(judges, "Judge")}</div>
        </section>

        <section class="profile-grid">
          <div class="profile-panel"><h2>Тренери</h2>${this.renderUsers(trainers, "Trainer")}</div>
          <div class="profile-panel"><h2>Спортсмени</h2>${this.renderUsers(athletes, "Athlete")}</div>
        </section>

        <section class="profile-panel"><h2>Останні події</h2>${this.renderEvents(events)}</section>
        <section class="profile-panel"><h2>Запрошення</h2>${this.renderInvitations(invitations)}</section>
      </section>
    `;
    this.bind();
  }

  private bind() {
    document.getElementById("create-team-btn")?.addEventListener("click", () => new CreateTeamModal("app").show());
    this.container.querySelectorAll<HTMLButtonElement>("[data-team-name]").forEach(button => button.addEventListener("click", () => {
      const name = button.dataset.teamName;
      if (name) new TeamPageLook("app", name).render();
    }));
    this.container.querySelectorAll<HTMLButtonElement>("[data-profile-login]").forEach(button => button.addEventListener("click", () => {
      const login = button.dataset.profileLogin;
      const role = button.dataset.profileRole as "Athlete" | "Trainer" | "Judge";
      if (login && role) new UserProfilePage("app", login, role).render();
    }));
  }

  private resolveOrganizationEvents(data: AnyObj) {
    const rawRecent = this.list(data.recentEvents || data.events || data.organizationEvents);
    const byKey = new Map<string, AnyObj>();

    for (const event of this.catalogEvents) {
      const canManage = Boolean(event.canManageEvent || event.accessLevel === "Manage");
      const matchesRecent = rawRecent.some(recent => this.eventKey(recent) === this.eventKey(event) || this.sameEventNameDate(recent, event));
      if (canManage || matchesRecent) byKey.set(this.eventKey(event), normalizeEventForUi(event));
    }

    for (const recent of rawRecent) {
      const matched = this.catalogEvents.find(event => this.sameEventNameDate(recent, event));
      byKey.set(this.eventKey(recent), normalizeEventForUi(matched || recent));
    }

    return Array.from(byKey.values()).sort((a, b) => new Date(b.dataStart || b.DataStart || 0).getTime() - new Date(a.dataStart || a.DataStart || 0).getTime()).slice(0, 8);
  }

  private renderTeams(teams: AnyObj[]) {
    if (!teams.length) return `<div class="profile-empty">Команд поки немає</div>`;
    return `<div class="profile-list">${teams.map(team => {
      const name = team.teamName || team.nameTeam || team.TeamName || team.NameTeam || team.nameComand || team.NameComand || "-";
      return `<button class="profile-list-item clickable" type="button" data-team-name="${this.attr(name)}"><b>${this.escape(name)}</b><span>${this.escape(team.typeSport || team.TypeSport || "")}</span><small>Тренер: ${this.escape(team.loginTrainer || team.LoginTrainer || team.trainerLogin || "-")}</small></button>`;
    }).join("")}</div>`;
  }

  private renderUsers(users: AnyObj[], role: "Athlete" | "Trainer" | "Judge") {
    if (!users.length) return `<div class="profile-empty">Поки немає</div>`;
    return `<div class="profile-list">${users.map(user => {
      const login = user.login || user.Login || user.loginAthlete || user.loginTrainer || user.loginJudge || "";
      const name = user.fullName || user.FullName || `${user.firsName || user.FirsName || ""} ${user.lastName || user.LastName || ""}`.trim() || login || "-";
      return `<button class="profile-list-item clickable" type="button" data-profile-login="${this.attr(login)}" data-profile-role="${role}"><b>${this.escape(name)}</b><span>${this.escape(login)}</span><small>${this.escape(user.typeSport || user.TypeSport || "")}</small></button>`;
    }).join("")}</div>`;
  }

  private renderEvents(events: AnyObj[]) {
    if (!events.length) return `<div class="profile-empty">Подій поки немає</div>`;
    return `<div class="profile-event-list">${events.map(eventInput => {
      const event = normalizeEventForUi(eventInput);
      const stats = calculateEventStats(event);
      const name = event.nameEvent || event.NameEvent || "-";
      const sport = event.typeSport || event.TypeSport || "-";
      const date = this.date(event.dataStart || event.DataStart);
      return `<article class="profile-event-card"><b>${this.escape(name)}</b><span>${this.escape(sport)} · ${stats.totalMatches}</span><small>${date} · ${formatFinishedCounter(event)} матчів завершено</small></article>`;
    }).join("")}</div>`;
  }

  private renderInvitations(invitations: AnyObj[]) {
    if (!invitations.length) return `<div class="profile-empty">Запрошень поки немає</div>`;
    return `<div class="profile-list">${invitations.map(invite => `<article class="profile-list-item"><b>${this.escape(invite.targetLogin || invite.login || invite.email || invite.Email || "Запрошення")}</b><span>${this.escape(invite.role || invite.Role || invite.status || invite.Status || "")}</span><small>${this.date(invite.createdAt || invite.CreatedAt || invite.deadlineAt || invite.DeadlineAt)}</small></article>`).join("")}</div>`;
  }

  private list(value: unknown): AnyObj[] { return Array.isArray(value) ? value as AnyObj[] : []; }

  private eventKey(event: AnyObj) {
    const id = event.idEvent || event.IdEvent;
    if (id) return `id:${id}`;
    return `${String(event.nameEvent || event.NameEvent || "").toLowerCase()}|${this.date(event.dataStart || event.DataStart)}`;
  }

  private sameEventNameDate(a: AnyObj, b: AnyObj) {
    const nameA = String(a.nameEvent || a.NameEvent || "").trim().toLowerCase();
    const nameB = String(b.nameEvent || b.NameEvent || "").trim().toLowerCase();
    return Boolean(nameA && nameB && nameA === nameB && this.date(a.dataStart || a.DataStart) === this.date(b.dataStart || b.DataStart));
  }

  private date(value: unknown) {
    if (!value) return "-";
    const date = new Date(String(value));
    return Number.isNaN(date.getTime()) ? "-" : date.toLocaleDateString("uk-UA");
  }

  private escape(value: unknown) { return String(value ?? "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;"); }
  private attr(value: unknown) { return this.escape(value).replace(/`/g, "&#096;"); }
}
