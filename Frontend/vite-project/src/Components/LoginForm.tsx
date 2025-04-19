import { useState } from "react"
import { api } from "../axios/api";
import { useNavigate } from "react-router-dom";

type LoginFormProps = {
    show: React.Dispatch<React.SetStateAction<boolean>>;
};

export default function LoginForm({ show }: LoginFormProps) {

    const [loginData, setLoginData] = useState({ password: "", email: "" });
    const [errorMessage, setErrorMessage] = useState("");
    const navigate = useNavigate();

    async function handleLogin(e: React.FormEvent) {
        e.preventDefault();
        try {
            const response = await api.post("/api/auth/login", {
                password: loginData.password,
                email: loginData.email
            });
            if (response.status === 200) {
                navigate("/home");
                return;
            }
        } catch (error: unknown) {
            if (error && typeof error === 'object' && 'response' in error) {
                setErrorMessage((error as { response: { data: string } }).response.data);
            } else {
                setErrorMessage(String(error));
            }
        }
    }


    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
            <form onSubmit={(e) => handleLogin(e)} className="w-11/12 md:w-1/3 lg:w-1/4 bg-slate-100 rounded-[10px] backdrop-blur-[10px] border-2 border-white/10 shadow-[0_0_40px_rgba(8,7,16,0.6)] p-6 md:p-[50px_35px] relative">
                <div className="flex flex-row">
                    {errorMessage != "" && <p className="text-red-600"> {errorMessage} </p>}
                    <button type="button" onClick={() => show(prev => !prev)} className="absolute top-2 right-2 text-xl mt-4 mr-4">X</button>
                </div>
                <div className="flex flex-col gap-4 pt-8">
                    <input required className="pl-6 h-14 rounded-lg border border-gray-300" type="text" id="email-input" placeholder="Enter email" onChange={(e) => setLoginData(prev => ({ ...prev, email: e.target.value }))} />
                    <input required className="pl-6 h-14 rounded-lg border border-gray-300" type="password" id="password-input" placeholder="Enter password" onChange={(e) => setLoginData(prev => ({ ...prev, password: e.target.value }))} />
                    <button className="mt-8 h-12 font-bold text-white font-mono bg-gradient-to-br from-[#EF4765] to-[#FF9A5A] rounded-lg">Login</button>
                </div>
            </form>
        </div>
    )
}

