//API RESPONSE STATUS
export const SUCCESS = "SUCCESS";
export const FAILURE = "FAILURE";
export const CONFLICT = "CONFLICT";

//API RESPONSE KEYS
export const RESPONSE_BODY = "responseBody";
export const RESPONSE_MESSAGE = "responseMessage";
export const RESPONSE_ERRORS = "responseErrors";

//USER OBJECT KEYS
export const USERNAME_KEY = "Username";

//GRUUP CHAT CONNECTION EVENTS
export const CONNECTION_ESTABLISHED = "Connected!";
export const CONNECTION_FAILED = "Connection failed: ";
export const RETRIEVE_USER_GROUPS = "RetrieveUserGroups";
export const RECEIVE_CONNECTION_ID = "RecieveConnID";
export const INVITE_USER_SUCCESSFUL = "InviteUserSuccessful";
export const INVITE_TO_GROUP = "InviteToGroup";
export const RETURN_EXCEPTION = "ReturnException";
export const RECEIVE_MESSAGE = "ReceiveMessage";
export const SEND_PRIVATE_MESSAGE = "SendPrivateMessage";
export const SEND_CHAT_MESSAGE = "SendMessage"; 
export const CONNECTION_IDLE = "No connection to server yet.";

//REDUX REDUCER KEYS
export const PAYLOAD = "payload"; 

//API STATUS CODES
export const CONFLICT_CODE = 409;
export const SUCCESS_CODE = 200;
export const BADREQUEST_CODE = 500;

//UI TYPES
export const LIKE = "Like";
export const UNLIKE = "Unlike";
export const PHOTO = "Photo";
export const VIDEO = "Video";
export const LINK = "Link";

//COOKIE TYPES
export const TOKEN = 'token';
export const REFRESH_TOKEN = 'refreshToken';
export const USER_TOKEN = 'user';
export const CONNECTION_ID = 'connID';
export const USERNAME = 'UserName';
export const PROFILE = 'Profile';
export const PICTURE = 'Picture';

//UI OBJECT TYPES
export const QUERY_TYPES = {
    "ALL": "ALL"
}

//GRUUP CHAT OBJECTS
export const GROUP_MODIFICATION_TYPES = {
    "INVITE": "INVITE"
}

//CUI ENGINE TYPES
export const CUI_ENGINE_TYPES = {
    "EVENTS": "EVENTS",
    "VLINE": "VLINE",
    "ISTREAM": "ISTREAM",
    "TLINE":"TLINE"
}