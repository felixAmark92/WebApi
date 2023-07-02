import { useEffect } from "react";
import UploadForm from "./components/UploadForm";
import axios from "axios";

function App() {
  useEffect(() => {
    const getVideo = async () => {
      await axios
        .get(
          "https://localhost:7156/Video/407c0963-590f-417d-9fe9-2a633cad1ba3.mp4"
        )
        .then((response) => {
          console.log(response);
        })
        .catch((error) => {
          console.error(error);
        });
    };
    getVideo();
  }, []);

  return (
    <>
      <div className="m-5">
        <UploadForm />
      </div>
      <video width="420" height="240" controls className="ms-5">
        <source
          src="https://localhost:7156/Video/28a3b34e-7e14-4b80-9fa8-a0a3f451ff9f.mp4"
          type="video/mp4"
        />
      </video>
    </>
  );
}

export default App;
