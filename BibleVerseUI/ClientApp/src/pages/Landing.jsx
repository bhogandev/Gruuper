import React, { useState } from 'react';
import { Container, Col, Row, Button } from 'react-bootstrap';
import { useHistory } from 'react-router-dom';
import ImageCarousel from '../components/ImageCarousel';
import SignUpForm from '../components/SignUpForm';
import LoginForm from '../components/LoginForm';

const Landing = (props) => {
  const [showLoginForm, setShowLoginForm] = useState(true);
  const history = useHistory();

  const toggleForm = () => {
    setShowLoginForm(!showLoginForm);
  };

  return (
    <div>
      <div style={{ height: '100vh', width: '100vw', position: 'relative' }}>
        <div style={{ position: 'absolute', top: '0', right: '0'}}>
          <img src="" alt="logo" height="50px" style={{ marginRight: '10px' }} />
          <h3 style={{ display: 'inline-block' }}>Gruuper</h3>
        </div>
        <Row>
          <Col>
            <ImageCarousel />
          </Col>
          <Col style={{ marginTop: '10vh' }}>
            <div style={{ marginBottom: '8vh', textAlign: 'center', marginTop: '1vh' }}>
              <h1>{showLoginForm ? 'Welcome Back!' : 'Get Started'}</h1>
            </div>
            <Container>
              {showLoginForm ? (
                <LoginForm history={history} />
              ) : (
                <SignUpForm history={history} />
              )}
              <div style={{ marginTop: '1rem', textAlign: 'center' }}>
                <Button variant="link" onClick={toggleForm}>
                  {showLoginForm
                    ? "Don't have an account? Sign up now!"
                    : 'Already have an account? Log in here.'}
                </Button>
              </div>
            </Container>
          </Col>
        </Row>
      </div>
    </div>
  );
};

export default Landing;
