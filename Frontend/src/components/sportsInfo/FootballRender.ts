import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class FootballStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any) : string {
    
    return `
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
              <div class="stat-card draws">
                <span class="stat-value">${stats.Draws}</span>
                <span class="stat-label">Нічиї</span>
              </div>
              <div class="stat-card losses">
                <span class="stat-value">${stats.Losses}</span>
                <span class="stat-label">Поразки</span>
              </div>
            </div>
          </div>

          <div class="performance-section">
            <h2>Продуктивність</h2>
            <div class="performance-stats">
              <div class="performance-card">
                <span class="performance-value">${stats.Goals}</span>
                <span class="performance-label">Голи</span>
              </div>
              <div class="performance-card">
                <span class="performance-value">${stats.Assists}</span>
                <span class="performance-label">Асисти</span>
              </div>
              <div class="performance-card">
                <span class="performance-value">${stats.YellowCards}</span>
                <span class="performance-label">Жовтих карток</span>
              </div>
              <div class="performance-card">
                <span class="performance-value">${stats.RedCards}</span>
                <span class="performance-label">Червоних карток</span>
              </div>
            </div>
          </div>
        `;
  }
}