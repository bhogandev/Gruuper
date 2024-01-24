import React from 'react';
import {Container, Row, Col} from 'react-bootstrap';
import { Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import Cookies from 'universal-cookie';
import {getParameterByName} from '../js/browserMethods';
import bbvapi, { confirmUser } from '../middleware/BBVAPI';
import { SUCCESS, TOKEN } from '../middleware/types';

class ConfirmEmail extends React.Component{
    static displayName = ConfirmEmail.name;

    constructor(props) {
        super(props);
        var cookies = new Cookies();
        this.state = {
            id: getParameterByName('id', window.location.href),
            confirmationToken: getParameterByName(TOKEN, window.location.href),
            valid: false,
            err: []
        }
    }

    async confirmUser(){
        if(this.state.id != null && this.state.confirmationToken != null)
        {
            //api call to verify
            var result = await confirmUser(this.state.id, this.state.confirmationToken);

            if(await result == SUCCESS)
            {
                await this.setState({valid: true});
            } else {
                console.log(await result);
                const errBag = await result.map(x => {
                    return <b key={x.Code}>{x.Description}</b>
                });
               
                 this.setState({err: await errBag});                
            }
        }
    }

    componentDidMount(){
        /*
            - Verify Params
            - If no params, display Error Message and provide link to login/sign up screen
            - If params are here execute api call to verify user
            - If Success provide Success Message with link to login
        */
        this.confirmUser();
    }

    componentDidUpdate()
    {
        
    }

    render(){
        if(this.state.valid){
        return (
            <div>
                <p>Your Email Has been Confirmed!</p>
                <p>Click <Link to="/">here</Link> to login!</p>
            </div>
        )
        }else if(this.state.err.length > 0){
            return (
                <div>
                    <p>Errors:</p>
                    {console.log(this.state.err)}
                    {this.state.err};
                </div>
            )
        } else {
            return(
            <div>Loading...</div>
            )
        }
    }
}

export default ConfirmEmail;