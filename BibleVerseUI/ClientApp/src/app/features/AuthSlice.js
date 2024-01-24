import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { useHistory } from 'react-router';
import Cookies from 'universal-cookie';
import { login, logout } from '../../middleware/BBVAPI';
import { DEFAULT_ERROR } from '../../middleware/errors';
import { BADREQUEST_CODE, CONFLICT_CODE, PAYLOAD, REFRESH_TOKEN, RESPONSE_ERRORS, SUCCESS_CODE, TOKEN } from '../../middleware/types';

const cookies = new Cookies();

export const loginUser = createAsyncThunk(
  "auth/login",
  async (payload, { rejectWithValue }) => {
    try {
      // Make request to authentication API here
      const response = login(payload.email, payload.password);
      // Return JSON object containing token and refresh token
      return response
    } catch (error) {
      // Handle any errors that may occur during the request
      return rejectWithValue(error);
    }
  }
);

export const logUserOut = createAsyncThunk(
    "auth/logout",
    async (payload ,{rejectWithValue}) => {
        try {
            const response = await logout(payload.token);

            return response
        }catch(error){

            return rejectWithValue(error);
        }
    }
);

const initialState = { 
    token: cookies.get(TOKEN),
    refreshToken: cookies.get(REFRESH_TOKEN),
    loading: false,
    error: null
 }

const auth = createSlice ({
    name: 'auth',
    initialState
    ,
    reducers: {
        logIn: (state, action) => {
            let cookies = new Cookies();
            state.token = cookies.get(TOKEN);
            state.refreshToken = cookies.get(REFRESH_TOKEN);
            
        },
        logOut: (state, action) => {
            state.token = null;
            state.refreshToken = null;

            let cookies = new Cookies();
            cookies.remove(TOKEN);
            cookies.remove(REFRESH_TOKEN);
        },
        provideErrors: (state) => {
            return state.error;
        },
        provideAuth: (state) => {
            let cookies = new Cookies();
            state.token = cookies.get(TOKEN);
            state.refreshToken = cookies.get(REFRESH_TOKEN);
            return ({"token": state.token,
        "refreshToken": state.refreshToken}
            )
        }
    },
    extraReducers: {
        [loginUser.pending]: (state,action) => {
            state.loading = true;
        },
        [loginUser.fulfilled]: (state,action)=>{
            state.loading = false;
            let res = JSON.stringify(action.payload[PAYLOAD].ResponseErrors);

            switch (action.payload.responseCode) {
                case SUCCESS_CODE:
                    state.token = action.payload[PAYLOAD].Token;
                    state.refreshToken = action.payload[PAYLOAD].RefreshToken;
                    state.error = null;

                    let cookies = new Cookies();
                    cookies.set(TOKEN, action.payload[PAYLOAD].Token);
                    cookies.set(REFRESH_TOKEN, action.payload[PAYLOAD].RefreshToken);
                    break;
                
                case CONFLICT_CODE:          
                    state.error = [...action.payload[PAYLOAD].ResponseErrors];
                    console.log(state.error);
                    break;
                
                case BADREQUEST_CODE:
                    state.error = [...action.payload[PAYLOAD].ResponseErrors];
                    break;
                
                default:
                    console.log(action);
                    state.error = [...action.payload[PAYLOAD].ResponseErrors];
                    console.log(state.error);
                    break;
            }
        },
    [loginUser.rejected]: (state, action) => {
      // Update the state with the error from the action
      state.error = action.payload;
    },
    [logUserOut.pending]: (state,action) => {
        state.loading = true;
    },
    [logUserOut.fulfilled]: (state,action)=>{
        state.loading = false;
        let res = JSON.stringify(action.payload[PAYLOAD].ResponseErrors);

        switch (action.payload.responseCode) {
            case SUCCESS_CODE:
                state.token = null;
                state.refreshToken = null;
                state.error = null;

                let cookies = new Cookies();
                cookies.remove(TOKEN);
                cookies.remove(REFRESH_TOKEN);
                break;
            
            case CONFLICT_CODE:          
                state.error = [...action.payload[PAYLOAD].ResponseErrors];
                cookies.remove(TOKEN);
                cookies.remove(REFRESH_TOKEN);

                console.log(state.error);
                break;
            
            case BADREQUEST_CODE:
                state.error = [...action.payload[PAYLOAD].ResponseErrors];
                cookies.remove(TOKEN);
                cookies.remove(REFRESH_TOKEN);
                break;
            
            default:
                console.log(action);
                state.error = [...action.payload[PAYLOAD].ResponseErrors];
                console.log(state.error);
                cookies.remove(TOKEN);
                cookies.remove(REFRESH_TOKEN);
                break;
        }
    },
[logUserOut.rejected]: (state, action) => {
  // Update the state with the error from the action
  state.error = action.payload;
}}
});



export const { logIn, logOut, provideErrors, provideAuth } = auth.actions;

export default auth.reducer;

export const SelectCurrentToken = (state) => state.Auth.token;

export const provideUserToken = (state) =>   {
   console.log(state);
    return ({ "token": state.auth.token,
    "refreshToken": state.auth.refreshToken
})
}

//export const provideUserRefreshToken = (state) => state.refreshToken;