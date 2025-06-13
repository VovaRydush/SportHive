import { ISportStatsRenderer } from "./ISportStatsRenderer";

export class CheckersChessStatsRenderer implements ISportStatsRenderer {
  renderStats(stats: any): string {
    return `
      <div class="stats-section">
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
              <span class="performance-value">${stats.EloRating}</span>
              <span class="performance-label">Рейтинг Elo</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.FastestWinMoves}</span>
              <span class="performance-label">Найшвидша перемога (ходів)</span>
            </div>
            <div class="performance-card">
              <span class="performance-value">${stats.AverageMoveTimeSeconds.toFixed(1)}с</span>
              <span class="performance-label">Сер. час на хід</span>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
