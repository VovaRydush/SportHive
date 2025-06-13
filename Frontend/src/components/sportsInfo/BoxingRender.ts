import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class BoxingStatsRenderer implements ISportStatsRenderer {
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
            <div class="stat-card ko">
              <span class="stat-value">${stats.Knockouts}</span>
              <span class="stat-label">Нокаути</span>
            </div>
          </div>
        </div>

        <div class="performance-section">
          <h2>Продуктивність</h2>
          <div class="performance-stats">
            <div class="performance-card">
              <span class="performance-value">${stats.RoundsFought}</span>
              <span class="performance-label">Раундів</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.AverageScorePerRound.toFixed(1)}</span>
              <span class="performance-label">Сер. бал за раунд</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.WeightCategory}</span>
              <span class="performance-label">Вага</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
