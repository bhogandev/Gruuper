import axios from 'axios';
import Cookies from 'universal-cookie';
import {determineDevice, browser} from '../js/browserMethods';
import {SUCCESS, FAILURE, CONFLICT, CONFLICT_CODE, RESPONSE_ERRORS, SUCCESS_CODE, BADREQUEST_CODE} from './types';
import {DEFAULT_ERROR} from './errors';
import {APIBASE} from './Constants';
import { useDispatch } from 'react-redux';
import { setCredentials } from '../app/features/AuthSlice';


    //Summary - Log in user to application
    async function login(e, p) {
        const cookies = new Cookies();

        try {
            let res = await (await fetch(APIBASE + "Login/LoginUser", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    Email: e,
                    Password: p
                })
            }))

            //Verify fetch completed successfully

            var resAction = {
                responseCode: null,
                payload: null
            };

            if (res && res.status == SUCCESS_CODE) {
                var result = await JSON.parse(await res.json());
                
                resAction.responseCode = SUCCESS_CODE;
                resAction.payload = result;
                
                /*
                //Set JWT cookie in browser cookie storage
                cookies.set('token', (result["Token"]));
                //Set Refresh cookie in browser cookie storage
                cookies.set('refreshToken', result['RefreshToken']);
                cookies.set('user', result['User']);
*/

                //Redirect to app dashboard
                return resAction;

            } else if (res && res.status == CONFLICT_CODE) {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                resAction.responseCode = CONFLICT_CODE;
                resAction.payload = result;

                return resAction;
            } else {
                //find a way to return errrors and loading state
                
                resAction.responseCode = BADREQUEST_CODE;
                resAction.payload = DEFAULT_ERROR;

                //this.setState({ errors: defaultError, loading: false });
                return resAction;
            }
        } catch (exception) {
            //find a way to return errrors and loading state
            //console.log(exception);
            var defaultError = [{
                "Description": exception
            }]
            //this.setState({ errors: defaultError, loading: false });

            resAction.responseCode = BADREQUEST_CODE;
            resAction.payload = defaultError;

            return defaultError;
        }

    }

    async function signUp(e, u, p, ref, dob, phone){
        const cookies = new Cookies();

        try {

            let res = await (await fetch(APIBASE + "Registration/CreateUser", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    UserName: u,
                    Password: p,
                    Email: e,
                    RefCode: ref,
                    PhoneNumber: phone,
                    DOB: dob
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {

                //Redirect to confirm email page dashboard
                return SUCCESS;

            } else if ( res != null &&  res.status == CONFLICT_CODE) {
                var result = await res.json();

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });
                await console.log(await result[RESPONSE_ERRORS]);
                await console.log(await typeof(result[RESPONSE_ERRORS]));
                return await result[RESPONSE_ERRORS];
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    DEFAULT_ERROR
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }
        } catch (exception) {
            //find a way to return errrors and loading state
            //console.log(exception);
            var defaultError = [{
                "Description": exception
            }]
            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

    }

    //Summary - Validate the refresh token so user can continue action
    async function validateRefreshToken(t, rt) {
        const cookies = new Cookies();

        try {

            let res = await (await fetch(APIBASE + "Login/RefreshToken", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + t,
                    'Content-Type': 'application/json',
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                   AccessToken: t,
                   RefreshToken: rt
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                var result = await JSON.parse(await res.json());

                //console.log(result);

                //Set JWT cookie in browser cookie storage
                cookies.set('token', (result["AccessToken"]));
                //Set Refresh cookie in browser cookie storage
                cookies.set('refreshToken', result['RefreshTokens'][0]['Token']);
                cookies.set('user', result);

                //Redirect to app dashboard
                return true;

            } else if (res && res.status == CONFLICT_CODE) {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                return result["ResponseErrors"];
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    DEFAULT_ERROR
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }
        } catch (exception) {
            //find a way to return errrors and loading state
            //console.log(exception);
            var defaultError = [{
                "Description": exception
            }]
            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }
    }

    //Summary - Log out user from application
    async function logout(t) {
        const cookies = new Cookies();
        try {
            let res = await (await fetch(APIBASE + "Logout/LogoutUser", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString(),
                    'Authorization': `Bearer ${cookies.get('token')}`,
                    'Token': t
                },
                credentials: 'same-origin'
            }));
        
            // Verify fetch completed successfully
        
            if (res && res.status === SUCCESS_CODE) {
                // Remove cookies and redirect to login page
                cookies.remove('token');
                cookies.remove('refreshToken');
                cookies.remove('user');
                window.location.href = '/';
            } else {
                console.log('Error: Logout failed');
            }
        } catch (exception) {
            console.log(exception);
        }
        
    }
