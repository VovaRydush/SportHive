import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class WeightliftingStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <h2>Статистика важкої атлетики</h2>
        <div class="stat-item">
          <strong>Категорія ваги:</strong> ${stats.WeightCategory}
        </div>
        <div class="stat-item">
          <strong>Найкращий ривок (Snatch), кг:</strong> ${stats.BestSnatchKg}
        </div>
        <div class="stat-item">
          <strong>Найкращий поштовх (Clean and Jerk), кг:</strong> ${stats.BestCleanAndJerkKg}
        </div>
        <div class="stat-item total">
          <strong>Сума (Total), кг:</strong> ${stats.TotalKg}
        </div>
      </div>
    `;
  }
}
