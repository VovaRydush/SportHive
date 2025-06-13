import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class VolleyballStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Статистика волейболу</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${stats.Win}</span>
              <span class="stat-label">Перемог</span>
            </div>
            <div class="stat-card losses">
              <span class="stat-value">${stats.Losses}</span>
              <span class="stat-label">Поразок</span>
            </div>
          </div>
        </div>

        <div class="performance-section">
          <h2>Продуктивність</h2>
          <div class="performance-stats">
            <div class="performance-card">
              <span class="performance-value">${stats.Blocks}</span>
              <span class="performance-label">Блоків</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.Errors}</span>
              <span class="performance-label">Помилок</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
