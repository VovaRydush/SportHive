import { managementApi } from "../api/statisticsApi";
import "./managementPanel.css";

type Done = () => void | Promise<void>;

function closeOverlay(overlay: HTMLElement) {
  overlay.remove();
}

function valueOf(overlay: HTMLElement, selector: string) {
  return ((overlay.querySelector(selector) as HTMLInputElement | HTMLSelectElement | null)?.value || "").trim();
}

async function runSafe(action: () => Promise<unknown>, overlay: HTMLElement, onDone?: Done) {
  try {
    await action();
    await onDone?.();
    closeOverlay(overlay);
  } catch (error) {
    alert(error instanceof Error ? error.message : "Помилка виконання дії");
  }
}

export function openTeamEditor(team: {
  teamName: string;
  typeSport?: string;
  loginTrainer?: string;
}, onDone?: Done) {
  const overlay = document.createElement("div");
  overlay.className = "mgmt-overlay";
  overlay.innerHTML = `
    <div class="mgmt-modal">
      <button class="mgmt-close" type="button">×</button>
      <h2>Редагування команди</h2>

      <label>
        Назва команди
        <input id="mgmt-team-name" value="${escapeAttr(team.teamName)}" />
      </label>

      <label>
        Вид спорту
        <input id="mgmt-team-sport" value="${escapeAttr(team.typeSport || "")}" placeholder="Chess / Football / Basketball" />
      </label>

      <label>
        Логін тренера
        <input id="mgmt-team-trainer" value="${escapeAttr(team.loginTrainer || "")}" placeholder="trainer_login" />
      </label>

      <div class="mgmt-actions">
        <button id="mgmt-save-team" type="button">Зберегти</button>
        <button id="mgmt-delete-team" class="danger" type="button">Видалити команду</button>
      </div>

      <hr />

      <h3>Склад команди</h3>

      <label>
        Логін спортсмена
        <input id="mgmt-team-athlete" placeholder="athlete_login" />
      </label>

      <label>
        Статус спортсмена
        <select id="mgmt-athlete-status">
          <option value="Active">Active</option>
          <option value="Main">Main</option>
          <option value="Reserve">Reserve</option>
          <option value="Captain">Captain</option>
        </select>
      </label>

      <div class="mgmt-actions">
        <button id="mgmt-add-athlete" type="button">Додати / оновити</button>
        <button id="mgmt-remove-athlete" class="danger" type="button">Видалити спортсмена</button>
      </div>
    </div>
  `;

  document.body.appendChild(overlay);

  overlay.querySelector(".mgmt-close")?.addEventListener("click", () => closeOverlay(overlay));

  overlay.querySelector("#mgmt-save-team")?.addEventListener("click", () => runSafe(async () => {
    await managementApi.updateTeam(team.teamName, {
      newTeamName: valueOf(overlay, "#mgmt-team-name"),
      typeSport: valueOf(overlay, "#mgmt-team-sport"),
      loginTrainer: valueOf(overlay, "#mgmt-team-trainer"),
    });
  }, overlay, onDone));

  overlay.querySelector("#mgmt-delete-team")?.addEventListener("click", () => {
    if (!confirm(`Видалити команду "${team.teamName}"?`)) return;

    runSafe(async () => {
      await managementApi.deleteTeam(team.teamName);
    }, overlay, onDone);
  });

  overlay.querySelector("#mgmt-add-athlete")?.addEventListener("click", () => runSafe(async () => {
    const login = valueOf(overlay, "#mgmt-team-athlete");
    const status = valueOf(overlay, "#mgmt-athlete-status") || "Active";

    if (!login) throw new Error("Вкажи login спортсмена");

    await managementApi.addTeamAthlete(team.teamName, login, status);
  }, overlay, onDone));

  overlay.querySelector("#mgmt-remove-athlete")?.addEventListener("click", () => runSafe(async () => {
    const login = valueOf(overlay, "#mgmt-team-athlete");

    if (!login) throw new Error("Вкажи login спортсмена");
    if (!confirm(`Видалити ${login} з команди?`)) return;

    await managementApi.removeTeamAthlete(team.teamName, login);
  }, overlay, onDone));
}

export function openOrganizationRosterEditor(organizationLogin: string, onDone?: Done) {
  const overlay = document.createElement("div");
  overlay.className = "mgmt-overlay";
  overlay.innerHTML = `
    <div class="mgmt-modal">
      <button class="mgmt-close" type="button">×</button>
      <h2>Керування складом організації</h2>

      <label>
        Роль
        <select id="mgmt-org-role">
          <option value="Athlete">Спортсмен</option>
          <option value="Trainer">Тренер</option>
          <option value="Judge">Суддя</option>
        </select>
      </label>

      <label>
        Login користувача
        <input id="mgmt-org-login" placeholder="user_login" />
      </label>

      <div class="mgmt-actions">
        <button id="mgmt-remove-member" class="danger" type="button">Видалити з організації</button>
      </div>
    </div>
  `;

  document.body.appendChild(overlay);

  overlay.querySelector(".mgmt-close")?.addEventListener("click", () => closeOverlay(overlay));

  overlay.querySelector("#mgmt-add-member")?.addEventListener("click", () => runSafe(async () => {
    const role = valueOf(overlay, "#mgmt-org-role");
    const login = valueOf(overlay, "#mgmt-org-login");

    if (!login) throw new Error("Вкажи login користувача");

    await managementApi.addOrganizationMember(organizationLogin, role, login);
  }, overlay, onDone));

  overlay.querySelector("#mgmt-remove-member")?.addEventListener("click", () => runSafe(async () => {
    const role = valueOf(overlay, "#mgmt-org-role");
    const login = valueOf(overlay, "#mgmt-org-login");

    if (!login) throw new Error("Вкажи login користувача");
    if (!confirm(`Видалити ${login} з організації?`)) return;

    await managementApi.removeOrganizationMember(organizationLogin, role, login);
  }, overlay, onDone));
}

export async function removeOrganizationMember(organizationLogin: string, role: string, login: string, onDone?: Done) {
  if (!confirm(`Видалити ${login} з організації?`)) return;

  try {
    await managementApi.removeOrganizationMember(organizationLogin, role, login);
    await onDone?.();
  } catch (error) {
    alert(error instanceof Error ? error.message : "Не вдалося видалити учасника");
  }
}

export async function removeTeamAthlete(teamName: string, athleteLogin: string, onDone?: Done) {
  if (!confirm(`Видалити ${athleteLogin} з команди?`)) return;

  try {
    await managementApi.removeTeamAthlete(teamName, athleteLogin);
    await onDone?.();
  } catch (error) {
    alert(error instanceof Error ? error.message : "Не вдалося видалити спортсмена");
  }
}

function escapeAttr(value: unknown) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}
