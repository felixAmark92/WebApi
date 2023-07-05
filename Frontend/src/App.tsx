import { Route, Routes } from "react-router-dom";
import About from "./pages/About";
import Home from "./pages/Home";
import { Fragment, useEffect, useState } from "react";
import Navigationbar from "./components/Navigationbar";
import SignIn from "./pages/SignIn";
import User from "./models/user";
import apiClient from "./services/apiClient";

function App() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  useEffect(() => {
    if (currentUser == null || currentUser == undefined) {
      apiClient
        .post("/User/TryGetSession")
        .then((response) => {
          setCurrentUser(response.data);
        })
        .catch((error) => console.error(error.response.data));
    }

    console.log("user is ", currentUser?.firstName);
  }, [currentUser]);

  return (
    <Fragment>
      <Navigationbar user={currentUser} />
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
