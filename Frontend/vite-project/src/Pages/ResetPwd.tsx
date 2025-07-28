import { useState, FormEvent, ChangeEvent, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom"
import { api } from "../axios/api";

interface ApiError {
    response?: {
        data: string;
        status: number;
    };
    message: string;
}

export default function ResetPwd() {
    const [password, setPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");
    const [errorMessage, setErrorMessage] = useState<string>("");
    const [success, setSuccess] = useState<boolean>(false);
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");
    const email = searchParams.get("email");
    const navigate = useNavigate();

    useEffect(() => {
        if (email === null || token === null) navigate("/");
    }, [])

    async function handleChange(e: FormEvent<HTMLFormElement>): Promise<void> {
        e.preventDefault();
        if (password !== confirmPassword) {
            setErrorMessage("Passwords must match!");
            return;
        }
        if (password.length < 8) {
            setErrorMessage("Passwords must be longer than 7 characters!");
            return;
        }
        if (!/[A-Z]/.test(password)) {
            setErrorMessage("Password must contain at least one uppercase letter!");
            return;
        }
        try {
            const response = await api.post("/api/User/reset-password",
                JSON.stringify({ Email: email, Token: token, NewPassword: password }),
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                });
            if (response.status == 200) {
                navigate("/")
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


    const handlePasswordChange = (e: ChangeEvent<HTMLInputElement>): void => {
        setPassword(e.target.value);
    };

    const handleConfirmPasswordChange = (e: ChangeEvent<HTMLInputElement>): void => {
        setConfirmPassword(e.target.value);
    };

    return (
        <div className="flex items-center justify-center h-screen">
            <div className="w-[22rem] h-[27rem] border-gray-200 border-2 rounded-xl p-8">
                <p className="text-2xl font-mono font-black border-b-2 border-gray-200 pb-4">
                    Change Password
                </p>
                <form onSubmit={(e) => handleChange(e)} className="flex flex-col">
                    <label htmlFor="newPassword" className="mt-8 mb-2 font-bold"> New Password</label>
                    <input id="newPassword" type="password" onChange={(e) => setPassword(e.target.value)} className="w-full h-10 border-[#dddddd] border-2 rounded-md pl-4" placeholder="New Password" />
                    <label htmlFor="confirmNewPassword" className="mt-4 mb-2 font-bold"> Confirm New Password</label>
                    <input id="confirmNewPassword" type="password" onChange={(e) => setConfirmPassword(e.target.value)} className="w-full h-10 border-[#dddddd] border-2 rounded-md pl-4" placeholder="Confirm New Password" />
                    <button type="submit" className="mt-8 border-2 bg-[#5cb85c] border-[#4cae4c] text-white w-44 h-10 font-mono flex items-center justify-center" > Change Password </button>
                    {errorMessage != "" && <p className="mt-4 text-red-600"> {errorMessage} </p>}
                </form>
            </div>
        </div>
    );
}