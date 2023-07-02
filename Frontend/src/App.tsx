import { useEffect } from "react";
import UploadForm from "./components/UploadForm";
import axios from "axios";
import VideoPlayer from "./components/VideoPlayer";

function App() {
  return (
    <>
      <div className="m-5">
        <UploadForm />
        <VideoPlayer></VideoPlayer>
      </div>
    </>
  );
}

export default App;
