import { useEffect, useState } from "react"
import { api } from "../axios/api";
import Headbar from "./Headbar";
import LoginForm from "../Components/LoginForm";
import RegisterForm from "../Components/RegisterForm";


type Language = {
    languageName: string;
    flag: string;
    id: number;
};

export default function ChooseNewLanguage() {

    const [languages, setLanguages] = useState<Language[]>([]);
    const [showRegisterForm, setShowRegisterForm] = useState(false);
    const [showLoginForm, setShowLoginForm] = useState(false);

    useEffect(() => {
        async function fetchLanguages() {
            const response = await api.get("/api/language");
            setLanguages(response.data);
        }
        fetchLanguages();
    }, [])

    return (
        <>
            <Headbar showRegisterForm={setShowRegisterForm} showLoginForm={setShowLoginForm} />
            <div className="flex flex-col items-center justify-center h-screen text-center gap-36">
                <div className="font-mono text-6xl font-bold">Which language do you want to learn?</div>
                <div className="flex flex-row gap-4">
                    {languages.map(l => (
                        <div
                            key={l.id}
                            className="flex flex-col items-center w-32 transition-transform duration-300 hover:scale-110"
                        >
                            <div className="w-24 h-20">
                                <img src={l.flag} />
                            </div>
                            <div className="font-bold text-2xl">{l.languageName}</div>
                        </div>
                    ))}
                </div>
            </div>
            {showLoginForm && <LoginForm show={setShowLoginForm} />}
            {showRegisterForm && <RegisterForm show={setShowRegisterForm} />}
        </>

    )
}