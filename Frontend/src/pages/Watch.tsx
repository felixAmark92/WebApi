import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Video from "../models/video";
import apiClient from "../services/apiClient";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";

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
    <div
      style={{ display: "flex" }}
      className="justify-content-center justify-items-center pt-5"
    >
      <video
        controls
        autoPlay
        height={600}
        src={`https://localhost:7156/Video/${video?.fileName}`}
      ></video>
    </div>
  );
};

export default Watch;
