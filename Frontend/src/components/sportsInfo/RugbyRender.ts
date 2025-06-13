import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class RugbyStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Регбі</h2>
          <div class="stats-grid">
            <div class="stat-card matches">
              <span class="stat-value">${stats.Matches}</span>
              <span class="stat-label">Матчів</span>
            </div>
            <div class="stat-card tries">
              <span class="stat-value">${stats.Tries}</span>
              <span class="stat-label">Трайсів</span>
            </div>
            <div class="stat-card tackles">
              <span class="stat-value">${stats.Tackles}</span>
              <span class="stat-label">Теклів</span>
            </div>
            <div class="stat-card points">
              <span class="stat-value">${stats.PointsScored}</span>
              <span class="stat-label">Очок</span>
            </div>
            <div class="stat-card wins">
              <span class="stat-value">${stats.Wins}</span>
              <span class="stat-label">Перемог</span>
            </div>
            <div class="stat-card losses">
              <span class="stat-value">${stats.Losses}</span>
              <span class="stat-label">Поразок</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
