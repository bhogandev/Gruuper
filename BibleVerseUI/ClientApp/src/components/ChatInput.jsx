import React, { useState } from 'react';

const ChatInput = (props) => {
    const [groupId, setGroupId] = useState('');
    const [message, setMessage] = useState('');
    const [recipients, setRecipients] = useState('');

    const onSubmit = (e) => {
        e.preventDefault();

        const isRecipeitnsProvided = recipients && recipients !== [];
        const isMessageProvided = message && message !== '';

        if (isRecipeitnsProvided && isMessageProvided) {
            var groupMem = [recipients];
            props.sendMessage('',message, groupMem);
        } 
        else {
            alert('Please select a recipent and a message.');
        }
    }

    const onUserUpdate = (e) => {
        setRecipients(e.target.value);
    }

    const onMessageUpdate = (e) => {
        setMessage(e.target.value);
    }

    return (
        <form 
            onSubmit={onSubmit}>
            <label htmlFor="user">User:</label>
            <br />
            <input 
                id="user" 
                name="user" 
                value={recipients}
                onChange={onUserUpdate} />
            <br/>
            <label htmlFor="message">Message:</label>
            <br />
            <input 
                type="text"
                id="message"
                name="message" 
                value={message}
                onChange={onMessageUpdate} />
            <br/><br/>
            <button>Submit</button>
        </form>
    )
};

export default ChatInput;