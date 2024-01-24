import React from 'react';
import Layout from './components/Layout';
import Home from './pages/Home';
import { Route } from 'react-router';
import { Switch } from 'react-router-dom';
import './css/App.css';
import Landing from './pages/Landing';
import ProtectedRoute from './components/ProtectedRoute';
import Profile from './pages/Profile';
import ConfirmEmail from './pages/ConfirmEmail';
import Chat from './pages/Chat';
import SignUp from './pages/SignUp';
import { store } from './app/store';


/*
*   <Switch>   
*       <Route to Homepage aka landing page />
*       <Route to Profile Page />
*   </Switch>
*/



const App = (props) => {
    return (
        <Switch>
            <Route path="/" exact component={Landing} />
            <Route path="/ConfirmEmail" component={ConfirmEmail}/>
            <Route path="/SignUp" component={SignUp} />
            {/* Routes below should be authenticated routes only (involve some type of auth middleware)*/}
            <ProtectedRoute path="/home" component={Home} />
            <ProtectedRoute path="/profile" component={Profile} />
            <ProtectedRoute path="/Messenger" component={Chat} />
            {/* Routes above should be authenticated routes only (involve some type of auth middleware)*/}
        </Switch>
        )
}

export default App;