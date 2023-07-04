import { useEffect, useState } from "react";
import UploadForm from "./components/UploadForm";
import apiClient from "./services/apiClient";
import VideoPlayer from "./components/VideoPlayer";
import SignInForm from "./components/SignInForm";

interface User {
  id: number;
  fistName: string;
  lastName: string;
  username: string;
  email: string;
}

function App() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  const AuthorizeUser = async (email: string, password: string) => {
    const formData = new FormData();
    formData.append("email", email);
    formData.append("password", password);
    await apiClient
      .post("/User/Authorization", formData)
      .then((response) => {
        console.log(response.data);
        setCurrentUser(response.data);
      })
      .catch((error) => {
        console.error(error);
        console.error(error.response.data);
      });
  };

  return (
    <>
      <div className="m-5">
        {currentUser !== null ? (
          <>
            <UploadForm userId={currentUser.id} />
            <VideoPlayer></VideoPlayer>
          </>
        ) : (
          <SignInForm onSubmit={AuthorizeUser}></SignInForm>
        )}
      </div>
    </>
  );
}

export default App;
