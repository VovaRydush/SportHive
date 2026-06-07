import { entitySearchApi } from "../api/entitySearchApi";
import type { PickerConfig, PickerEntity } from "../api/entityPickerTypes";
import "./createTeam.css";

export class AdaptiveEntityPicker {
  private root: HTMLElement;
  private config: PickerConfig;
  private selected = new Map<string, PickerEntity>();
  private results: PickerEntity[] = [];
  private activeIndex = -1;
  private timer?: number;

  constructor(root: HTMLElement, config: PickerConfig) {
    this.root = root;
    this.config = {
      multiple: true,
      minLength: 2,
      ...config,
    };

    (this.config.selected || []).forEach(item => this.selected.set(item.id, item));
  }

  render() {
    this.root.innerHTML = `
      <div class="entity-picker">
        <div class="entity-selected" data-selected></div>

        <div class="entity-input-wrap">
          <input data-input class="entity-input" autocomplete="off" placeholder="${this.escapeHtml(this.config.placeholder || this.placeholder())}" />
          <button data-clear class="entity-clear" type="button">×</button>
        </div>

        <div data-results class="entity-results" hidden></div>
      </div>
    `;

    this.renderSelected();
    this.bind();
  }

  getSelected() {
    return Array.from(this.selected.values());
  }

  setSelected(items: PickerEntity[]) {
    this.selected.clear();
    items.forEach(item => this.selected.set(item.id, item));
    this.renderSelected();
    this.config.onChange(this.getSelected());
  }

  private bind() {
    const input = this.input();

    input?.addEventListener("input", () => {
      window.clearTimeout(this.timer);
      this.timer = window.setTimeout(() => this.search(input.value), 240);
    });

    input?.addEventListener("keydown", event => {
      if (event.key === "ArrowDown") {
        event.preventDefault();
        this.move(1);
      }

      if (event.key === "ArrowUp") {
        event.preventDefault();
        this.move(-1);
      }

      if (event.key === "Enter") {
        event.preventDefault();

        const item = this.results[this.activeIndex];
        if (item) this.pick(item);
      }

      if (event.key === "Escape") this.hide();
    });

    this.root.querySelector<HTMLButtonElement>("[data-clear]")?.addEventListener("click", () => {
      this.selected.clear();
      this.renderSelected();
      this.config.onChange(this.getSelected());

      const input = this.input();
      if (input) input.value = "";

      this.hide();
    });

    document.addEventListener("click", event => {
      if (!this.root.contains(event.target as Node)) this.hide();
    });
  }

  private async search(query: string) {
    const text = query.trim();

    if (text.length < (this.config.minLength || 2)) {
      this.hide();
      return;
    }

    const root = this.resultsRoot();
    if (!root) return;

    root.hidden = false;
    root.innerHTML = `<div class="entity-empty">Пошук...</div>`;

    try {
      const selectedIds = new Set(this.selected.keys());

      this.results = (await entitySearchApi.search(this.config.entityType, text))
        .filter(item => !selectedIds.has(item.id));

      this.activeIndex = this.results.length ? 0 : -1;
      this.renderResults();
    } catch (error) {
      console.error(error);
      root.innerHTML = `<div class="entity-empty error">Помилка пошуку</div>`;
    }
  }

  private renderResults() {
    const root = this.resultsRoot();
    if (!root) return;

    if (!this.results.length) {
      const roleText =
        this.config.entityType === "trainer"
          ? "Тренерів не знайдено. Перевір, чи тренер прийняв запрошення і є в OrganizationTrainer."
          : this.config.entityType === "athlete"
            ? "Спортсменів не знайдено. Перевір, чи спортсмен прийняв запрошення або існує в Athlete."
            : "Нічого не знайдено.";

      root.hidden = false;
      root.innerHTML = `<div class="entity-empty">${roleText}</div>`;
      return;
    }

    root.hidden = false;
    root.innerHTML = this.results.map((item, index) => `
      <button class="entity-result ${index === this.activeIndex ? "active" : ""}" data-id="${this.escapeAttr(item.id)}" type="button">
        ${item.photo ? `<img src="${this.escapeAttr(item.photo)}" alt="" />` : `<span class="entity-avatar">${this.escapeHtml(item.title[0] || "?")}</span>`}
        <span>
          <strong>${this.escapeHtml(item.title)}</strong>
          ${item.subtitle ? `<small>${this.escapeHtml(item.subtitle)}</small>` : ""}
        </span>
      </button>
    `).join("");

    root.querySelectorAll<HTMLButtonElement>(".entity-result").forEach(button => {
      button.addEventListener("click", () => {
        const item = this.results.find(x => x.id === button.dataset.id);
        if (item) this.pick(item);
      });
    });
  }

  private pick(item: PickerEntity) {
    if (!this.config.multiple) this.selected.clear();

    this.selected.set(item.id, item);
    this.renderSelected();
    this.config.onChange(this.getSelected());

    const input = this.input();
    if (input) {
      input.value = "";
      input.focus();
    }

    this.hide();
  }

  private move(delta: number) {
    if (!this.results.length) return;

    this.activeIndex += delta;

    if (this.activeIndex < 0) this.activeIndex = this.results.length - 1;
    if (this.activeIndex >= this.results.length) this.activeIndex = 0;

    this.renderResults();
  }

  private renderSelected() {
    const root = this.root.querySelector<HTMLElement>("[data-selected]");
    if (!root) return;

    const items = this.getSelected();

    root.innerHTML = items.map(item => `
      <span class="entity-chip">
        ${item.photo ? `<img src="${this.escapeAttr(item.photo)}" alt="" />` : `<b>${this.escapeHtml(item.title[0] || "?")}</b>`}
        <span>${this.escapeHtml(item.title)}</span>
        <button data-remove="${this.escapeAttr(item.id)}" type="button">×</button>
      </span>
    `).join("");

    root.querySelectorAll<HTMLButtonElement>("[data-remove]").forEach(button => {
      button.addEventListener("click", () => {
        if (!button.dataset.remove) return;

        this.selected.delete(button.dataset.remove);
        this.renderSelected();
        this.config.onChange(this.getSelected());
      });
    });
  }

  private hide() {
    const root = this.resultsRoot();
    if (root) root.hidden = true;
  }

  private input() {
    return this.root.querySelector<HTMLInputElement>("[data-input]");
  }

  private resultsRoot() {
    return this.root.querySelector<HTMLElement>("[data-results]");
  }

  private placeholder() {
    const map: Record<string, string> = {
      athlete: "Знайти спортсмена...",
      team: "Знайти команду...",
      trainer: "Знайти тренера організації...",
      judge: "Знайти суддю організації...",
      organization: "Знайти організацію...",
      user: "Знайти користувача...",
    };

    return map[this.config.entityType] || "Пошук...";
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
