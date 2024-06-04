import { useState, useEffect, useRef } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import './MainPage_MapComponent.css';
import config from './../config.json';
import { useAuth } from './../Components/AuthProvider';
import userManager from './../AuthFiles/authConfig';

delete L.Icon.Default.prototype._getIconUrl;

L.Icon.Default.mergeOptions({
    iconRetinaUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png',
    iconUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
});

const AddMarkerOnClick = ({ setNewMarker }) => {
    useMapEvents({
        click(e) {
            setNewMarker({ title: '', description: '', position: [e.latlng.lat, e.latlng.lng] });
        }
    });
    return null;
};

const MainPage_MapComponent = () => {
    const [markers, setMarkers] = useState([
        { position: [40.7128, -74.0060], title: "New York", description: "This is New York" },
        { position: [34.0522, -118.2437], title: "Los Angeles", description: "This is Los Angeles" },
        { position: [41.8781, -87.6298], title: "Chicago", description: "This is Chicago" },
        { position: [29.7604, -95.3698], title: "Houston", description: "This is Houston" },
        { position: [39.7392, -104.9903], title: "Denver", description: "This is Denver" }
    ]);
    const { user, userData, loading, isAuthorized } = useAuth();


    const [newMarker, setNewMarker] = useState({ title: '', description: '', position: null });

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setNewMarker({ ...newMarker, [name]: value });
    };

    const handleAddMarker = async () => {
        const response = await fetch(`${config.apiBaseUrl}/CreateUserPoint`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${user.access_token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newMarker)
        });
        setMarkers([...markers, newMarker]);
        setNewMarker({ title: '', description: '', position: null });
    };


    useEffect(() => {
        async function GetUserPoints() {
            if (isAuthorized) {
                try {
                    const response = await fetch(`${config.apiBaseUrl}/GetUserMapPoints`, {
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${user.access_token}`,
                            'Content-Type': 'application/json'
                        }
                    });

                    const data = await response.json();

                    console.log(data);
                    setMarkers(data);
                } catch (error) {
                    console.log('Error while GetUserMapPoints');
                }
            }
        }
        GetUserPoints();
    }, []);


    return (
        <div className="map-container">
            <MapContainer center={[40.7128, -74.0060]} zoom={4} style={{ height: "100%", width: "100%" }}>
                <TileLayer
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                />
                {markers.map((marker, idx) => (
                    <Marker key={idx} position={marker.position}>
                        <Popup>
                            <strong>{marker.title}</strong><br />{marker.description}
                        </Popup>
                    </Marker>
                ))}
                {newMarker.position && (
                    <Marker position={newMarker.position}>
                        <Popup>
                            <div>
                                <input
                                    type="text"
                                    name="title"
                                    placeholder="Title"
                                    value={newMarker.title}
                                    onChange={handleInputChange}
                                />
                                <br />
                                <textarea
                                    name="description"
                                    placeholder="Description"
                                    value={newMarker.description}
                                    onChange={handleInputChange}
                                />
                                <br />
                                <button onClick={handleAddMarker}>Add Marker</button>
                            </div>
                        </Popup>
                    </Marker>
                )}
                <AddMarkerOnClick setNewMarker={setNewMarker} />
            </MapContainer>
        </div>
    );
};

export default MainPage_MapComponent;
