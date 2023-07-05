import { Route, Routes } from "react-router-dom";
import About from "./pages/About";
import Home from "./pages/Home";
import { Fragment, useEffect, useState } from "react";
import Navigationbar from "./components/Navigationbar";
import SignIn from "./pages/SignIn";
import User from "./models/user";
import apiClient from "./services/apiClient";
import userService from "./services/userService";

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
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/about" element={<About />} />
        <Route
          path="/sign-in"
          element={<SignIn SetUser={(e) => setCurrentUser(e)} />}
        ></Route>
      </Routes>
    </Fragment>
  );
}

export default App;
