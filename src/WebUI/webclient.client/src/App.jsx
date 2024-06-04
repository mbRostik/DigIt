
import userManager from './AuthFiles/authConfig';
import React, { useState, useEffect } from 'react';
import { isAuthenticated } from './Functions/CheckAuthorization';
import { ThreeDots } from 'react-loader-spinner';
import config from './config.json'; 
import { useAuth } from './Components/AuthProvider';
import { Link, useNavigate } from 'react-router-dom';
import './App.css'
import MainPage_Market from './MainPage_Market/MainPage_Market';
import MainPage_Map from './MainPage_Map/MainPage_MapComponent';

function App() {
    const [posts, setPosts] = useState(null);
    const navigate = useNavigate();

    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    

    const menuItems = [
        { id: 1, name: "Item 1", description: "TFEFEfefwrgwrghwrlg wrgnlwrjrg lwlrigj wrolgrlwng klw rgowr glwr go wlrgnowrbhgwl rhowrhg", price: 1000 },
        { id: 2, name: "Item 2", description: "TFEFEfefwrgwrghwrlg wrgnlwrjrg lwlrigj wrolgrlwng klw rgowr glwr go wlrgnowrbhgwl rhowrhg", price: 1000 },
        { id: 2, name: "Item 3", description: "TFEFEfefwrgwrghwrlg wrgnlwrjrg lwlrigj wrolgrlwng klw rgowr glwr go wlrgnowrbhgwl rhowrhg", price: 1000 },
        { id: 2, name: "Item 4", description: "TFEFEfefwrgwrghwrlg wrgnlwrjrg lwlrigj wrolgrlwng klw rgowr glwr go wlrgnowrbhgwl rhowrhg", price: 1000 }
    ];


    return (
        <div>
            {loading ? (
                <div className={`overlay ${loading ? 'visible' : ''}`}>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} />
                </div>
            ) : !posts ? (
                    <div className="Main_Container">
                        <div className="Main_First_Container">
                            <div className="Main_First_Container_Info">
                                <div className="Main_First_Container_Info_Title">
                                    СКІФСЬКЕ ЗОЛОТО
                                </div>
                                <div className="Main_First_Container_Info_Text">
                                    Скіфське золото - це колекція золотих виробів, знайдених в скіфських курганах на території сучасної України. Скіфське золото є свідченням багатства та культурного розвитку скіфського народу.
                                </div>
                            </div>
                        </div>

                        <div className="Main_Second_Container">

                            <div className="Main_Second_Container_Title">Купуйте нещодавньо додані товари</div>
                            {menuItems !== null && (
                                <div className="Main_Second_Container_Items">

                                    {menuItems.slice(0, 5).map((item, index) => (
                                        <MainPage_Market key={item.id} item={item} />
                                    ))}

                                </div>
                            )}
                            

                        </div>

                        <div className="Main_Third_Container">

                            <div className="Main_Third_Container_Info">
                                <div className="Main_Third_Container_Info_Title">
                                    Продавайте разом з нами 
                                </div>
                                <div className="Main_Third_Container_Info_Text">
                                    Не втрачайте можливість стати частиною нашої процвітаючої спільноти продавців. Приєднуйтесь до нас сьогодні та розкажіть світові про свої неповторні продукти!
                                </div>

                                <div className="Main_Third_Container_Info_Button">
                                    Розпочати продажу
                                </div>
                            </div>

                            <img className="Main_Third_Container_Info_Image" src="../../public/Rectangle 102.png" alt="Home" />

                            
                        </div>


                        <div className="Main_Firth_Container">

                            <div className="Main_Third_Container_Info_Title">
                                Дізнайтеся більше про цікавинки на нашому сайті   
                            </div>

                            <div className="Main_Firth_Container_Photos">

                                <div className="Main_Firth_Container_FirstPhoto">
                                    <div className="image-container">
                                        <img className="Main_Firth_Container_FirstPhoto_Image" src="../../public/Rectangle 103.png" alt="Home" />
                                    </div>
                                    <div className="Main_Firth_Container_Text">
                                        Стародавні монети, які були знайдені на території України, різних племенів
                                    </div>
                                </div>
                                <div className="Main_Firth_Container_FirstPhoto">

                                    <div className="image-container">
                                        <img className="Main_Firth_Container_SecondPhoto_Image" src="../../public/Rectangle 104.png" alt="Home" />
                                    </div>
                                    <div className="Main_Firth_Container_Text">
                                        Статуетки на території України                                    </div>
                                </div>
                                <div className="Main_Firth_Container_FirstPhoto">
                                    <div className="image-container">
                                        <img className="Main_Firth_Container_SecondPhoto_Image" src="../../public/Писар_Михайло_Василевич.jpg" alt="Home" />
                                    </div>
                                    <div className="Main_Firth_Container_Text">
                                        Пергаменти за часи середньовіччя                                 </div>
                                </div>
                            </div>

                        </div>

                        <div className="Main_Fifth_Container">
                            <div className="Main_Third_Container_Info_Title">
                                Карта всіх знахідок нашої спільноти
                            </div>
                            <div className="Main_Fifth_Container_Map">
                                <MainPage_Map />
                            </div>
                            
                        </div>
                </div>

            ) : null}
        </div>
    );
}
export default App;
