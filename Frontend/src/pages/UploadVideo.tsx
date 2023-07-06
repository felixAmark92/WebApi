import React from "react";
import UploadForm from "../components/UploadForm";
import User from "../models/user";

interface Props {
  user: User | null;
}

const UploadVideo = ({ user }: Props) => {
  return (
    <>
      {user !== null ? (
        <UploadForm userId={user.id} />
      ) : (
        <h1>You need to be logged in to upload videos</h1>
      )}
    </>
  );
};

export default UploadVideo;
