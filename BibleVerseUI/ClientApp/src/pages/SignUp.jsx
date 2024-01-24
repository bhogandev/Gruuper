import React from 'react';
import {Container, Row, Col} from 'react-bootstrap';
import { Button, Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import Cookies from 'universal-cookie';
import SignUpForm from '../components/SignUpForm';
import {getParameterByName} from '../js/browserMethods';
import bbvapi from '../middleware/BBVAPI';
import { SUCCESS, TOKEN } from '../middleware/types';
import { useHistory } from 'react-router';


const SignUp = (props) => {
    
    var h = useHistory();

 return (
    <div>
    {/* This is where the Nav will go */}
    <Navbar bg="dark" expand="md" variant="dark">
        <Navbar.Brand>Grouper</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
    <Nav className="ml-auto">
        <Nav.Link>
            what?
        </Nav.Link>
        <Nav.Link>
            who?
        </Nav.Link>
        <Nav.Link>
            when?
        </Nav.Link>
        <Nav.Link>
            where?
        </Nav.Link>
    </Nav>
    </Navbar>
    <div style={{height:"100vh", width: "100vw"}}>
        {/* This is where the login form will go*/}
        <Row>
            <Col>
                <img style={{height:"100vh", width:"100%"}} src={require("../assets/imgs/lanphoman.jpg")}/>
            </Col>
            <Col style={{marginTop: "15vh"}}>
                <div style={{marginBottom: "20vh", textAlign:"center"}}>
                    this is where my logo or something like that will go 
                </div>
                <Container>
                    <SignUpForm history={h}/>
                </Container>
            </Col>
        </Row>
    </div>
    <Nav className="ml-auto">
        <Nav.Link>Contact</Nav.Link>
        <Nav.Link>Contact</Nav.Link>
        <Nav.Link>Contact</Nav.Link>
        <Nav.Link>Contact</Nav.Link>
        <Nav.Link>Contact</Nav.Link>
        <Nav.Link>Contact</Nav.Link>
    </Nav>
</div>
 )

}

export default SignUp;