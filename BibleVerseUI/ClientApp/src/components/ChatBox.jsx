import React from 'react';

import Message from './Message';

const ChatBox = (props) => {
    const chat = props.chat
        .map(m => <Message 
            key={Date.now() * Math.random()}
            user={m.user}
            message={m.message}
            time={m.time}
            />);

    return(
        <div>
            {chat}
        </div>
    )
};

export default ChatBox;