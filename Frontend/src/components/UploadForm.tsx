import React, { ChangeEvent, Fragment, useState } from "react";
import axios from "axios";
import { ProgressBar } from "react-bootstrap";

const UploadForm: React.FC = () => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [textDescription, setTextDescription] = useState("");
  const [progress, SetProgress] = useState(0);

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] || null;
    setSelectedFile(file);
  };

  const handleDescriptionChange = (event: ChangeEvent<HTMLInputElement>) => {
    const description = event.target.value;
    setTextDescription(description);
  };

  const handleUpload = async () => {
    if (!selectedFile || !textDescription) return;

    const formData = new FormData();

    formData.append("file", selectedFile);
    formData.append("description", textDescription);

    console.log(formData.get("file"));
    console.log(formData.get("description"));

    await axios
      .post("https://localhost:7156/Upload", formData, {
        onUploadProgress: (progressEvent) => {
          if (progressEvent.total !== undefined) {
            SetProgress(
              Math.round((progressEvent.loaded / progressEvent.total) * 100)
            );
          }
        },
      })
      .then((response) => {
        console.log(response.data);
      })
      .catch((error) => {
        console.error(error);
        console.error(error.response.data.errors[""][0]);
      });
  };

  return (
    <Fragment>
      <div className="upload-form">
        <div className="mb-3">
          <label htmlFor="file-upload" className="btn btn-secondary">
            Select file
          </label>
          <input
            id="file-upload"
            hidden
            type="file"
            onChange={handleFileChange}
          />
          <label className="upload-label">{selectedFile?.name}</label>
        </div>
        <label className="form-text" htmlFor="description">
          Description
        </label>
        <input
          id="description"
          className="form-control mb-4"
          type="text"
          onChange={handleDescriptionChange}
        />
        <button
          className="btn btn-primary mb-4 btn-upload"
          onClick={handleUpload}
        >
          Upload
        </button>
        <ProgressBar
          className="my-progress-bar"
          now={progress}
          min={0}
          max={100}
        />
      </div>
      {/* TODO: cancel button appearing when downloading is in proccess  */}
    </Fragment>
  );
};

export default UploadForm;
