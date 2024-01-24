import React, {useEffect, useRef} from 'react'
import {Container, Row, Col} from 'react-bootstrap';
import { useDispatch, useSelector } from 'react-redux';
import { Button } from 'react-bootstrap';
import Cookies from 'universal-cookie';
import Sidebar from '../components/Sidebar';
import Timeline from '../components/Timeline';
import AppNav from '../components/AppNav';
import { USER_TOKEN } from '../middleware/types';
import GroupView from '../components/GroupView';
import FriendsView from '../components/FriendsView';
import ActivityView from '../components/ActivityView';
import StoryView from '../components/StoryView';
import CreatePostForm from '../components/CreatePostForm';
import { retrieveUserData } from '../app/features/UserSlice';
import { logOut, logUserOut, provideAuth, provideUserRefreshToken, provideUserToken } from '../app/features/AuthSlice';
import { PROFILE_DEFUALT } from '../middleware/UIDefaults';



/*
                *   -AppNav
                *   -(Col) Sidebar
                *   - (Col) TL(Feed)
                *   -(Col) Extra + Ads
                */


export default function Home(props) {
   const t = useSelector((state) => state.auth.token);
   const rt = useSelector((state) => state.auth.refreshToken);
   
    const isInitialMount = useRef(true);

    const dispatch = useDispatch();

    const userName = useSelector(state => state.user.username);
    const profile =  useSelector(state => state.user.profile);

    //Retrieve UserInfo
    useEffect(() => {
        if(isInitialMount.current)
        {
            const fetchUserData = async () => {
                const result = await dispatch(retrieveUserData({token: t, refreshToken: rt}));
              }
           
              fetchUserData();
              

            isInitialMount.current = false;
        }else {
        if (document.getElementById("errors") != null) {
          
        }
    }
    });

    

    return (
        <div>
        <AppNav navBG="light" userName={userName} profile={profile ? profile : PROFILE_DEFUALT}/>
        <Container fluid>
            <Row>
                <Col lg={3}>
                    <GroupView />
                    <FriendsView token={t} userName={userName}/>
                </Col>
                <Col lg={6}>
                    <StoryView />
                    <Timeline token={t} rtoken={rt} userName={userName}/>
                </Col>
                <Col lg={3}>
                    <CreatePostForm userName={userName} profile={profile ? profile : PROFILE_DEFUALT}/>
                     <ActivityView />
                </Col>
             </Row>
            </Container>
            </div>
        )
}; 
