﻿import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import './Profile.css'
import axios from '../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import config from '../../config.json'; 
import { useAuth } from '../AuthProvider';
import MyPosts from '../Profile/Posts/MyPosts';

import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
import MainPage_Market from './../../MainPage_Market/MainPage_Market';
import MainPage_Map from './../../MainPage_Map/MainPage_MapComponent';
const Profile = () => {
    const navigate = useNavigate();
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };
    const handleMouseEnter = () => {
        setIsHovered(true);
    }

    const handleMouseLeave = () => {
        setIsHovered(false);
    }

    const handleImageUpload = (e) => {
        setLoadingState(true);
        const file = e.target.files[0];
        const maxSize = 5 * 1024 * 1024;
        const maxResolution = 1920;

        if (file && !isImageFile(file)) {
            toast.error('Allowed extensions: image/jpeg, image/png, image/svg+xml, image/webp.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingState(false);
            return;
        }

        if (file && file.size > maxSize) {
            toast.error('The max size of photo: 2mb.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingState(false);
            return;
        }

        if (file) {
            const img = new Image();
            img.onload = () => {
                const width = img.width;
                const height = img.height;
                if (width > maxResolution || height > maxResolution) {
                    toast.error(`The maximum image resolution must be ${maxResolution}x${maxResolution} pixels.`, {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                    });
                    e.target.value = null;
                    setLoadingState(false);
                    return;
                }
                else {
                    const reader = new FileReader();
                    reader.onloadend = async () => {
                        const imageData = reader.result;
                        const blob = new Blob([new Uint8Array(imageData)], { type: file.type });
                        const base64Avatar = await new Promise((resolve) => {
                            const reader = new FileReader();
                            reader.onloadend = () => resolve(reader.result.split(',')[1]);
                            reader.readAsDataURL(blob);

                        });
                        try {
                            const accessToken = await userManager.getUser().then(user => user.access_token);
                            const response = await fetch(`${config.apiBaseUrl}/userProfilePhotoUpload`, {
                                method: 'POST',
                                headers: {
                                    'Authorization': `Bearer ${accessToken}`,
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({ avatar: base64Avatar })
                            });

                            if (!response.ok) {
                                if (response.status === 400) {
                                    const errorData = await response.json();
                                    const errors = errorData.errors;

                                    for (const key in errors) {
                                        if (errors.hasOwnProperty(key)) {
                                            const errorMessages = errors[key];
                                            errorMessages.forEach(message => {
                                                toast.error(`${key}: ${message}`, {
                                                    position: "top-right",
                                                    autoClose: 5000,
                                                    hideProgressBar: false,
                                                    closeOnClick: true,
                                                    pauseOnHover: true,
                                                    draggable: true,
                                                    progress: undefined,
                                                });
                                            });
                                        }
                                    }
                                } else {
                                    toast.error(`HTTP error! Status: ${response.status}`, {
                                        position: "top-right",
                                        autoClose: 5000,
                                        hideProgressBar: false,
                                        closeOnClick: true,
                                        pauseOnHover: true,
                                        draggable: true,
                                        progress: undefined,
                                    });
                                }
                                throw new Error(`HTTP error! Status: ${response.status}`);
                            }

                            else {
                                const data = await response.json();
                                setUserDataState(data);
                                toast.success('Profile photo uploaded successfully.', {
                                    position: "top-right",
                                    autoClose: 5000,
                                    hideProgressBar: false,
                                    closeOnClick: true,
                                    pauseOnHover: true,
                                    draggable: true,
                                    progress: undefined,
                                });
                            }
                           
                        } catch (err) {
                            toast.error(`Error occurred: ${err.message}`, {
                                position: "top-right",
                                autoClose: 5000,
                                hideProgressBar: false,
                                closeOnClick: true,
                                pauseOnHover: true,
                                draggable: true,
                                progress: undefined,
                            });
                            console.error('Error while sending the request', err);
                        } finally {
                            setLoadingState(false);
                        }
                    };
                    reader.readAsArrayBuffer(file);
                }
            };
            img.src = URL.createObjectURL(file);
        }
    };

    const handleImageDelete = async  (e) => {
        setLoadingState(true);
        if (userData.photo == null || (Array.isArray(userData.photo) && userData.photo.length === 0) || userData.photo =='') {
            toast.error('You do not have a photo', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;

            setLoadingState(false);
            return;
        }
        
        try {

            const accessToken = await userManager.getUser().then(user => user.access_token);
            await fetch(`${config.apiBaseUrl}/userProfilePhotoUpload`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ avatar: "" })
            });
            userData.photo = null;
            toast.success('Profile photo was deleted successfully.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
        catch (err) {
            toast.error(err, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
        setLoadingState(false);
    };

    const isImageFile = (file) => {
        const acceptedImageTypes = ['image/jpeg', 'image/png', 'image/svg+xml', 'image/webp'];

        return acceptedImageTypes.includes(file.type);
    };


    return (
        <div>
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar newestOnTop closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loading ? <div className={`overlay ${loading ? 'visible' : ''}`}>
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                </div>
                

            </div>
                : isAuthorized === false ? (
                <div>UnAuthorized</div>
                ) : userData === null ? (
                        <div>
                            <div>There is no information</div>
                            <button onClick={onLogout} className="ProfileButton">Logout</button>
                        </div>
                
            ) : (
                <div className="profile">
                    {userData && (
                        <>
                        <div className="UpProfile">
                                    <div className="First_UpProfile">

                                        <div className="avatar-container" onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
                                                    <img src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "../../public/NoPhoto.jpg"} alt="Avatar" className="avatar" />
                                            <div className="buttons-container">
                                                <label className="edit-button">
                                                    New
                                                    <input type="file" name="clientAvatar" accept="image/*" onChange={handleImageUpload} style={{ display: 'none' }} capture="false" />
                                                </label>
                                                <button className="delete-button" onClick={handleImageDelete}>Delete</button>
                                                    </div>

                                        </div>



                                        <div className="RightSideProfile">

                                            <div className="RightSideFromAvatar">
                                                <div className="Profile_Information">
                                                            <div className="profile-info">
                                                                <div className="FF_Count">
                                                                    <div className="FF_Count_Name">{userData.nickName}</div>
                                                                    <div className="FF_Count_F">Followers: {userData.followersCount}</div>
                                                                    <div className="FF_Count_F">Following: {userData.followsCount}</div>
                                                                    <div className="FF_Count_Buttons">
                                                                        <NavLink to="/Profile_Settings" className="button outline" >Settings</NavLink>
                                                                        <button onClick={onLogout} className="button outline">LogOut</button>
                                                                    </div>
                                                                    
                                                                </div>
                                                        <div className="ProfileBio">
                                                            {userData.bio}
                                                        </div>
                                                            </div>
                                                    
                                                </div>


                                            </div>
                                                    
                                            </div>
                                              
                                    </div>

                                        </div>
                                        <div className="Profile_Container_Map">
                                            <div className="Profile_Container_Map_Title">Моя персональна карта</div>
                                            <MainPage_Map />
                                        </div>
                                <MyPosts />
                        </>
                    )}
                </div>
            )}
        </div>
    );
};

export default Profile;
