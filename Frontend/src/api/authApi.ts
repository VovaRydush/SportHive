import { apiRequest } from "./http";
import type {
  AuthResponse,
  OrganizationInfoDto,
  OrganizationJudgeDto,
  RoleInfoDto,
  UserInfoDto,
  UserVerificationDto,
} from "./authTypes";

function toFormData(data: Record<string, any>) {
  const formData = new FormData();

  for (const key in data) {
    const value = data[key];

    if (value !== undefined && value !== null) {
      formData.append(key, value);
    }
  }

  return formData;
}

export const authApi = {
  register(data: UserInfoDto) {
    return apiRequest<AuthResponse>("/registr", {
      method: "POST",
      body: JSON.stringify(data),
      auth: false,
    });
  },

  login(data: UserInfoDto) {
    return apiRequest<AuthResponse>("/login", {
      method: "POST",
      body: JSON.stringify(data),
      auth: false,
    });
  },

  verify(data: UserVerificationDto) {
    return apiRequest<string>("/verify", {
      method: "POST",
      body: JSON.stringify(data),
      auth: false,
    });
  },

  completeProfile(data: RoleInfoDto) {
    return apiRequest<string>("/complite-profile", {
      method: "POST",
      body: toFormData(data),
    });
  },

  completeOrganizationProfile(data: OrganizationInfoDto) {
    return apiRequest<string>("/complite-profile-organization", {
      method: "POST",
      body: toFormData(data),
    });
  },

  linkEmployee(data: OrganizationJudgeDto) {
    return apiRequest<string>("/link-employee", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  refreshToken(login: string) {
    return apiRequest<AuthResponse>(`/refresh-token?login=${encodeURIComponent(login)}`, {
      method: "GET",
    });
  },

  logout(login: string) {
    return apiRequest<string>("/logout", {
      method: "DELETE",
      body: JSON.stringify(login),
    });
  },

  sendRecoveryEmail(email: string) {
    return apiRequest<string>("/send-recovery-email", {
      method: "POST",
      body: JSON.stringify(email),
      auth: false,
    });
  },

  checkRecoveryCode(data: UserVerificationDto) {
    return apiRequest<string>("/check-code-recavery", {
      method: "GET",
      body: JSON.stringify(data),
      auth: false,
    });
  },

  changePassword(data: UserInfoDto) {
    return apiRequest<string>("/change-password", {
      method: "POST",
      body: JSON.stringify(data),
      auth: false,
    });
  },

  getUserPhoto(login: string) {
    return `${ "http://localhost:5154"}/get-user-photo/${login}`;
  },

  getStatisticInfo(login: string) {
    return apiRequest<any>(`/get-statistic-info/${encodeURIComponent(login)}`, {
      method: "GET",
    });
  },

  searchAthlete(fullName: string) {
    return apiRequest<any[]>(
      `/get-search-athlete/${encodeURIComponent(fullName)}?FullName=${encodeURIComponent(fullName)}`,
      {
        method: "GET",
      }
    );
  },
};