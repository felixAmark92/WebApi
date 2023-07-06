import Container from "react-bootstrap/Container";
import Nav from "react-bootstrap/Nav";
import Navbar from "react-bootstrap/Navbar";
import { Link } from "react-router-dom";
import User from "../models/user";
import apiClient from "../services/apiClient";
import { NavItem } from "react-bootstrap";
import { Fragment } from "react";

interface Props {
  user: User | null;
  signOut: () => void;
}

function Navigationbar({ user, signOut }: Props) {
  const terminateSession = () => {
    apiClient
      .post("/User/EndSession", undefined, { withCredentials: true })
      .then((response) => {
        console.log(response.data);
      })
      .catch((error) => console.error(error.response.data));
  };

  return (
    <Navbar bg="dark" data-bs-theme="dark">
      <Container>
        <Link className="navbar-brand" to="/">
          Navbar
        </Link>
        <Nav className="me-auto">
          <Link className="nav-link" to="/">
            Home
          </Link>
          <Link className="nav-link" to="#features">
            Features
          </Link>
          <Link className="nav-link" to="#pricing">
            Pricing
          </Link>

          {user !== null && (
            <Link className="nav-link" to="/upload-video">
              Upload
            </Link>
          )}

          <Link className="nav-link" to="about">
            About
          </Link>
        </Nav>
        <Nav>
          {user == null ? (
            <Link className="nav-link" to="/sign-in">
              Sign in
            </Link>
          ) : (
            <Fragment>
              <NavItem className="nav-link nav-user">
                {user.firstName + " " + user.lastName}
              </NavItem>
              <Link
                onClick={() => {
                  terminateSession();
                  signOut();
                }}
                className="nav-link"
                to="/sign-out"
              >
                Sign out
              </Link>
            </Fragment>
          )}
        </Nav>
      </Container>
    </Navbar>
  );
}

export default Navigationbar;
