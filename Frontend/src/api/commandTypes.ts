export type TeamAthleteDto = {
  nameTeam?: string;
  loginAthlets?: string;
  athleteStatus?: string;
};

export type TeamModelDto = {
  nameTeam?: string;
  loginTrainer?: string;
  typeSport?: string;
  photo?: File | null;
  athletsJson?: string;
  athlets?: TeamAthleteDto[];
};

export type OrganizationTeamDto = {
  loginOrganization?: string;
  nameTeam?: string;
};

export type NewSatatusAthlete = {
  nameTeam?: string;
  loginAthlete?: string;
  newStatus?: string;
};

export type SearchAthleteResult = {
  login: string;
  fullName: string;
  sport?: string;
  photo?: string;
  raw?: any;
};
