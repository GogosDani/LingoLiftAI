import { useEffect, useState } from "react"
import { api } from "../axios/api";
import Headbar from "./Headbar";
import LoginForm from "../Components/LoginForm";
import RegisterForm from "../Components/RegisterForm";
import { useNavigate } from "react-router-dom";


type Language = {
    languageName: string;
    flag: string;
    id: number;
};

type ChooseNewLanguageProps = {
    setLanguageId: React.Dispatch<React.SetStateAction<number>>;
    setStage: React.Dispatch<React.SetStateAction<number>>;
    languageId: number;
};


export default function ChooseNewLanguage({ setLanguageId, languageId, setStage }: ChooseNewLanguageProps) {

    const [languages, setLanguages] = useState<Language[]>([]);
    const [showRegisterForm, setShowRegisterForm] = useState(false);
    const [showLoginForm, setShowLoginForm] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        async function fetchLanguages() {
            const response = await api.get("/api/language");
            setLanguages(response.data);
        }
        fetchLanguages();
    }, [])


    async function continueWithoutTest() {
        try {
            const response = await api.post("/api/language", {
                languageId: languageId
            });
            if (response.status == 200) navigate("/home");
        }
        catch (error) {
            console.error(error)
        }
    }


    return (
        <>
            <Headbar showRegisterForm={setShowRegisterForm} showLoginForm={setShowLoginForm} />
            <div className="flex flex-col items-center justify-center min-h-screen text-center py-24">
                <div className="font-mono font-bold text-4xl sm:text-6xl pb-20">
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
                <div className="mt-12 flex flex-col items-center">
                    <div className="font-mono font-bold text-xl sm:text-lg text-center">
                        Do you have any experience in this language?
                    </div>
                    <div className="flex flex-row gap-8 mt-4">
                        <button onClick={() => setStage(1)} className="flex items-center justify-center text-white font-bold text-lg px-6 py-4 rounded-[14px] border-0 bg-[#38D2D2] shadow-[inset_-3px_-3px_9px_rgba(255,255,255,0.25),inset_0px_3px_9px_rgba(255,255,255,0.3),inset_0px_1px_1px_rgba(255,255,255,0.6),inset_0px_-8px_36px_rgba(0,0,0,0.3),inset_0px_1px_5px_rgba(255,255,255,0.6),2px_19px_31px_rgba(0,0,0,0.2)] h-12">
                            YES
                        </button>
                        <button onClick={() => continueWithoutTest()} className="flex items-center justify-center text-white font-bold text-lg px-6 py-4 rounded-[14px] border-0 bg-[#38D2D2] shadow-[inset_-3px_-3px_9px_rgba(255,255,255,0.25),inset_0px_3px_9px_rgba(255,255,255,0.3),inset_0px_1px_1px_rgba(255,255,255,0.6),inset_0px_-8px_36px_rgba(0,0,0,0.3),inset_0px_1px_5px_rgba(255,255,255,0.6),2px_19px_31px_rgba(0,0,0,0.2)] h-12">
                            NO
                        </button>

                    </div>
                </div>

            </div>
            {showLoginForm && <LoginForm show={setShowLoginForm} />}
            {showRegisterForm && <RegisterForm show={setShowRegisterForm} />}
        </>
    )
}