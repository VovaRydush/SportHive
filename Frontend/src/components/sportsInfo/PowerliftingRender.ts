import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class PowerliftingStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
        <div class="stats-summary">
          <h2>Пауерліфтинг</h2>
          <div class="stats-grid">
            <div class="stat-card category">
              <span class="stat-value">${stats.WeightCategory}</span>
              <span class="stat-label">Вагова категорія</span>
            </div>
            <div class="stat-card squat">
              <span class="stat-value">${stats.BestSquatKg} кг</span>
              <span class="stat-label">Присідання</span>
            </div>
            <div class="stat-card bench">
              <span class="stat-value">${stats.BestBenchPressKg} кг</span>
              <span class="stat-label">Жим лежачи</span>
            </div>
            <div class="stat-card deadlift">
              <span class="stat-value">${stats.BestDeadliftKg} кг</span>
              <span class="stat-label">Станова тяга</span>
            </div>
            <div class="stat-card total">
              <span class="stat-value">${stats.TotalKg} кг</span>
              <span class="stat-label">Загальний результат</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
