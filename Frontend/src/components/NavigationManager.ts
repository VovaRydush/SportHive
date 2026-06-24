import { LoginModal } from "./LoginModalWin";
import { RegistrationModal } from "./RegistrationModal";
import { HomePage } from "./HomePage";
import { OrganizationProfile } from "./OrganizationProfile";
import { CreateEventPage } from "./EventPage";
import { CreateTeamModal } from "./CreateTeam";
import { TournamentsPage } from "./TournamentsPage";
import { StatisticsDashboard } from "./StatisticsDashboard";
import { UserProfilePage } from "./UserProfile";
import type { UserRole } from "../api/authToken";

export class NavigationManager {
  private navElement: HTMLElement | null;

  constructor(navId: string = "nav-buttons") {
    this.navElement = document.getElementById(navId);
  }

  public init() {
    if (!this.navElement) return;

    const token = localStorage.getItem("accessToken");
    const role = this.currentRole();

    if (!token) {
      this.renderGuestNav();
    } else {
      this.renderUserNav(role);
    }

    new HomePage("app").render();
  }

  private currentRole(): UserRole {
    const raw = localStorage.getItem("userRole") || localStorage.getItem("role") || "Athlete";
    const role = String(raw).trim();

    if (role === "Organization") return "Organization";
    if (role === "Trainer") return "Trainer";
    if (role === "Judge") return "Judge";
    return "Athlete";
  }

  private currentLogin() {
    return localStorage.getItem("login") || localStorage.getItem("userLogin") || "";
  }

  private renderGuestNav() {
    this.navElement!.innerHTML = `
      <button class="nav-btn" id="tournaments-btn">Турніри</button>
      <button class="nav-btn" id="statistics-btn">Статистика</button>
      <button class="nav-btn" id="login-btn">Увійти</button>
      <button class="nav-btn" id="register-btn">Зареєструватись</button>
    `;

    document.getElementById("homePagest")?.addEventListener("click", () => new HomePage("app").render());
    document.getElementById("tournaments-btn")?.addEventListener("click", () => new TournamentsPage("app").render());
    document.getElementById("statistics-btn")?.addEventListener("click", () => new StatisticsDashboard("app", "global").render());
    document.getElementById("login-btn")?.addEventListener("click", () => new LoginModal().show());
    document.getElementById("register-btn")?.addEventListener("click", () => new RegistrationModal());
  }

  private renderUserNav(role: UserRole) {
    let content = `
      <button class="nav-btn" id="tournaments-btn">Турніри</button>
      <button class="nav-btn" id="statistics-btn">Статистика</button>
    `;

    if (role === "Organization") {
      content += `
        <button class="nav-btn" id="org-statistics-btn">Статистика організації</button>
        <button class="nav-btn" id="event-btn">Створити захід</button>
        <button class="nav-btn" id="profil-btn">Профіль Організації</button>
      `;
    } else if (role === "Trainer") {
      content += `
        <button class="nav-btn" id="member-org-statistics-btn">Статистика моїх організацій</button>
        <button class="nav-btn" id="team-btn">Створити команду</button>
        <button class="nav-btn" id="profile-btn">Мій профіль</button>
      `;
    } else {
      content += `
        <button class="nav-btn" id="member-org-statistics-btn">Статистика моїх організацій</button>
        <button class="nav-btn" id="profile-btn">Мій профіль</button>
      `;
    }

    this.navElement!.innerHTML = `
      ${content}
      <button class="nav-btn" id="logout-btn">Вийти</button>
    `;

    const login = this.currentLogin();

    document.getElementById("homePagest")?.addEventListener("click", () => new HomePage("app").render());
    document.getElementById("tournaments-btn")?.addEventListener("click", () => new TournamentsPage("app").render());
    document.getElementById("statistics-btn")?.addEventListener("click", () => new StatisticsDashboard("app", "global").render());
    document.getElementById("org-statistics-btn")?.addEventListener("click", () => new StatisticsDashboard("app", "organization", login).render());
    document.getElementById("member-org-statistics-btn")?.addEventListener("click", () => new StatisticsDashboard("app", "member").render());
    document.getElementById("team-btn")?.addEventListener("click", () => new CreateTeamModal().show());

    document.getElementById("profile-btn")?.addEventListener("click", () => {
      const actualLogin = this.currentLogin();

      if (!actualLogin) {
        alert("Не знайдено login. Перелогінься.");
        return;
      }

      new UserProfilePage("app", actualLogin, role).render();
    });

    document.getElementById("profil-btn")?.addEventListener("click", () => new OrganizationProfile("app").render());
    document.getElementById("event-btn")?.addEventListener("click", () => new CreateEventPage("app").render());

    document.getElementById("logout-btn")?.addEventListener("click", () => {
      localStorage.removeItem("accessToken");
      localStorage.removeItem("token");
      localStorage.removeItem("jwt");
      localStorage.removeItem("userRole");
      localStorage.removeItem("role");
      localStorage.removeItem("login");
      localStorage.removeItem("userLogin");
      location.reload();
    });
  }
}
