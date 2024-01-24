import React from 'react';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import Cookies from 'universal-cookie';
import { APIBASE } from '../middleware/Constants';
import { CONFLICT_CODE } from '../middleware/types';
import { fieldIsNotEmpty } from '../middleware/validation/UIValidator';

class CreateCommentForm extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            comment: ''
        }
    }

    componentDidMount() {}
    componentDidUpdate() {}

    setPropVal(prop, val) {
        //val = val.trim();

        this.setState({
            [prop]: val
        });
    }


    /*Function to post comment */
   async postComment() {
       if(fieldIsNotEmpty(this.state.comment))
       {
       const cookies = new Cookies();

       this.setState({ btnDisabled: true });

       try {

           let res = await (await fetch(APIBASE + "Post/Interact", {
               method: 'post',
               headers: {
                   'Accept': 'application/json',
                   'Authorization':'Bearer ' + cookies.get('token'),
                   'Content-Type': 'application/json',
                   'Token': cookies.get('token'),
                   'IntType': "Comment"
               },
               credentials: 'same-origin',
               body: JSON.stringify({
                   ParentId: this.props.PostId,
                   Body: this.state.comment
               })
           }))

           //Verify fetch completed successfully

           if (res && res.ok) {
           } else if (res && res.status == CONFLICT_CODE) {
               var result = await JSON.parse(await res.json());
               /*
               if (IsLiked == "Like") {
                   counter--;
               } else {
                   counter++;
               }
               this.setState({ likes: counter });
               */
           } else {
           }
       } catch (exception) {
           /*
           if (IsLiked == "Like") {
               counter--;
           } else {
               counter++;
           }
           this.setState({ likes: counter });
           */
           //console.log(exception);
       }
    } else {
        //bubble up error
    }
    }

    render() {
        return (
            <div>
                <Form>
                    <FormGroup>
                        <Label />
                        <Input type="text" name="comment" id="comment" placeholder="add comment" value={this.state.comment ? this.state.comment : ''} onChange={(val) => this.setPropVal("comment", val.target.value)} />
                    </FormGroup>
                    <Button onClick={() => this.postComment()}>Add Comment</Button>
                </Form>
            </div>
            )
    }
}

export default CreateCommentForm;