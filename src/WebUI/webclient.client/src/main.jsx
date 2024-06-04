import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Route, Routes, useLocation } from 'react-router-dom';
import { AuthProvider } from './Components/AuthProvider';
import App from './App';
import SignIn_CallbackPage from './AuthFiles/SignIn_CallbackPage';
import SignOut_CallBackPage from './AuthFiles/SignOut_CallBackPage';
import { useAuth } from './Components/AuthProvider';
import { useNavigate } from 'react-router-dom';
import './index.css';
import { useState, useEffect } from 'react';
import NavBar from './Components/NavBar/NavBar';
import ListOfChats from './Components/Messages/ListOfChats';
import Profile from './Components/Profile/Profile';
import Profile_Settings from './Components/Profile/Profile_Settings';
import Someones_Profile from './Components/Profile/Someones_Profile';
const root = ReactDOM.createRoot(document.getElementById('root'));

function AppContainer() {
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, unknownsmbData, setunknownsmbDataState, hubConnection, setChatsState } = useAuth();   

    const location = useLocation();
   
    const navigate = useNavigate();
    return (
        <div className="All_container">
            <div className="Up_Bar">
                <NavBar />
            </div>
            <div className="Main_container">
                <Routes>
                    <Route path="/signin-oidc" element={<SignIn_CallbackPage />} />
                    <Route path="/signout-callback-oidc" element={<SignOut_CallBackPage />} />
                    <Route path="/" element={<App />} />
                    <Route path="/Chats" element={<ListOfChats />} />
                    <Route path="/Profile" element={<Profile />} />
                    <Route path="/Someones_Profile/:ProfileId" element={<Someones_Profile />} />
                    <Route path="/Profile_Settings" element={<Profile_Settings />} />

                </Routes>
            </div>
        </div>
    );
}

root.render(
    <React.StrictMode>
        <AuthProvider>
            <Router>
                <AppContainer />
            </Router>
        </AuthProvider>
    </React.StrictMode>
);