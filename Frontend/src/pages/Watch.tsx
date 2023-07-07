import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Video from "../models/video";
import apiClient from "../services/apiClient";

const Watch = () => {
  const { id } = useParams();
  const [video, setVideo] = useState<Video | null>(null);

  useEffect(() => {
    apiClient
      .get<Video>("/video/watch", {
        params: { id: id },
      })
      .then((respone) => {
        console.log(respone.data);
        setVideo(respone.data);
      })
      .catch((error) => {
        console.log(error.respone.data);
      });
  }, []);

  return (
    <div>
      <video
        controls
        autoPlay
        width={1200}
        height={800}
        src={`https://localhost:7156/Video/${video?.fileName}`}
      ></video>
    </div>
  );
};

export default Watch;
