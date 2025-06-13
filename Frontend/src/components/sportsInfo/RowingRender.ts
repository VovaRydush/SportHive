import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class RowingStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Веслування</h2>
          <div class="stats-grid">
            <div class="stat-card matches">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Змагань</span>
            </div>
            <div class="stat-card boat-type">
              <span class="stat-value">${stats.BoatType}</span>
              <span class="stat-label">Тип човна</span>
            </div>
            <div class="stat-card discipline">
              <span class="stat-value">${stats.Discipline}</span>
              <span class="stat-label">Дисципліна</span>
            </div>
            <div class="stat-card best-distance">
              <span class="stat-value">${stats.BestDistance}</span>
              <span class="stat-label">Найкраща дистанція</span>
            </div>
            <div class="stat-card best-time">
              <span class="stat-value">${stats.BestTimeSeconds}s</span>
              <span class="stat-label">Найкращий час</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
