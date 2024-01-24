import React, { useState } from 'react';

const ChatInvite = (props) => {
    const [inviteUser, setInviteUser] = useState('');

    const onSubmit = (e) => {
        e.preventDefault();

        const isInviteUserProvided = inviteUser && inviteUser !== '';

        if (isInviteUserProvided) {
            var groupMem = [];
            props.sendInvite(groupMem, inviteUser);
        } 
        else {
            alert('Please select a recipent and a message.');
        }
    } 

    const onInviteUserChange = (e) => {
        setInviteUser(e.target.value);
    }

    return (
        <form onSubmit={onSubmit}>
            <input type="text" id="inviteUser" name="inviteUser" onChange={onInviteUserChange}/>
            <button>Invite</button>
        </form>
    )
}

export default ChatInvite;