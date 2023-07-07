import { Route, Routes } from "react-router-dom";
import About from "./pages/About";
import Home from "./pages/Home";
import { Fragment, useEffect, useState } from "react";
import Navigationbar from "./components/Navigationbar";
import SignIn from "./pages/SignIn";
import User from "./models/user";
import apiClient from "./services/apiClient";
import SignOut from "./pages/SignOut";
import UploadVideo from "./pages/UploadVideo";
import UserVideos from "./pages/UserVideos";

function App() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  useEffect(() => {
    if (currentUser == null || currentUser == undefined) {
      apiClient
        .post("/User/TryGetSession", undefined, { withCredentials: true })
        .then((response) => {
          setCurrentUser(response.data);
        })
        .catch((error) => console.error(error.response.data));
    }

    console.log("user is ", currentUser?.firstName);
  }, [currentUser]);

  return (
    <Fragment>
      <Navigationbar user={currentUser} signOut={() => setCurrentUser(null)} />
      <div className="container ms-5 me-5">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/about" element={<About />} />
          <Route
            path="/sign-in"
            element={<SignIn SetUser={(e) => setCurrentUser(e)} />}
          ></Route>
          <Route path="/sign-out" element={<SignOut />} />
          <Route
            path="/upload-video"
            element={<UploadVideo user={currentUser} />}
          />
          <Route
            path="videos"
            element={<UserVideos user={currentUser} />}
          ></Route>
        </Routes>
      </div>
    </Fragment>
  );
}

export default App;
