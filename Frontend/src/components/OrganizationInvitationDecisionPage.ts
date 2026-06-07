import { organizationApi } from "../api/organizationApi";
import { NotificationKarina } from "./Notification";

export class OrganizationInvitationDecisionPage {
  private container: HTMLElement;

  constructor(containerId: string = "app") {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
  }

  async render() {
    const params = new URLSearchParams(window.location.search);
    const token = params.get("token") || "";
    const action = params.get("action") || "";

    this.container.innerHTML = `
      <section style="max-width:720px;margin:40px auto;border:2px solid #111;padding:24px;background:#fff">
        <h1>Запрошення SportHive</h1>
        <p id="invite-status">Обробка запрошення...</p>
      </section>
    `;

    const status = document.getElementById("invite-status");
    const notify = new NotificationKarina();

    if (!token || !["accept", "decline"].includes(action)) {
      if (status) status.textContent = "Некоректне посилання запрошення.";
      return;
    }

    try {
      const result = action === "accept"
        ? await organizationApi.acceptInvitation(token)
        : await organizationApi.declineInvitation(token);

      if (status) {
        status.textContent = result.status === "Accepted"
          ? "Запрошення прийнято. Організація отримала повідомлення."
          : "Запрошення відхилено. Організація отримала повідомлення.";
      }

      notify.show("Запрошення оброблено", "success");
    } catch (error) {
      const message = error instanceof Error ? error.message : "Не вдалося обробити запрошення";
      if (status) status.textContent = message;
      notify.show(message, "error");
    }
  }
}
