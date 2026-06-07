import { AdaptiveEntityPicker } from "./AdaptiveEntityPicker";
import type { PickerEntity } from "../api/entityPickerTypes";
import { NotificationKarina } from "./Notification";
import { commandApi } from "../api/commandApi";
import "./adaptiveEntityPicker.css";

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

  /*
    IMPORTANT COMPATIBILITY:
    Existing files call:
    new CreateTeamModal(...).show()

    So show() must exist.
  */
  show() {
    this.render();
  }

  /*
    Some newer files may call open().
  */
  open() {
    this.render();
  }

  render() {
    this.container.innerHTML = `
      <section class="create-team-page">
        <h1>Створити команду</h1>

        <form id="create-team-form" class="create-team-form">
          <label>Назва команди</label>
          <input name="NameTeam" required placeholder="Наприклад: SportHive Lions" />

          <label>Вид спорту</label>
          <select name="TypeSport" required>
            <option value="Football">Football</option>
            <option value="Basketball">Basketball</option>
            <option value="Tennis">Tennis</option>
            <option value="Boxing">Boxing</option>
            <option value="Volleyball">Volleyball</option>
            <option value="Chess">Chess</option>
          </select>

          <div class="event-picker-section">
            <h2>Тренер</h2>
            <p>Обери тренера, привʼязаного до організації.</p>
            <div id="team-trainer-picker"></div>
          </div>

          <div class="event-picker-section">
            <h2>Спортсмени</h2>
            <p>Обери спортсменів через адаптивний пошук.</p>
            <div id="team-athletes-picker"></div>
            <div id="selected-athletes-statuses" class="selected-athletes-statuses"></div>
          </div>

          <label>Логотип команди</label>
          <input name="Photo" type="file" accept="image/*" />

          <button class="primary-btn" type="submit">Створити команду</button>
        </form>
      </section>
    `;

    this.mountPickers();
    this.bindSubmit();
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
      <h3>Статус спортсменів</h3>
      ${this.selectedAthletes.map(athlete => `
        <div class="athlete-status-row">
          <span>${this.escapeHtml(athlete.title)} <small>${this.escapeHtml(athlete.id)}</small></span>
          <select data-athlete-status="${this.escapeAttr(athlete.id)}">
            <option value="Active" ${this.selectedStatuses.get(athlete.id) === "Active" ? "selected" : ""}>Active</option>
            <option value="Reserve" ${this.selectedStatuses.get(athlete.id) === "Reserve" ? "selected" : ""}>Reserve</option>
            <option value="Injured" ${this.selectedStatuses.get(athlete.id) === "Injured" ? "selected" : ""}>Injured</option>
          </select>
        </div>
      `).join("")}
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
      const nameTeam = String(formData.get("NameTeam") || "");

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

/*
  Compatibility exports:
  Existing project files may import:
  - CreateTeamModal
  - CreateTeam
  - default
*/
export class CreateTeam extends CreateTeamModal {}
export default CreateTeamModal;
