import React from 'react';
import Cookies from 'universal-cookie';
import {getProfile, getUserPosts} from '../middleware/BBVAPI';
import CreatePostForm from '../components/CreatePostForm';
import {Container} from 'react-bootstrap';
import Post from '../components/Post';
import { PROFILE_DEFUALT } from '../middleware/UIDefaults';
import AppNav from '../components/AppNav';

class Profile extends React.Component {
    static displayName = Profile.name;

    constructor(props){
        super(props);

        this.state = {
            posts: [],
            updatetl: false,
            updateProfile: false,
            user: window.location.pathname.substring(9),
            profile: null,
            page: 1
        };
    }

    storeUser(){
        try{
            var userName = window.location.pathname.substring(9);
        }catch(ex){
            let cookies = new Cookies();

            userName = cookies.get('user')['UserName'];
        }
      console.log(userName);
    }

    profileUpdate()
    {
        this.setState({updateProfile: true});
    }

    async GetProfile(){
        let cookie = new Cookies();

        try{
            var response = await getProfile(cookie.get('token') , cookie.get('refreshToken'), this.state.user);
            if(await response == null){
                //send user to error page
            } else {
                //if (await response['responseMessage'] != 'Success'.toUpperCase()){
                    console.log(await response[0]);
                    //cookie.remove('token');
                    //window.location.reload();
                //} else {
                   await this.setState({profile: JSON.parse(response[0])});
                    this.GetPosts();
                //}
            }
        }catch(ex){

        } 
    }

    async GetPosts() {
        let cookie = new Cookies();
        try {
           var response = await getUserPosts(cookie.get('token'), cookie.get('refreshToken'), this.state.page, this.state.user);

            if (typeof (await response) == typeof ('')) {
                //return error to timeline
            } else {
                if (await response['responseMessage'] != 'Success'.toUpperCase()) {
                    console.log(await response['responseMessage']);
                    //cookie.remove('token');
                    //window.location.reload();
                } else {
                    const postList = await JSON.parse(response[0]).map(post => {
                        var parsedCExt = post.CommentsExt;
                        //console.log(parsedCExt);
                        var user = cookie.get('user');

                        var isOwner = false;
                        if (post.Username == user['UserName']) {
                            isOwner = true;
                        }

                       
                        return (
                            <Post key={post.PostId} PostId={post.PostId} Username={post.Username} CreateDateTime={post.CreateDateTime} Body={post.Body} Attachments={post.Attachments} Likes={post.Likes} IsLiked={post.LikeStatus} Comments={post.Comments} CExt={JSON.parse(post.CommentsExt)} isOwner={isOwner} GetTL={() => this.GetTL()} />
                        )
                    })
                    this.setState({ posts: postList })
                    //console.log(postList);
                }
            }
        } catch (Ex) {
            console.log(Ex);
        }
    }

    infiniteScroll = () => {
        // End of the document reached?
        if (window.innerHeight + document.documentElement.scrollTop
        === document.documentElement.offsetHeight){
         
           let newPage = this.state.page;
           newPage++;
            this.setState({
                 page: newPage
            });
           this.GetPosts();
           }
}

    componentDidMount(){
        this.GetProfile();
        window.addEventListener('scroll', this.infiniteScroll)
    }

    componentDidUpdate(){
        
        if(this.state.updateProfile){
            this.GetProfile();
            this.setState({updateProfile: false});
        }
        
    }

    render(){
        if (this.state.profile != null) {
            return (
                <div>
                    <AppNav />
                <Container style={{ textAlign: "left" }}>
                    <h1>{this.state.profile["UserName"]}</h1>
                    <img src={this.state.profile.ProfilePictureURL} />
                    <CreatePostForm tlUpdate={this.profileUpdate} userName={this.state.profile.UserName} profile={this.state.profile ? this.state.profile : PROFILE_DEFUALT} />
                    <button>{this.state.profile.Relationship}</button>
                </Container>
                </div>
            );
        }
        return (
            <div><p>Loading...</p></div>
            )
    }
}

export default Profile;