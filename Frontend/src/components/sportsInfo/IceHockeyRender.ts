import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class IceHockeyStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Хокей</h2>
          <div class="stats-grid">
            <div class="stat-card total">
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
            <div class="stat-card draws">
              <span class="stat-value">${stats.Draws}</span>
              <span class="stat-label">Нічиї</span>
            </div>
            <div class="stat-card goals">
              <span class="stat-value">${stats.Goals}</span>
              <span class="stat-label">Голи</span>
            </div>
            <div class="stat-card assists">
              <span class="stat-value">${stats.Assists}</span>
              <span class="stat-label">Асисти</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
