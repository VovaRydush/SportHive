import { AdaptiveEntityPicker } from "./AdaptiveEntityPicker";
import type { PickerEntity } from "../api/entityPickerTypes";
import { NotificationKarina } from "./Notification";
import { commandApi } from "../api/commandApi";
import type { TeamModelDto } from "../api/commandTypes";
import "./createTeam.css";

type SelectedAthlete = {
  loginAthlets: string;
  nameTeam: string;
  athleteStatus: string;
};

function cleanStorage(value: string | null) {
  return (value || "").replace(/^"(.+)"$/, "$1").trim();
}

function getCurrentOrganizationLogin() {
  return (
    cleanStorage(localStorage.getItem("organizationLogin")) ||
    cleanStorage(localStorage.getItem("loginOrganization")) ||
    cleanStorage(localStorage.getItem("login")) ||
    cleanStorage(localStorage.getItem("userLogin"))
  );
}

export class CreateTeamModal {
  private container: HTMLElement;
  private selectedTrainer: PickerEntity | null = null;
  private selectedAthletes: PickerEntity[] = [];
  private selectedStatuses = new Map<string, string>();

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);

    if (!element) {
      throw new Error(`Element with id '${containerId}' not found`);
    }

    this.container = element;
  }

  show() {
    this.render();
  }

  open() {
    this.render();
  }

  render() {
    this.container.innerHTML = `
      <section class="team-create-page">
        <header class="team-create-header">
          <div>
            <span class="team-create-kicker">SportHive · команда</span>
            <h1>Створити команду</h1>
            <p>Обери тренера організації, спортсменів і створи команду в одному місці.</p>
          </div>
        </header>

        <form id="create-team-form" class="team-create-form" novalidate>
          <section class="team-card">
            <h2>Основна інформація</h2>

            <div class="team-form-grid">
              <label>
                Назва команди
                <input id="create-team-name" name="nameTeam" type="text" required autocomplete="off" placeholder="Наприклад: SportHive Lions" />
              </label>

              <label>
                Вид спорту
                <select id="create-team-sport" name="typeSport" required>
                  <option value="Football">Football</option>
                  <option value="Basketball">Basketball</option>
                  <option value="Tennis">Tennis</option>
                  <option value="Boxing">Boxing</option>
                  <option value="Volleyball">Volleyball</option>
                  <option value="Chess">Chess</option>
                  <option value="Hockey">Hockey</option>
                  <option value="Baseball">Baseball</option>
                </select>
              </label>
            </div>
          </section>

          <section class="team-card">
            <div class="team-section-head">
              <div>
                <h2>Тренер</h2>
                <p>Обери тренера, який привʼязаний до твоєї організації.</p>
              </div>
            </div>

            <div id="team-trainer-picker" class="team-picker"></div>
          </section>

          <section class="team-card">
            <div class="team-section-head">
              <div>
                <h2>Спортсмени</h2>
                <p>Обери спортсменів через адаптивний пошук. Після вибору можна задати статус.</p>
              </div>
            </div>

            <div id="team-athletes-picker" class="team-picker"></div>
            <div id="selected-athletes-statuses" class="selected-athletes-statuses"></div>
          </section>

          <section class="team-card">
            <h2>Логотип команди</h2>

            <label class="team-file-input">
              <span>Завантажити фото</span>
              <input id="create-team-photo" name="photo" type="file" accept="image/*" />
            </label>
          </section>

          <div class="team-actions">
            <button class="team-btn secondary" id="team-back-btn" type="button">Назад</button>
            <button class="team-btn primary" id="create-team-submit" type="submit">Створити команду</button>
          </div>
        </form>
      </section>
    `;

    this.mountPickers();
    this.bindSubmit();

    document.getElementById("team-back-btn")?.addEventListener("click", () => {
      window.history.back();
    });
  }

  private mountPickers() {
    const trainerRoot = document.getElementById("team-trainer-picker");
    const athletesRoot = document.getElementById("team-athletes-picker");

    if (trainerRoot) {
      new AdaptiveEntityPicker(trainerRoot, {
        entityType: "trainer",
        multiple: false,
        placeholder: "Знайти тренера організації...",
        onChange: items => {
          this.selectedTrainer = items[0] || null;
        },
      }).render();
    }

    if (athletesRoot) {
      new AdaptiveEntityPicker(athletesRoot, {
        entityType: "athlete",
        multiple: true,
        placeholder: "Знайти спортсмена...",
        onChange: items => {
          this.selectedAthletes = items;

          items.forEach(item => {
            if (!this.selectedStatuses.has(item.id)) {
              this.selectedStatuses.set(item.id, "Active");
            }
          });

          for (const key of Array.from(this.selectedStatuses.keys())) {
            if (!items.some(item => item.id === key)) {
              this.selectedStatuses.delete(key);
            }
          }

          this.renderAthleteStatuses();
        },
      }).render();
    }
  }

  private renderAthleteStatuses() {
    const root = document.getElementById("selected-athletes-statuses");

    if (!root) return;

    if (!this.selectedAthletes.length) {
      root.innerHTML = "";
      return;
    }

    root.innerHTML = `
      <h3>Обрані спортсмени</h3>
      <div class="athlete-status-list">
        ${this.selectedAthletes.map(athlete => `
          <div class="athlete-status-row">
            <div>
              <b>${this.escapeHtml(athlete.title)}</b>
              <small>${this.escapeHtml(athlete.id)}${athlete.subtitle ? ` · ${this.escapeHtml(athlete.subtitle)}` : ""}</small>
            </div>

            <select data-athlete-status="${this.escapeAttr(athlete.id)}">
              <option value="Active" ${this.selectedStatuses.get(athlete.id) === "Active" ? "selected" : ""}>Active</option>
              <option value="Reserve" ${this.selectedStatuses.get(athlete.id) === "Reserve" ? "selected" : ""}>Reserve</option>
              <option value="Injured" ${this.selectedStatuses.get(athlete.id) === "Injured" ? "selected" : ""}>Injured</option>
            </select>
          </div>
        `).join("")}
      </div>
    `;

    root.querySelectorAll<HTMLSelectElement>("[data-athlete-status]").forEach(select => {
      select.addEventListener("change", () => {
        const login = select.dataset.athleteStatus;

        if (login) {
          this.selectedStatuses.set(login, select.value);
        }
      });
    });
  }

  private bindSubmit() {
    const form = document.getElementById("create-team-form") as HTMLFormElement | null;
    const submitButton = document.getElementById("create-team-submit") as HTMLButtonElement | null;

    if (!form) return;

    form.onsubmit = async event => {
      event.preventDefault();
      event.stopPropagation();

      const notify = new NotificationKarina();

      const nameInput = document.getElementById("create-team-name") as HTMLInputElement | null;
      const sportInput = document.getElementById("create-team-sport") as HTMLSelectElement | null;
      const photoInput = document.getElementById("create-team-photo") as HTMLInputElement | null;

      const nameTeam = (nameInput?.value || "").trim();
      const typeSport = (sportInput?.value || "").trim();
      const photo = photoInput?.files?.[0] ?? null;
      const loginOrganization = getCurrentOrganizationLogin();

      console.log("[CreateTeam] submit values:", {
        nameTeam,
        typeSport,
        loginOrganization,
        trainer: this.selectedTrainer?.id,
        athletes: this.selectedAthletes.map(a => a.id),
      });

      if (!nameTeam) {
        notify.show("Назва команди обов'язкова", "error");
        nameInput?.focus();
        return;
      }

      if (!typeSport) {
        notify.show("Вид спорту обов'язковий", "error");
        sportInput?.focus();
        return;
      }

      if (!this.selectedTrainer?.id) {
        notify.show("Обери тренера команди", "error");
        return;
      }

      if (!this.selectedAthletes.length) {
        notify.show("Обери хоча б одного спортсмена", "error");
        return;
      }

      if (!loginOrganization) {
        notify.show("Не знайдено login організації. Перелогінься як організація.", "error");
        return;
      }

      const athlets: SelectedAthlete[] = this.selectedAthletes.map(athlete => ({
        loginAthlets: athlete.id,
        nameTeam,
        athleteStatus: this.selectedStatuses.get(athlete.id) || "Active",
      }));

      const payload = {
        nameTeam,
        loginTrainer: this.selectedTrainer.id,
        typeSport,
        photo,
        athlets,
        athletsJson: JSON.stringify(athlets),
        loginOrganization,
      } as TeamModelDto & { loginOrganization: string };

      try {
        if (submitButton) submitButton.disabled = true;

        await commandApi.createTeam(payload);

        notify.show("Команду створено і прив'язано до організації", "success");
      } catch (error) {
        notify.show(error instanceof Error ? error.message : "Не вдалося створити або прив'язати команду", "error");
      } finally {
        if (submitButton) submitButton.disabled = false;
      }
    };
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

export class CreateTeam extends CreateTeamModal {}
export default CreateTeamModal;
