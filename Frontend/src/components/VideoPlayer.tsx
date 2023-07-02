import axios from "axios";
import React, { Fragment, useState, useEffect } from "react";
import { Button } from "react-bootstrap";

const VideoPlayer = () => {
  const [videoId, setVideoId] = useState(0);
  const [id, setId] = useState(0);

  return (
    <Fragment>
      <input type="number" onChange={(e) => setId(parseInt(e.target.value))} />

      <Button
        onClick={() => {
          setVideoId(id);
        }}
      ></Button>
      <p>{videoId}</p>

      <video width="420" height="240" controls className="ms-5">
        <source
          src={"https://localhost:7156/Video?Id=" + videoId}
          type="video/mp4"
        />
      </video>
    </Fragment>
  );
};

export default VideoPlayer;
