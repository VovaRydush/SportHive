import { apiRequest, AUTH_API_URL } from "./http";
import type {
  AuthResponse,
  OrganizationInfoDto,
  OrganizationJudgeDto,
  RoleInfoDto,
  UserInfoDto,
  UserVerificationDto,
} from "./authTypes";

function appendIfExists(formData: FormData, key: string, value: unknown) {
  if (value !== undefined && value !== null && value !== "") {
    formData.append(key, value as string | Blob);
  }
}

function roleInfoToFormData(data: RoleInfoDto) {
  const formData = new FormData();

  appendIfExists(formData, "fistName", data.fistName);
  appendIfExists(formData, "lastName", data.lastName);
  appendIfExists(formData, "login", data.login);
  appendIfExists(formData, "dateBirhsday", data.dateBirhsday);
  appendIfExists(formData, "typeSport", data.typeSport);

  if (data.profilePhoto) {
    formData.append("profilePhoto", data.profilePhoto);
  }

  return formData;
}

function organizationInfoToFormData(data: OrganizationInfoDto) {
  const formData = new FormData();

  appendIfExists(formData, "nameOrganization", data.nameOrganization);
  appendIfExists(formData, "typeOrganozation", data.typeOrganozation);
  appendIfExists(formData, "email", data.email);
  appendIfExists(formData, "country", data.country);
  appendIfExists(formData, "description", data.description);

  if (data.profilePhoto) {
    formData.append("profilePhoto", data.profilePhoto);
  }

  return formData;
}

export const authApi = {
  register(data: UserInfoDto) {
    return apiRequest<AuthResponse | string>("/registr", {
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
      body: roleInfoToFormData(data),
    });
  },

  completeOrganizationProfile(data: OrganizationInfoDto) {
    return apiRequest<string>("/complite-profile-organization", {
      method: "POST",
      body: organizationInfoToFormData(data),
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
      method: "POST",
      body: JSON.stringify({
        email: data.email ?? "",
        code: data.code ?? "",
      }),
      auth: false,
    });
  },

  changePassword(data: UserInfoDto) {
    return apiRequest<string>("/change-password", {
      method: "POST",
      body: JSON.stringify({
        email: data.email ?? "",
        login: data.login ?? "",
        password: data.password ?? "",
        role: data.role ?? "",
      }),
      auth: false,
    });
  },

  getUserPhotoUrl(login: string) {
    return `${AUTH_API_URL}/get-user-photo/${encodeURIComponent(login)}`;
  },

  getUserPhoto(login: string) {
    return apiRequest<any>(`/get-user-photo/${encodeURIComponent(login)}`, {
      method: "GET",
    });
  },

  getStatisticInfo(login: string) {
    return apiRequest<any>(`/get-statistic-info/${encodeURIComponent(login)}`, {
      method: "GET",
    });
  },

  async searchAthlete(fullName: string) {
    const query = encodeURIComponent(fullName);

    try {
      return await apiRequest<any[]>(`/get-search-athlete/${query}?FullName=${query}`, {
        method: "GET",
      });
    } catch {
      return apiRequest<any[]>(`/get-search-athlete?FullName=${query}`, {
        method: "GET",
      });
    }
  },
};
