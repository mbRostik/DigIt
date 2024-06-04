import React, { useEffect } from 'react';
import './MainPage_Market.css';

const MainPage_Market = ({ item }) => {


    return (
        <div className="MainPage_Market_Item">
            <div className="MainPage_Market_Item_Title">{item.name}</div>
            <div className="MainPage_Market_Item_Photo"></div>
            <div className="MainPage_Market_Item_Description">{item.description}</div>
            <div className="MainPage_Market_Item_Price">{item.price}</div>
        </div>
    );
}

export default MainPage_Market;
