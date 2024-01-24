import { configureStore } from "@reduxjs/toolkit";
import AuthSlice from "./features/AuthSlice";
import UserSlice from "./features/UserSlice";

export const store = configureStore({
    reducer: {
        auth: AuthSlice,
        user: UserSlice
        
    }
})