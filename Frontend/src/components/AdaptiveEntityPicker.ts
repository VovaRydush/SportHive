import { entitySearchApi } from "../api/entitySearchApi";
import type { PickerConfig, PickerEntity } from "../api/entityPickerTypes";
import "./adaptiveEntityPicker.css";

export class AdaptiveEntityPicker {
  private root: HTMLElement;
  private config: PickerConfig;
  private selected = new Map<string, PickerEntity>();
  private results: PickerEntity[] = [];
  private activeIndex = -1;
  private searchTimer?: number;

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
      <div class="entity-picker" data-type="${this.config.entityType}">
        <div class="entity-selected" data-selected></div>

        <div class="entity-input-wrap">
          <input
            class="entity-input"
            data-input
            autocomplete="off"
            placeholder="${this.escapeHtml(this.config.placeholder || this.defaultPlaceholder())}"
          />
          <button class="entity-clear" data-clear type="button" title="Очистити">×</button>
        </div>

        <div class="entity-results" data-results hidden></div>
      </div>
    `;

    this.renderSelected();
    this.bindEvents();
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

  private bindEvents() {
    const input = this.input();
    const clear = this.root.querySelector<HTMLButtonElement>("[data-clear]");

    input?.addEventListener("input", () => {
      window.clearTimeout(this.searchTimer);
      this.searchTimer = window.setTimeout(() => this.search(input.value), 250);
    });

    input?.addEventListener("keydown", event => {
      if (event.key === "ArrowDown") {
        event.preventDefault();
        this.moveActive(1);
      }

      if (event.key === "ArrowUp") {
        event.preventDefault();
        this.moveActive(-1);
      }

      if (event.key === "Enter") {
        event.preventDefault();
        const item = this.results[this.activeIndex];
        if (item) this.pick(item);
      }

      if (event.key === "Escape") {
        this.hideResults();
      }
    });

    input?.addEventListener("focus", () => {
      if (this.results.length) this.showResults();
    });

    document.addEventListener("click", event => {
      if (!this.root.contains(event.target as Node)) {
        this.hideResults();
      }
    });

    clear?.addEventListener("click", () => {
      this.selected.clear();
      this.results = [];
      if (input) input.value = "";
      this.renderSelected();
      this.hideResults();
      this.config.onChange(this.getSelected());
    });
  }

  private async search(query: string) {
    const text = query.trim();

    if (text.length < (this.config.minLength || 2)) {
      this.results = [];
      this.renderResults();
      return;
    }

    this.renderLoading();

    try {
      const selectedIds = new Set(this.selected.keys());
      this.results = (await entitySearchApi.search(this.config.entityType, text))
        .filter(item => !selectedIds.has(item.id));

      this.activeIndex = this.results.length ? 0 : -1;
      this.renderResults();
    } catch {
      this.results = [];
      this.renderError();
    }
  }

  private pick(item: PickerEntity) {
    if (!this.config.multiple) {
      this.selected.clear();
    }

    this.selected.set(item.id, item);
    this.renderSelected();
    this.config.onChange(this.getSelected());

    const input = this.input();

    if (input) {
      input.value = "";
      input.focus();
    }

    this.results = [];
    this.hideResults();
  }

  private moveActive(delta: number) {
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

    root.innerHTML = items.length
      ? items.map(item => `
        <span class="entity-chip">
          ${item.photo ? `<img src="${this.escapeAttr(item.photo)}" alt="" />` : `<b>${this.escapeHtml(item.title[0] || "?")}</b>`}
          <span>${this.escapeHtml(item.title)}</span>
          <button data-remove="${this.escapeAttr(item.id)}" type="button">×</button>
        </span>
      `).join("")
      : "";

    root.querySelectorAll<HTMLButtonElement>("[data-remove]").forEach(button => {
      button.addEventListener("click", () => {
        const id = button.dataset.remove;
        if (!id) return;

        this.selected.delete(id);
        this.renderSelected();
        this.config.onChange(this.getSelected());
      });
    });
  }

  private renderLoading() {
    const root = this.resultsRoot();
    if (!root) return;

    root.hidden = false;
    root.innerHTML = `<div class="entity-empty">Пошук...</div>`;
  }

  private renderError() {
    const root = this.resultsRoot();
    if (!root) return;

    root.hidden = false;
    root.innerHTML = `<div class="entity-empty error">Помилка пошуку</div>`;
  }

  private renderResults() {
    const root = this.resultsRoot();
    if (!root) return;

    if (!this.results.length) {
      root.hidden = false;
      root.innerHTML = `<div class="entity-empty">Нічого не знайдено</div>`;
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

  private showResults() {
    const root = this.resultsRoot();
    if (root) root.hidden = false;
  }

  private hideResults() {
    const root = this.resultsRoot();
    if (root) root.hidden = true;
  }

  private input() {
    return this.root.querySelector<HTMLInputElement>("[data-input]");
  }

  private resultsRoot() {
    return this.root.querySelector<HTMLElement>("[data-results]");
  }

  private defaultPlaceholder() {
    const labels: Record<string, string> = {
      athlete: "Знайти спортсмена...",
      team: "Знайти команду...",
      trainer: "Знайти тренера...",
      judge: "Знайти суддю...",
      organization: "Знайти організацію...",
      user: "Знайти користувача...",
    };

    return labels[this.config.entityType] || "Пошук...";
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
