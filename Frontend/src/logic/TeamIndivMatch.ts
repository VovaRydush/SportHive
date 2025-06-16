export interface TeamIndivMatch {
    typeSport: string;
    matches: TeamIndivMatchRes[];
}

export interface TeamIndivMatchRes {
    firstTeamScore: string;
    secondTeamScore: string;
    nameEntity1: string;
    photoFirstEntity: string;
    nameEntity2: string;
    photoSecondEntity: string;
    statusMatch: string; // "Win", "Loss", "Draw" або подібне
}
