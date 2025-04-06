import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { api } from "../axios/api";

type HeaderProps = {
    showRegisterForm: React.Dispatch<React.SetStateAction<boolean>>;
    showLoginForm: React.Dispatch<React.SetStateAction<boolean>>;
};

export default function Headbar({ showRegisterForm, showLoginForm }: HeaderProps) {
    const navigate = useNavigate();
    const [loggedIn, setLoggedIn] = useState(false);

    useEffect(() => {
        async function checkIfUserLoggedIn() {
            const response = await api.get("/api/auth/check");
            if (response.status == 200) setLoggedIn(true);
            else {
                setLoggedIn(false);
            }
        }
        checkIfUserLoggedIn();
    }, [])

    async function handleLogout() {
        try {
            await api.post('/api/auth/logout');
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
                    <img src="/profile.png" className="w-6 h-6 self-center" />
                    <button onClick={() => handleLogout()} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> LOG OUT </button>
                </div>
            ) : (
                <div className="flex flex-row gap-4 md:gap-8 justify-center">
                    <button onClick={() => showLoginForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Log in </button>
                    <button onClick={() => showRegisterForm(true)} className="self-center bg-blue-600 rounded-xl h-3/4 w-20 md:w-24 text-white font-bold"> Sign up </button>
                </div>
            )}

        </div>
    );
}