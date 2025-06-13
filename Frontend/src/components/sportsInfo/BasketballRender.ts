import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class BasketballStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Статистика</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${stats.Wins}</span>
              <span class="stat-label">Перемоги</span>
            </div>
            <div class="stat-card losses">
              <span class="stat-value">${stats.Losses}</span>
              <span class="stat-label">Поразки</span>
            </div>
            <div class="stat-card points">
              <span class="stat-value">${stats.Points}</span>
              <span class="stat-label">Очки</span>
            </div>
          </div>
        </div>

        <div class="performance-section">
          <h2>Продуктивність</h2>
          <div class="performance-stats">
            <div class="performance-card">
              <span class="performance-value">${stats.Rebounds}</span>
              <span class="performance-label">Підбирання</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.Assists}</span>
              <span class="performance-label">Асисти</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.Blocks}</span>
              <span class="performance-label">Блоки</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.ThreePointPercentage.toFixed(1)}%</span>
              <span class="performance-label">Трьохочкові</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.FreeThrowPercentage.toFixed(1)}%</span>
              <span class="performance-label">Штрафні</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
