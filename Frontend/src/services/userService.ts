import User from "../models/user";
import apiClient from "./apiClient";

class UserService {
  TryGetSession = (request: string): User | null => {
    apiClient
      .post<User>("/User/" + request, undefined, { withCredentials: true })
      .then((response) => {
        return response.data;
      })
      .catch((error) => console.error(error.response.data));
    return null;
  };

  FetchData = async (request: string, id: number) => {
    try {
      const response = await apiClient.post(request, undefined, {
        params: { uploaderId: id },
      });
      return response.data;
    } catch (error) {
      console.log(error);
      throw error;
    }
  };
  AuthorizeUser = async (email: string, password: string) => {
    const formData = new FormData();
    formData.append("email", email);
    formData.append("password", password);
    await apiClient
      .post("/User/Authorization", formData, { withCredentials: true })
      .then((response) => {
        console.log(response.data);
        return response.data;
      })
      .catch((error) => {
        console.error(error);
        console.error(error.response.data);
      });
  };
}

export default new UserService();
