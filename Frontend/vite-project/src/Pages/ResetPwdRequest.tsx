import { useState, FormEvent } from "react";
import { api } from "../axios/api";
import Headbar from "../Components/Headbar";

interface ApiError {
    response?: {
        data: string;
        status: number;
    };
    message: string;
}

export default function ResetPwdRequest() {
    const [email, setEmail] = useState<string>("");
    const [errorMessage, setErrorMessage] = useState<string>("");
    const [success, setSuccess] = useState<boolean>(false);

    async function handleReset(e: FormEvent<HTMLFormElement>): Promise<void> {
        e.preventDefault();

        try {
            const response = await api.post("/api/User/forgot-password",
                JSON.stringify({ Email: email }),
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                }
            );

            if (response.status === 200) {
                setErrorMessage("");
                setSuccess(true);
            }
        } catch (error) {
            const apiError = error as ApiError;
            if (apiError.response) {
                setSuccess(false);
                setErrorMessage(apiError.response.data);
            } else {
                console.error("Error message:", apiError.message);
            }
        }
    }


    return (
        <>
            <Headbar />
            <div className="flex items-center justify-center h-screen">
                <div className="w-[22rem] h-[27rem] border-gray-200 border-2 rounded-xl p-8">
                    <p className="text-2xl font-mono font-black border-b-2 border-gray-200 pb-4"> PASSWORD RESET </p>
                    <p className="my-4 border-[#e6db55] rounded-lg text-gray-500 bg-[#ffffe0] border-2 p-4">Forgotten your password? Enter your e-mail address below, and we'll send you an e-mail allowing you to reset it.</p>
                    <form onSubmit={(e) => handleReset(e)} className="flex flex-col">
                        <input onChange={(e) => setEmail(e.target.value)} className="w-full mt-2 h-10 border-[#dddddd] border-2 rounded-md pl-4" placeholder="E-mail Address" />
                        <button type="submit" className="mt-8 border-2 bg-[#5cb85c] border-[#4cae4c] text-white w-44 h-10 font-mono flex items-center justify-center" > Reset Password </button>
                        {errorMessage != "" && <p className="mt-4 text-red-600"> {errorMessage} </p>}
                        {success && <p className="text-green-500 mt-4"> Email Sent! </p>}
                    </form>
                </div>
            </div>
        </>

    );
}