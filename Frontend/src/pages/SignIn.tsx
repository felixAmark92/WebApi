import React from "react";
import apiClient from "../services/apiClient";
import SignInForm from "../components/SignInForm";
import User from "../models/user";
import { useNavigate } from "react-router-dom";

interface Props {
  SetUser: (user: User) => void;
}

const SignIn = ({ SetUser }: Props) => {
  const navigate = useNavigate();
  const AuthorizeUser = async (email: string, password: string) => {
    const formData = new FormData();
    formData.append("email", email);
    formData.append("password", password);
    await apiClient
      .post("/User/Authorization", formData, { withCredentials: true })
      .then((response) => {
        console.log(response.data);
        console.log(response.data.firstName);
        SetUser(response.data);
        navigate("/");
      })
      .catch((error) => {
        console.error(error);
        console.error(error.response.data);
      });
  };

  return (
    <div className="container m-5">
      <SignInForm onSubmit={AuthorizeUser}></SignInForm>
    </div>
  );
};

export default SignIn;
