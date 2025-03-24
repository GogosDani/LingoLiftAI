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

type ChooseNewLanguageProps = {
    setLanguageId: React.Dispatch<React.SetStateAction<number>>;
};


export default function ChooseNewLanguage({ setLanguageId }: ChooseNewLanguageProps) {

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
            <div className="flex flex-col items-center justify-center min-h-screen text-center gap-12 sm:gap-36 py-24">
                <div className="font-mono font-bold text-4xl sm:text-6xl">
                    Which language do you want to learn?
                </div>
                <div className="flex flex-col sm:flex-row gap-4">
                    {languages.map((l) => (
                        <div
                            key={l.id}
                            onClick={() => setLanguageId(l.id)}
                            className="flex flex-col items-center w-24 sm:w-32 transition-transform duration-300 hover:scale-110"
                        >
                            <div className="w-20 h-16 sm:w-24 sm:h-20">
                                <img src={l.flag} alt={`${l.languageName} flag`} className="w-full h-full object-cover" />
                            </div>
                            <div className="font-bold text-xl sm:text-2xl">{l.languageName}</div>
                        </div>
                    ))}
                </div>
            </div>
            {showLoginForm && <LoginForm show={setShowLoginForm} />}
            {showRegisterForm && <RegisterForm show={setShowRegisterForm} />}
        </>
    )
}