import { useEffect } from "react";
import UploadForm from "./components/UploadForm";
import axios from "axios";
import VideoPlayer from "./components/VideoPlayer";
import SignInForm from "./components/SignInForm";

function App() {
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
