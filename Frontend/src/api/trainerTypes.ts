export type TrainerTeamDto = {
  nameTeam: string;
  typeSport: string;
  photoTeam?: string | null;
  athletesCount: number;
};

export type TrainerOrganizationDto = {
  loginOrganization: string;
  nameOrganization: string;
  typeOrganozation: string;
  country: string;
};

export type TrainerStatsDto = {
  teamsCount: number;
  athletesCount: number;
  organizationsCount: number;
  totalMatches: number;
  wins: number;
  draws: number;
  losses: number;
};

export type TrainerProfileDto = {
  login: string;
  firsName: string;
  lastName: string;
  dataBirth: string;
  photo?: string | null;
  teams: TrainerTeamDto[];
  organizations: TrainerOrganizationDto[];
  stats: TrainerStatsDto;
};
