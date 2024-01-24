import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import axios from 'axios';
import bbvapi, { getUserTimeline } from '../middleware/BBVAPI';
import Cookie from 'universal-cookie';
import Post from './Post';
import CreatePostForm from './CreatePostForm';
import { Container } from 'react-bootstrap'
import BBVAPI from '../middleware/BBVAPI';
import { REFRESH_TOKEN, RESPONSE_BODY, RESPONSE_MESSAGE, SUCCESS, TOKEN, USERNAME_KEY, USER_TOKEN } from '../middleware/types';
import { useSelector } from 'react-redux';
import { DEFAULT_ERROR } from '../middleware/errors';

export default function Timeline({ token, rtoken, userName }) {
    const [posts, setPosts] = useState([]);
    const [updateTimeline, setUpdateTimeline] = useState(false);
    const [postPage, setPostPage] = useState(1);
    const [errors, setErrors] = useState('');

    const isInitialMount = useRef(true);

    const GetTL = useCallback(async () => {
        let cookie = new Cookie();

        try {
            const response = await getUserTimeline(token, rtoken, postPage);

            if (response[RESPONSE_MESSAGE] !== SUCCESS) {
                //return error to timeline
                setErrors(response[0]['DEFAULT_ERROR'].Description);
            } else {
                const jsonResponse = JSON.parse(response[RESPONSE_BODY]);
                const postList = jsonResponse.map(post => {
                    const parsedCExt = post.CommentsExt;
                    const isOwner = post.Username === userName;
                   
                    return (
                        <Post key={post.PostId} PostId={post.PostId} Username={post.Username} CreateDateTime={post.CreateDateTime} Body={post.Body} Attachments={post.Attachments} Likes={post.Likes} IsLiked={post.LikeStatus} Comments={post.Comments} CExt={JSON.parse(post.CommentsExt)} isOwner={isOwner} GetTL={GetTL} />
                    )
                })
                setPosts(postList);
            }
        } catch (ex) {
            console.log(ex);
        }
    }, [token, rtoken, postPage, userName]);

    useEffect(() => {
        if (isInitialMount.current) {
            GetTL();
            isInitialMount.current = false;
        } else {
            const errors = document.getElementById("errors");
            if (errors) {
              
            }
        }
    }, [GetTL]);

    const postList = useMemo(() => posts, [posts]);

    if (postList && postList.length > 0) {
        return (
            <Container

style={{ textAlign: "left" }}>
<h1>{postList}</h1>
</Container>
);
}
return (
<div>
    <div>{errors}</div>
    <p>Loading...</p>
</div>
);
}