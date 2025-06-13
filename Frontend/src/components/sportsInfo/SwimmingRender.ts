import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class SwimmingStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    const discipline = stats.Disciplines && stats.Disciplines.length > 0 ? stats.Disciplines[0] : null;

    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Статистика плавання</h2>
          ${discipline ? `
          <div class="stats-grid">
            <div class="stat-card stroke-type">
              <span class="stat-value">${discipline.StrokeType}</span>
              <span class="stat-label">Тип стилю</span>
            </div>
            <div class="stat-card distance">
              <span class="stat-value">${discipline.DistanceMeters} м</span>
              <span class="stat-label">Дистанція</span>
            </div>
            <div class="stat-card best-time">
              <span class="stat-value">${discipline.BestTime}</span>
              <span class="stat-label">Кращий час</span>
            </div>
          </div>
          ` : `<p>Дані відсутні</p>`}
        </div>
      </div>
    `;
  }
}
