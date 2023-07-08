import { useEffect, useState } from "react";
import User from "../models/user";
import Video from "../models/video";
import apiClient from "../services/apiClient";
import { Link } from "react-router-dom";
import VideoCard from "../components/VideoCard";
import { Col, Container, Row } from "react-bootstrap";

interface Props {
  user: User | null;
}

const UserVideos = ({ user }: Props) => {
  const [uservideos, setUserVideos] = useState<Video[]>([]);

  useEffect(() => {
    if (user !== null) {
      apiClient
        .post<Video[]>("/Video", undefined, {
          params: { uploaderId: user.id },
        })
        .then((respone) => {
          console.log(respone.data);
          setUserVideos(respone.data);
        })
        .catch((error) => {
          console.log(error.respone.data);
        });
    }
  }, [user]);

  return (
    <Container className="video-grid">
      {uservideos.map((video) => (
        <Link
          style={{ textDecoration: "none" }}
          key={video.id}
          to={"../watch/" + video.id}
        >
          <VideoCard video={video} />
        </Link>
      ))}
    </Container>
  );
};

export default UserVideos;
