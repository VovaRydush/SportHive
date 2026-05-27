export type UserInfoDto = {
  email?: string;
  login?: string;
  password?: string;
  role?: string;
};

export type UserVerificationDto = {
  email?: string;
  code?: string;
};

export type RoleInfoDto = {
  fistName?: string;
  lastName?: string;
  login?: string;
  dateBirhsday: string;
  profilePhoto?: File | null;
  typeSport?: string;
};

export type OrganizationInfoDto = {
  nameOrganization?: string;
  typeOrganozation?: string;
  email?: string;
  country?: string;
  profilePhoto?: File | null;
  description?: string;
};

export type OrganizationJudgeDto = {
  loginOrganization?: string;
  role?: string;
  loginEntyty?: string;
};

export type AuthResponse = {
  accessToken?: string;
  token?: string;
  refreshToken?: string;
  login?: string;
  email?: string;
  role?: string;
};