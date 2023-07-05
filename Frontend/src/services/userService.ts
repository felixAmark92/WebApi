import apiClient from "./apiClient";

class UserService {
  handleSubmit = async (email: string, password: string) => {
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
