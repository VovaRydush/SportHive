import { TeamIndivMatchRes } from "../../logic/TeamIndivMatch";
import { IUserMathch } from "./IUserMathch";

export class UserTeamMatchRender implements IUserMathch {
  render(matches: TeamIndivMatchRes[]): string {
    const getMatchResultLetter = (status: string) => {
      switch (status) {
        case "Win": return { letter: "В", class: "win" };
        case "Loss": return { letter: "П", class: "loss" };
        case "Draw": return { letter: "Н", class: "draw" };
        default: return { letter: "?", class: "" };
      }
    };

    const matchCards = matches.map(match => {
      const { letter, class: resultClass } = getMatchResultLetter(match.statusMatch);
      return `
        <div class="match-card ${resultClass}">
          <div class="match-result">${letter}</div>
          <div class="match-teams">
            <div class="team">
              <img src="${match.photoFirstEntity}" alt="${match.nameEntity1}">
              <span>${match.nameEntity1}</span>
            </div>
            <div class="match-score">${match.firstTeamScore+"-"+match.secondTeamScore}</div>
            <div class="team">
              <img src="${match.photoSecondEntity}" alt="${match.nameEntity2}">
              <span>${match.nameEntity2}</span>
            </div>
          </div>
        </div>
      `;
    }).join("");

    return `
      <div class="matches-section">
        <h2>Останні матчі</h2>
        <div class="matches-list">
          ${matchCards}
        </div>
      </div>
    `;
  }
}
