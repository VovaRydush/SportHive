import { managementApi } from "../api/statisticsApi";
import "./managementPanel.css";

export class OrganizationRosterManager {
  private organizationLogin: string;
  private onDone?: () => void;

  constructor(organizationLogin: string, onDone?: () => void) {
    this.organizationLogin = organizationLogin;
    this.onDone = onDone;
  }

  show() {
    const overlay = document.createElement("div");
    overlay.className = "mgmt-overlay";
    overlay.innerHTML = `
      <div class="mgmt-modal">
        <button class="mgmt-close" type="button">×</button>
        <h2>Керування складом організації</h2>

        <label>
          Роль
          <select id="mgmt-role">
            <option value="Athlete">Спортсмен</option>
            <option value="Trainer">Тренер</option>
            <option value="Judge">Суддя</option>
          </select>
        </label>

        <label>
          Логін користувача
          <input id="mgmt-login" placeholder="taras_horobets" />
        </label>

        <div class="mgmt-actions">
          <button id="mgmt-add" type="button">Додати</button>
          <button id="mgmt-remove" type="button" class="danger">Видалити</button>
        </div>
      </div>
    `;

    document.body.appendChild(overlay);

    overlay.querySelector(".mgmt-close")?.addEventListener("click", () => overlay.remove());

    overlay.querySelector("#mgmt-add")?.addEventListener("click", async () => {
      const role = (overlay.querySelector("#mgmt-role") as HTMLSelectElement).value;
      const login = (overlay.querySelector("#mgmt-login") as HTMLInputElement).value.trim();

      if (!login) return alert("Вкажи login");

      await managementApi.addOrganizationMember(this.organizationLogin, role, login);
      this.onDone?.();
      overlay.remove();
    });

    overlay.querySelector("#mgmt-remove")?.addEventListener("click", async () => {
      const role = (overlay.querySelector("#mgmt-role") as HTMLSelectElement).value;
      const login = (overlay.querySelector("#mgmt-login") as HTMLInputElement).value.trim();

      if (!login) return alert("Вкажи login");
      if (!confirm(`Видалити ${login} з організації?`)) return;

      await managementApi.removeOrganizationMember(this.organizationLogin, role, login);
      this.onDone?.();
      overlay.remove();
    });
  }
}

export class TeamEditModal {
  private teamName: string;
  private onDone?: () => void;

  constructor(teamName: string, onDone?: () => void) {
    this.teamName = teamName;
    this.onDone = onDone;
  }

  show() {
    const overlay = document.createElement("div");
    overlay.className = "mgmt-overlay";
    overlay.innerHTML = `
      <div class="mgmt-modal">
        <button class="mgmt-close" type="button">×</button>
        <h2>Редагування команди</h2>

        <label>
          Назва команди
          <input id="team-name" value="${this.escape(this.teamName)}" />
        </label>

        <label>
          Вид спорту
          <input id="team-sport" placeholder="Football / Chess / Basketball" />
        </label>

        <label>
          Логін тренера
          <input id="team-trainer" placeholder="trainer_login" />
        </label>

        <div class="mgmt-actions">
          <button id="team-save" type="button">Зберегти</button>
          <button id="team-delete" type="button" class="danger">Видалити команду</button>
        </div>

        <hr />

        <h3>Склад команди</h3>

        <label>
          Логін спортсмена
          <input id="team-athlete" placeholder="athlete_login" />
        </label>

        <div class="mgmt-actions">
          <button id="team-add-athlete" type="button">Додати спортсмена</button>
          <button id="team-remove-athlete" type="button" class="danger">Видалити спортсмена</button>
        </div>
      </div>
    `;

    document.body.appendChild(overlay);

    overlay.querySelector(".mgmt-close")?.addEventListener("click", () => overlay.remove());

    overlay.querySelector("#team-save")?.addEventListener("click", async () => {
      const newTeamName = (overlay.querySelector("#team-name") as HTMLInputElement).value.trim();
      const typeSport = (overlay.querySelector("#team-sport") as HTMLInputElement).value.trim();
      const loginTrainer = (overlay.querySelector("#team-trainer") as HTMLInputElement).value.trim();

      await managementApi.updateTeam(this.teamName, { newTeamName, typeSport, loginTrainer });
      this.onDone?.();
      overlay.remove();
    });

    overlay.querySelector("#team-delete")?.addEventListener("click", async () => {
      if (!confirm(`Видалити команду ${this.teamName}?`)) return;
      await managementApi.deleteTeam(this.teamName);
      this.onDone?.();
      overlay.remove();
    });

    overlay.querySelector("#team-add-athlete")?.addEventListener("click", async () => {
      const athlete = (overlay.querySelector("#team-athlete") as HTMLInputElement).value.trim();
      if (!athlete) return alert("Вкажи login спортсмена");

      await managementApi.addTeamAthlete(this.teamName, athlete);
      this.onDone?.();
      overlay.remove();
    });

    overlay.querySelector("#team-remove-athlete")?.addEventListener("click", async () => {
      const athlete = (overlay.querySelector("#team-athlete") as HTMLInputElement).value.trim();
      if (!athlete) return alert("Вкажи login спортсмена");

      await managementApi.removeTeamAthlete(this.teamName, athlete);
      this.onDone?.();
      overlay.remove();
    });
  }

  private escape(value: unknown) {
    return String(value ?? "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }
}
