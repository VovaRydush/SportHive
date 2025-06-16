import './boxDataEntry.css';

interface BoxMatchData {
  tour: number;
  winner: WinStruggleResult | null;
  FullNamePlayer1: string;
  FullNamePlayer2: string;
  points: RoundPoints[];
  fouls: PlayerFoul[];
}

interface WinStruggleResult {
  FullNamePlayer: string;
  loginPlayer: string;
  round: number | null;
  countPoints: number;
  win: ResultType;
}

interface RoundPoints {
  FullNamePlayer: string;
  loginPlayer: string;
  round: number;
  countPoints: number;
}

interface PlayerFoul {
  FullNamePlayer: string;
  loginPlayer: string;
  timeFoul: string;
  foul: FoulType;
  card?: CardType;
}

enum ResultType {
  Fall = "Fall", // Туше
  TechnicalSuperiority = "TechnicalSuperiority",
  Points = "Points",
  Injury = "Injury",
  Rejection = "Rejection",
  Knockout = "Knockout",
  Disqualification = "Disqualification",
  Pass = "Pass",
  TechnicalKnockout = "TechnicalKnockout",
  UnanimousDecision = "UnanimousDecision",
  SplitDecision = "SplitDecision"
}

enum FoulType {
  LowBlow = "LowBlow",
  LateHit = "LateHit",
  RabbitPunch = "RabbitPunch",
  Holding = "Holding",
  Pushing = "Pushing",
  Elbow = "Elbow",
  Headbutt = "Headbutt"
}

enum CardType {
  Warning = "Warning",
  Minifine = "Minifine",
  BigFine = "BigFine",
  Disqualification = "Disqualification"
}

export class BoxingMatchEntry {
  private container: HTMLElement;
  private matchData: BoxMatchData;

  constructor(containerId: string, matchData?: BoxMatchData) {
    const element = document.getElementById(containerId);
    if (!element) throw new Error(`Element with id '${containerId}' not found`);
    this.container = element;
    this.matchData = matchData || this.getDefaultMatchData();
  }

  private getDefaultMatchData(): BoxMatchData {
    return {
      tour: 1,
      winner: null,
      FullNamePlayer1: "Боксер 1",
      FullNamePlayer2: "Боксер 2",
      points: [],
      fouls: []
    };
  }

