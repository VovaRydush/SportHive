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

export type AthleteTeamInfo = {
  firsName?: string;
  FirsName?: string;
  lastName?: string;
  LastName?: string;
  fullName?: string;
  FullName?: string;
  login?: string;
  Login?: string;
  typeSport?: string;
  TypeSport?: string;
  athleteStatus?: string;
  AthleteStatus?: string;
  photo?: string;
  Photo?: string;
};

export type OrganizationTeamInfo = {
  loginOrganization?: string;
  LoginOrganization?: string;
  nameOrganization?: string;
  NameOrganization?: string;
  country?: string;
  Country?: string;
  typeOrganozation?: string;
  TypeOrganozation?: string;
};

export type TeamMatchInfo = {
  id?: number;
  idEvent?: number;
  nameEvent?: string;
  firstTeam?: string;
  secondTeam?: string;
  score?: string;
  statusMatch?: number;
  tour?: number;
  group?: number;
  dataMatch?: string;
  timeMatch?: string;
};

export type TeamInfoDto = {
  nameTeam?: string;
  NameTeam?: string;

  trainerFirstName?: string;
  TrainerFirstName?: string;
  trainerLastName?: string;
  TrainerLastName?: string;
  trainerPhotp?: string;
  TrainerPhotp?: string;
  trainerLogin?: string;
  TrainerLogin?: string;

  photoTeam?: string;
  PhotoTeam?: string;
  typeSport?: string;
  TypeSport?: string;

  athletes?: AthleteTeamInfo[];
  Athletes?: AthleteTeamInfo[];

  organizations?: OrganizationTeamInfo[];
  Organizations?: OrganizationTeamInfo[];

  athletesCount?: number;
  AthletesCount?: number;

  matches?: TeamMatchInfo[];
  Matches?: TeamMatchInfo[];

  stats?: {
    totalMatches: number;
    finishedMatches: number;
    wins: number;
    losses: number;
    draws: number;
  };
};
