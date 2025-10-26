import { environment } from "../../../environments/environment";

export const API_CONFIG = {
  baseUrl: environment.apiBaseUrl,
  timeout: 5000,
  headers: {
    Accept: "application/json"
  },
};
