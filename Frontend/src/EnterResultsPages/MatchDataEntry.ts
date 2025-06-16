// MatchDataEntry.ts
import './matchDataEntry.css';

interface TeamDiscipline {
    idEvent: number;
    idMatch: number;
    Tour: number;
    Group: number;
    NameDesipline: string;
    winner: string;
    composition: string[];
    firstTeamScore: string;
    secondTeamScore: string;
    NameWinner: string;
    NameLosser: string;
    Draws: string[];
    fouls: PlayerFoul[];
    twoPlayersMoves: TwoPlayerMove[];
    playMoves: PlayMove[];
    attacksMoves: AttackMove[];
    timeOuts?: TimeOut[];
    touchdowns?: Touchdown[];
}

interface PlayerFoul {
    FullNamePlayer: string;
    loginPlayer: string;
    timeFoul: string;
    foul: FoulType;
    card?: CardType;
}

interface TwoPlayerMove {
    IdMatch: number;
    FullNamePlayer: string;
    loginPlayer: string;
    typeMove: TypeMovePlayer;
    time: string;
}

interface PlayMove {
    IdMatch: number;
    FullNamePlayer: string;
    loginPlayer: string;
    typeMove: TypeMove;
    timeMove: string;
}

interface AttackMove {
    IdMatch: number;
    FullNamePlayer: string;
    loginPlayer: string;
    time: string;
    move: TypeMoves;
    realization: boolean;
}

interface TimeOut {
    IdMatch: number;
    NameTeam: string;
    TimeStartTimeOut: string;
    TimeEndTimeOut: string;
}

interface Touchdown {
    IdMatch: number;
    FullNamePlayer: string;
    loginPlayer: string;
    typeTouchdown: TypeTouchdown;
    yards: number;
    realization: boolean;
    time: string;
}

enum CardType {
    Red = "Red",
    Yellow = "Yellow",
    FreeKick = "FreeKick",
    Removal = "Removal",
    Disqualification = "Disqualification",
    Warning = "Warning",
    Minifine = "Minifine",
    BigFine = "BigFine",
    PenaltyKick = "PenaltyKick"
}

enum FoulType {
    // Team sports
    Personal = "Personal",
    Technical = "Technical",
    Handball = "Handball",
    Blocking = "Blocking",
    Shooting = "Shooting",
    Tripping = "Tripping",
    // ... other foul types
}

enum TypeMovePlayer {
    Interception = "Interception",
    BlockShot = "BlockShot",
    Replacement = "Replacement"
}

enum TypeMove {
    Tries = "Tries",
    StealBasketball = "StealBasketball",
    Offside = "Offside",
    Goal = "Goal",
    AutoGoal = "AutoGoal",
    OffensiveRebound = "OffensiveRebound",
    DefensiveRebound = "DefensiveRebound",
    LossBoll = "DefensiveRebound",
    Save = "Save"
}

enum TypeMoves {
    Serving = "Serving",
    Attack = "Attack",
    Block = "Block",
    // ... other move types
}

enum TypeTouchdown {
    pass = "pass",
    rush = "rush"
}

export class MatchDataEntry {
    private container: HTMLElement;
    private matchData: TeamDiscipline;
    private sportType: string;

    constructor(containerId: string, sportType: string, matchData?: TeamDiscipline) {
        const element = document.getElementById(containerId);
        if (!element) {
            throw new Error(`Element with id '${containerId}' not found`);
        }
        this.container = element;
        this.sportType = sportType;
        this.matchData = matchData || this.getDefaultMatchData();
    }

    private getDefaultMatchData(): TeamDiscipline {
        return {
            idEvent: 1,
            idMatch: 1,
            Tour: 1,
            Group: 1,
            NameDesipline: "Футбол",
            winner: "",
            composition: ["Команда 1", "Команда 2"],
            firstTeamScore: "0",
            secondTeamScore: "0",
            NameWinner: "",
            NameLosser: "",
            Draws: [],
            fouls: [],
            twoPlayersMoves: [],
            playMoves: [],
            attacksMoves: [],
            timeOuts: [],
            touchdowns: []
        };
    }

