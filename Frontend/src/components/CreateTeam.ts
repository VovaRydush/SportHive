import { AdaptiveEntityPicker } from "./AdaptiveEntityPicker";
import type { PickerEntity } from "../api/entityPickerTypes";
import { NotificationKarina } from "./Notification";
import { commandApi } from "../api/commandApi";
import "./createTeam.css";

type SelectedAthlete = {
  loginAthlets: string;
  nameTeam: string;
  athleteStatus: string;
};

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

        <form id="create-team-form" class="team-create-form">
          <section class="team-card">
            <h2>Основна інформація</h2>

            <div class="team-form-grid">
              <label>
                Назва команди
                <input name="NameTeam" required placeholder="Наприклад: SportHive Lions" />
              </label>

              <label>
                Вид спорту
                <select name="TypeSport" required>
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
              <input name="Photo" type="file" accept="image/*" />
            </label>
          </section>

          <div class="team-actions">
            <button class="team-btn secondary" id="team-back-btn" type="button">Назад</button>
            <button class="team-btn primary" type="submit">Створити команду</button>
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

    form?.addEventListener("submit", async event => {
      event.preventDefault();

      const formData = new FormData(form);
      const nameTeam = String(formData.get("NameTeam") || "").trim();

      if (!nameTeam) {
        new NotificationKarina().show("Введи назву команди", "error");
        return;
      }

      if (!this.selectedTrainer) {
        new NotificationKarina().show("Обери тренера команди", "error");
        return;
      }

      if (!this.selectedAthletes.length) {
        new NotificationKarina().show("Обери хоча б одного спортсмена", "error");
        return;
      }

      const athletes: SelectedAthlete[] = this.selectedAthletes.map(athlete => ({
        loginAthlets: athlete.id,
        nameTeam,
        athleteStatus: this.selectedStatuses.get(athlete.id) || "Active",
      }));

      formData.set("NameTeam", nameTeam);
      formData.set("LoginTrainer", this.selectedTrainer.id);
      formData.set("AthletsJson", JSON.stringify(athletes));

      try {
        await commandApi.createTeam(formData as any);
        new NotificationKarina().show("Команду створено", "success");
      } catch (error) {
        new NotificationKarina().show(error instanceof Error ? error.message : "Не вдалося створити команду", "error");
      }
    });
  }

   private escapeHtml(value: unknown) {
    return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
  }

  private escapeAttr(value: unknown) { return this.escapeHtml(value).replace(/`/g, "&#096;"); }
}


export class CreateTeam extends CreateTeamModal {}
export default CreateTeamModal;
