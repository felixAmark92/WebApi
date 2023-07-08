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
import { Container } from "react-bootstrap";
import Video from "./models/video";
import Watch from "./pages/Watch";

function App() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [currentVideo, setCurrentVideo] = useState<Video | null>(null);

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
      <Container className="">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/about" element={<About />} />
          <Route
            path="/sign-in"
            element={<SignIn SetUser={(e) => setCurrentUser(e)} />}
          />
          <Route path="/sign-out" element={<SignOut />} />
          <Route
            path="/upload-video"
            element={<UploadVideo user={currentUser} />}
          />
          <Route path="videos" element={<UserVideos user={currentUser} />} />
          <Route path="watch/:id" element={<Watch />} />
        </Routes>
      </Container>
    </Fragment>
  );
}

export default App;
