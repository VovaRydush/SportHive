export interface AthconsteProf {
    name: string;
    sport: string;
    photo: string;
    stats: string;
    Team: string;
    DataBirth: string;
    login: string;
    Position: string;
    Matches: string[];
    dataMathes: string; //потім вибрати один чи другий обєкт і витянути з нього дані
}
export interface Organization {
    login: string;
    photo:string;
    NameOrganization: string;
    TypeOrganozation: string;
    Description: string;
    Country: string;
    Teams: TeamIndivid[];
    OrganizationJudge: string[];
    OrganizationTrainer: string[];
    Events: string[];
}
export interface Trainer {
    FirsName: string;
    LastName: string;
    SportType: string;
    Photo: string;
    Teams: string[];
    Organizations: string[];
    DataBirth: string;
    login: string;
    Position: string;
    Matches: string[];
    stats?: Stats;
}
export interface Judge {
    login: string;
    Photo: string;
    FirsName: string;
    LastName: string;
    Category: string;
}
export interface Match {
    idMatch: string;
    NameEvent: string;
    systems: string;
    DataStart: string;
    DataEnd: string;
    sport: string;
    Time: string;
    description: string;
    status: string;
    team1: string;
    team2: string;
    score: string;
    NameWinner: string;
    loginJudge: string;
    date: string;
    LocationName: string;
    Tour: string;
    Group: string;
    dataFotball: dataMatchFotball[];
    AddInformation: string;
}
export interface Event {
    NameEvent: string;
    TypeMatch:string;
    description: string;
    date: string;
    endDate: string;
    location: string;
    sport: string;
    athletes: string[];
    teams: string[];
    matches: string[];
    standings: Standing[];

    
}
export interface TeamIndivid {
    name: string;
    logo: string;
    LoginTrainer: string;
    AthleteLogins: string[];
    sport: string;
    wins: string;
    stats: Stats;
    OrganizationTeam?: Organization[];
    Matches: Match[];
}
export interface Stats {
    TotalMatches: string;
    Wins: string;
    Draws: string;
    Losses: string;
    Trophies: string;
}
export interface dataMatchFotball {
    id: number,
    time: string,
    type: string,
    player: string,
    team: string,
    description: string
}
export interface fottballProfile {
    name: string;
    Matches: number;
    Wins: number;
    Draws: number;
    Losses: number;
    Goals: number;
    Assists: number;
    YellowCards: number;
    RedCards: number;
}
export interface Standing {
    participant: string;
    points: number;
    wins: number;
    draws: number;
    losses: number;
}
export interface dataMatchBox {
    namePlayer: string;
    id: string;
    time: string;
    round: number;
    type: string;
}
export interface boxlProfile {
    Matches: number;
    Wins: number;
    Knockouts: number;
    Losses: number;
    RoundsFought: number;
    AverageScorePerRound: number;
    WeightCategory: number;
}
export const users: AthconsteProf[] = [];
export const trainers: Trainer[] = [];
export const organizations: Organization[] = [];
export const judges: Judge[] = [];
export const Teams: TeamIndivid[] = [];
export const event: Event[] = [];
export const match: Match[] = [];
export const allEvents: Event[] = [];
export const allRecentResults: Match[] = [];
export const allLiveMatches: Match[] = [];
export const allUpcomingMatches: Match[] = [];
export const allTopTeams: TeamIndivid[] = [];
export const allTopAthletes: AthconsteProf[] = [];

