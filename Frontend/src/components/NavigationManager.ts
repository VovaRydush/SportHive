import { LoginModal } from "./LoginModalWin";
import { RegistrationModal } from "./RegistrationModal";
import { HomePage } from "./HomePage";
import { OrganizationProfile } from "./OrganizationProfile";
import { CreateEventPage } from "./EventPage";
import { CreateTeamModal } from "./CreateTeam";
import { TournamentsPage } from "./TournamentsPage";
import { StatisticsDashboard } from "./StatisticsDashboard";
import { UserProfilePage } from "./UserProfile";
import { getAccessToken, getCurrentLogin, getCurrentRole, clearAuthStorage, type UserRole } from "../api/authToken";
import {
  applyI18n,
  bindLanguageSwitcher,
  initI18n,
  renderLanguageSwitcher,
  t,
} from "../i18n/i18n";
import "../i18n/i18n.css";

export class NavigationManager {
  private navElement: HTMLElement | null;

  constructor(navId: string = "nav-buttons") {
    this.navElement = document.getElementById(navId);
  }

  public init() {
    initI18n();

    if (!this.navElement) return;

    const token = getAccessToken();
    const role = getCurrentRole();

    if (!token) {
      this.renderGuestNav();
    } else {
      this.renderUserNav(role);
    }

    new HomePage("app").render();
    applyI18n(document.body);
  }

  private renderGuestNav() {
    this.navElement!.innerHTML = `
      <button class="nav-btn" id="tournaments-btn">${t("Турніри")}</button>
      <button class="nav-btn" id="statistics-btn">${t("Статистика")}</button>
      <button class="nav-btn" id="login-btn">${t("Увійти")}</button>
      <button class="nav-btn" id="register-btn">${t("Зареєструватись")}</button>
      ${renderLanguageSwitcher()}
    `;

    this.bindCommonNav();
    document.getElementById("login-btn")?.addEventListener("click", () => new LoginModal().show());
    document.getElementById("register-btn")?.addEventListener("click", () => new RegistrationModal());
    bindLanguageSwitcher(() => applyI18n(document.body));
  }

  private renderUserNav(role: UserRole) {
    let content = `
      <button class="nav-btn" id="tournaments-btn">${t("Турніри")}</button>
      <button class="nav-btn" id="statistics-btn">${t("Статистика")}</button>
    `;

    if (role === "Organization") {
      content += `
        <button class="nav-btn" id="org-statistics-btn">${t("Статистика організації")}</button>
        <button class="nav-btn" id="event-btn">${t("Створити захід")}</button>
        <button class="nav-btn" id="profil-btn">${t("Профіль Організації")}</button>
      `;
    } else if (role === "Trainer") {
      content += `
        <button class="nav-btn" id="member-org-statistics-btn">${t("Статистика моїх організацій")}</button>
        <button class="nav-btn" id="team-btn">${t("Створити команду")}</button>
        <button class="nav-btn" id="profile-btn">${t("Мій профіль")}</button>
      `;
    } else {
      content += `
        <button class="nav-btn" id="member-org-statistics-btn">${t("Статистика моїх організацій")}</button>
        <button class="nav-btn" id="profile-btn">${t("Мій профіль")}</button>
      `;
    }

    this.navElement!.innerHTML = `
      ${content}
      <button class="nav-btn" id="logout-btn">${t("Вийти")}</button>
      ${renderLanguageSwitcher()}
    `;

    this.bindCommonNav();

    const login = getCurrentLogin();

    document.getElementById("org-statistics-btn")?.addEventListener("click", () => {
      new StatisticsDashboard("app", "organization", login).render();
    });

    document.getElementById("member-org-statistics-btn")?.addEventListener("click", () => {
      new StatisticsDashboard("app", "member").render();
    });

    document.getElementById("team-btn")?.addEventListener("click", () => new CreateTeamModal().show());

    document.getElementById("profile-btn")?.addEventListener("click", () => {
      const currentLogin = getCurrentLogin();

      if (!currentLogin) {
        alert(t("Не знайдено login. Перелогінься."));
        return;
      }

      new UserProfilePage("app", currentLogin, role).render();
    });

    document.getElementById("profil-btn")?.addEventListener("click", () => {
      new OrganizationProfile("app").render();
    });

    document.getElementById("event-btn")?.addEventListener("click", () => {
      new CreateEventPage("app").render();
    });

    document.getElementById("logout-btn")?.addEventListener("click", () => {
      clearAuthStorage();
      location.reload();
    });

    bindLanguageSwitcher(() => applyI18n(document.body));
  }

  private bindCommonNav() {
    document.getElementById("homePagest")?.addEventListener("click", () => new HomePage("app").render());
    document.getElementById("tournaments-btn")?.addEventListener("click", () => new TournamentsPage("app").render());
    document.getElementById("statistics-btn")?.addEventListener("click", () => new StatisticsDashboard("app", "global").render());
  }
}