    async render() {
        this.container.innerHTML = `
      <div class="match-data-entry">
        <!-- Match Header -->
        <div class="match-header">
          <h2>${this.matchData.NameDesipline} - Тур ${this.matchData.Tour}</h2>
          <div class="teams-score">
            <div class="team">
              <span>${this.matchData.composition[0]}</span>
              <input type="number" id="team1Score" value="${this.matchData.firstTeamScore}" min="0">
            </div>
            <span>vs</span>
            <div class="team">
              <input type="number" id="team2Score" value="${this.matchData.secondTeamScore}" min="0">
              <span>${this.matchData.composition[1]}</span>
            </div>
          </div>
        </div>

        <!-- Main Content Tabs -->
        <div class="tabs">
          <button class="tab-button active" data-tab="fouls">Фоли</button>
          <button class="tab-button" data-tab="moves">Дії гравців</button>
          <button class="tab-button" data-tab="attacks">Атаки</button>
          <button class="tab-button" data-tab="timeouts">Тайм-аути</button>
          ${this.sportType === 'Американський футбол' ?
                '<button class="tab-button" data-tab="touchdowns">Тачдауни</button>' : ''}
        </div>

        <!-- Fouls Tab -->
        <div class="tab-content active" id="foulsTab">
          <div class="add-foul-form">
            <h3>Додати фол</h3>
            <div class="form-group">
              <label for="foulPlayer">Гравець:</label>
              <select id="foulPlayer">
                ${this.getPlayersOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="foulTime">Час:</label>
              <input type="text" id="foulTime" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="foulType">Тип фолу:</label>
              <select id="foulType">
                ${this.getFoulTypesOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="foulCard">Картка:</label>
              <select id="foulCard">
                <option value="">Немає</option>
                ${Object.values(CardType).map(card =>
                    `<option value="${card}">${card}</option>`).join('')}
              </select>
            </div>
            <button class="btn btn-primary" id="addFoulBtn">Додати фол</button>
          </div>
          <div class="fouls-list">
            <h3>Зареєстровані фоли</h3>
            <table>
              <thead>
                <tr>
                  <th>Гравець</th>
                  <th>Час</th>
                  <th>Тип фолу</th>
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

        <!-- Player Moves Tab -->
        <div class="tab-content" id="movesTab">
          <div class="move-type-selector">
            <h3>Тип дії:</h3>
            <select id="moveTypeSelect">
              <option value="twoPlayers">Двох гравців</option>
              <option value="playMoves">Ігрові дії</option>
            </select>
          </div>

          <!-- Two Players Moves -->
          <div class="move-form two-players-form">
            <h3>Дія двох гравців</h3>
            <div class="form-group">
              <label for="twoPlayersPlayer">Гравець:</label>
              <select id="twoPlayersPlayer">
                ${this.getPlayersOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="twoPlayersTime">Час:</label>
              <input type="text" id="twoPlayersTime" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="twoPlayersMoveType">Тип дії:</label>
              <select id="twoPlayersMoveType">
                ${Object.values(TypeMovePlayer).map(move =>
                        `<option value="${move}">${move}</option>`).join('')}
              </select>
            </div>
            <button class="btn btn-primary" id="addTwoPlayersMoveBtn">Додати дію</button>
          </div>

          <!-- Play Moves -->
          <div class="move-form play-moves-form">
            <h3>Ігрова дія</h3>
            <div class="form-group">
              <label for="playMovePlayer">Гравець:</label>
              <select id="playMovePlayer">
                ${this.getPlayersOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="playMoveTime">Час:</label>
              <input type="text" id="playMoveTime" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="playMoveType">Тип дії:</label>
              <select id="playMoveType">
                ${this.getPlayMoveTypesOptions()}
              </select>
            </div>
            <button class="btn btn-primary" id="addPlayMoveBtn">Додати дію</button>
          </div>

          <!-- Moves List -->
          <div class="moves-list">
            <h3>Зареєстровані дії</h3>
            <div class="moves-tabs">
              <button class="moves-tab active" data-move-type="twoPlayers">Дії двох гравців</button>
              <button class="moves-tab" data-move-type="playMoves">Ігрові дії</button>
            </div>
            
            <div class="moves-content active" data-move-type="twoPlayers">
              <table>
                <thead>
                  <tr>
                    <th>Гравець</th>
                    <th>Час</th>
                    <th>Тип дії</th>
                    <th>Дії</th>
                  </tr>
                </thead>
                <tbody id="twoPlayersMovesTableBody">
                  ${this.matchData.twoPlayersMoves.map(move => this.renderTwoPlayersMoveRow(move)).join('')}
                </tbody>
              </table>
            </div>
            
            <div class="moves-content" data-move-type="playMoves">
              <table>
                <thead>
                  <tr>
                    <th>Гравець</th>
                    <th>Час</th>
                    <th>Тип дії</th>
                    <th>Дії</th>
                  </tr>
                </thead>
                <tbody id="playMovesTableBody">
                  ${this.matchData.playMoves.map(move => this.renderPlayMoveRow(move)).join('')}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Attacks Tab -->
        <div class="tab-content" id="attacksTab">
          <div class="add-attack-form">
            <h3>Додати атаку</h3>
            <div class="form-group">
              <label for="attackPlayer">Гравець:</label>
              <select id="attackPlayer">
                ${this.getPlayersOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="attackTime">Час:</label>
              <input type="text" id="attackTime" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="attackType">Тип атаки:</label>
              <select id="attackType">
                ${Object.values(TypeMoves).map(move =>
                            `<option value="${move}">${move}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="attackRealization">Реалізація:</label>
              <input type="checkbox" id="attackRealization">
            </div>
            <button class="btn btn-primary" id="addAttackBtn">Додати атаку</button>
          </div>
          <div class="attacks-list">
            <h3>Зареєстровані атаки</h3>
            <table>
              <thead>
                <tr>
                  <th>Гравець</th>
                  <th>Час</th>
                  <th>Тип атаки</th>
                  <th>Реалізація</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody id="attacksTableBody">
                ${this.matchData.attacksMoves.map(attack => this.renderAttackRow(attack)).join('')}
              </tbody>
            </table>
          </div>
        </div>

        <!-- Timeouts Tab -->
        <div class="tab-content" id="timeoutsTab">
          <div class="add-timeout-form">
            <h3>Додати тайм-аут</h3>
            <div class="form-group">
              <label for="timeoutTeam">Команда:</label>
              <select id="timeoutTeam">
                <option value="${this.matchData.composition[0]}">${this.matchData.composition[0]}</option>
                <option value="${this.matchData.composition[1]}">${this.matchData.composition[1]}</option>
              </select>
            </div>
            <div class="form-group">
              <label for="timeoutStart">Початок:</label>
              <input type="text" id="timeoutStart" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="timeoutEnd">Кінець:</label>
              <input type="text" id="timeoutEnd" placeholder="ХХ:ХХ">
            </div>
            <button class="btn btn-primary" id="addTimeoutBtn">Додати тайм-аут</button>
          </div>
          <div class="timeouts-list">
            <h3>Зареєстровані тайм-аути</h3>
            <table>
              <thead>
                <tr>
                  <th>Команда</th>
                  <th>Початок</th>
                  <th>Кінець</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody id="timeoutsTableBody">
                ${this.matchData.timeOuts?.map(timeout => this.renderTimeoutRow(timeout)).join('') || ''}
              </tbody>
            </table>
          </div>
        </div>

        ${this.sportType === 'Американський футбол' ? `
        <!-- Touchdowns Tab -->
        <div class="tab-content" id="touchdownsTab">
          <div class="add-touchdown-form">
            <h3>Додати тачдаун</h3>
            <div class="form-group">
              <label for="touchdownPlayer">Гравець:</label>
              <select id="touchdownPlayer">
                ${this.getPlayersOptions()}
              </select>
            </div>
            <div class="form-group">
              <label for="touchdownTime">Час:</label>
              <input type="text" id="touchdownTime" placeholder="ХХ:ХХ">
            </div>
            <div class="form-group">
              <label for="touchdownType">Тип:</label>
              <select id="touchdownType">
                ${Object.values(TypeTouchdown).map(type =>
                                `<option value="${type}">${type}</option>`).join('')}
              </select>
            </div>
            <div class="form-group">
              <label for="touchdownYards">Ярди:</label>
              <input type="number" id="touchdownYards" min="0">
            </div>
            <div class="form-group">
              <label for="touchdownRealization">Реалізація:</label>
              <input type="checkbox" id="touchdownRealization">
            </div>
            <button class="btn btn-primary" id="addTouchdownBtn">Додати тачдаун</button>
          </div>
          <div class="touchdowns-list">
            <h3>Зареєстровані тачдауни</h3>
            <table>
              <thead>
                <tr>
                  <th>Гравець</th>
                  <th>Час</th>
                  <th>Тип</th>
                  <th>Ярди</th>
                  <th>Реалізація</th>
                  <th>Дії</th>
                </tr>
              </thead>
              <tbody id="touchdownsTableBody">
                ${this.matchData.touchdowns?.map(td => this.renderTouchdownRow(td)).join('') || ''}
              </tbody>
            </table>
          </div>
        </div>
        ` : ''}

        <!-- Save Button -->
        <div class="save-section">
          <button class="btn btn-save">Зберегти дані матчу</button>
        </div>
      </div>
    `;

        this.setupEventListeners();
    }

    private getPlayersOptions(): string {
        // In a real app, this would come from the team roster
        return `
      <option value="Гравець 1">Гравець 1</option>
      <option value="Гравець 2">Гравець 2</option>
      <option value="Гравець 3">Гравець 3</option>
    `;
    }

    private getFoulTypesOptions(): string {
        // Filter foul types based on sport type
        const teamSportsFouls = [
            FoulType.Personal,
            FoulType.Technical,
            FoulType.Handball,
            FoulType.Blocking,
            FoulType.Shooting,
            FoulType.Tripping
        ];

        return teamSportsFouls.map(foul =>
            `<option value="${foul}">${foul}</option>`).join('');
    }

    private getPlayMoveTypesOptions(): string {
        // Filter move types based on sport type
        const teamSportsMoves = [
            TypeMove.Tries,
            TypeMove.StealBasketball,
            TypeMove.Offside,
            TypeMove.Goal,
            TypeMove.AutoGoal,
            TypeMove.OffensiveRebound,
            TypeMove.DefensiveRebound,
            TypeMove.LossBoll,
            TypeMove.Save
        ];

        return teamSportsMoves.map(move =>
            `<option value="${move}">${move}</option>`).join('');
    }

    private renderFoulRow(foul: PlayerFoul): string {
        return `
      <tr>
        <td>${foul.FullNamePlayer}</td>
        <td>${foul.timeFoul}</td>
        <td>${foul.foul}</td>
        <td>${foul.card || '-'}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${foul.loginPlayer}-${foul.timeFoul}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${foul.loginPlayer}-${foul.timeFoul}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private renderTwoPlayersMoveRow(move: TwoPlayerMove): string {
        return `
      <tr>
        <td>${move.FullNamePlayer}</td>
        <td>${move.time}</td>
        <td>${move.typeMove}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${move.loginPlayer}-${move.time}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${move.loginPlayer}-${move.time}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private renderPlayMoveRow(move: PlayMove): string {
        return `
      <tr>
        <td>${move.FullNamePlayer}</td>
        <td>${move.timeMove}</td>
        <td>${move.typeMove}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${move.loginPlayer}-${move.timeMove}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${move.loginPlayer}-${move.timeMove}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private renderAttackRow(attack: AttackMove): string {
        return `
      <tr>
        <td>${attack.FullNamePlayer}</td>
        <td>${attack.time}</td>
        <td>${attack.move}</td>
        <td>${attack.realization ? 'Так' : 'Ні'}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${attack.loginPlayer}-${attack.time}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${attack.loginPlayer}-${attack.time}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private renderTimeoutRow(timeout: TimeOut): string {
        return `
      <tr>
        <td>${timeout.NameTeam}</td>
        <td>${timeout.TimeStartTimeOut}</td>
        <td>${timeout.TimeEndTimeOut}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${timeout.NameTeam}-${timeout.TimeStartTimeOut}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${timeout.NameTeam}-${timeout.TimeStartTimeOut}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private renderTouchdownRow(td: Touchdown): string {
        return `
      <tr>
        <td>${td.FullNamePlayer}</td>
        <td>${td.time}</td>
        <td>${td.typeTouchdown}</td>
        <td>${td.yards}</td>
        <td>${td.realization ? 'Так' : 'Ні'}</td>
        <td>
          <button class="btn btn-small btn-edit" data-id="${td.loginPlayer}-${td.time}">Редагувати</button>
          <button class="btn btn-small btn-delete" data-id="${td.loginPlayer}-${td.time}">Видалити</button>
        </td>
      </tr>
    `;
    }

    private setupEventListeners() {
        // Tab switching
        document.querySelectorAll('.tab-button').forEach(button => {
            button.addEventListener('click', () => {
                const tabId = button.getAttribute('data-tab');

                // Update active tab
                document.querySelectorAll('.tab-button').forEach(btn =>
                    btn.classList.remove('active'));
                button.classList.add('active');

                // Show corresponding content
                document.querySelectorAll('.tab-content').forEach(content =>
                    content.classList.remove('active'));
                document.getElementById(`${tabId}Tab`)?.classList.add('active');
            });
        });

        // Move type switching
        document.getElementById('moveTypeSelect')?.addEventListener('change', (e) => {
            const moveType = (e.target as HTMLSelectElement).value;
            document.querySelectorAll('.move-form').forEach(form =>
                form.classList.remove('active'));
            document.querySelector(`.${moveType}-form`)?.classList.add('active');
        });

        // Moves tabs switching
        document.querySelectorAll('.moves-tab').forEach(tab => {
            tab.addEventListener('click', () => {
                const moveType = tab.getAttribute('data-move-type');

                // Update active tab
                document.querySelectorAll('.moves-tab').forEach(t =>
                    t.classList.remove('active'));
                tab.classList.add('active');

                // Show corresponding content
                document.querySelectorAll('.moves-content').forEach(content =>
                    content.classList.remove('active'));
                document.querySelector(`.moves-content[data-move-type="${moveType}"]`)?.classList.add('active');
            });
        });

        // Add Foul
        document.getElementById('addFoulBtn')?.addEventListener('click', () => {
            const player = (document.getElementById('foulPlayer') as HTMLSelectElement).value;
            const time = (document.getElementById('foulTime') as HTMLInputElement).value;
            const foulType = (document.getElementById('foulType') as HTMLSelectElement).value as FoulType;
            const card = (document.getElementById('foulCard') as HTMLSelectElement).value as CardType;

            const newFoul: PlayerFoul = {
                FullNamePlayer: player,
                loginPlayer: player.toLowerCase().replace(' ', '_'),
                timeFoul: time,
                foul: foulType,
                card: card || undefined
            };

            this.matchData.fouls.push(newFoul);
            document.getElementById('foulsTableBody')!.innerHTML += this.renderFoulRow(newFoul);

            // Clear form
            (document.getElementById('foulTime') as HTMLInputElement).value = '';
        });

        // Add Two Players Move
        document.getElementById('addTwoPlayersMoveBtn')?.addEventListener('click', () => {
            const player = (document.getElementById('twoPlayersPlayer') as HTMLSelectElement).value;
            const time = (document.getElementById('twoPlayersTime') as HTMLInputElement).value;
            const moveType = (document.getElementById('twoPlayersMoveType') as HTMLSelectElement).value as TypeMovePlayer;

            const newMove: TwoPlayerMove = {
                IdMatch: this.matchData.idMatch,
                FullNamePlayer: player,
                loginPlayer: player.toLowerCase().replace(' ', '_'),
                typeMove: moveType,
                time: time
            };

            this.matchData.twoPlayersMoves.push(newMove);
            document.getElementById('twoPlayersMovesTableBody')!.innerHTML += this.renderTwoPlayersMoveRow(newMove);

            // Clear form
            (document.getElementById('twoPlayersTime') as HTMLInputElement).value = '';
        });

        // Add Play Move
        document.getElementById('addPlayMoveBtn')?.addEventListener('click', () => {
            const player = (document.getElementById('playMovePlayer') as HTMLSelectElement).value;
            const time = (document.getElementById('playMoveTime') as HTMLInputElement).value;
            const moveType = (document.getElementById('playMoveType') as HTMLSelectElement).value as TypeMove;

            const newMove: PlayMove = {
                IdMatch: this.matchData.idMatch,
                FullNamePlayer: player,
                loginPlayer: player.toLowerCase().replace(' ', '_'),
                typeMove: moveType,
                timeMove: time
            };

            this.matchData.playMoves.push(newMove);
            document.getElementById('playMovesTableBody')!.innerHTML += this.renderPlayMoveRow(newMove);

            // Clear form
            (document.getElementById('playMoveTime') as HTMLInputElement).value = '';
        });

        // Add Attack Move
        document.getElementById('addAttackBtn')?.addEventListener('click', () => {
            const player = (document.getElementById('attackPlayer') as HTMLSelectElement).value;
            const time = (document.getElementById('attackTime') as HTMLInputElement).value;
            const moveType = (document.getElementById('attackType') as HTMLSelectElement).value as TypeMoves;
            const realization = (document.getElementById('attackRealization') as HTMLInputElement).checked;

            const newAttack: AttackMove = {
                IdMatch: this.matchData.idMatch,
                FullNamePlayer: player,
                loginPlayer: player.toLowerCase().replace(' ', '_'),
                time: time,
                move: moveType,
                realization: realization
            };

            this.matchData.attacksMoves.push(newAttack);
            document.getElementById('attacksTableBody')!.innerHTML += this.renderAttackRow(newAttack);

            // Clear form
            (document.getElementById('attackTime') as HTMLInputElement).value = '';
            (document.getElementById('attackRealization') as HTMLInputElement).checked = false;
        });

        // Add Timeout
        document.getElementById('addTimeoutBtn')?.addEventListener('click', () => {
            const team = (document.getElementById('timeoutTeam') as HTMLSelectElement).value;
            const start = (document.getElementById('timeoutStart') as HTMLInputElement).value;
            const end = (document.getElementById('timeoutEnd') as HTMLInputElement).value;

            const newTimeout: TimeOut = {
                IdMatch: this.matchData.idMatch,
                NameTeam: team,
                TimeStartTimeOut: start,
                TimeEndTimeOut: end
            };

            this.matchData.timeOuts?.push(newTimeout);
            document.getElementById('timeoutsTableBody')!.innerHTML += this.renderTimeoutRow(newTimeout);

            // Clear form
            (document.getElementById('timeoutStart') as HTMLInputElement).value = '';
            (document.getElementById('timeoutEnd') as HTMLInputElement).value = '';
        });

        // Add Touchdown (if applicable)
        if (this.sportType === 'Американський футбол') {
            document.getElementById('addTouchdownBtn')?.addEventListener('click', () => {
                const player = (document.getElementById('touchdownPlayer') as HTMLSelectElement).value;
                const time = (document.getElementById('touchdownTime') as HTMLInputElement).value;
                const type = (document.getElementById('touchdownType') as HTMLSelectElement).value as TypeTouchdown;
                const yards = parseInt((document.getElementById('touchdownYards') as HTMLInputElement).value);
                const realization = (document.getElementById('touchdownRealization') as HTMLInputElement).checked;

                const newTouchdown: Touchdown = {
                    IdMatch: this.matchData.idMatch,
                    FullNamePlayer: player,
                    loginPlayer: player.toLowerCase().replace(' ', '_'),
                    time: time,
                    typeTouchdown: type,
                    yards: yards,
                    realization: realization
                };

                this.matchData.touchdowns?.push(newTouchdown);
                document.getElementById('touchdownsTableBody')!.innerHTML += this.renderTouchdownRow(newTouchdown);

                // Clear form
                (document.getElementById('touchdownTime') as HTMLInputElement).value = '';
                (document.getElementById('touchdownYards') as HTMLInputElement).value = '';
                (document.getElementById('touchdownRealization') as HTMLInputElement).checked = false;
            });
        }

        // Save button
        document.querySelector('.btn-save')?.addEventListener('click', () => {
            // Update scores
            this.matchData.firstTeamScore = (document.getElementById('team1Score') as HTMLInputElement).value;
            this.matchData.secondTeamScore = (document.getElementById('team2Score') as HTMLInputElement).value;

            // Determine winner
            const team1Score = parseInt(this.matchData.firstTeamScore);
            const team2Score = parseInt(this.matchData.secondTeamScore);

            if (team1Score > team2Score) {
                this.matchData.winner = this.matchData.composition[0];
                this.matchData.NameWinner = this.matchData.composition[0];
                this.matchData.NameLosser = this.matchData.composition[1];
                this.matchData.Draws = [];
            } else if (team2Score > team1Score) {
                this.matchData.winner = this.matchData.composition[1];
                this.matchData.NameWinner = this.matchData.composition[1];
                this.matchData.NameLosser = this.matchData.composition[0];
                this.matchData.Draws = [];
            } else {
                this.matchData.winner = "Draw";
                this.matchData.NameWinner = "";
                this.matchData.NameLosser = "";
                this.matchData.Draws = this.matchData.composition;
            }

            // In a real app, you would send this data to the server
            console.log('Match data saved:', this.matchData);
            alert('Дані матчу збережено!');
        });

        // Delete buttons (delegated event listeners)
        this.container.addEventListener('click', (e) => {
            const target = e.target as HTMLElement;

            if (target.classList.contains('btn-delete')) {
                const id = target.getAttribute('data-id');
                const tableType = target.closest('tbody')?.id;

                if (tableType?.includes('fouls')) {
                    this.matchData.fouls = this.matchData.fouls.filter(f =>
                        `${f.loginPlayer}-${f.timeFoul}` !== id);
                } else if (tableType?.includes('twoPlayers')) {
                    this.matchData.twoPlayersMoves = this.matchData.twoPlayersMoves.filter(m =>
                        `${m.loginPlayer}-${m.time}` !== id);
                } else if (tableType?.includes('playMoves')) {
                    this.matchData.playMoves = this.matchData.playMoves.filter(m =>
                        `${m.loginPlayer}-${m.timeMove}` !== id);
                } else if (tableType?.includes('attacks')) {
                    this.matchData.attacksMoves = this.matchData.attacksMoves.filter(a =>
                        `${a.loginPlayer}-${a.time}` !== id);
                } else if (tableType?.includes('timeouts')) {
                    this.matchData.timeOuts = this.matchData.timeOuts?.filter(t =>
                        `${t.NameTeam}-${t.TimeStartTimeOut}` !== id);
                } else if (tableType?.includes('touchdowns')) {
                    this.matchData.touchdowns = this.matchData.touchdowns?.filter(td =>
                        `${td.loginPlayer}-${td.time}` !== id);
                }

                target.closest('tr')?.remove();
            }

            // Edit buttons would work similarly but with a more complex implementation
            // to populate the form with existing data
        });
    }
}