import { faAnchorCircleExclamation, faL } from "@fortawesome/free-solid-svg-icons";
import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { BADREQUEST_CODE, CONFLICT_CODE, PAYLOAD, RESPONSE_BODY, SUCCESS_CODE, USERNAME } from "../../middleware/types";
import Cookies from 'universal-cookie';
import { getUserInfo } from "../../middleware/BBVAPI";



export const retrieveUserData = createAsyncThunk(
    "user/info",
    async (payload, {rejectWithValue}) => {
    try {
        const response = await getUserInfo(payload.token, payload.refreshToken);

        return await response;
    } catch (error) {
        return rejectWithValue(error);
    }
});

const initialState = {
    userId: null,
    username: null,
    profile: null,
    status: null,
    onlineStatus: null,
    organizationId: null,
    orgName: null,
    expPoints: null,
    rwdPoints: null,
    email: null,
    level: null,
    age: null,
    friends: null,
    loading: false,
    error: null
};

const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setUserInfo(state, action){
            let cookies = new Cookies();
            //state.username = cookies.get(USERNAME);
            //state.profileImageURL = cookies.get()
        }
    },
    extraReducers: {
        [retrieveUserData.pending]: (state, action) => {
            state.loading = true;
        },
        [retrieveUserData.fulfilled]: (state, action) => {
            state.loading = false;

            switch(action.payload.responseCode){
                case SUCCESS_CODE:

                    let responseBody = JSON.parse(action.payload[PAYLOAD][RESPONSE_BODY][0]);

                    state.userId = responseBody.UserID;
                    state.username = responseBody.UserName;
                    state.status = responseBody.Status;
                    state.email = responseBody.Email;
                    state.expPoints = responseBody.ExpPoints;
                    state.member = responseBody.Member;
                    state.organizationId = responseBody.OrganizationId;
                    state.orgName = responseBody.OrgName;
                    state.onlineStatus = responseBody.OnlineStatus;
                    state.age = responseBody.Age;
                    state.friends = responseBody.Friends;
                    state.rwdPoints = responseBody.RwdPoints;
                    state.profile = responseBody.Profile;

                    break;

                case CONFLICT_CODE:
                    state.error = [...action.payload[PAYLOAD].ResponseErrors];
                    break;
                
                case BADREQUEST_CODE:
                    state.error = [...action.payload[PAYLOAD].ResponseErrors];
                    break;
                
                default:
                    state.error = [...action.payload];
                    break;
            }

        },
        [retrieveUserData.rejected]: (state, action) => {
            state.loading = false;
        }
    }
});

export const {setUserInfo} = userSlice.actions;

export default userSlice.reducer;