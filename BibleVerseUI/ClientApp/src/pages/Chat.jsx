import React, { useState, useEffect, useRef } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import ChatBox from '../components/ChatBox';
import ChatInput from '../components/ChatInput';
import Cookies from 'universal-cookie';
import ChatInvite from '../components/ChatInvite';
import { CONNECTION_ESTABLISHED, CONNECTION_FAILED, CONNECTION_ID, CONNECTION_IDLE, GROUP_MODIFICATION_TYPES, INVITE_TO_GROUP, INVITE_USER_SUCCESSFUL, RECEIVE_CONNECTION_ID, RECEIVE_MESSAGE, RETRIEVE_USER_GROUPS, RETURN_EXCEPTION, SEND_CHAT_MESSAGE, SEND_PRIVATE_MESSAGE, TOKEN } from '../middleware/types';
import { APIBASE } from '../middleware/Constants';

const Chat = () => {
    const [ connection, setConnection ] = useState(null);
    const [ chat, setChat ] = useState([]);
    const latestChat = useRef(null);

    var cookie = new Cookies();

    const hubURL = APIBASE + "gruupmessenger";

    latestChat.current = chat;

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(hubURL, {
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(result => {
                    console.log(CONNECTION_ESTABLISHED);

                    connection.on(RECEIVE_CONNECTION_ID, (conID) => {
                        cookie.set(CONNECTION_ID, conID);
                        connection.invoke(RETRIEVE_USER_GROUPS, retrieveGroups(cookie.get(CONNECTION_ID), '', cookie.get(TOKEN)))
                    })

                    connection.on(RETRIEVE_USER_GROUPS, (groups) => {
                        console.log(groups);
                        const userGroups = groups.map( group => {
                            localStorage.setItem(group.groupName, group.groupUID)
                        });
                    })

                    connection.on(INVITE_USER_SUCCESSFUL, msg => {
                        alert(msg);
                    })

                    connection.on(RETURN_EXCEPTION, exception => {
                        alert(exception);
                    })

                    connection.on(RECEIVE_MESSAGE, message => {
                        const updatedChat = [...latestChat.current];
                        updatedChat.push(JSON.parse(message));
                        
                        setChat(updatedChat);
                    });
                })
                .catch(e => console.log(CONNECTION_FAILED, e));
        }
    }, [connection]);

    function retrieveGroups (conID, userName, token){
        const uCallPayload = {
            conID: conID,
            userName: userName,
            token: token
        }

        return JSON.stringify(uCallPayload);
    }

    async function sendNewMessage(groupId, message, recipients)
    {  
        const msg = {
            MessageId: '',
            Username: '',
            Body: message,
            Recipient: '',
            IsDeleted: false,
            GroupRecipients: recipients      
        }

        const ClientMessage = {
            conId: cookie.get(CONNECTION_ID),
            token: cookie.get(TOKEN),
            groupId: groupId,
            Message: msg
        }

        var composedMsg = JSON.stringify(ClientMessage);

        if (connection.connectionStarted) {
            try {
                await connection.send(SEND_PRIVATE_MESSAGE, composedMsg);
            }
            catch(e) {
                console.log(e);
            }
        }
        else {
            alert(CONNECTION_IDLE);
        }
    }

    const sendInvite = async (groupId, inviteUser) => {
        //Need to get groupID from original retrieveGroupCall


        const groupModificationRequest = {
            conID: cookie.get(CONNECTION_ID),
            token: cookie.get(TOKEN),
            userName: inviteUser,
            groupId: localStorage.getItem(groupId),
            modification: GROUP_MODIFICATION_TYPES.INVITE
        }

        if (connection.connectionStarted) {
            try {
                await connection.send(INVITE_TO_GROUP, JSON.stringify(groupModificationRequest));
            }
            catch(e) {
                console.log(e);
            }
        }
        else {
            alert(CONNECTION_IDLE);
        }

    }

    const sendMessage = async (user, message) => {
        const chatMessage = {
            user: user,
            message: message
        };

        if (connection.connectionStarted) {
            try {
                await connection.send(SEND_CHAT_MESSAGE, chatMessage);
            }
            catch(e) {
                console.log(e);
            }
        }
        else {
            alert(CONNECTION_IDLE);
        }
    }

    return (
        <div>
            <ChatInput sendMessage={sendNewMessage} />
            <ChatInvite sendInvite={sendInvite} />
            <hr />
            <ChatBox chat={chat}/>
        </div>
    );
};

export default Chat;