import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class ArcheryStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Статистика</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${stats.Competitions}</span>
              <span class="stat-label">Змагань</span>
            </div>
            <div class="stat-card matches">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card bullseyes">
              <span class="stat-value">${stats.Bullseyes}</span>
              <span class="stat-label">Влучань в центр</span>
            </div>
            <div class="stat-card distance">
              <span class="stat-value">${stats.DistanceType}</span>
              <span class="stat-label">Тип дистанції</span>
            </div>
          </div>
        </div>

        <div class="performance-section">
          <h2>Продуктивність</h2>
          <div class="performance-stats">
            <div class="performance-card">
              <span class="performance-value">${stats.MaxPointsPerRound}</span>
              <span class="performance-label">Макс. очок за раунд</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.AveragePointsPerRound.toFixed(2)}</span>
              <span class="performance-label">Середнє за раунд</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
