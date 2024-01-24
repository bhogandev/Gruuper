import React from 'react';
import { FormGroup } from 'react-bootstrap';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter} from 'reactstrap';
import DatePicker from 'react-datepicker';
import Input from 'reactstrap/lib/Input';
import Label from 'reactstrap/lib/Label';
import BBVAPI, { signUp } from '../middleware/BBVAPI';
import {DEFAULT_ERROR } from '../middleware/errors';
import {SUCCESS} from '../middleware/types';
import { Form, NavItem } from 'reactstrap/lib';
import "react-datepicker/dist/react-datepicker.css";

class SignUpForm extends React.Component {
    constructor(props){
        super(props);

        this.state = {
            username: "",
            password: "",
            email: "",
            dob: new Date(),
            phone: "",
            ref: "",
            errors: []
        }
    }

    componentDidUpdate(){
        if (document.getElementById("errors") != null) {

            var errorBox = "";

            this.state.errors.forEach(
                x => errorBox += (x)
            )

            document.getElementById("errors").innerHTML = errorBox;
        }
    }
    
    setPropVal(prop, val) {
        if(!typeof(val) == "object")
        val = val.trim();
    
        this.setState({
            [prop]: val
        });
    }

   async SignUp(){
        try {
            let res = await(await signUp(this.state.email, this.state.username, this.state.password, this.state.ref, this.state.dob, this.state.phone));

            if (res && res == SUCCESS) {
                //redirect user to sign in portal
                await this.props.history.push('./');

            } else if (res && typeof(res) == "object") {
                //Return API error to UI
                await this.setState({
                    errors: await res
                });
                
            } else {
                throw "An error occured";
            }

        }catch(exception){
            //return default error to user and have them try again

            await this.setState({
                errors: [DEFAULT_ERROR]
            });
        }

    }

    render(){
        return (
            <div>
                <Form>
                <FormGroup>
                        <div id="errors" style={{ color: "red" }}>
                        </div>
                        <Label for="email">Email</Label>
                    <Input type="email" name="email" id="email" placeholder="Email" value={this.state.email ? this.state.email : ''} onChange={(val) => this.setPropVal("email", val.target.value)}/>
                </FormGroup>
                <FormGroup>
                        <Label for="password">Password</Label>
                    <Input type="password" name="password" id="password" placeholder="Password" value={this.state.password ? this.state.password : ''} onChange={(val) => this.setPropVal("password", val.target.value)}/>
                </FormGroup>
                <FormGroup>
                    <Label for="username">UserName</Label>
                    <Input type="text" name="username" id="username" placeholder="UserName" value={this.state.username ? this.state.username : ''} onChange={(val) => this.setPropVal("username", val.target.value)} />
                </FormGroup>
                <FormGroup>
                    <Label for="dob">DOB</Label>
                    <DatePicker id="dob" name="dob" value={this.state.dob ? this.state.dob : new Date()} selected={ this.state.dob } onChange={(val) => this.setPropVal('dob', val)} />
                </FormGroup>
                <FormGroup>
                    <Label for="ref">Reference Code</Label>
                    <Input type="text" name="ref" id="ref" placeholder="Reference Code" value={this.state.ref ? this.state.ref : ''} onChange={(val) => this.setPropVal('ref', val.target.value)}/>
                </FormGroup>
                        <Button style={{ textAlign: "center" }} onClick={() => this.SignUp()}>Sign Up</Button>
                </Form>
            </div>
        )
    }
}

export default SignUpForm;