import React, { useState } from 'react';
import { Button, Form, FormGroup, Input, CustomInput } from 'reactstrap';
import Cookies from 'universal-cookie';
import { CONFLICT_CODE, REFRESH_TOKEN, RESPONSE_ERRORS, TOKEN } from '../middleware/types';
import { DEFAULT_ERROR } from '../middleware/errors';
import { Row, Col } from 'react-bootstrap';
import { APIBASE } from '../middleware/Constants';

const CreatePostForm = (props) => {
  const [post, setPost] = useState('');
  const [photos, setPhotos] = useState([]);
  const [videos, setVideos] = useState([]);
  const [errors, setErrors] = useState([]);
  const [btnDisabled, setBtnDisable] = useState(false);

  async function uploadPost() {
    const cookies = new Cookies();
    setBtnDisable(false);

    try {
      let res = await (await fetch(APIBASE+"Post/CreatePost", {
        method: 'post',
        headers: {
          'Accept': 'application/json',
          'Authorization':'Bearer ' + cookies.get(TOKEN),
          'Content-Type': 'application/json',
          'Token': cookies.get(TOKEN),
          'RefreshToken': cookies.get(REFRESH_TOKEN)
        },
        credentials: 'same-origin',
        body: JSON.stringify({
          Body: post,
          Images: photos,
          Videos: videos
        })
      }));

      if (res && res.ok) {
        setPost('');
        props.tlUpdate();
      } else if (res && res.status == CONFLICT_CODE) {
        var result = await JSON.parse(await res.json());
        setErrors(result[RESPONSE_ERRORS]);
      } else {
        var defaultError = [{
          DEFAULT_ERROR
        }];
        setErrors(DEFAULT_ERROR);
      }
    } catch (exception) {
      // console.log(exception);
    }
  }

  function addImage() {
    // code to handle adding image
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true;
    input.accept = 'image/*';
    input.onchange = (event) => {
      const files = event.target.files;
      const photosArray = Array.from(files);
      setPhotos(photosArray);
    };
    input.click();
  }

  function addVideo() {
    // code to handle adding video
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true;
    input.accept = 'video/*';
    input.onchange = (event) => {
      const files = event.target.files;
      const videosArray = Array.from(files);
      setVideos(videosArray);
    };
    input.click();
  }

  return (
    <div className='create-post-container'>
      <Row>
        <img className='u_post_cover' src='#' alt='Cover' />
      </Row>
      <Row>
        <Col>
          <img className='u_post_profile' src={props.profile.Picture ? props.profile.Picture != "" : ""} alt='Profile' />
        </Col>
        <Col>
          <Row><h3>{props.userName}</h3></Row>
          <Row>{props.profile.Description}</Row>
        </Col>
      </Row>
      <Row>
        <Col>
          <Row>{props.profile.Followers}</Row>
          <Row>Followers</Row>
        </Col>
        <Col>
          <Row>{props.profile.Following}</Row>
          <Row>Following</Row>
        </Col>
      </Row>
      <Row>
        <Form>
        <FormGroup>
        <Input type='textarea' name='post' id='post' placeholder='What are you thinking?' value={post} onChange={(e) => setPost(e.target.value)} />
      </FormGroup>
      <FormGroup>
        <Button color='primary' disabled={btnDisabled} onClick={() => uploadPost()}>
          Post
        </Button>
      </FormGroup>
    </Form>
  </Row>
  <Row>
    <Col>
      <CustomInput type='file' label='Add Photos' onChange={(event) => setPhotos(Array.from(event.target.files))} multiple accept='image/*' />
    </Col>
    <Col>
      <CustomInput type='file' label='Add Videos' onChange={(event) => setVideos(Array.from(event.target.files))} multiple accept='video/*' />
    </Col>
  </Row>
  {errors && errors.length > 0 && (
    <Row className='mt-2'>
      <Col>
        {errors.map((error, index) => (
          <div key={index} className='alert alert-danger'>
            {error}
          </div>
        ))}
      </Col>
    </Row>
  )}
</div>
  );
};

export default CreatePostForm;
           
