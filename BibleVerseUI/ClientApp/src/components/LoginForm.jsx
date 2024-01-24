import React, { useEffect, useRef } from 'react';
import { useState } from 'react';
import Spinner  from '../components/Spinner';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import Auth from '../middleware/Auth';
import { useDispatch } from 'react-redux';
import { logIn, loginUser, provideErrors } from '../app/features/AuthSlice';
import { PAYLOAD, RESPONSE_ERRORS, SUCCESS_CODE } from '../middleware/types';
import { DEFAULT_ERROR } from '../middleware/errors';

export default function LoginForm(props) {
    const [email, setEmail] = useState(localStorage.getItem('rememberedEmail') || '');
    const [password, setPassword] = useState('');
    const [btnDisabled , toggleButton] = useState(false);
    const [loading , toggleLoading] = useState(false);
    const [errors, setErrors] = useState([]);

    const isInitialMount = useRef(true);

    const dispatch = useDispatch();

    async function Login() {
        toggleButton(true);
        toggleLoading(true);
        const result = await dispatch(loginUser({email: email, password: password}))

        console.log(result.payload);

        if(result.payload != undefined)
        {
        if(result.payload.responseCode == SUCCESS_CODE)
        {
            console.log('here at success');
            props.history.push('./home');
        }else if(result.payload.responseCode !== SUCCESS_CODE && result.payload[PAYLOAD].ResponseErrors != null){
            setErrors(result.payload[PAYLOAD].ResponseErrors);
            toggleLoading(false);
            toggleButton(false);
        }else{
            console.log('here at else')
           setErrors([DEFAULT_ERROR]);
           toggleLoading(false);
            toggleButton(false);
        }
    }else {
        setErrors([DEFAULT_ERROR]);
        toggleLoading(false);
         toggleButton(false);
    }
   }

   async function checkLogin() {
        var result =  logIn();

        if(result)
        {
            //Send user to dashboard
            await this.props.history.push('./home');
        } 
   }

    function renderForm()
    {
        if(loading)
        {
            return (
                <Spinner />
            )
        } else {
            return (
                <div>
                    <Form>
                        <FormGroup>
                            <div id="errors" style={{ color: "red" }}>
                            </div>
                            <Label for="email">Email</Label>
                            <Input type="email" name="email" id="email" placeholder="Email" value={email} onChange={event => setEmail(event.target.value)}/>
                        </FormGroup>
                        <FormGroup>
                            <Label for="password">Password</Label>
                            <Input type="password" name="password" id="password" placeholder="Password" value={password} onChange={event => setPassword(event.target.value)}/>
                        </FormGroup>
                        <FormGroup check>
                            <Label check>
                                <Input type="checkbox" onChange={event => {
                                    if (event.target.checked) {
                                        localStorage.setItem('rememberedEmail', email);
                                    } else {
                                        localStorage.removeItem('rememberedEmail');
                                    }
                                }}/>{' '}
                                Remember me
                            </Label>
                        </FormGroup>
                        <Button style={{ textAlign: "center" }} disabled={btnDisabled} onClick={() => Login()}>Log In</Button>
                    </Form>
                </div>
            )
        }
    }

    useEffect(() => {
        if(isInitialMount.current)
        {
            isInitialMount.current = false;
        }else {
            if (document.getElementById("errors") != null) {
                errors.map(
                    x => document.getElementById("errors").innerHTML = (x["Description"])
                )
            }
        }
    });

    return (
        <div>
            {renderForm()}
        </div>
    )
}
