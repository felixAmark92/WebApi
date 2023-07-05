import axios from "axios";

export default axios.create({
  withCredentials: true,
  baseURL: "https://localhost:7156",
});
