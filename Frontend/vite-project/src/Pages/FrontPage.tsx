import { useState } from "react"
import LoginForm from "../Components/LoginForm";
import RegisterForm from "../Components/RegisterForm";
import Headbar from "../Components/Headbar";

export default function FrontPage() {

    const [showRegisterForm, setShowRegisterForm] = useState(false);
    const [showLoginForm, setShowLoginForm] = useState(false);
    const [success, setSuccess] = useState(false);



    return (
        <>
            <Headbar showRegisterForm={setShowRegisterForm} showLoginForm={setShowLoginForm} />
            <div className="flex flex-col md:flex-row px-4 md:px-32 pt-12 md:pt-24 items-center flex-wrap z-0">
                <div className="flex-1 flex justify-center">
                    <img src="logo.png" className="w-4/5 animate-pulse-scale"></img>
                </div>
                <div className="flex flex-col items-center flex-1 text-center">
                    <div className="text-green-500 text-3xl h-10"> {success && "Registration was successful!"} </div>
                    <div className="text-gray-600 font-bold text-2xl md:text-4xl w-4/5">
                        BEST WAY TO LEARN FOREIGN LANGUAGES WITH AI!
                    </div>
                    <div className="pt-8 flex flex-col gap-2 items-center w-4/5">
                        <button className="bg-blue-600 rounded-xl h-12 w-3/5 text-white font-bold" onClick={() => setShowRegisterForm(prev => !prev)}>
                            LET'S GET STARTED!
                        </button>
                        <button className="bg-blue-600 rounded-xl h-12 w-3/5 text-white font-bold" onClick={() => setShowLoginForm(prev => !prev)}>
                            ALREADY HAVE A PROFILE?
                        </button>
                    </div>
                </div>
                {showLoginForm && <LoginForm show={setShowLoginForm} />}
                {showRegisterForm && <RegisterForm show={setShowRegisterForm} setSuccess={setSuccess} />}
            </div>
        </>


    )
}