//TIMELINE CALLS

    //Summary - Get users timemline
    async function getUserTimeline(t, rt, page) {

        const cookies = new Cookies();
        var resOk = false;

        try {
            let res = await (await fetch(APIBASE + "Post/GetTimeline?page=" + page, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + t,
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt,
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            //await console.log(await res.json());

            if (await res != null && await res.ok) {
                
                var result = await res.json();
                return result;

            } else if (res && res.status == CONFLICT_CODE) {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            } else {
            //find a way to return errrors and loading state
            var defaultError = [{
                DEFAULT_ERROR
            }]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    //Summary - Get a user's posts to populate timeline or profile
    async function getUserPosts(t, rt, page, username){
        const cookies = new Cookies();
        var resOk = false;

        try {
            let res = await (await fetch(APIBASE + "Post/GetUserPosts?username=" + username + "&page=" + page, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + cookies.get('token'),
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt,
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            //await console.log(await res.json());

            if (await res != null && await res.ok) {
                
                var result = await res.json();
                //console.log(JSON.parse(await result));
                return JSON.parse(await result);

            } else if (res && res.status == CONFLICT_CODE) {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            } else {
            //find a way to return errrors and loading state
            var defaultError = [{
                DEFAULT_ERROR
            }]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    async function getUserInfo(t, rt)
    {
        const cookies = new Cookies();
        var resOk = false;
        var resAction = {
            responseCode: null,
            payload: null
        };

        try {
            let res = await (await fetch(APIBASE + "User/GetUserInfo", {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + t,
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt,
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            if (await res != null && await res.ok) {

                var result = await res.json();

                resAction.responseCode = SUCCESS_CODE;
                resAction.payload = result;
                
                return await resAction;

            } else if (res && res.status == CONFLICT_CODE) {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            }
             else if (res && res.status == BADREQUEST_CODE) {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            }
             else {
            //find a way to return errrors and loading state
            var defaultError = [
                DEFAULT_ERROR
            ]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    async function getFriendList(token, userId) {
        const cookies = new Cookies();
        const headers = {
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
            'Token': token,
            'Device': 'Web',
            'Identity': determineDevice(),
            'Version': browser.version.toString()
        };
    
        try {
            const response = await fetch(`${APIBASE}User/GetFriends?username=${userId}`, {
                method: 'GET',
                headers: headers,
                validateStatus: () => true,
                credentials: 'same-origin'
            });
    
            if (response.ok) {
                const result = await response.json();
                return result;
            } else {
                const error = await response.json();
                throw new Error(error.message).message;
            }
        } catch (error) {
            throw new Error('An error occurred while retrieving the friend list.').message;
        }
    }
    

    //Summary - Get a user's profile information to render profile path
    async function getProfile(t, rt, username){
        
        const cookies = new Cookies();
        var resOk = false;

        try {
            let res = await (await fetch(APIBASE + "User/GetProfile?username=" + username, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + cookies.get('token'),
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt,
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            //await console.log(await res.json());

            if (await res != null && await res.ok) {
                
                var result = await res.json();
                //console.log(JSON.parse(await result));
                return JSON.parse(await result);

            } else if (res && res.status == CONFLICT_CODE) {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            } else {
            //find a way to return errrors and loading state
            var defaultError = [
                DEFAULT_ERROR
            ]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    //Summary - Query/Search Gruuper Site. Different filters can be applied to narrow search results
    async function query(range, val) {
        const cookies = new Cookies();

        try {
            let res = await (await fetch(APIBASE + "User/Query?query=" + val + "&qFilter=" + range, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + cookies.get('token'),
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token'),
                    'qFilter': range,
                    'Query': val,
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                credentials: 'same-origin'
            }));

            if (res && res.ok) {

                var result = await res.json();
                return JSON.parse(JSON.parse(result));

            } else if (res && res.status == CONFLICT_CODE) {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                throw new Error("an error occured");
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    DEFAULT_ERROR
                }]

                //this.setState({ errors: defaultError, loading: false });
                throw new Error(DEFAULT_ERROR.Description);
            }

        } catch (ex) {
            console.log(ex);
        }
    }

    //Summary - Send request to confirm user via email confirmation
    async function confirmUser(id, token){
        const cookies = new Cookies();

        try {
            let res = await (await fetch(APIBASE + "Registration/ConfirmEmail?id=" + id + "&token=" + token, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Device': "Web",
                    'Identity': determineDevice(),
                    'Version': browser.version.toString()
                },
                credentials: 'same-origin'
            }));

            if (res && res.ok) {

                return SUCCESS;

            } else if (res && res.status == CONFLICT_CODE) {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });
                //console.log(await result[0]);
                return result;
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    DEFAULT_ERROR
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }

        } catch (ex) {

        }
    }

//POST API CALLS

    //Summary - Interact with user post likes (like or unlike post)
    async function interactWithPostLikes(t, parentId, IsLiked)
    {
        const cookies = new Cookies();
        
        try {

            let res = await (await fetch(APIBASE + "Post/Interact", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + cookies.get('token'),
                    'Content-Type': 'application/json',
                    'Token': t,
                    'IntType': IsLiked
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    ParentId: parentId,
                    LikeType: IsLiked
                })
            }))

            //Verify fetch completed successfully
            var result = await JSON.parse(await res.json());
            if (res && res.ok) {
                return SUCCESS
            } else if (res && res.status == CONFLICT_CODE) {
                return CONFLICT
            } else {
                return FAILURE;
            }
        } catch (exception) {
            //Need to return failure result here
            return FAILURE;
        }

    }

    //Summary - Delete own post
    async function deletePost(t, rt, postId){
        try {

            let res = await (await fetch(APIBASE + "Post/DeletePost", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Authorization':'Bearer ' + t,
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    PostId: postId,
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                return SUCCESS;
            } else if (res && res.status == CONFLICT_CODE) {
                //Handle Error
                return FAILURE;
            } else {
                //General Error Message
            }
        } catch (exception) {
            //Do something with exception
            //console.log(exception);
        }
    }

export { login, signUp, validateRefreshToken, logout, getUserTimeline, getUserPosts, getUserInfo, getProfile, query, confirmUser, interactWithPostLikes, deletePost, getFriendList };