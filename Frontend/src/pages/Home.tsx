import { Fragment, useRef } from "react";
import ReactPlayer from "react-player";
import MyVideoPlayer from "../components/MyVideoPlayer";

const Home = () => {
  return (
    <Fragment>
      <div className="container mt-3">
        <MyVideoPlayer />
      </div>
    </Fragment>
  );
};

export default Home;
