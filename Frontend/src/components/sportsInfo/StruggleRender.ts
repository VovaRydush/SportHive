import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class StruggleStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    console.log(stats);
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Статистика боротьби</h2>
          <div class="stats-grid">
            <div class="stat-card matches">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${stats.Wins}</span>
              <span class="stat-label">Перемог</span>
            </div>
            <div class="stat-card losses">
              <span class="stat-value">${stats.Losses}</span>
              <span class="stat-label">Поразок</span>
            </div>
            <div class="stat-card tech-wins">
              <span class="stat-value">${stats.TechnicalWins}</span>
              <span class="stat-label">Технічних перемог</span>
            </div>
            <div class="stat-card pin-wins">
              <span class="stat-value">${stats.PinWins}</span>
              <span class="stat-label">Перемог утриманням</span>
            </div>
            <div class="stat-card weight-category">
              <span class="stat-value">${stats.WeightCategory || "n/a"}</span>
              <span class="stat-label">Вагова категорія</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
