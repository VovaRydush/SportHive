import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class CyclingStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    const raceTypes = stats.RaceTypes?.length
      ? stats.RaceTypes.join(", ")
      : "Невідомо";

    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Велоспорт</h2>
          <div class="stats-grid">
            <div class="stat-card total">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Заїздів</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${stats.Wins}</span>
              <span class="stat-label">Перемог</span>
            </div>
            <div class="stat-card distance">
              <span class="stat-value">${stats.BestDistance}</span>
              <span class="stat-label">Найкраща дистанція</span>
            </div>
            <div class="stat-card time">
              <span class="stat-value">${stats.BestTime.toFixed(1)}с</span>
              <span class="stat-label">Найкращий час</span>
            </div>
            <div class="stat-card speed">
              <span class="stat-value">${stats.AverageSpeedKmH.toFixed(1)} км/г</span>
              <span class="stat-label">Середня швидкість</span>
            </div>
          </div>
        </div>

        <div class="performance-section">
          <h3>Типи заїздів</h3>
          <p>${raceTypes}</p>
        </div>
      </div>
    `;
  }
}
