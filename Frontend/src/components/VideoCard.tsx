import Button from "react-bootstrap/Button";
import Card from "react-bootstrap/Card";
import Video from "../models/video";

interface Props {
  video: Video;
}

const VideoCard = ({ video }: Props) => {
  return (
    <Card>
      <Card.Img variant="top" src="holder.js/100px160" />
      <Card.Body>
        <Card.Title>{video.description}</Card.Title>
        <Card.Text>{video.dateTime}</Card.Text>
      </Card.Body>
    </Card>
  );
};

export default VideoCard;
