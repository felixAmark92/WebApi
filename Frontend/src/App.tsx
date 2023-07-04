import { useEffect } from "react";
import UploadForm from "./components/UploadForm";
import axios from "axios";
import VideoPlayer from "./components/VideoPlayer";
import SignInForm from "./components/SignInForm";

function App() {
  // Set a cookie
  axios
    .get("http://localhost:5173/", {
      headers: {
        Cookie: "something",
      },
    })
    .then(() => {
      console.log("Cookie set successfully!");
    })
    .catch((error) => {
      console.error("Error setting cookie:", error);
    });

  // Read a cookie
  axios
    .get("http://localhost:5173/", {
      headers: {
        Cookie: "something",
      },
    })
    .then((response) => {
      console.log("Cookie value:", response.headers["set-cookie"]);
    })
    .catch((error) => {
      console.error("Error reading cookie:", error);
    });
  return (
    <>
      <div className="m-5">
        <SignInForm></SignInForm>
        <UploadForm />
        <VideoPlayer></VideoPlayer>
      </div>
    </>
  );
}

export default App;
