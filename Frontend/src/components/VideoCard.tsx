import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Video from "../models/video";
import { useEffect, useState } from "react";

interface Props {
  video: Video;
}

const VideoCard = ({ video }: Props) => {
  const [className, setClassName] = useState("video-card-thumbnail-wide");

  const handleImageLoad = (event: React.SyntheticEvent<HTMLImageElement>) => {
    const { naturalWidth, naturalHeight } = event.currentTarget;
    if (naturalWidth < naturalHeight) {
      setClassName("video-card-thumbnail-high");
    }
  };

  return (
    <Card className="video-card">
      <Card.Img
        className={"video-card-thumbnail " + className}
        variant="top"
        src={"https://localhost:7156/Video/thumbnail/" + video.fileName}
        onLoad={handleImageLoad}
      />
      <Card.Body>
        <Card.Title>{video.description}</Card.Title>
        <Card.Text>{video.dateTime}</Card.Text>
      </Card.Body>
    </Card>
  );
};

export default VideoCard;
