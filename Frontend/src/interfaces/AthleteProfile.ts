import { FootballStats } from "./FootballStats";

export interface AthleteProfile {
  Id: string;
  FullName: string;
  login: string;
  SportType: string;
  dateLastUpdate: string;
  TournamentHistory: any[];
  SportStats: FootballStats | null;
}
