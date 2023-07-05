import { Fragment } from "react";
import VideoPlayer from "../components/VideoPlayer";

const Home = () => {
  return (
    <Fragment>
      <div className="container mt-3">
        <VideoPlayer></VideoPlayer>
      </div>
    </Fragment>
  );
};

export default Home;
