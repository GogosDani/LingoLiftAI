import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { api } from "../axios/api";

type HeaderProps = {
    showRegisterForm?: React.Dispatch<React.SetStateAction<boolean>>;
    showLoginForm?: React.Dispatch<React.SetStateAction<boolean>>;
};

export default function Headbar({ showRegisterForm, showLoginForm }: HeaderProps = {}) {
    const navigate = useNavigate();
    const [loggedIn, setLoggedIn] = useState(false);
    const [dropdown, setDropdown] = useState(false);
    const [userInfos, setUserInfos] = useState({ username: "", email: "", id: "" });

    useEffect(() => {
        checkIfUserLoggedIn();
        getUserInfos();
    }, [])

    async function checkIfUserLoggedIn() {
        const response = await api.get("/api/auth/check");
        if (response.status == 200) setLoggedIn(true);
        else {
            setLoggedIn(false);
            navigate("/");
        }
    }
    async function getUserInfos() {
        const response = await api.get("/api/user");
        const data = await response.data;
        setUserInfos(data);
        console.log(data);
    }

    async function handleLogout() {
        try {
            const response = await api.post('/api/auth/logout');
            if (response.status == 200) setLoggedIn(false);
        } catch (error) {
            console.error('Logout failed:', error);
        }
    }


    return (
        <div className="fixed top-0 w-full border-b-2 flex flex-row h-12 px-4 md:px-20 justify-between bg-white z-10">
            <div className="flex flex-row gap-4 md:gap-8">
                <button className="text-lg md:text-xl font-bold" onClick={() => navigate("/home")}> LingoLift </button>
                <button className="text-sm md:text-lg" onClick={() => navigate("/daily-challenge")}> Daily challenge </button>
                <button className="text-sm md:text-lg" onClick={() => navigate("/leaderboard")}> Leaderboard </button>
            </div>
            {loggedIn ? (
                <div className="flex flex-row gap-4 md:gap-8 justify-center">
                    <img src="/profile.png" onClick={() => setDropdown(prev => !prev)} className="w-6 h-6 self-center" />
                    {dropdown && (
                        <div className="absolute top-12 right-0 bg-white border border-gray-200 rounded-lg shadow-md w-48 z-20">
                            <div className="px-4 py-3 border-b border-gray-200">
                                <p className="text-sm font-semibold text-gray-900">My Account</p>
                                <p className="text-xs text-gray-500 truncate">{userInfos.email}</p>
                            </div>
                            <ul className="py-1">
                                <li onClick={() => { navigate("/profile"); setDropdown(false); }} className="px-4 py-2 text-sm text-gray-700 hover:bg-blue-100 flex items-center cursor-pointer" > Profile </li>
                                <li onClick={() => { navigate(`/wordsets/${userInfos.id}}`); setDropdown(false); }} className="px-4 py-2 text-sm text-gray-700 hover:bg-blue-100 flex items-center cursor-pointer"> Wordsets </li>
                                <li onClick={() => { handleLogout(); setDropdown(false); }} className="px-4 py-2 text-sm text-red-600 hover:bg-red-100 flex items-center cursor-pointer" > Log out </li>
                            </ul>
                        </div>
                    )}
                </div>
            ) : (
                <div className="flex flex-row gap-4 md:gap-8 justify-center">
                    <button onClick={() => showLoginForm && showLoginForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Log in </button>
                    <button onClick={() => showRegisterForm && showRegisterForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Sign up </button>
                </div>
            )}
        </div>
    );
}