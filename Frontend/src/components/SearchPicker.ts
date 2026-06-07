export type SearchPickerItem = {
  id: string;
  title: string;
  subtitle?: string;
  photo?: string;
  raw?: unknown;
};

export class SearchPicker {
  private root: HTMLElement;
  private selected = new Map<string, SearchPickerItem>();
  private search: (query: string) => Promise<SearchPickerItem[]>;
  private onChange: (items: SearchPickerItem[]) => void;
  private placeholder: string;
  private minLength: number;

  constructor(
    root: HTMLElement,
    options: {
      placeholder?: string;
      minLength?: number;
      search: (query: string) => Promise<SearchPickerItem[]>;
      onChange: (items: SearchPickerItem[]) => void;
    }
  ) {
    this.root = root;
    this.search = options.search;
    this.onChange = options.onChange;
    this.placeholder = options.placeholder || "Пошук...";
    this.minLength = options.minLength ?? 2;
  }

  render() {
    this.root.innerHTML = `
      <div class="picker">
        <div id="picker-selected" class="picker-selected"></div>
        <input id="picker-input" class="picker-input" placeholder="${this.escapeHtml(this.placeholder)}" />
        <div id="picker-results" class="picker-results"></div>
      </div>
    `;

    const input = this.root.querySelector<HTMLInputElement>("#picker-input");
    let timer: number | undefined;

    input?.addEventListener("input", () => {
      window.clearTimeout(timer);
      timer = window.setTimeout(() => this.load(input.value), 250);
    });

    this.renderSelected();
  }

  private async load(query: string) {
    const resultsRoot = this.root.querySelector<HTMLElement>("#picker-results");
    if (!resultsRoot) return;

    if (query.trim().length < this.minLength) {
      resultsRoot.innerHTML = "";
      return;
    }

    resultsRoot.innerHTML = `<div class="picker-empty">Пошук...</div>`;

    try {
      const results = await this.search(query.trim());
      const filtered = results.filter(item => !this.selected.has(item.id));

      if (!filtered.length) {
        resultsRoot.innerHTML = `<div class="picker-empty">Нічого не знайдено</div>`;
        return;
      }

      resultsRoot.innerHTML = filtered.map(item => `
        <button class="picker-result" data-id="${this.escapeAttr(item.id)}" type="button">
          ${item.photo ? `<img src="${this.escapeAttr(item.photo)}" alt="" />` : `<span class="picker-avatar">${this.escapeHtml(item.title[0] || "?")}</span>`}
          <span>
            <b>${this.escapeHtml(item.title)}</b>
            ${item.subtitle ? `<small>${this.escapeHtml(item.subtitle)}</small>` : ""}
          </span>
        </button>
      `).join("");

      resultsRoot.querySelectorAll<HTMLButtonElement>(".picker-result").forEach(button => {
        button.addEventListener("click", () => {
          const item = filtered.find(x => x.id === button.dataset.id);
          if (!item) return;

          this.selected.set(item.id, item);
          resultsRoot.innerHTML = "";

          const input = this.root.querySelector<HTMLInputElement>("#picker-input");
          if (input) input.value = "";

          this.renderSelected();
          this.onChange(Array.from(this.selected.values()));
        });
      });
    } catch {
      resultsRoot.innerHTML = `<div class="picker-empty">Помилка пошуку</div>`;
    }
  }

  private renderSelected() {
    const selectedRoot = this.root.querySelector<HTMLElement>("#picker-selected");
    if (!selectedRoot) return;

    const items = Array.from(this.selected.values());

    selectedRoot.innerHTML = items.length
      ? items.map(item => `
          <span class="picker-chip">
            ${this.escapeHtml(item.title)}
            <button data-id="${this.escapeAttr(item.id)}" type="button">×</button>
          </span>
        `).join("")
      : "";

    selectedRoot.querySelectorAll<HTMLButtonElement>("button").forEach(button => {
      button.addEventListener("click", () => {
        if (!button.dataset.id) return;
        this.selected.delete(button.dataset.id);
        this.renderSelected();
        this.onChange(Array.from(this.selected.values()));
      });
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

  private escapeAttr(value: unknown) {
    return this.escapeHtml(value).replace(/`/g, "&#096;");
  }
}
