import React, { Fragment, useState } from "react";
import { Button } from "react-bootstrap";

const VideoPlayer = () => {
  const [videoId, setVideoId] = useState(0);
  const [id, setId] = useState(0);

  const videoSource = `https://localhost:7156/Video?Id=${videoId}`;
  return (
    <Fragment>
      <input type="number" onChange={(e) => setId(parseInt(e.target.value))} />

      <Button onClick={() => setVideoId(id)}>Select</Button>
      <p>{videoId}</p>
      <div className="d-flex justify-content-center">
        <video key={videoSource} width="auto" controls autoPlay className="">
          <source src={videoSource} type="video/mp4" />
        </video>
      </div>
    </Fragment>
  );
};

export default VideoPlayer;
