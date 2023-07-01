import React, { ChangeEvent, useState } from "react";
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

    console.log(formData.get("file"));
    console.log(formData.get("description"));

    await axios
      .post("https://localhost:7181/Upload", formData, {
        onUploadProgress: (progressEvent) => {
          if (progressEvent.total !== undefined) {
            SetProgress(
              Math.round((progressEvent.loaded / progressEvent.total) * 100)
            );
            // Update the progress bar value or perform any other UI updates
            console.log(`Upload Progress: ${progress}%`);
          }
        },
      })
      .then((response) => {
        console.log(response.data);
      })
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <div className="mb-3">
      <label htmlFor="file-upload" className="btn btn-primary">
        Select file
      </label>
      <input id="file-upload" hidden type="file" onChange={handleFileChange} />

      <label className="form-text" htmlFor="description">
        Description
      </label>
      <input
        id="description"
        className="form-control"
        type="text"
        onChange={handleDescriptionChange}
      />
      <button className="btn btn-primary" onClick={handleUpload}>
        Upload
      </button>
      <ProgressBar className="" min={0} max={100} now={progress}></ProgressBar>
      <img src="" alt="" />
    </div>
  );
};

export default UploadForm;