  async render() {
    this.container.innerHTML = `
      <div class="boxing-match-entry">
        <!-- Match Header -->
        <div class="boxing-header">
          <h2>Боксерський поєдинок - Тур ${this.matchData.tour}</h2>
          <div class="boxers-names">
            <div class="boxer">
              <span>${this.matchData.FullNamePlayer1}</span>
              <span class="vs">VS</span>
              <span>${this.matchData.FullNamePlayer2}</span>
            </div>
          </div>
        </div>

        <!-- Rounds Management -->
        <div class="rounds-section">
          <h3>Раунди</h3>
          <div class="rounds-controls">
            <button class="btn btn-add-round">Додати раунд</button>
          </div>
          <div class="rounds-list" id="roundsList">
            ${this.matchData.points.map(round => this.renderRoundRow(round)).join('')}
          </div>
        </div>

        <!-- Points Entry -->
        <div class="points-entry">
          <h3>Внесення балів</h3>
          <div class="points-form">
            <div class="form-group">
              <label for="roundNumber">Раунд:</label>
              <select id="roundNumber">
                ${Array.from({length: 12}, (_, i) => i + 1)
                  .map(r => `<option value="${r}">${r}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="boxerSelect">Боксер:</label>
              <select id="boxerSelect">
                <option value="${this.matchData.FullNamePlayer1}">${this.matchData.FullNamePlayer1}</option>
                <option value="${this.matchData.FullNamePlayer2}">${this.matchData.FullNamePlayer2}</option>
              </select>
            </div>
            <div class="form-group">
              <label for="pointsCount">Бали:</label>
              <input type="number" id="pointsCount" min="0" max="10" value="0">
            </div>
            <button class="btn btn-primary" id="addPointsBtn">Додати бали</button>
          </div>
        </div>

        <!-- Fouls Management -->
        <div class="fouls-section">
          <h3>Порушення</h3>
          <div class="fouls-form">
            <div class="form-group">
              <label for="foulBoxer">Боксер:</label>
              <select id="foulBoxer">
                <option value="${this.matchData.FullNamePlayer1}">${this.matchData.FullNamePlayer1}</option>
                <option value="${this.matchData.FullNamePlayer2}">${this.matchData.FullNamePlayer2}</option>
              </select>
            </div>
            <div class="form-group">
              <label for="foulRound">Раунд:</label>
              <select id="foulRound">
                ${Array.from({length: 12}, (_, i) => i + 1)
                  .map(r => `<option value="${r}">${r}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="foulType">Тип порушення:</label>
              <select id="foulType">
                ${Object.values(FoulType).map(f => 
                  `<option value="${f}">${this.formatFoulName(f)}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="foulCard">Картка:</label>
              <select id="foulCard">
                <option value="">Немає</option>
                ${Object.values(CardType).map(c => 
                  `<option value="${c}">${c}</option>`).join('')}
              </select>
            </div>
            <button class="btn btn-primary" id="addFoulBtn">Додати порушення</button>
          </div>
          <div class="fouls-list">
            <table>
              <thead>
                <tr>
                  <th>Боксер</th>
                  <th>Раунд</th>
                  <th>Порушення</th>
                  <th>Картка</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody id="foulsTableBody">
                ${this.matchData.fouls.map(foul => this.renderFoulRow(foul)).join('')}
              </tbody>
            </table>
          </div>
        </div>

        <!-- Match Result -->
        <div class="result-section">
          <h3>Результат поєдинку</h3>
          <div class="result-form">
            <div class="form-group">
              <label for="winnerSelect">Переможець:</label>
              <select id="winnerSelect">
                <option value="">-- Оберіть --</option>
                <option value="${this.matchData.FullNamePlayer1}">${this.matchData.FullNamePlayer1}</option>
                <option value="${this.matchData.FullNamePlayer2}">${this.matchData.FullNamePlayer2}</option>
              </select>
            </div>
            <div class="form-group">
              <label for="resultType">Тип результату:</label>
              <select id="resultType">
                ${Object.values(ResultType).map(r => 
                  `<option value="${r}">${this.formatResultName(r)}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="winRound">Раунд (якщо є):</label>
              <input type="number" id="winRound" min="1" max="12">
            </div>
            <button class="nav-btn" id="saveResultBtn">Зберегти результат</button>
          </div>
        </div>

        <!-- Summary -->
        <div class="summary-section">
          <h3>Підсумок</h3>
          <div class="summary-box">
            <div class="summary-row">
              <span>Боксер 1:</span>
              <span>${this.getTotalPoints(this.matchData.FullNamePlayer1)} балів</span>
            </div>
            <div class="summary-row">
              <span>Боксер 2:</span>
              <span>${this.getTotalPoints(this.matchData.FullNamePlayer2)} балів</span>
            </div>
            ${this.matchData.winner ? `
              <div class="summary-row winner-row">
                <span>Переможець:</span>
                <span>${this.matchData.winner.FullNamePlayer} (${this.formatResultName(this.matchData.winner.win)})</span>
              </div>
            ` : ''}
          </div>
        </div>
      </div>
    `;

    this.setupEventListeners();
  }

  private renderRoundRow(round: RoundPoints): string {
    return `
      <div class="round-row">
        <span>Раунд ${round.round}</span>
        <span>${round.FullNamePlayer}: ${round.countPoints} балів</span>
      </div>
    `;
  }

  private renderFoulRow(foul: PlayerFoul): string {
    return `
      <tr>
        <td>${foul.FullNamePlayer}</td>
        <td>${foul.timeFoul}</td>
        <td>${this.formatFoulName(foul.foul)}</td>
        <td>${foul.card || '-'}</td>
        <td>
          <button class="btn btn-small btn-delete" data-id="${foul.loginPlayer}-${foul.timeFoul}">Видалити</button>
        </td>
      </tr>
    `;
  }

  private formatFoulName(foul: FoulType): string {
    const names: Record<FoulType, string> = {
      [FoulType.LowBlow]: "Удар нижче пояса",
      [FoulType.LateHit]: "Удар після команди 'стоп'",
      [FoulType.RabbitPunch]: "Удар у потилицю",
      [FoulType.Holding]: "Утримання",
      [FoulType.Pushing]: "Штовхання",
      [FoulType.Elbow]: "Удар ліктем",
      [FoulType.Headbutt]: "Удар головою"
    };
    return names[foul] || foul;
  }

  private formatResultName(result: ResultType): string {
    const names: Record<ResultType, string> = {
      [ResultType.Fall]: "Туше",
      [ResultType.TechnicalSuperiority]: "Технічна перевага",
      [ResultType.Points]: "За балами",
      [ResultType.Injury]: "Травма",
      [ResultType.Rejection]: "Відмова",
      [ResultType.Knockout]: "Нокаут",
      [ResultType.Disqualification]: "Дискваліфікація",
      [ResultType.Pass]: "Пасс",
      [ResultType.TechnicalKnockout]: "Технічний нокаут",
      [ResultType.UnanimousDecision]: "Одностайне рішення",
      [ResultType.SplitDecision]: "Роздільне рішення"
    };
    return names[result] || result;
  }

  private getTotalPoints(boxerName: string): number {
    return this.matchData.points
      .filter(p => p.FullNamePlayer === boxerName)
      .reduce((sum, p) => sum + p.countPoints, 0);
  }

  private setupEventListeners() {
    // Add Points
    document.getElementById('addPointsBtn')?.addEventListener('click', () => {
      const round = parseInt((document.getElementById('roundNumber') as HTMLSelectElement).value);
      const boxer = (document.getElementById('boxerSelect') as HTMLSelectElement).value;
      const points = parseInt((document.getElementById('pointsCount') as HTMLInputElement).value);

      const newPoints: RoundPoints = {
        FullNamePlayer: boxer,
        loginPlayer: boxer.toLowerCase().replace(' ', '_'),
        round,
        countPoints: points
      };

      this.matchData.points.push(newPoints);
      document.getElementById('roundsList')!.innerHTML += this.renderRoundRow(newPoints);
    });

    // Add Foul
    document.getElementById('addFoulBtn')?.addEventListener('click', () => {
      const boxer = (document.getElementById('foulBoxer') as HTMLSelectElement).value;
      const round = (document.getElementById('foulRound') as HTMLSelectElement).value;
      const foulType = (document.getElementById('foulType') as HTMLSelectElement).value as FoulType;
      const card = (document.getElementById('foulCard') as HTMLSelectElement).value as CardType;

      const newFoul: PlayerFoul = {
        FullNamePlayer: boxer,
        loginPlayer: boxer.toLowerCase().replace(' ', '_'),
        timeFoul: `Раунд ${round}`,
        foul: foulType,
        card: card || undefined
      };

      this.matchData.fouls.push(newFoul);
      document.getElementById('foulsTableBody')!.innerHTML += this.renderFoulRow(newFoul);
    });

    // Save Result
    document.getElementById('saveResultBtn')?.addEventListener('click', () => {
      const winner = (document.getElementById('winnerSelect') as HTMLSelectElement).value;
      const resultType = (document.getElementById('resultType') as HTMLSelectElement).value as ResultType;
      const winRound = (document.getElementById('winRound') as HTMLInputElement).value;

      if (!winner) {
        alert("Оберіть переможця!");
        return;
      }

      this.matchData.winner = {
        FullNamePlayer: winner,
        loginPlayer: winner.toLowerCase().replace(' ', '_'),
        round: winRound ? parseInt(winRound) : null,
        countPoints: this.getTotalPoints(winner),
        win: resultType
      };

      alert("Результат збережено!");
      this.render(); // Refresh to show winner in summary
    });

    // Delete Foul
    this.container.addEventListener('click', (e) => {
      const target = e.target as HTMLElement;
      if (target.classList.contains('btn-delete')) {
        const id = target.getAttribute('data-id');
        this.matchData.fouls = this.matchData.fouls.filter(f => 
          `${f.loginPlayer}-${f.timeFoul}` !== id);
        target.closest('tr')?.remove();
      }
    });
  }
}