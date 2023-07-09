import React, { useEffect, useRef } from "react";
import ReactPlayer from "react-player";
import Hls from "hls.js";

const MyVideoPlayer = () => {
  const playerRef = useRef<ReactPlayer>(null);

  useEffect(() => {
    const videoElement =
      playerRef.current?.getInternalPlayer()?.player?.current;
    if (videoElement && Hls.isSupported()) {
      const hls = new Hls();
      hls.loadSource(
        "https://localhost:7156/Video/53b82d4a-3c5f-4faa-8e0c-ba7cc98935c1%5Cmaster.m3u8"
      );
      hls.attachMedia(videoElement);
    }
  }, []);

  return (
    <ReactPlayer
      ref={playerRef}
      url="https://localhost:7156/Video/53b82d4a-3c5f-4faa-8e0c-ba7cc98935c1%5Cmaster.m3u8"
      controls
      width="640px"
      height="360px"
    />
  );
};

export default MyVideoPlayer;
