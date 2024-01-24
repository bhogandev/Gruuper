import React from 'react';
import { Nav, Navbar, NavDropdown, Button, Container, Form, Col, Row } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import SearchBar from './SearchBar';
import { bell, bookmark, compass, group, heart, house, message } from '../middleware/IconsStore';
import { logOut, logUserOut } from '../app/features/AuthSlice';
import { useDispatch, useSelector } from 'react-redux';
import Cookies from 'universal-cookie';
import { TOKEN } from '../middleware/types';
import { PROFILE_DEFUALT } from '../middleware/UIDefaults';

const AppNav = (props) => {
  const dispatch = useDispatch();
  const userName = useSelector(state => state.user.username);

  const defaultProfile = PROFILE_DEFUALT;
  const profile = useSelector(state => state.user.profile);

  async function handleLogout(){
    // Perform logout functionality
    console.log('Logged out');

    let cookies = new Cookies();

    const res = await dispatch(logUserOut({token: cookies.get(TOKEN)}));

    console.log(res);
  }

  function renderNav(){
    if(profile != null){
      return (
<Navbar className="nav" expand="md" variant={props.navBG}>
      <Container fluid>
        <Navbar.Brand href='/home'>
          Gruuper
        </Navbar.Brand>
        <Nav className="me-auto" justify>
          <Nav.Item className='nav-icon'>
            <Nav.Link href='/home'>
              <FontAwesomeIcon icon={house} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/explore">
              <FontAwesomeIcon icon={compass} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/gruups">
              <FontAwesomeIcon icon={group} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/gruupMessenger">
              <FontAwesomeIcon icon={message} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <SearchBar />
          </Nav.Item>
        </Nav>
        <Nav>
          <Nav.Item>
            <FontAwesomeIcon icon={bookmark} size="xl" />
          </Nav.Item>
          <Nav.Item>
            <FontAwesomeIcon icon={bell} size="xl" />
          </Nav.Item>
          <NavDropdown title={<img className='u_nav_profile_img' src={profile.Picture} style={{ height: '2.5rem', width: '2.5rem', borderRadius: '50%', marginRight: '1rem' }} />} id="basic-nav-dropdown">
            <NavDropdown.Item href={"/profile/" + userName}>{userName}</NavDropdown.Item>
            <NavDropdown.Divider />
            <NavDropdown.Item onClick={handleLogout}>Log out</NavDropdown.Item>
          </NavDropdown>
        </Nav>
      </Container>
    </Navbar>
      )
    }else {
      return (
      <Navbar className="nav" expand="md" variant={props.navBG}>
      <Container fluid>
        <Navbar.Brand href='/home'>
          Gruuper
        </Navbar.Brand>
        <Nav className="me-auto" justify>
          <Nav.Item className='nav-icon'>
            <Nav.Link href='/home'>
              <FontAwesomeIcon icon={house} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/explore">
              <FontAwesomeIcon icon={compass} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/gruups">
              <FontAwesomeIcon icon={group} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link href="/gruupMessenger">
              <FontAwesomeIcon icon={message} size="xl" />
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <SearchBar />
          </Nav.Item>
        </Nav>
        <Nav>
          <Nav.Item>
            <FontAwesomeIcon icon={bookmark} size="xl" />
          </Nav.Item>
          <Nav.Item>
            <FontAwesomeIcon icon={bell} size="xl" />
          </Nav.Item>
          <NavDropdown title={<img className='u_nav_profile_img' src={defaultProfile.Picture} style={{ height: '2.5rem', width: '2.5rem', borderRadius: '50%', marginRight: '1rem' }} />} id="basic-nav-dropdown">
            <NavDropdown.Item href={"/profile/" + userName}>{userName}</NavDropdown.Item>
            <NavDropdown.Divider />
            <NavDropdown.Item onClick={handleLogout}>Log out</NavDropdown.Item>
          </NavDropdown>
        </Nav>
      </Container>
    </Navbar>
      )
    }
  }

  return (
    renderNav()
  );
}

export default AppNav;
