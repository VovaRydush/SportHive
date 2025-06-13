import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class RacketSportsStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Ракеткові види спорту</h2>
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
          </div>
        </div>
      </div>
    `;
  }
}
