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
  loginOrganization?: string;
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

export type AthleteTeamInfo = {
  login?: string;
  Login?: string;
  firsName?: string;
  FirsName?: string;
  lastName?: string;
  LastName?: string;
  fullName?: string;
  FullName?: string;
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
  typeOrganization?: string;
  TypeOrganization?: string;
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
  dataMatch?: string | null;
  timeMatch?: string | null;
};

export type TeamInfoDto = {
  nameTeam?: string;
  NameTeam?: string;
  trainerFirstName?: string;
  TrainerFirstName?: string;
  trainerLastName?: string;
  TrainerLastName?: string;
  trainerPhoto?: string;
  TrainerPhoto?: string;
  trainerPhotp?: string;
  TrainerPhotp?: string;
  trainerLogin?: string;
  TrainerLogin?: string;
  photoTeam?: string;
  PhotoTeam?: string;
  teamPhoto?: string;
  TeamPhoto?: string;
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
